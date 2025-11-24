using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Project.WebUI.Models.Entities.Membership;

namespace Project.Models.DataContexts.Configurations.Membership
{
    public class ProjectUserClaimEntityTypeConfiguration : IEntityTypeConfiguration<ProjectUserClaim>
    {
        public void Configure(EntityTypeBuilder<ProjectUserClaim> builder)
        {
            builder.ToTable("UserClaims", "Membership");
        }
    }
}

