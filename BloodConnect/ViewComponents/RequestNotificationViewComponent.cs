using BloodConnect.Data;
using BloodConnect.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BloodConnect.ViewComponents
{
    public class RequestNotificationViewComponent : ViewComponent
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public RequestNotificationViewComponent(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            if (!User.Identity.IsAuthenticated) return View(0);

            var user = await _userManager.GetUserAsync(HttpContext.User);

            var pendingCount = await _context.BloodRequests
                .CountAsync(r => r.DonorId == user.Id && r.Status == RequestStatus.Pending);

            return View(pendingCount);
        }
    }
}