using Microsoft.EntityFrameworkCore;
using TaskTrack.Repo.Models;

namespace TaskTrack.Repo.Data;

public class TaskTrackDbContext(DbContextOptions<TaskTrackDbContext> options) : DbContext(options)
{
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<TaskItem> Tasks => Set<TaskItem>();
    public DbSet<Tag> Tags => Set<Tag>();
    public DbSet<TaskTag> TaskTags => Set<TaskTag>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Department>(entity =>
        {
            entity.ToTable("Department");
            entity.HasKey(e => e.DepartmentId).HasName("Department_pkey");
            entity.Property(e => e.DepartmentId).HasColumnName("DepartmentID");
            entity.Property(e => e.DepartmentName).HasColumnName("DepartmentName").HasMaxLength(100);
            entity.Property(e => e.DepartmentDescription).HasColumnName("DepartmentDescription").HasMaxLength(300);
            entity.Property(e => e.IsActive).HasColumnName("IsActive").HasDefaultValue(true);
        });

        modelBuilder.Entity<Project>(entity =>
        {
            entity.ToTable("Project");
            entity.HasKey(e => e.ProjectId).HasName("Project_pkey");
            entity.Property(e => e.ProjectId).HasColumnName("ProjectID");
            entity.Property(e => e.ProjectName).HasColumnName("ProjectName").HasMaxLength(200);
            entity.Property(e => e.Description).HasColumnName("Description");
            entity.Property(e => e.StartDate).HasColumnName("StartDate");
            entity.Property(e => e.EndDate).HasColumnName("EndDate");
            entity.Property(e => e.Status).HasColumnName("Status").HasDefaultValue((short)0);
            entity.Property(e => e.DepartmentId).HasColumnName("DepartmentID");
            entity.Property(e => e.IsActive).HasColumnName("IsActive").HasDefaultValue(true);
            entity.Property(e => e.CreatedDate).HasColumnName("CreatedDate").HasColumnType("timestamp without time zone").HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.HasOne(e => e.Department).WithMany(e => e.Projects).HasForeignKey(e => e.DepartmentId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("FK_Project_Department");
        });

        modelBuilder.Entity<TaskItem>(entity =>
        {
            entity.ToTable("Task");
            entity.HasKey(e => e.TaskId).HasName("Task_pkey");
            entity.Property(e => e.TaskId).HasColumnName("TaskID");
            entity.Property(e => e.Title).HasColumnName("Title").HasMaxLength(300);
            entity.Property(e => e.Description).HasColumnName("Description");
            entity.Property(e => e.Status).HasColumnName("Status").HasDefaultValue((short)0);
            entity.Property(e => e.Priority).HasColumnName("Priority").HasDefaultValue((short)1);
            entity.Property(e => e.DueDate).HasColumnName("DueDate");
            entity.Property(e => e.ProjectId).HasColumnName("ProjectID");
            entity.Property(e => e.IsActive).HasColumnName("IsActive").HasDefaultValue(true);
            entity.Property(e => e.CreatedDate).HasColumnName("CreatedDate").HasColumnType("timestamp without time zone").HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.ModifiedDate).HasColumnName("ModifiedDate").HasColumnType("timestamp without time zone");
            entity.HasOne(e => e.Project).WithMany(e => e.Tasks).HasForeignKey(e => e.ProjectId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("FK_Task_Project");
        });

        modelBuilder.Entity<Tag>(entity =>
        {
            entity.ToTable("Tag");
            entity.HasKey(e => e.TagId).HasName("Tag_pkey");
            entity.HasIndex(e => e.TagName).IsUnique().HasDatabaseName("Tag_TagName_key");
            entity.Property(e => e.TagId).HasColumnName("TagID");
            entity.Property(e => e.TagName).HasColumnName("TagName").HasMaxLength(50);
            entity.Property(e => e.Color).HasColumnName("Color").HasMaxLength(7);
        });

        modelBuilder.Entity<TaskTag>(entity =>
        {
            entity.ToTable("TaskTag");
            entity.HasKey(e => new { e.TaskId, e.TagId }).HasName("PK_TaskTag");
            entity.Property(e => e.TaskId).HasColumnName("TaskID");
            entity.Property(e => e.TagId).HasColumnName("TagID");
            entity.HasOne(e => e.Task).WithMany(e => e.TaskTags).HasForeignKey(e => e.TaskId).OnDelete(DeleteBehavior.Cascade).HasConstraintName("FK_TaskTag_Task");
            entity.HasOne(e => e.Tag).WithMany(e => e.TaskTags).HasForeignKey(e => e.TagId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("FK_TaskTag_Tag");
        });
    }
}
