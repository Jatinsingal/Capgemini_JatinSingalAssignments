using InventoryManagement.API.Data;
using InventoryManagement.API.DTOs;
using InventoryManagement.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace InventoryManagement.API.Controllers
{
    [Route("api")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private static readonly string[] AllowedRoles = ["Admin", "InventoryManager"];

        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;

        public AuthController(ApplicationDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [HttpPost("register")]
        public IActionResult Register(RegisterModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var email = model.Email.Trim().ToLowerInvariant();
            var role = model.UserRole.Trim();

            if (!AllowedRoles.Contains(role))
                return BadRequest("User role must be Admin or InventoryManager");

            if (_context.Users.Any(x => x.Email == email))
                return Conflict("Email is already registered");

            var user = new User
            {
                Email = email,
                Password = model.Password,
                Username = model.Username.Trim(),
                MobileNumber = model.MobileNumber.Trim(),
                UserRole = role
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            return Ok(new
            {
                message = "User registered successfully",
                user = new
                {
                    user.UserId,
                    user.Email,
                    user.Username,
                    user.MobileNumber,
                    user.UserRole
                }
            });
        }

        [HttpPost("login")]
        public IActionResult Login(LoginModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var email = model.Email.Trim().ToLowerInvariant();

            var user = _context.Users
                .FirstOrDefault(x => x.Email == email && x.Password == model.Password);

            if (user == null)
                return Unauthorized("Invalid credentials");

            var token = GenerateToken(user);

            return Ok(new
            {
                token,
                user = new
                {
                    user.UserId,
                    user.Email,
                    user.Username,
                    user.MobileNumber,
                    user.UserRole
                }
            });
        }

        [Authorize]
        [HttpGet("me")]
        public IActionResult GetCurrentUser()
        {
            var email = User.Identity?.Name;

            if (string.IsNullOrWhiteSpace(email))
                return Unauthorized();

            var user = _context.Users
                .Where(x => x.Email == email)
                .Select(x => new
                {
                    x.UserId,
                    x.Email,
                    x.Username,
                    x.MobileNumber,
                    x.UserRole
                })
                .FirstOrDefault();

            return user == null ? NotFound("User not found") : Ok(user);
        }

        private string GenerateToken(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Email ?? string.Empty),
                new Claim(ClaimTypes.Role, user.UserRole ?? string.Empty),
                new Claim("Username", user.Username ?? string.Empty),
                new Claim("UserId", user.UserId.ToString())
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(2),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
