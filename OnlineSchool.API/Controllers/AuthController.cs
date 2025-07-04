using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using OnlineSchool.Shared.DTOs;
using OnlineSchool.Shared.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace OnlineSchool.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;

        public AuthController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            var userExists = await _userManager.FindByEmailAsync(registerDto.Email);
            if (userExists != null)
            {
                return BadRequest("User with this email already exists.");
            }

            var user = new ApplicationUser
            {
                Email = registerDto.Email,
                UserName = registerDto.Email, // Используем Email как UserName
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            // По умолчанию все новые пользователи - студенты
            await _userManager.AddToRoleAsync(user, "Student");

            return Ok("User created successfully!");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, loginDto.Password))
            {
                return Unauthorized("Invalid credentials.");
            }

            var userRoles = await _userManager.GetRolesAsync(user);

            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Email),
               new Claim(ClaimTypes.NameIdentifier, user.Id),
               new Claim(ClaimTypes.Email, user.Email),
               new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            foreach (var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                expires: DateTime.Now.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expiration = token.ValidTo
            });
        }

        [HttpPost("make-admin")]
        public async Task<IActionResult> MakeAdmin([FromBody] string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Убедимся, что роли существуют
            if (!await _roleManager.RoleExistsAsync("Admin"))
                await _roleManager.CreateAsync(new IdentityRole("Admin"));

            if (!await _roleManager.RoleExistsAsync("Student"))
                await _roleManager.CreateAsync(new IdentityRole("Student"));

            // Убираем роль студента и добавляем роль админа
            await _userManager.RemoveFromRoleAsync(user, "Student");
            await _userManager.AddToRoleAsync(user, "Admin");

            return Ok($"User {user.Email} is now an Admin.");
        }
    }
}