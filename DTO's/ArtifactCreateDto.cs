namespace KolokwiumTemplate.DTO_s;

public class ArtifactCreateDto
{
    public int    ArtifactId    { get; set; }
    public string Name          { get; set; }
    public DateTime OriginDate { get; set; }
    public int    InstitutionId { get; set; }
}