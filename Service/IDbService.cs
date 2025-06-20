namespace KolokwiumTemplate.Service;
using KolokwiumTemplate.DTO_s;

public interface IDbService
{
    
    Task<PreservationProjectDto> GetProjectByIdAsync(int projectId);

    Task<PreservationProjectDto> CreateArtifactAndProjectAsync(ArtifactWithProjectCreateDto dto);
}
