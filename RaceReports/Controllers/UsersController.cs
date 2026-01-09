using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceReports.Data;
using RaceReports.Data.DTOs;
using RaceReports.Data.Entities;
using RaceReports.Data.Services;

namespace RaceReports.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly RaceReportsContext _context;

    public UsersController(RaceReportsContext context)
    {
        _context = context;
    }

    // POST: api/users/register
    // Skapar ett konto med username + password + email
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserRegisterDto dto)
    {
        // Kontroll: username eller email får inte redan finnas
        var usernameExists = await _context.Users.AnyAsync(u => u.Username == dto.Username);
        if (usernameExists)
            return Conflict("Username är redan upptaget.");

        var emailExists = await _context.Users.AnyAsync(u => u.Email == dto.Email);
        if (emailExists)
            return Conflict("Email är redan registrerad.");

        // Skapa user-entity och spara hashat lösenord
        var user = new User
        {
            Username = dto.Username,
            Email = dto.Email,
            PasswordHash = PasswordService.HashPassword(dto.Password)
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Returnera 201 + lite info (inte password)
        return CreatedAtAction(nameof(GetById), new { id = user.Id }, new
        {
            user.Id,
            user.Username,
            user.Email
        });
    }

    // POST: api/users/login
    // Krav: returnera userId när user loggar in
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserLoginDto dto)
    {
        // Hämta användaren via username
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == dto.Username);
        if (user is null)
            return Unauthorized("Fel username eller lösenord.");

        // Kontrollera lösenord
        var ok = PasswordService.VerifyPassword(dto.Password, user.PasswordHash);
        if (!ok)
            return Unauthorized("Fel username eller lösenord.");

        // Returnera userId (krav)
        var response = new UserLoginResponseDto
        {
            UserId = user.Id,
            Username = user.Username
        };

        return Ok(response);
    }

    // GET: api/users/{id}
    // Inte ett krav men bra för test/debug + CreatedAtAction
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById([FromRoute] int id)
    {
        var user = await _context.Users
            .Select(u => new { u.Id, u.Username, u.Email })
            .FirstOrDefaultAsync(u => u.Id == id);

        if (user is null)
            return NotFound("User hittades inte.");

        return Ok(user);
    }

    // PUT: api/users/{id}
    // Krav: kunna uppdatera konto
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UserUpdateDto dto)
    {
        // Krav-variant utan JWT: "inloggad userId" måste matcha kontot som uppdateras
        if (dto.UserId != id)
            return Forbid("Du kan bara uppdatera ditt eget konto.");

        var user = await _context.Users.FindAsync(id);
        if (user is null)
            return NotFound("User hittades inte.");

        // Om någon försöker byta till ett username som redan finns
        var usernameTaken = await _context.Users.AnyAsync(u => u.Username == dto.Username && u.Id != id);
        if (usernameTaken)
            return Conflict("Username är redan upptaget.");

        // Om någon försöker byta till en email som redan finns
        var emailTaken = await _context.Users.AnyAsync(u => u.Email == dto.Email && u.Id != id);
        if (emailTaken)
            return Conflict("Email är redan registrerad.");

        // Uppdatera fälten
        user.Username = dto.Username;
        user.Email = dto.Email;

        // Om man skickar nytt lösenord → uppdatera hash
        if (!string.IsNullOrWhiteSpace(dto.NewPassword))
        {
            user.PasswordHash = PasswordService.HashPassword(dto.NewPassword);
        }

        await _context.SaveChangesAsync();

        return Ok(new { user.Id, user.Username, user.Email });
    }

    // DELETE: api/users/{id}
    // Krav: kunna ta bort konto
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete([FromRoute] int id, [FromQuery] int userId)
    {
        // Krav-variant utan JWT: bara inloggad user kan ta bort sig själv
        if (userId != id)
            return Forbid("Du kan bara ta bort ditt eget konto.");

        var user = await _context.Users.FindAsync(id);
        if (user is null)
            return NotFound("User hittades inte.");

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
