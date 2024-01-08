using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TwitterApi.Database;
using TwitterApi.Models;


namespace TwitterApi.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class SeedController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly UserManager<TwitterUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public SeedController(DataContext context, UserManager<TwitterUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpPost]
        [ResponseCache(NoStore=true)]
        public async Task<IActionResult> AuthData()
        {
            int rolesCreated = 0;
            int usersAddedToRoles = 0;

            if (!await _roleManager.RoleExistsAsync("Admin"))
            {
                var adminRole = new IdentityRole("Admin");
                await _roleManager.CreateAsync(adminRole);
                rolesCreated++;
            }

            if (!await _roleManager.RoleExistsAsync("User"))
            {
                var userRole = new IdentityRole("User");
                await _roleManager.CreateAsync(userRole);
                rolesCreated++;
            }

            var adminUser = new TwitterUser
            {
                UserName = "admin",
                Email = "admin.test.com",
                FullName = "Admin",
                Bio = "Admin",
                Location = "Web",
                IsVerified = false,
                FollowersCount = 0,
                FollowingCount = 0,
                TweetsCount = 0
            };

            if (await _userManager.FindByEmailAsync(adminUser.Email) == null)
            {
                var result = await _userManager.CreateAsync(adminUser, "admin");
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(adminUser, "Admin");
                    usersAddedToRoles++;
                }
            }

            return StatusCode(201,
                new
                {
                    message = "Auth data has been seeded successfully.",
                    rolesCreated = rolesCreated,
                    usersAddedToRoles = usersAddedToRoles
                }
            );
        }
    }
}