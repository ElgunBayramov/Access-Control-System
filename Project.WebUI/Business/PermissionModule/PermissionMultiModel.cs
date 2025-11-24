using Project.WebUI.Models.Entities;
using System.Collections.Generic;

namespace Project.WebUI.Business.PermissionModule
{
    public class PermissionMultiModel
    {
        public List<Permission> Permission { get; set; }

        public PermissionCreateCommand CreateCommand { get; set; }
    }
}
