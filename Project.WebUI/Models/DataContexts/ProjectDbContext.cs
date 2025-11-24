using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Project.WebUI.Models.Entities;
using Project.WebUI.Models.Entities.Membership;

namespace Project.WebUI.Models.DataContexts
{
    public class ProjectDbContext : IdentityDbContext<ProjectUser,ProjectRole,int,ProjectUserClaim,ProjectUserRole,ProjectUserLogin,
        ProjectRoleClaim,ProjectUserToken>
    {
        public ProjectDbContext(DbContextOptions options)
            : base(options)
        {

        }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Direction> Directions { get; set; }
        public DbSet<Profession> Professions { get; set; }
        public DbSet<Permission> Permissions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ProjectDbContext).Assembly);
        }

    }
}
