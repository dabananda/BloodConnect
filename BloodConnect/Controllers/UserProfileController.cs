using BloodConnect.Data;
using BloodConnect.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BloodConnect.Controllers
{
    public class UserProfileController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public UserProfileController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        // View Profile
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            return View(user);
        }

        // GET: Edit Profile Form
        public async Task<IActionResult> Edit()
        {
            var user = await _userManager.GetUserAsync(User);
            return View(user);
        }

        // POST: Save Profile Changes
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ApplicationUser model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                user.PhoneNumber = model.PhoneNumber;
                user.IsAvailable = model.IsAvailable;
                user.Department = model.Department;
                user.Session = model.Session;
                user.BloodGroup = model.BloodGroup;

                await _userManager.UpdateAsync(user);
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }
    }
}
