using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Project.WebUI.Models.Entities.Membership;

namespace Project.Domain.Models.DataContexts.Configurations.Membership
{
    public class ProjectUserTokenEntityTypeConfiguration : IEntityTypeConfiguration<ProjectUserToken>
    {
        public void Configure(EntityTypeBuilder<ProjectUserToken> builder)
        {
            builder.ToTable("UserTokens", "Membership");
        }
    }
}

