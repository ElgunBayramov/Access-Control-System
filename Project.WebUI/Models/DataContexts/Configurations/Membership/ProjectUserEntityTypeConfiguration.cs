using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Project.WebUI.Models.Entities.Membership;

namespace Project.Models.DataContexts.Configurations.Membership
{
    public class ProjectUserEntityTypeConfiguration : IEntityTypeConfiguration<ProjectUser>
    {
        public void Configure(EntityTypeBuilder<ProjectUser> builder)
        {
            builder.ToTable("Users", "Membership");
        }
    }
}

