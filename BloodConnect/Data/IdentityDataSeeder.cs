using BloodConnect.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace BloodConnect.Data
{
    public static class IdentityDataSeeder
    {
        public static async Task SeedRolesAndAdminAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            string[] roles = { "Admin", "User" };

            // Ensure Roles Exist
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // Create Admin User
            string adminEmail = Environment.GetEnvironmentVariable("SEED_ADMIN_EMAIL");
            string adminPassword = Environment.GetEnvironmentVariable("SEED_ADMIN_PASSWORD");

            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                var newAdmin = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true,
                    FullName = "Dabananda Mitra",
                    IsApprovedByAdmin = true,
                    BloodGroup = "B+",
                    CurrentAddress = "Mirpur 13, Dhaka",
                    StudentId = "031920028",
                    Department = "Admin",
                    Session = "2019-20",
                    PhoneNumber = "01304080014"
                };

                var result = await userManager.CreateAsync(newAdmin, adminPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(newAdmin, "Admin");
                }
            }

            //  Ensure All Other Users Are in "User" Role
            var allUsers = await userManager.Users.ToListAsync();

            foreach (var user in allUsers)
            {
                if (user.Email != adminEmail && !(await userManager.IsInRoleAsync(user, "User")))
                {
                    await userManager.AddToRoleAsync(user, "User");
                }
            }
        }
    }
}
