using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TwitterApi.DTO;
using TwitterApi.Models;
using TwitterApi.Database;
using System.Linq.Expressions;
using System.ComponentModel.DataAnnotations;
// using TwitterApi.Attributes;
using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;



namespace TwitterApi.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly ILogger<AccountController> _logger;
        private readonly IConfiguration _configuration;
        private readonly UserManager<TwitterUser> _userManager;
        private readonly SignInManager<TwitterUser> _signInManager;

        public AccountController(DataContext context, ILogger<AccountController> logger, IConfiguration configuration, UserManager<TwitterUser> userManager, SignInManager<TwitterUser> signInManager)
        {
            _context = context;
            _logger = logger;
            _configuration = configuration;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpPost]
        [ResponseCache(CacheProfileName = "NoCache")]
        public async Task<ActionResult> Register(RegisterDTO input)
        {
            try {
                if (ModelState.IsValid)
                {
                    var newUser = new TwitterUser
                    {
                        UserName = input.username,
                        Email = input.email,
                        FullName = input.fullName,
                        Bio = input.bio,
                        Location = input.location,
                        IsVerified = false,
                        FollowersCount = 0,
                        FollowingCount = 0,
                        TweetsCount = 0
                    };
                    var result = await _userManager.CreateAsync(newUser, input.password);
                    if (result.Succeeded)
                    {
                        _logger.LogInformation(
                            $"User {input.username} has been registered successfully."

                        );
                        return StatusCode(201,
                            new
                            {
                                message = "User has been registered successfully.",
                                user = newUser
                            }
                        );
                    } else {
                        return StatusCode(500,
                            new
                            {
                                message = "User has failed to register.",
                                errors = result.Errors
                            }
                        );
                    }   
                } else {
                    return StatusCode(400,
                        new
                        {
                            message = "User has failed to register.",
                            errors = ModelState.Values.SelectMany(v => v.Errors)
                        }
                    );
                }
            } catch (Exception e)
            {
                return StatusCode(500,
                    new
                    {
                        message = "User has failed to register.",
                        errors = e.Message
                    }
                );
            }
        }

        [HttpPost]
        [ResponseCache(CacheProfileName = "NoCache")]
        public async Task<ActionResult> Login()
        {
            throw new NotImplementedException();
        }
    }
}