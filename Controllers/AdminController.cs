using Microsoft.AspNetCore.Mvc;
using BloemFinder.API.Services;

namespace BloemFinder.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AdminController : ControllerBase
{
    private readonly SupabaseService _supabase;

    public AdminController(SupabaseService supabase)
    {
        _supabase = supabase;
    }

    [HttpGet("pending")]
    public async Task<IActionResult> GetPendingProperties()
    {
        var properties = await _supabase.GetPendingPropertiesAsync();
        return Ok(properties);
    }

    [HttpGet("reports")]
    public async Task<IActionResult> GetAllReports()
    {
        var reports = await _supabase.GetReportsAsync();
        return Ok(reports);
    }

    [HttpPut("approve/{id}")]
    public async Task<IActionResult> ApproveProperty(Guid id)
    {
        var updates = new { is_verified = true };
        var success = await _supabase.UpdatePropertyAsync(id, updates);

        if (success)
            return Ok(new { message = "Property approved" });

        return BadRequest(new { error = "Failed to approve" });
    }
}