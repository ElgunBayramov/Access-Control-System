using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Project.WebUI.Models.Entities.Membership;

namespace Project.Models.DataContexts.Configurations.Membership
{
    public class ProjectUserRoleEntityTypeConfiguration : IEntityTypeConfiguration<ProjectUserRole>
    {
        public void Configure(EntityTypeBuilder<ProjectUserRole> builder)
        {
            builder.HasKey(k => new
            {
                k.UserId,
                k.RoleId
            });
            builder.ToTable("UserRoles", "Membership");
        }
    }
}

