using Project.WebUI.Models.Entities.Membership;
using System;

namespace Project.WebUI.Models.Entities
{
    public class AccessLog
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public virtual ProjectUser User { get; set; }
        public DateTime Timestamp { get; set; }
        public string Direction { get; set; }  
        public string ScannedBy { get; set; }  
        public string Note { get; set; }
    }
}