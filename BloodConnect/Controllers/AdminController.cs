using BloodConnect.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BloodConnect.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // List all users pending approval
        public async Task<IActionResult> PendingUsers()
        {
            var pendingUsers = await _context.Users
                .Where(u => !u.IsApprovedByAdmin && u.EmailConfirmed)
                .ToListAsync();

            return View(pendingUsers);
        }

        // Approve user by Id
        public async Task<IActionResult> Approve(string id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                user.IsApprovedByAdmin = true;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(PendingUsers));
        }
    }
}
