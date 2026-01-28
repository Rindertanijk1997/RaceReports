using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RaceReports.Data.DTOs;
using RaceReports.Data.Services;
using System.Security.Claims;

namespace RaceReports.Controllers;

[ApiController]
[Route("api/reports")]
public class ReportsController : ControllerBase
{
    private readonly ReportsService _reportsService;

    public ReportsController(ReportsService reportsService)
    {
        _reportsService = reportsService;
    }

    // GET: api/reports
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var reports = await _reportsService.GetAllAsync();
        return Ok(new
        {
            message = "Inlägg hämtade",
            data = reports
        });
    }

    // GET: api/reports/{id}
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var report = await _reportsService.GetByIdAsync(id);
        if (report is null)
            return NotFound(new { message = "Inlägget hittades inte" });

        return Ok(new
        {
            message = "Inlägg hämtat",
            data = report
        });
    }

    // POST: api/reports
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] RaceReportCreateDto dto)
    {
        var userIdFromToken =
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        try
        {
            var report = await _reportsService.CreateAsync(dto, userIdFromToken);

            return CreatedAtAction(
                nameof(GetById),
                new { id = report.Id },
                new
                {
                    message = "Inlägg skapat",
                    data = report
                });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // PUT: api/reports/{id}
    [Authorize]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] RaceReportUpdateDto dto)
    {
        var userIdFromToken =
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        try
        {
            var updated = await _reportsService.UpdateAsync(id, dto, userIdFromToken);

            return Ok(new
            {
                message = "Inlägget har uppdaterats",
                data = updated
            });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    // DELETE: api/reports/{id}
    [Authorize]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var userIdFromToken =
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        try
        {
            await _reportsService.DeleteAsync(id, userIdFromToken);

            return Ok(new
            {
                message = "Inlägget har raderats",
                reportId = id
            });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    // GET: api/reports/search
    [HttpGet("search")]
    public async Task<IActionResult> Search(
        [FromQuery] string? title,
        [FromQuery] int? categoryId)
    {
        var results = await _reportsService.SearchAsync(title, categoryId);

        return Ok(new
        {
            message = "Sökresultat",
            data = results
        });
    }

    // GET: api/reports/mine
    [Authorize]
    [HttpGet("mine")]
    public async Task<IActionResult> GetMine()
    {
        var userIdFromToken =
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var reports = await _reportsService.GetMineAsync(userIdFromToken);

        return Ok(new
        {
            message = "Dina inlägg",
            data = reports
        });
    }
}
