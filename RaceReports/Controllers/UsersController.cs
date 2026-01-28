using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RaceReports.Data.DTOs;
using RaceReports.Data.Services;
using System.Security.Claims;

namespace RaceReports.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly UserService _userService;
    private readonly JwtService _jwtService;

    public UsersController(UserService userService, JwtService jwtService)
    {
        _userService = userService;
        _jwtService = jwtService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserRegisterDto dto)
    {
        try
        {
            var user = await _userService.RegisterAsync(dto);

            return CreatedAtAction(
                nameof(GetById),
                new { id = user.Id },
                new
                {
                    message = "Användare skapad",
                    data = new
                    {
                        user.Id,
                        user.Username,
                        user.Email
                    }
                });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserLoginDto dto)
    {
        var user = await _userService.ValidateLoginAsync(dto);

        if (user is null)
            return Unauthorized(new { message = "Fel username eller lösenord." });

        var token = _jwtService.CreateJwt(user);

        return Ok(new
        {
            message = "Inloggning lyckades",
            data = new UserLoginResponseDto
            {
                UserId = user.Id,
                Username = user.Username,
                Token = token
            }
        });
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var user = await _userService.GetByIdAsync(id);

        if (user is null)
            return NotFound(new { message = "User hittades inte." });

        return Ok(new
        {
            message = "User hämtad",
            data = user
        });
    }

    [Authorize]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UserUpdateDto dto)
    {
        var userIdFromToken =
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        if (userIdFromToken != id)
            return Forbid("Du kan bara uppdatera ditt eget konto.");

        try
        {
            var updatedUser = await _userService.UpdateAsync(id, dto);

            return Ok(new
            {
                message = "Kontot har uppdaterats",
                data = updatedUser
            });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [Authorize]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var userIdFromToken =
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        if (userIdFromToken != id)
            return Forbid("Du kan bara ta bort ditt eget konto.");

        try
        {
            await _userService.DeleteAsync(id);

            return Ok(new
            {
                message = "Kontot har raderats",
                userId = id
            });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}
