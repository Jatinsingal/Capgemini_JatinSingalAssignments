using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CosmosDB_Demo.Data;
using CosmosDB_Demo.Helpers;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace CosmosDB_Demo.Controllers
{
    public class AuthController : Controller
    {
        private readonly JwtService _jwtService;
        private readonly QrService _qrService;
        private readonly CosmosDbService _cosmosDbService;
        private readonly IConfiguration _configuration;

        public AuthController(
            JwtService jwtService,
            QrService qrService,
            CosmosDbService cosmosDbService,
            IConfiguration configuration)
        {
            _jwtService = jwtService;
            _qrService = qrService;
            _cosmosDbService = cosmosDbService;
            _configuration = configuration;
        }

        public IActionResult GenerateQr()
        {
            var token = _jwtService.GenerateToken("user123");
            var publicBaseUrl = _configuration["AppUrls:PublicBaseUrl"];
            var url = !string.IsNullOrWhiteSpace(publicBaseUrl)
                ? QueryHelpers.AddQueryString(
                    $"{publicBaseUrl.TrimEnd('/')}/Auth/LoginWithToken",
                    "token",
                    token)
                : Url.Action(nameof(LoginWithToken), "Auth", new { token }, Request.Scheme);

            if (string.IsNullOrWhiteSpace(url))
            {
                return StatusCode(500, "Unable to generate the QR login URL.");
            }

            ViewBag.Url = url;
            return View();
        }

        public IActionResult GetQrImage(string url)
        {
            var qrBytes = _qrService.GenerateQrCode(url);
            return File(qrBytes, "image/png");
        }

        [HttpGet]
        public async Task<IActionResult> LoginWithToken(string token)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(token))
                {
                    return BadRequest("Token is required.");
                }

                var handler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!);
                var principal = handler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = _configuration["Jwt:Issuer"],
                    ValidAudience = _configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ClockSkew = TimeSpan.Zero
                }, out _);

                var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value
                    ?? principal.Claims.FirstOrDefault(c => c.Type.Contains("nameidentifier"))?.Value;

                var items = await _cosmosDbService.GetItemsAsync("SELECT * FROM c");

                ViewBag.UserId = userId;
                return View("ItemsData", items);
            }
            catch
            {
                return Content("Invalid Token");
            }
        }
    }
}
