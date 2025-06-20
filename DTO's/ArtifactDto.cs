namespace KolokwiumTemplate.DTO_s;

public class ArtifactDto
{
    public string Name       { get; set; }
    public DateTime OriginDate { get; set; }
    public InstitutionDto Institution { get; set; }
}