using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceReports.Data;

namespace RaceReports.Controllers;

[ApiController]
[Route("api/categories")]
public class CategoriesController : ControllerBase
{
    private readonly RaceReportsContext _context;

    public CategoriesController(RaceReportsContext context)
    {
        _context = context;
    }

    // GET: api/categories
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var categories = await _context.Categories.ToListAsync();
        return Ok(categories);
    }
}
