using Quote.Service.Services;
using Dressed.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Quote.Service.Controllers;

[ApiController]
[Route("api/[controller]")]
public class QuotesController : ControllerBase
{
    private readonly IQuoteService _quoteService;

    public QuotesController(IQuoteService quoteService)
    {
        _quoteService = quoteService;
    }

    [HttpPost]
    [Authorize(Roles = "Supplier")]
    public async Task<IActionResult> CreateQuote([FromBody] CreateQuoteRequest request)
    {
        var supplierIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(supplierIdClaim))
            return Unauthorized();

        var supplierId = int.Parse(supplierIdClaim);
        var quote = await _quoteService.CreateQuote(supplierId, request);

        if (quote == null)
            return BadRequest(new { message = "Failed to create quote" });

        return CreatedAtAction(nameof(GetQuote), new { id = quote.Id }, quote);
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> GetQuote(int id)
    {
        var quote = await _quoteService.GetQuote(id);
        if (quote == null)
            return NotFound();

        return Ok(quote);
    }

    [HttpGet("design/{designId}")]
    [Authorize]
    public async Task<IActionResult> GetQuotesByDesign(int designId)
    {
        var quotes = await _quoteService.GetQuotesByDesign(designId);
        return Ok(quotes);
    }

    [HttpGet("supplier/{supplierId}")]
    [Authorize]
    public async Task<IActionResult> GetQuotesBySupplier(int supplierId)
    {
        var quotes = await _quoteService.GetQuotesBySupplier(supplierId);
        return Ok(quotes);
    }

    [HttpPatch("{id}/status")]
    [Authorize]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] string status)
    {
        var success = await _quoteService.UpdateQuoteStatus(id, status);
        if (!success)
            return NotFound();

        return Ok(new { message = "Quote status updated successfully" });
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Supplier")]
    public async Task<IActionResult> DeleteQuote(int id)
    {
        var success = await _quoteService.DeleteQuote(id);
        if (!success)
            return NotFound();

        return Ok(new { message = "Quote deleted successfully" });
    }

    [HttpGet("health")]
    public IActionResult Health()
    {
        return Ok(new { status = "healthy", service = "Quote.Service" });
    }
}
