using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Project.WebUI.Models.Entities.Membership;

namespace Project.Models.DataContexts.Configurations.Membership
{
    public class ProjectUserLoginEntityTypeConfiguration : IEntityTypeConfiguration<ProjectUserLogin>
    {
        public void Configure(EntityTypeBuilder<ProjectUserLogin> builder)
        {
            builder.ToTable("UserLogins", "Membership");
        }
    }
}

