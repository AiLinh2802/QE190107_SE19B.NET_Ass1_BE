namespace TaskTrack.Repo.Models;

public class Department
{
    public int DepartmentId { get; set; }
    public string DepartmentName { get; set; } = null!;
    public string DepartmentDescription { get; set; } = null!;
    public bool IsActive { get; set; } = true;

    public ICollection<Project> Projects { get; set; } = new List<Project>();
}
