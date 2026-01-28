using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RaceReports.Data.DTOs;
using RaceReports.Data.Services;
using System.Security.Claims;

namespace RaceReports.Controllers;

[ApiController]
[Route("api/comments")]
public class CommentsController : ControllerBase
{
    private readonly CommentsService _commentsService;

    // ✅ Service injection (INGEN DbContext)
    public CommentsController(CommentsService commentsService)
    {
        _commentsService = commentsService;
    }

    // ============================
    // POST: api/comments
    // ============================
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CommentCreateDto dto)
    {
        var userIdFromToken =
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        try
        {
            var comment = await _commentsService.CreateAsync(dto, userIdFromToken);

            return CreatedAtAction(nameof(GetById), new { id = comment.Id }, comment);
        }
        catch (InvalidOperationException ex)
        {
            // t.ex. ogiltig användare / eget inlägg / report saknas
            return BadRequest(ex.Message);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    // ============================
    // GET: api/comments/report/5
    // ============================
    [HttpGet("report/{reportId:int}")]
    public async Task<IActionResult> GetByReport(int reportId)
    {
        var comments = await _commentsService.GetByReportAsync(reportId);
        return Ok(comments);
    }

    // ============================
    // GET: api/comments/{id}
    // ============================
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var comment = await _commentsService.GetByIdAsync(id);

        if (comment is null)
            return NotFound();

        return Ok(comment);
    }
}
