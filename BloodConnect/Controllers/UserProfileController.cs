using BloodConnect.Data;
using BloodConnect.Helpers;
using BloodConnect.Models;
using BloodConnect.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BloodConnect.Controllers
{
    [Authorize]
    public class UserProfileController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly CloudinaryService _cloudinaryService;

        public UserProfileController(UserManager<ApplicationUser> userManager, ApplicationDbContext context, CloudinaryService cloudinaryService)
        {
            _userManager = userManager;
            _context = context;
            _cloudinaryService = cloudinaryService;
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
            ViewBag.BloodGroups = StaticData.BloodGroups;
            ViewBag.Departments = StaticData.Departments;
            return View(user);
        }

        // POST: Save Profile Changes + Profile Picture
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ApplicationUser model, IFormFile ProfilePictureFile)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                user.PhoneNumber = model.PhoneNumber;
                user.CurrentAddress = model.CurrentAddress;
                user.Email = model.Email;
                user.IsAvailable = model.IsAvailable;
                user.Department = model.Department;
                user.Session = model.Session;
                user.BloodGroup = model.BloodGroup;

                // Handle profile picture upload
                if (ProfilePictureFile != null && ProfilePictureFile.Length > 0)
                {
                    var imageUrl = await _cloudinaryService.UploadImageAsync(ProfilePictureFile);
                    if (imageUrl != null)
                    {
                        user.ProfilePictureUrl = imageUrl;
                        TempData["SuccessMessage"] = "Profile picture updated successfully!";
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Failed to upload profile picture.";
                    }
                }

                await _userManager.UpdateAsync(user);
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }
    }
}
