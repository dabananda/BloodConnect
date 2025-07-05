using BloodConnect.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BloodConnect.Models;
using System.Threading.Tasks;

namespace BloodConnect.ViewComponents
{
    public class SentRequestNotificationViewComponent : ViewComponent
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public SentRequestNotificationViewComponent(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (user == null) return Content("");

            var count = await _context.BloodRequests
                .Where(r => r.RequestorId == user.Id && r.Status == RequestStatus.Accepted)
                .CountAsync();

            return View(count);
        }
    }
}
