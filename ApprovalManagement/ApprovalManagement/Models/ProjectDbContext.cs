using Microsoft.EntityFrameworkCore;

namespace ApprovalManagement.Models
{
    public class ProjectDbContext:DbContext
    {
        public ProjectDbContext(DbContextOptions<ProjectDbContext> options) : base(options)
        {
        }
        public DbSet<Project> Projects { get; set; }
    }
}
