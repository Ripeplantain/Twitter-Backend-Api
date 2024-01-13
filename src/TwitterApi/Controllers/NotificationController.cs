using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using TwitterApi.Services.NotificationService;
using TwitterApi.Models;


namespace TwitterApi.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class NotificationController(
        UserManager<TwitterUser> userManager,
        ILogger<NotificationController> logger,
        NotificationService notificationService
        ) : ControllerBase
    {
        private readonly UserManager<TwitterUser> _userManager = userManager;
        private readonly ILogger<NotificationController> _logger = logger;
        private readonly NotificationService _notificationService = notificationService;


        [HttpGet]
        public async Task<IActionResult> GetNotifications()
        {
            try {
                if (User.Identity == null || User.Identity.Name == null)
                {
                    return Unauthorized();
                }
                var user = await _userManager.FindByNameAsync(User.Identity.Name);

                if (user == null)
                {
                    return NotFound();
                }

                var notifications = await _notificationService.FetchNotifications(user.Id);

                return Ok(notifications);
            } catch (Exception e) {
                _logger.LogError(e, "Error fetching notifications");
                return StatusCode(500, "Error fetching notifications");
            }
        }
    }
}