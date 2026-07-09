using Project.WebUI.Models.Entities.Membership;
using System;

namespace Project.WebUI.Models.Entities
{
    public class Permission
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Profession { get; set; }
        public string Email { get; set; }        
        public Status Status { get; set; } = Status.Pending;
        public DateTime? Date { get; set; }
        public DateTime? DeletedDate { get; set; }
        public string Duration { get; set; }
        public string Reason { get; set; }
        public string RejectReason { get; set; } 
        public string QrToken { get; set; }      
        public string QrCodePath { get; set; }   
        public int? DirectionId { get; set; }
        public virtual Direction Direction { get; set; }
        public int ProjectUserId { get; set; }
        public virtual ProjectUser ProjectUser { get; set; }
    }
}