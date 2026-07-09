using Project.WebUI.Models.Entities;
using System.Collections.Generic;

namespace Project.WebUI.Models.ViewModels
{
    public class GuardWatchViewModel
    {
        public List<AccessLog> AccessLogs { get; set; }
        public List<Permission> Permissions { get; set; }
    }
}