using Design.Service.Services;
using Dressed.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Design.Service.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DesignsController : ControllerBase
{
    private readonly IDesignService _designService;

    public DesignsController(IDesignService designService)
    {
        _designService = designService;
    }

    [HttpPost]
    [Authorize(Roles = "Designer")]
    public async Task<IActionResult> CreateDesign([FromBody] CreateDesignRequest request)
    {
        var designerIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(designerIdClaim))
            return Unauthorized();

        var designerId = int.Parse(designerIdClaim);
        var design = await _designService.CreateDesign(designerId, request);

        if (design == null)
            return BadRequest(new { message = "Failed to create design" });

        return CreatedAtAction(nameof(GetDesign), new { id = design.Id }, design);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetDesign(int id)
    {
        var design = await _designService.GetDesign(id);
        if (design == null)
            return NotFound();

        return Ok(design);
    }

    [HttpGet("designer/{designerId}")]
    [Authorize]
    public async Task<IActionResult> GetDesignerDesigns(int designerId)
    {
        var designs = await _designService.GetDesignerDesigns(designerId);
        return Ok(designs);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllDesigns([FromQuery] string? category = null)
    {
        var designs = await _designService.GetAllDesigns(category);
        return Ok(designs);
    }

    [HttpPatch("{id}/status")]
    [Authorize]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] string status)
    {
        var success = await _designService.UpdateDesignStatus(id, status);
        if (!success)
            return NotFound();

        return Ok(new { message = "Status updated successfully" });
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Designer")]
    public async Task<IActionResult> DeleteDesign(int id)
    {
        var success = await _designService.DeleteDesign(id);
        if (!success)
            return NotFound();

        return Ok(new { message = "Design deleted successfully" });
    }

    [HttpGet("health")]
    public IActionResult Health()
    {
        return Ok(new { status = "healthy", service = "Design.Service" });
    }
}
