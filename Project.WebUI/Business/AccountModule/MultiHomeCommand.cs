using Project.WebUI.Models.Entities.Membership;

namespace Project.WebUI.Business.AccountModule
{
    public class MultiHomeCommand
    {
        public ProjectUser User { get; set; }
        public string UserIsNullDescription { get; set; }   
    }
}
