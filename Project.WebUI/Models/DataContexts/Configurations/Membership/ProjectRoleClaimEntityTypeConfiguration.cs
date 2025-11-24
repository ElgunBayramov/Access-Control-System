using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Project.WebUI.Models.Entities.Membership;

namespace Project.Models.DataContexts.Configurations.Membership
{
    public class ProjectRoleClaimEntityTypeConfiguration : IEntityTypeConfiguration<ProjectRoleClaim>
    {
        public void Configure(EntityTypeBuilder<ProjectRoleClaim> builder)
        {
            builder.ToTable("RoleClaims", "Membership");
        }
    }
}

