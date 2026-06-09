using Microsoft.AspNetCore.Mvc;
using BloemFinder.API.Services;

namespace BloemFinder.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PropertiesController : ControllerBase
{
    private readonly SupabaseService _supabase;

    public PropertiesController(SupabaseService supabase)
    {
        _supabase = supabase;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var properties = await _supabase.GetPropertiesAsync();
        return Ok(properties);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var property = await _supabase.GetPropertyByIdAsync(id);

        if (property == null)
            return NotFound();

        return Ok(property);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] object property)
    {
        var success = await _supabase.CreatePropertyAsync(property);

        if (success)
            return Ok(new { message = "Property created successfully" });

        return BadRequest(new { error = "Failed to create property" });
    }

    [HttpPut("{id}/approve")]
    public async Task<IActionResult> Approve(Guid id)
    {
        var updates = new { is_verified = true };
        var success = await _supabase.UpdatePropertyAsync(id, updates);

        if (success)
            return Ok(new { message = "Property approved" });

        return BadRequest(new { error = "Failed to approve" });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var success = await _supabase.DeletePropertyAsync(id);

        if (success)
            return Ok(new { message = "Property deleted" });

        return BadRequest(new { error = "Failed to delete" });
    }
}