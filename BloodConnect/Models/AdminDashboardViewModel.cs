using System;
using System.Collections.Generic;

namespace BloodConnect.Models
{
    public class AdminDashboardViewModel
    {
        public int PendingUsers { get; set; }
        public int TotalUsers { get; set; }
        public int TotalDonations { get; set; }
        public int TotalRequests { get; set; }
        public DateTime? LastDonationDate { get; set; }
        public DateTime? FirstDonationDate { get; set; }
        public double AvgDonationsPerMonth { get; set; }
        public List<ApplicationUser> SuspendedUsers { get; set; }
    }
}
