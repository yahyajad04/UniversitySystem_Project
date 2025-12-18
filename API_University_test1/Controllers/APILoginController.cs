using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace API_University_test1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class APILoginController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public APILoginController (IConfiguration configuration)
        {
            _configuration = configuration;
        }
        [HttpPost]
        public IActionResult Login([FromBody] User_Login request)
        {
            if(request.UserName == "admin" && request.Password == "Password")
            {
                var JwtSettings = _configuration.GetSection("JwtSettings");
                var secret_key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtSettings["Secret"]));
                var creds = new SigningCredentials(secret_key, SecurityAlgorithms.HmacSha256);

                var claims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, request.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };

                var token = new JwtSecurityToken(
                    issuer: JwtSettings["Issuer"],
                    audience: JwtSettings["Audience"],
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(30),
                    signingCredentials: creds
                    );
                return Ok( new {token = new JwtSecurityTokenHandler().WriteToken(token)});
            }
            return Unauthorized("Invalid Credentials");
        }
    }
}

public class User_Login
{
    public string? UserName { get; set; }
    public string? Password { get; set; }
}