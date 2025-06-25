using Microsoft.AspNetCore.Identity;

namespace BloodConnect.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
        public string StudentId { get; set; }
        public string Department { get; set; }
        public string Session { get; set; }
        public string BloodGroup { get; set; }
        public DateTime? LastDonationDate { get; set; }
        public bool IsAvailable { get; set; }
        public int TotalPoints { get; set; }
        public bool IsApprovedByAdmin { get; set; }
        public DateTime RegisteredAt { get; set; }
    }
}
