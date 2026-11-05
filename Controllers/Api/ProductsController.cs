using WebApplication1.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.DTOs;
using WebApplication1.Services;

namespace WebApplication1.Controllers.Api;

[Route("api/[controller]")]
[ApiController]
[IgnoreAntiforgeryToken]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        int page = 1, int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var (items, total) = await _productService.GetAllAsync(
            page, pageSize, cancellationToken);

        Response.Headers["X-Total-Count"] = total.ToString();
        return Ok(items);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var product = await _productService.GetByIdAsync(id, cancellationToken);
        if (product is null) return NotFound();
        return Ok(product);
    }

    [Authorize(Policy = Policies.CanManageCatalog)]
    [HttpPost]
    public async Task<IActionResult> Create(CreateProductDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var product = await _productService.CreateAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [Authorize(Policy = Policies.CanManageCatalog)]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        int id, UpdateProductDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var product = await _productService.UpdateAsync(id, dto, cancellationToken);
            if (product is null) return NotFound();
            return Ok(product);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [Authorize(Policy = Policies.CanManageCatalog)]
    [HttpPatch("{id}")]
    public async Task<IActionResult> Patch(
        int id, PatchProductDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var product = await _productService.PatchAsync(id, dto, cancellationToken);
            if (product is null) return NotFound();
            return Ok(product);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [Authorize(Policy = Policies.AdminOnly)]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var deleted = await _productService.DeleteAsync(id, cancellationToken);
        if (!deleted) return NotFound();
        return NoContent();
    }
}
