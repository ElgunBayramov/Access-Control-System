using Microsoft.AspNetCore.Identity;

namespace Project.WebUI.Models.Entities.Membership
{
    public class ProjectRole : IdentityRole<int>
    {
        public byte Rank { get; set; }
    }
}
