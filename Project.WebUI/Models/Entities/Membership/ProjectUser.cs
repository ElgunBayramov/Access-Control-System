using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.WebUI.Models.Entities.Membership
{
    public class ProjectUser : IdentityUser<int>
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string FinCode { get; set; }
        public DateTime RegisterDate { get; set; }
        public int? ProfessionId { get; set; }
        public virtual Profession Profession { get; set; }
        public int? DepartmentId { get; set; }
        public virtual Department Department { get; set; }  


    }
}
