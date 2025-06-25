using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BloodConnect.Models
{
    public enum RequestStatus
    {
        Pending,
        Accepted,
        Declined,
        Completed
    }

    public class BloodRequest
    {
        public int Id { get; set; }

        [Required]
        public string RequestorId { get; set; }

        [ForeignKey("RequestorId")]
        public ApplicationUser Requestor { get; set; }

        [Required]
        public string DonorId { get; set; }

        [ForeignKey("DonorId")]
        public ApplicationUser Donor { get; set; }

        public DateTime RequestDate { get; set; } = DateTime.UtcNow;

        public RequestStatus Status { get; set; } = RequestStatus.Pending;
    }
}
