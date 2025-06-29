using BloodConnect.Data;
using BloodConnect.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace BloodConnect.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, UserManager<ApplicationUser> userManager, IEmailSender emailSender)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
            _emailSender = emailSender;
        }

        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);

            var donorsQuery = _context.Users
                .Where(u => u.IsApprovedByAdmin && u.EmailConfirmed && u.IsAvailable && !u.IsSuspended);

            if (currentUser != null)
            {
                donorsQuery = donorsQuery.Where(u => u.Id != currentUser.Id);
            }

            var donors = await donorsQuery.ToListAsync();

            var topDonors = await _context.Users
                .Where(u => u.IsApprovedByAdmin && u.EmailConfirmed)
                .OrderByDescending(u => u.TotalPoints)
                .Take(3)
                .ToListAsync();

            ViewBag.TopDonors = topDonors;

            return View(donors);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
