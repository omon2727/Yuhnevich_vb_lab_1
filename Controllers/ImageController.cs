using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Yuhnevich_vb_lab.Data;

namespace Yuhnevich_vb_lab.Controllers
{
    public class ImageController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IWebHostEnvironment _env;

        public ImageController(UserManager<AppUser> userManager, IWebHostEnvironment env)
        {
            _userManager = userManager;
            _env = env;
        }

        [HttpGet]
        [Route("Image/GetAvatar")]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<IActionResult> GetAvatar()
        {
            try
            {
                if (!User.Identity.IsAuthenticated)
                {
                    var defaultAvatarPath = Path.Combine(_env.WebRootPath, "images", "image.png");
                    if (!System.IO.File.Exists(defaultAvatarPath))
                    {
                        throw new FileNotFoundException("Default avatar not found.", defaultAvatarPath);
                    }
                    return PhysicalFile(defaultAvatarPath, "image/png");
                }

                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    var defaultAvatarPath = Path.Combine(_env.WebRootPath, "images", "image.png");
                    if (!System.IO.File.Exists(defaultAvatarPath))
                    {
                        throw new FileNotFoundException("Default avatar not found.", defaultAvatarPath);
                    }
                    return PhysicalFile(defaultAvatarPath, "image/png");
                }

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    var defaultAvatarPath = Path.Combine(_env.WebRootPath, "images", "image.png");
                    if (!System.IO.File.Exists(defaultAvatarPath))
                    {
                        throw new FileNotFoundException("Default avatar not found.", defaultAvatarPath);
                    }
                    return PhysicalFile(defaultAvatarPath, "image/png");
                }

                if (user.Avatar == null || user.MimeType == null)
                {
                    var defaultAvatarPath = Path.Combine(_env.WebRootPath, "images", "image.png");
                    if (!System.IO.File.Exists(defaultAvatarPath))
                    {
                        throw new FileNotFoundException("Default avatar not found.", defaultAvatarPath);
                    }
                    return PhysicalFile(defaultAvatarPath, "image/png");
                }

                return File(user.Avatar, user.MimeType);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        [HttpGet]
        [Route("Image/TestAvatar/{userId}")]
        public async Task<IActionResult> TestAvatar(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null || user.Avatar == null || user.MimeType == null)
                {
                    var defaultAvatarPath = Path.Combine(_env.WebRootPath, "images", "image.png");
                    if (!System.IO.File.Exists(defaultAvatarPath))
                    {
                        throw new FileNotFoundException("Default avatar not found.", defaultAvatarPath);
                    }
                    return PhysicalFile(defaultAvatarPath, "image/png");
                }
                return File(user.Avatar, user.MimeType);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        [HttpGet]
        [Route("Image/TestUserManager/{userId}")]
        public async Task<IActionResult> TestUserManager(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return NotFound($"User with ID {userId} not found");
                }
                return Ok($"User: {user.UserName}, AvatarLength: {user.Avatar?.Length ?? 0}, MimeType: {user.MimeType}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        [HttpGet]
        [Route("Image/DebugAvatar/{userId}")]
        public async Task<IActionResult> DebugAvatar(string userId, [FromServices] ApplicationDbContext dbContext)
        {
            try
            {
                var user = await dbContext.Users.FindAsync(userId);
                if (user == null || user.Avatar == null || user.MimeType == null)
                {
                    var defaultAvatarPath = Path.Combine(_env.WebRootPath, "images", "image.png");
                    if (!System.IO.File.Exists(defaultAvatarPath))
                    {
                        throw new FileNotFoundException("Default avatar not found.", defaultAvatarPath);
                    }
                    return PhysicalFile(defaultAvatarPath, "image/png");
                }
                return File(user.Avatar, user.MimeType);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        [HttpGet]
        [Route("Image/Test")]
        public IActionResult Test()
        {
            return Ok("ImageController is accessible");
        }

        [HttpGet]
        [Route("Image/TestAuth")]
        public IActionResult TestAuth()
        {
            return Ok($"IsAuthenticated: {User.Identity?.IsAuthenticated}, User: {User.Identity?.Name}");
        }

        [HttpGet]
        [Route("Image/TestGetUserAsync")]
        public async Task<IActionResult> TestGetUserAsync()
        {
            try
            {
                if (!User.Identity.IsAuthenticated)
                {
                    return Unauthorized("User is not authenticated.");
                }

                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return NotFound("User not found.");
                }

                return Ok($"User: {user.UserName}, UserId: {user.Id}, AvatarLength: {user.Avatar?.Length ?? 0}, MimeType: {user.MimeType}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        public IActionResult Index()
        {
            return Ok("ImageController Index is accessible");
        }
    }
}