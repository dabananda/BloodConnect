using BloodConnect.Data;
using BloodConnect.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Authorize]
public class AdminController : Controller
{
    private readonly ApplicationDbContext _context;

    public AdminController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Dashboard()
    {
        var pendingUsers = await _context.Users.CountAsync(u => !u.IsApprovedByAdmin);
        var totalUsers = await _context.Users.CountAsync();
        var totalDonations = await _context.Users.SumAsync(u => u.TotalPoints);  // Assuming 1 point per donation
        var totalRequests = await _context.BloodRequests.CountAsync();
        var lastDonation = await _context.Users.MaxAsync(u => u.LastDonationDate);
        var firstDonation = await _context.Users.MinAsync(u => u.LastDonationDate);

        // Average donations per month
        var donationMonths = await _context.Users
            .Where(u => u.LastDonationDate.HasValue)
            .Select(u => u.LastDonationDate.Value.Month)
            .Distinct()
            .CountAsync();

        double avgDonationsPerMonth = donationMonths > 0 ? (double)totalDonations / donationMonths : 0;

        var suspendedUsers = await _context.Users
            .Where(u => u.IsSuspended)
            .ToListAsync();

        var model = new AdminDashboardViewModel
        {
            PendingUsers = pendingUsers,
            TotalUsers = totalUsers,
            TotalDonations = totalDonations,
            TotalRequests = totalRequests,
            LastDonationDate = lastDonation,
            FirstDonationDate = firstDonation,
            AvgDonationsPerMonth = avgDonationsPerMonth,
            SuspendedUsers = suspendedUsers
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> ToggleSuspend(string userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user != null)
        {
            user.IsSuspended = !user.IsSuspended;
            _context.Update(user);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Dashboard));
    }

    public async Task<IActionResult> PendingUsers()
    {
        var pendingUsers = await _context.Users
            .Where(u => !u.IsApprovedByAdmin)
            .ToListAsync();
        return View(pendingUsers);
    }

    public async Task<IActionResult> TotalUsers()
    {
        var users = await _context.Users.ToListAsync();
        return View(users);
    }

    public async Task<IActionResult> TotalDonations()
    {
        var donors = await _context.Users
            .Where(u => u.TotalPoints > 0)
            .ToListAsync();
        return View(donors);
    }

    public async Task<IActionResult> TotalRequests()
    {
        var requests = await _context.BloodRequests
            .Include(r => r.Requestor)
            .Include(r => r.Donor)
            .ToListAsync();
        return View(requests);
    }

}
