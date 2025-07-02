using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace BloodConnect.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
        public string StudentId { get; set; }
        public string Department { get; set; }
        public string Session { get; set; }
        public string BloodGroup { get; set; }
        public string CurrentAddress { get; set; }
        public DateTime? LastDonationDate { get; set; }
        public bool IsAvailable { get; set; }
        public int TotalPoints { get; set; }
        public bool IsApprovedByAdmin { get; set; }
        public bool IsSuspended { get; set; } = false;
        public DateTime RegisteredAt { get; set; }
        public string? ProfilePictureUrl { get; set; }
    }
}
