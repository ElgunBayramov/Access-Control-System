using System;

namespace Project.WebUI.Models.Entities
{
    public class Profession
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime? DeletedDate { get; set; }
    }
}
