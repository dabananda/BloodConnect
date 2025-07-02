using BloodConnect.Data;
using BloodConnect.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BloodConnect.Controllers
{
    [Authorize]
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

        // Step 1: Show Available Donors
        public async Task<IActionResult> AvailableDonors()
        {
            var currentUser = await _userManager.GetUserAsync(User);

            var donors = await _context.Users
                .Where(u => u.IsApprovedByAdmin && u.EmailConfirmed && u.IsAvailable && u.Id != currentUser.Id)
                .ToListAsync();

            return View(donors);
        }

        // ✅ Step 2: Handle Blood Request Form POST (from Modal)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RequestBlood(string DonorId, string Reason, string Location, string RequestorPhone)
        {
            var donor = await _context.Users.FindAsync(DonorId);
            var requestor = await _userManager.GetUserAsync(User);

            if (donor == null || requestor == null)
            {
                TempData["ErrorMessage"] = "Invalid donor or requestor.";
                return RedirectToAction("Index", "Home");
            }

            var newRequest = new BloodRequest
            {
                DonorId = donor.Id,
                RequestorId = requestor.Id,
                Status = RequestStatus.Pending,
                RequestDate = DateTime.Now,
                Reason = Reason,
                Location = Location,
                RequestorPhone = RequestorPhone
            };

            _context.BloodRequests.Add(newRequest);
            await _context.SaveChangesAsync();

            // Email notification to Donor
            string subject = "New Blood Donation Request Received";
            string message = $"Hello {donor.FullName},<br/><br/>" +
                             $"You have a new blood donation request from <strong>{requestor.FullName}</strong>.<br/><br/>" +
                             $"<strong>Reason:</strong> {Reason}<br/>" +
                             $"<strong>Location:</strong> {Location}<br/>" +
                             $"<strong>Requestor Phone:</strong> {RequestorPhone}<br/><br/>" +
                             $"Please login to Blood Connect to Accept or Decline the request.<br/><br/>Thank you!";

            await _emailSender.SendEmailAsync(donor.Email, subject, message);

            TempData["SuccessMessage"] = "Your blood request has been sent successfully!";
            return RedirectToAction("Index", "Home");
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

        // Step 5: Donor Accepts Request
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

                // Notify Requestor
                string subject = "Your Blood Request Has Been Accepted!";
                string message = $"Hello {request.Requestor.FullName},<br/><br/>" +
                                 $"<strong>{request.Donor.FullName}</strong> has accepted your blood donation request.<br/><br/>" +
                                 $"You can contact the donor at: <strong>{request.Donor.PhoneNumber}</strong>.<br/><br/>" +
                                 $"Thank you for using Blood Connect.";

                await _emailSender.SendEmailAsync(request.Requestor.Email, subject, message);
            }

            return RedirectToAction(nameof(MyReceivedRequests));
        }

        // Step 6: Donor Declines Request
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

                // Notify Requestor
                string subject = "Your Blood Request Was Declined";
                string message = $"Hello {request.Requestor.FullName},<br/><br/>" +
                                 $"<strong>{request.Donor.FullName}</strong> has declined your blood donation request.<br/><br/>" +
                                 $"You may send requests to other available donors.<br/><br/>Thank you for using Blood Connect.";

                await _emailSender.SendEmailAsync(request.Requestor.Email, subject, message);
            }

            return RedirectToAction(nameof(MyReceivedRequests));
        }

        // Step 7: Requestor Marks as Completed
        public async Task<IActionResult> MarkAsCompleted(int id)
        {
            var request = await _context.BloodRequests
                .Include(r => r.Donor)
                .Include(r => r.Requestor)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (request != null && request.Status == RequestStatus.Accepted)
            {
                request.Status = RequestStatus.Completed;

                // Award points and update last donation
                request.Donor.TotalPoints += 10;
                request.Donor.LastDonationDate = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                // Notify Donor
                string subject = "Blood Donation Completed!";
                string message = $"Hello {request.Donor.FullName},<br/><br/>" +
                                 $"<strong>{request.Requestor.FullName}</strong> has marked the blood donation as completed.<br/><br/>" +
                                 $"You have earned <strong>10 points</strong>.<br/><br/>Thank you for saving lives!";

                await _emailSender.SendEmailAsync(request.Donor.Email, subject, message);
            }

            return RedirectToAction(nameof(MySentRequests));
        }
    }
}
