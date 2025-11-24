using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Project.WebUI.Models.Entities.Membership;

namespace Project.Models.DataContexts.Configurations.Membership
{
    public class ProjectRoleEntityTypeConfiguration : IEntityTypeConfiguration<ProjectRole>
    {
        public void Configure(EntityTypeBuilder<ProjectRole> builder)
        {
            builder.ToTable("Roles", "Membership");
        }
    }
}

