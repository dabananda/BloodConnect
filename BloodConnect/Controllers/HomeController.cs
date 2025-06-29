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

        public async Task<IActionResult> Index(string search, string bloodGroup, string department, int page = 1)
        {
            const int pageSize = 12; // 12 donors per page

            var currentUser = await _userManager.GetUserAsync(User);
            var currentUserId = currentUser?.Id; // Handle null user

            var donorsQuery = _context.Users
                .Where(u => u.IsApprovedByAdmin && u.EmailConfirmed && u.IsAvailable &&
                           (currentUserId == null || u.Id != currentUserId));

            if (!string.IsNullOrEmpty(search))
            {
                donorsQuery = donorsQuery.Where(u => u.FullName.Contains(search) || u.Email.Contains(search));
            }

            if (!string.IsNullOrEmpty(bloodGroup))
            {
                donorsQuery = donorsQuery.Where(u => u.BloodGroup == bloodGroup);
            }

            if (!string.IsNullOrEmpty(department))
            {
                donorsQuery = donorsQuery.Where(u => u.Department == department);
            }

            // Get total count for pagination
            var totalDonors = await donorsQuery.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalDonors / pageSize);

            // Ensure page is within valid range
            if (page < 1) page = 1;
            if (page > totalPages && totalPages > 0) page = totalPages;

            // Get paginated donors
            var donors = await donorsQuery
                .OrderBy(u => u.FullName) // Add consistent ordering for pagination
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var topDonors = await _context.Users
                .Where(u => u.IsApprovedByAdmin && u.EmailConfirmed)
                .OrderByDescending(u => u.TotalPoints)
                .Take(3)
                .ToListAsync();

            ViewBag.TopDonors = topDonors;

            // Pagination info
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalDonors = totalDonors;
            ViewBag.PageSize = pageSize;
            ViewBag.HasPreviousPage = page > 1;
            ViewBag.HasNextPage = page < totalPages;

            // Filter parameters for pagination links
            ViewBag.Search = search;
            ViewBag.BloodGroup = bloodGroup;
            ViewBag.Department = department;

            // dropdown filter options
            ViewBag.BloodGroups = await _context.Users
                .Select(u => u.BloodGroup)
                .Distinct()
                .ToListAsync();

            ViewBag.Departments = await _context.Users
                .Select(u => u.Department)
                .Distinct()
                .ToListAsync();

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