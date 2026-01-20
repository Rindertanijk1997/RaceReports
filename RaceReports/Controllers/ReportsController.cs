using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceReports.Data;
using RaceReports.Data.DTOs;
using RaceReports.Data.Entities;

namespace RaceReports.Controllers;

[ApiController]
[Route("api/reports")]
public class ReportsController : ControllerBase
{
    private readonly RaceReportsContext _context;

    public ReportsController(RaceReportsContext context)
    {
        _context = context;
    }

    // GET: api/reports
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var reports = await _context.RaceReports
            .Include(r => r.User)
            .Include(r => r.Category)
            .Select(r => new
            {
                r.Id,
                r.Title,
                r.Text,
                r.CreatedAt,
                r.CategoryId,                 
                User = r.User!.Username,
                Category = r.Category!.Name
            })
            .ToListAsync();

        return Ok(reports);
    }


    // GET: api/reports/{id}
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var report = await _context.RaceReports
            .Where(r => r.Id == id)
            .Select(r => new
            {
                r.Id,
                r.Title,
                r.Text,
                r.CreatedAt,
                r.CategoryId,
                User = r.User!.Username,
                Category = r.Category!.Name,

                Comments = r.Comments
                    .OrderBy(c => c.CreatedAt)
                    .Select(c => new
                    {
                        c.Id,
                        c.Text,
                        c.CreatedAt,
                        User = c.User!.Username,
                        c.UserId
                    })
                    .ToList()
            })
            .FirstOrDefaultAsync();

        if (report == null)
            return NotFound();

        return Ok(report);
    }


    // POST: api/reports
    [HttpPost]
    public async Task<IActionResult> Create(RaceReportCreateDto dto)
    {
        // Kontrollera att user finns
        var userExists = await _context.Users.AnyAsync(u => u.Id == dto.UserId);
        if (!userExists)
            return Unauthorized("Ogiltig användare.");

        
        var input = dto.Category?.Trim();

        if (string.IsNullOrWhiteSpace(input))
            return BadRequest("Kategori krävs. Tillåtna värden: 5K, 10K, Halvmaraton, Maraton, Trail.");

        var category = await _context.Categories
            .FirstOrDefaultAsync(c => c.Name.ToLower() == input.ToLower());

        if (category is null)
            return BadRequest("Ogiltig kategori. Tillåtna värden: 5K, 10K, Halvmaraton, Maraton, Trail.");

        // Skapa report
        var report = new RaceReport
        {
            Title = dto.Title,
            Text = dto.Text,
            UserId = dto.UserId,
            CategoryId = category.Id
        };

        _context.RaceReports.Add(report);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = report.Id }, new
        {
            report.Id,
            report.Title,
            report.Text,
            report.CreatedAt,
            report.UserId,
            Category = category.Name
        });

    }


    // PUT: api/reports/{id}
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, RaceReportUpdateDto dto)
    {
        var report = await _context.RaceReports.FindAsync(id);
        if (report == null)
            return NotFound();

        if (report.UserId != dto.UserId)
            return StatusCode(403, "Du kan bara uppdatera dina egna inlägg.");

        report.Title = dto.Title;
        report.Text = dto.Text;
        report.CategoryId = dto.CategoryId;
        report.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok(report);
    }

    // DELETE: api/reports/{id}?userId=1
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, [FromQuery] int userId)
    {
        var report = await _context.RaceReports.FindAsync(id);
        if (report == null)
            return NotFound();

        if (report.UserId != userId)
            return StatusCode(403, "Du kan bara ta bort dina egna inlägg.");

        _context.RaceReports.Remove(report);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // GET: api/reports/search?title=varvet&categoryId=3
    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string? title, [FromQuery] int? categoryId)
    {
        var query = _context.RaceReports
            .Include(r => r.User)
            .Include(r => r.Category)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(title))
            query = query.Where(r => r.Title.Contains(title));

        if (categoryId.HasValue)
            query = query.Where(r => r.CategoryId == categoryId);

        var results = await query
            .Select(r => new
            {
                r.Id,
                r.Title,
                User = r.User!.Username,
                Category = r.Category!.Name
            })
            .ToListAsync();

        return Ok(results);
    }


    // GET: api/reports/mine?userId=2
    // Visar alla inlägg som tillhör en användare
    [HttpGet("mine")]
    public async Task<IActionResult> GetMine([FromQuery] int userId)
    {
        if (userId <= 0)
            return BadRequest("userId krävs.");

        // Kontrollera att användaren finns
        var userExists = await _context.Users.AnyAsync(u => u.Id == userId);
        if (!userExists)
            return Unauthorized("Ogiltig användare.");

        // Hämta bara inlägg som skapats av användaren
        var myReports = await _context.RaceReports
            .Where(r => r.UserId == userId)
            .Include(r => r.Category)
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => new
            {
                r.Id,
                r.Title,
                r.Text,
                r.CreatedAt,
                r.UpdatedAt,
                Category = r.Category!.Name,
                r.CategoryId
            })
            .ToListAsync();

        return Ok(myReports);
    }

}
