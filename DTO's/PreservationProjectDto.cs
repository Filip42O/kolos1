namespace KolokwiumTemplate.DTO_s;

public class PreservationProjectDto
{
    public int ProjectId     { get; set; }
    public string Objective  { get; set; }
    public DateTime StartDate{ get; set; }
    public DateTime? EndDate { get; set; }

    public ArtifactDto Artifact { get; set; }
    public List<StaffAssignmentDto> StaffAssignments { get; set; }
}