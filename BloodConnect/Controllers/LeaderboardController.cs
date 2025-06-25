using BloodConnect.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace BloodConnect.Controllers
{
    public class LeaderboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LeaderboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> TopDonors()
        {
            var topDonors = await _context.Users
                .Where(u => u.IsApprovedByAdmin && u.EmailConfirmed)
                .OrderByDescending(u => u.TotalPoints)
                .Take(10)
                .ToListAsync();

            return View(topDonors);
        }
    }
}
