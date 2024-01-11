using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TwitterApi.DTO;
using TwitterApi.Models;
using TwitterApi.Database;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;



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

        public AccountController(DataContext context, ILogger<AccountController> logger, IConfiguration configuration, UserManager<TwitterUser> userManager, SignInManager<TwitterUser> signInManager)
        {
            _context = context;
            _logger = logger;
            _configuration = configuration;
            _userManager = userManager;
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
                        await _userManager.AddToRoleAsync(newUser, "User");
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
        public async Task<ActionResult> Login(LoginDTO input)
        {
            try {
                if (ModelState.IsValid)
                {
                    var user = await _userManager.FindByNameAsync(input.username);
                    if (user == null
                        || !await _userManager.CheckPasswordAsync(user, input.password))
                    {
                        return StatusCode(400,
                            new
                            {
                                message = "User has failed to login.",
                                errors = "Invalid username or password."
                            }
                        );
                    } else {
                        var signingCredentials = new SigningCredentials(
                            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? string.Empty)),
                            SecurityAlgorithms.HmacSha256
                        );
                        var claims = new List<Claim>
                        {
                            new(ClaimTypes.Name, user.UserName ?? string.Empty)
                        };
                        claims.AddRange((await _userManager.GetRolesAsync(user)).Select(role => new Claim(ClaimTypes.Role, role)));

                        var jwtObject = new JwtSecurityToken(
                            issuer: _configuration["Jwt:Issuer"],
                            audience: _configuration["Jwt:Audience"],
                            claims: claims,
                            expires: DateTime.Now.AddDays(7),
                            signingCredentials: signingCredentials
                        );

                        var jwtString = new JwtSecurityTokenHandler().WriteToken(jwtObject);

                        return StatusCode(200,
                            new
                            {
                                message = "User has been logged in successfully.",
                                token = jwtString
                            }
                        );
                    }
                } else {
                    var details = new ValidationProblemDetails(ModelState)
                    {
                        Instance = HttpContext.Request.Path,
                        Status = StatusCodes.Status400BadRequest,
                        Type = "https://httpstatuses.com/400",
                        Detail = "Please refer to the errors property for additional details."
                    };
                    return StatusCode(400, details);
                }
            } catch (Exception e)
            {
                return StatusCode(500,
                    new
                    {
                        message = "User has failed to login.",
                        errors = e.Message
                    }
                );
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetUsers([FromQuery] string? search = null)
        {
            try {
                var authenticatedUser = await _userManager.FindByNameAsync(User?.Identity?.Name ?? string.Empty);
                if (authenticatedUser != null)
                {
                    var users = _context.Users.Where(u => u.Id != authenticatedUser.Id).AsQueryable();
                    if (search != null)
                    {
                        users = users.Where(u => u != null && 
                            ((u.UserName != null && u.UserName.Contains(search)) || 
                            (u.FullName != null && u.FullName.Contains(search))));
                    }
                    users = users.OrderByDescending(u => u.FollowersCount);
                    return StatusCode(200,
                        new
                        {
                            message = "Users have been retrieved successfully.",
                            count = users.Count(),
                            users = await users.ToListAsync()
                        }
                    );
                }
                else
                {
                    return StatusCode(401,
                        new
                        {
                            message = "User authentication failed.",
                            errors = "Invalid user."
                        }
                    );
                }
            } catch (Exception e)
            {
                return StatusCode(500,
                    new
                    {
                        message = "Users have failed to be retrieved.",
                        errors = e.Message
                    }
                );
            }
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetUser(string id)
        {
            try {
                var user = await _context.Users.FindAsync(id);
                if (user != null)
                {
                    return StatusCode(200,
                        new
                        {
                            message = "User has been retrieved successfully.",
                            user
                        }
                    );
                }
                else
                {
                    return StatusCode(404,
                        new
                        {
                            message = "User has failed to be retrieved.",
                            errors = "Invalid user."
                        }
                    );
                }
            } catch (Exception e)
            {
                return StatusCode(500,
                    new
                    {
                        message = "User has failed to be retrieved.",
                        errors = e.Message
                    }
                );
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAuthenticatedUser()
        {
            try {
                var authenticatedUser = await _userManager.FindByNameAsync(User?.Identity?.Name ?? string.Empty);
                if (authenticatedUser != null)
                {
                    return StatusCode(200,
                        new
                        {
                            message = "User has been retrieved successfully.",
                            user = authenticatedUser
                        }
                    );
                }
                else
                {
                    return StatusCode(401,
                        new
                        {
                            message = "User authentication failed.",
                            errors = "Invalid user."
                        }
                    );
                }
            } catch (Exception e)
            {
                return StatusCode(500,
                    new
                    {
                        message = "User has failed to be retrieved.",
                        errors = e.Message
                    }
                );
            }
        }
    }
}