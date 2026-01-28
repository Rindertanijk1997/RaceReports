using Microsoft.AspNetCore.Mvc;
using RaceReports.Data.Services;

namespace RaceReports.Controllers;

[ApiController]
[Route("api/categories")]
public class CategoriesController : ControllerBase
{
    private readonly CategoriesService _categoriesService;

    // ✅ Service injection (ingen DbContext i controller)
    public CategoriesController(CategoriesService categoriesService)
    {
        _categoriesService = categoriesService;
    }

    // GET: api/categories
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var categories = await _categoriesService.GetAllAsync();
        return Ok(categories);
    }
}
