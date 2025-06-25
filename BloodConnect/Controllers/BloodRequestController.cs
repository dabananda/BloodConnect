using BloodConnect.Data;
using BloodConnect.Models;
using Microsoft.AspNetCore.Identity;
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

        public BloodRequestController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
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

            var request = new BloodRequest
            {
                DonorId = donorId,
                RequestorId = currentUser.Id,
                Status = RequestStatus.Pending
            };

            _context.BloodRequests.Add(request);
            await _context.SaveChangesAsync();

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
            var request = await _context.BloodRequests.FindAsync(id);
            if (request != null)
            {
                request.Status = RequestStatus.Accepted;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(MyReceivedRequests));
        }

        // Step 6: Donor Decline Request
        public async Task<IActionResult> Decline(int id)
        {
            var request = await _context.BloodRequests.FindAsync(id);
            if (request != null)
            {
                request.Status = RequestStatus.Declined;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(MyReceivedRequests));
        }

        // Step 7: Requestor Marks as Completed (after donation)
        public async Task<IActionResult> MarkAsCompleted(int id)
        {
            var request = await _context.BloodRequests
                .Include(r => r.Donor)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (request != null && request.Status == RequestStatus.Accepted)
            {
                request.Status = RequestStatus.Completed;

                // Award points
                request.Donor.TotalPoints += 10;  // You can change point logic later
                request.Donor.LastDonationDate = DateTime.UtcNow;

                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(MySentRequests));
        }
    }
}
