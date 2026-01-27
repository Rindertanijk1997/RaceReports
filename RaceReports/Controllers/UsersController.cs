using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RaceReports.Data;
using RaceReports.Data.DTOs;
using RaceReports.Data.Entities;
using RaceReports.Data.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RaceReports.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly RaceReportsContext _context;
    private readonly IConfiguration _config;

    public UsersController(RaceReportsContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }

    // POST: api/users/register
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserRegisterDto dto)
    {
        if (await _context.Users.AnyAsync(u => u.Username == dto.Username))
            return Conflict("Username är redan upptaget.");

        if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
            return Conflict("Email är redan registrerad.");

        var user = new User
        {
            Username = dto.Username,
            Email = dto.Email,
            PasswordHash = PasswordService.HashPassword(dto.Password)
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = user.Id }, new
        {
            user.Id,
            user.Username,
            user.Email
        });
    }

    // POST: api/users/login
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserLoginDto dto)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == dto.Username);
        if (user is null)
            return Unauthorized("Fel username eller lösenord.");

        if (!PasswordService.VerifyPassword(dto.Password, user.PasswordHash))
            return Unauthorized("Fel username eller lösenord.");

        var token = CreateJwt(user);

        var response = new UserLoginResponseDto
        {
            UserId = user.Id,
            Username = user.Username,
            Token = token
        };

        return Ok(response);
    }

    // GET: api/users/{id}
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var user = await _context.Users
            .Select(u => new { u.Id, u.Username, u.Email })
            .FirstOrDefaultAsync(u => u.Id == id);

        if (user is null)
            return NotFound("User hittades inte.");

        return Ok(user);
    }

    // PUT: api/users/{id}
    [Authorize]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UserUpdateDto dto)
    {
        var userIdFromToken = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        if (userIdFromToken != id)
            return Forbid("Du kan bara uppdatera ditt eget konto.");

        var user = await _context.Users.FindAsync(id);
        if (user is null)
            return NotFound("User hittades inte.");

        user.Username = dto.Username;
        user.Email = dto.Email;

        if (!string.IsNullOrWhiteSpace(dto.NewPassword))
            user.PasswordHash = PasswordService.HashPassword(dto.NewPassword);

        await _context.SaveChangesAsync();

        return Ok(new { user.Id, user.Username, user.Email });
    }

    // DELETE: api/users/{id}
    [Authorize]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var userIdFromToken = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        if (userIdFromToken != id)
            return Forbid("Du kan bara ta bort ditt eget konto.");

        // Ladda user + beroenden som annars stoppar DELETE via FK constraints
        var user = await _context.Users
            .Include(u => u.RaceReports)
            .Include(u => u.Comments)
            .FirstOrDefaultAsync(u => u.Id == id);

        if (user is null)
            return NotFound("User hittades inte.");

        // 1) Ta bort kommentarer som användaren skrivit
        if (user.Comments.Count > 0)
            _context.Comments.RemoveRange(user.Comments);

        // 2) Ta bort kommentarer på användarens reports (andra kan ha kommenterat dem)
        if (user.RaceReports.Count > 0)
        {
            var reportIds = user.RaceReports.Select(r => r.Id).ToList();

            var commentsOnUsersReports = await _context.Comments
                .Where(c => reportIds.Contains(c.RaceReportId))
                .ToListAsync();

            if (commentsOnUsersReports.Count > 0)
                _context.Comments.RemoveRange(commentsOnUsersReports);

            _context.RaceReports.RemoveRange(user.RaceReports);
        }

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "Kontot har raderats"
        });

    }

    // JWT
    private string CreateJwt(User user)
    {
        var key = _config["Jwt:Key"]!;
        var issuer = _config["Jwt:Issuer"]!;
        var audience = _config["Jwt:Audience"]!;

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username)
        };

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var creds = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
