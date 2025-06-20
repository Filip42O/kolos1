namespace KolokwiumTemplate.DTO_s;

public class ProjectCreateDto
{
    public int      ProjectId { get; set; }
    public string   Objective { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate  { get; set; }
}