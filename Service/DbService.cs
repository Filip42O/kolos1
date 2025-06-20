namespace KolokwiumTemplate.Service;

using System.Data.SqlClient;
using KolokwiumTemplate.DTO_s;

public class DbService : IDbService
{
    private readonly IConfiguration _config;

    public DbService(IConfiguration config)
    {
        _config = config;
    }

    
    
    
    
  public async Task<PreservationProjectDto> GetProjectByIdAsync(int projectId)
{
    PreservationProjectDto dto = null;

    using var conn = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
    await conn.OpenAsync();

    const string sqlHeader = @"
      SELECT 
        pp.projectId, pp.objective, pp.startDate, pp.endDate,
        a.name, a.originDate,
        i.institutionId, i.name, i.foundedYear
      FROM Preservation_Project pp
      JOIN Artifact a ON pp.artifactId    = a.artifactId
      JOIN Institution i ON a.institutionId = i.institutionId
     WHERE pp.projectId = @ProjectId;
    ";

    using (var cmd = new SqlCommand(sqlHeader, conn))
    {
        cmd.Parameters.AddWithValue("@ProjectId", projectId);
        using var rdr = await cmd.ExecuteReaderAsync();
        if (!await rdr.ReadAsync())
            return null;

        dto = new PreservationProjectDto
        {
            ProjectId = rdr.GetInt32(0),
            Objective = rdr.GetString(1),
            StartDate = rdr.GetDateTime(2),
            EndDate   = rdr.IsDBNull(3) ? (DateTime?)null : rdr.GetDateTime(3),
            Artifact = new ArtifactDto
            {
                Name       = rdr.GetString(4),
                OriginDate = rdr.GetDateTime(5),
                Institution = new InstitutionDto
                {
                    InstitutionId = rdr.GetInt32(6),
                    Name          = rdr.GetString(7),
                    FoundedYear   = rdr.GetInt32(8)
                }
            },
            StaffAssignments = new List<StaffAssignmentDto>()
        };
    }

    const string sqlStaff = @"
      SELECT s.firstName, s.lastName, s.hireDate, sa.role
      FROM Staff_Assignment sa
      JOIN Staff s ON sa.staffId = s.staffId
     WHERE sa.projectId = @ProjectId;
    ";

    using (var cmd2 = new SqlCommand(sqlStaff, conn))
    {
        cmd2.Parameters.AddWithValue("@ProjectId", projectId);
        using var rdr2 = await cmd2.ExecuteReaderAsync();
        while (await rdr2.ReadAsync())
        {
            dto.StaffAssignments.Add(new StaffAssignmentDto {
                FirstName = rdr2.GetString(0),
                LastName  = rdr2.GetString(1),
                HireDate  = rdr2.GetDateTime(2),
                Role      = rdr2.GetString(3)
            });
        }
    }

    return dto;
}






  
  
  
  
  
  public async Task<PreservationProjectDto> CreateArtifactAndProjectAsync(ArtifactWithProjectCreateDto dto)
{
    using var conn = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
    await conn.OpenAsync();
    using var tx = conn.BeginTransaction();

    var cmdInst = new SqlCommand(
        "SELECT 1 FROM Institution WHERE institutionId = @iid",
        conn, tx);
    cmdInst.Parameters.AddWithValue("@iid", dto.Artifact.InstitutionId);
    var instExists = await cmdInst.ExecuteScalarAsync();
    if (instExists == null)
        throw new KeyNotFoundException($"Institution {dto.Artifact.InstitutionId} not found");

    var cmdArtDupe = new SqlCommand(
        "SELECT 1 FROM Artifact WHERE artifactId = @aid",
        conn, tx);
    cmdArtDupe.Parameters.AddWithValue("@aid", dto.Artifact.ArtifactId);
    if (await cmdArtDupe.ExecuteScalarAsync() != null)
        throw new InvalidOperationException($"Artifact {dto.Artifact.ArtifactId} already exists");

    var cmdPrjDupe = new SqlCommand(
        "SELECT 1 FROM Preservation_Project WHERE projectId = @pid",
        conn, tx);
    cmdPrjDupe.Parameters.AddWithValue("@pid", dto.Project.ProjectId);
    if (await cmdPrjDupe.ExecuteScalarAsync() != null)
        throw new InvalidOperationException($"Project {dto.Project.ProjectId} already exists");

    var cmdArtIns = new SqlCommand(@"
        INSERT INTO Artifact(artifactId, name, originDate, institutionId)
        VALUES(@aid,@name,@orig,@iid)",
        conn, tx);
    cmdArtIns.Parameters.AddWithValue("@aid",  dto.Artifact.ArtifactId);
    cmdArtIns.Parameters.AddWithValue("@name", dto.Artifact.Name);
    cmdArtIns.Parameters.AddWithValue("@orig", dto.Artifact.OriginDate);
    cmdArtIns.Parameters.AddWithValue("@iid",  dto.Artifact.InstitutionId);
    await cmdArtIns.ExecuteNonQueryAsync();

    var cmdPrjIns = new SqlCommand(@"
        INSERT INTO Preservation_Project(projectId, artifactId, startDate, endDate, objective)
        VALUES(@pid,@aid,@start,@end,@obj)",
        conn, tx);
    cmdPrjIns.Parameters.AddWithValue("@pid",   dto.Project.ProjectId);
    cmdPrjIns.Parameters.AddWithValue("@aid",   dto.Artifact.ArtifactId);
    cmdPrjIns.Parameters.AddWithValue("@start", dto.Project.StartDate);
    cmdPrjIns.Parameters.AddWithValue("@end",   (object)dto.Project.EndDate ?? DBNull.Value);
    cmdPrjIns.Parameters.AddWithValue("@obj",   dto.Project.Objective);
    await cmdPrjIns.ExecuteNonQueryAsync();

    tx.Commit();

    return await GetProjectByIdAsync(dto.Project.ProjectId);
}


    
    
    
}
