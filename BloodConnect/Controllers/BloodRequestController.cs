using BloodConnect.Data;
using BloodConnect.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace BloodConnect.Controllers
{
    public class BloodRequestController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;

        public BloodRequestController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IEmailSender emailSender)
        {
            _context = context;
            _userManager = userManager;
            _emailSender = emailSender;
        }

        // Step 1: Show available donors
        public async Task<IActionResult> AvailableDonors()
        {
            var currentUser = await _userManager.GetUserAsync(User);

            var donors = await _context.Users
                .Where(u => u.IsApprovedByAdmin && u.EmailConfirmed && u.IsAvailable && u.Id != currentUser.Id)
                .ToListAsync();

            return View(donors);
        }

        // Step 2: Send a blood request
        public async Task<IActionResult> RequestBlood(string donorId)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var donor = await _userManager.FindByIdAsync(donorId);

            if (donor == null || currentUser == null)
            {
                return NotFound();
            }

            var request = new BloodRequest
            {
                DonorId = donorId,
                RequestorId = currentUser.Id,
                Status = RequestStatus.Pending
            };

            _context.BloodRequests.Add(request);
            await _context.SaveChangesAsync();

            // Email Notification to Donor
            string subject = "New Blood Donation Request";
            string message = $"Hello {donor.FullName},<br/><br/>" +
                             $"You have received a new blood donation request from <strong>{currentUser.FullName}</strong>.<br/>" +
                             $"Please login to Blood Connect and respond.<br/><br/>Thank you!";

            await _emailSender.SendEmailAsync(donor.Email, subject, message);

            return RedirectToAction(nameof(MySentRequests));
        }

        // Step 3: View My Sent Requests
        public async Task<IActionResult> MySentRequests()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var myRequests = await _context.BloodRequests
                .Include(r => r.Donor)
                .Where(r => r.RequestorId == currentUser.Id)
                .ToListAsync();

            return View(myRequests);
        }

        // Step 4: View Requests Received (For Donors)
        public async Task<IActionResult> MyReceivedRequests()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var received = await _context.BloodRequests
                .Include(r => r.Requestor)
                .Where(r => r.DonorId == currentUser.Id)
                .ToListAsync();

            return View(received);
        }

        // Step 5: Donor Accept Request
        public async Task<IActionResult> Accept(int id)
        {
            var request = await _context.BloodRequests
                .Include(r => r.Requestor)
                .Include(r => r.Donor)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (request != null && request.Status == RequestStatus.Pending)
            {
                request.Status = RequestStatus.Accepted;
                await _context.SaveChangesAsync();

                // Send email notification to Requestor
                string subject = "Your Blood Request Has Been Accepted!";
                string message = $"Hello {request.Requestor.FullName},<br/><br/>" +
                                 $"Good news! <strong>{request.Donor.FullName}</strong> has accepted your blood donation request.<br/>" +
                                 $"Please contact the donor at: <strong>{request.Donor.PhoneNumber}</strong>.<br/><br/>" +
                                 $"Thank you for using Blood Connect.";

                await _emailSender.SendEmailAsync(request.Requestor.Email, subject, message);
            }

            return RedirectToAction(nameof(MyReceivedRequests));
        }

        // Step 6: Donor Decline Request
        public async Task<IActionResult> Decline(int id)
        {
            var request = await _context.BloodRequests
                .Include(r => r.Requestor)
                .Include(r => r.Donor)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (request != null && request.Status == RequestStatus.Pending)
            {
                request.Status = RequestStatus.Declined;
                await _context.SaveChangesAsync();

                // Email notification to Requestor
                string subject = "Your Blood Request Was Declined";
                string message = $"Hello {request.Requestor.FullName},<br/><br/>" +
                                 $"Unfortunately, <strong>{request.Donor.FullName}</strong> has declined your blood donation request.<br/>" +
                                 $"You may consider sending a request to another donor.<br/><br/>Thank you for using Blood Connect.";

                await _emailSender.SendEmailAsync(request.Requestor.Email, subject, message);
            }

            return RedirectToAction(nameof(MyReceivedRequests));
        }

        // Step 7: Requestor Marks as Completed (after donation)
        public async Task<IActionResult> MarkAsCompleted(int id)
        {
            var request = await _context.BloodRequests
                .Include(r => r.Donor)
                .Include(r => r.Requestor)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (request != null && request.Status == RequestStatus.Accepted)
            {
                request.Status = RequestStatus.Completed;

                // Award points
                request.Donor.TotalPoints += 10;
                request.Donor.LastDonationDate = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                // Email notification to Donor
                string subject = "Your Blood Donation is Marked Completed!";
                string message = $"Hello {request.Donor.FullName},<br/><br/>" +
                                 $"Great news! <strong>{request.Requestor.FullName}</strong> has marked the blood donation request as completed.<br/>" +
                                 $"You have earned <strong>10 points</strong> for this donation.<br/><br/>Thank you for saving lives!";

                await _emailSender.SendEmailAsync(request.Donor.Email, subject, message);
            }

            return RedirectToAction(nameof(MySentRequests));
        }
    }
}
