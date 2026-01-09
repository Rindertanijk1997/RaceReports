using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceReports.Data;
using RaceReports.Data.DTOs;
using RaceReports.Data.Entities;

namespace RaceReports.Controllers;

[ApiController]
[Route("api/comments")]
public class CommentsController : ControllerBase
{
    private readonly RaceReportsContext _context;

    public CommentsController(RaceReportsContext context)
    {
        _context = context;
    }

    // POST: api/comments
    // Inloggad user kan kommentera andras inlägg (inte sina egna)
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CommentCreateDto dto)
    {
        // 1) Kontrollera att användaren finns
        var userExists = await _context.Users.AnyAsync(u => u.Id == dto.UserId);
        if (!userExists)
            return Unauthorized("Ogiltig användare.");

        // 2) Hämta inlägget som ska kommenteras
        var report = await _context.RaceReports.FirstOrDefaultAsync(r => r.Id == dto.RaceReportId);
        if (report is null)
            return NotFound("Inlägget hittades inte.");

        // 3) Stoppa kommentar på eget inlägg (KRAV)
        if (report.UserId == dto.UserId)
            return BadRequest("Du kan inte kommentera ditt eget inlägg.");

        // 4) Skapa kommentaren
        var comment = new Comment
        {
            UserId = dto.UserId,
            RaceReportId = dto.RaceReportId,
            Text = dto.Text
        };

        _context.Comments.Add(comment);
        await _context.SaveChangesAsync();

        // Returnera 201 + kommentaren
        return CreatedAtAction(nameof(GetById), new { id = comment.Id }, new
        {
            comment.Id,
            comment.Text,
            comment.CreatedAt,
            comment.UserId,
            comment.RaceReportId
        });
    }

    // GET: api/comments/report/5
    // Alla kan läsa kommentarer för ett specifikt inlägg (bra för test)
    [HttpGet("report/{reportId:int}")]
    public async Task<IActionResult> GetByReport(int reportId)
    {
        var comments = await _context.Comments
            .Where(c => c.RaceReportId == reportId)
            .Include(c => c.User)
            .OrderBy(c => c.CreatedAt)
            .Select(c => new
            {
                c.Id,
                c.Text,
                c.CreatedAt,
                User = c.User!.Username,
                c.UserId,
                c.RaceReportId
            })
            .ToListAsync();

        return Ok(comments);
    }

    // GET: api/comments/10
    // Hjälpmetod för CreatedAtAction
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var comment = await _context.Comments
            .Include(c => c.User)
            .Where(c => c.Id == id)
            .Select(c => new
            {
                c.Id,
                c.Text,
                c.CreatedAt,
                User = c.User!.Username,
                c.UserId,
                c.RaceReportId
            })
            .FirstOrDefaultAsync();

        if (comment is null)
            return NotFound();

        return Ok(comment);
    }
}
