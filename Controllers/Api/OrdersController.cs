using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.DTOs;
using WebApplication1.Security;
using WebApplication1.Services;

namespace WebApplication1.Controllers.Api;

[Route("api/[controller]")]
[ApiController]
[IgnoreAntiforgeryToken]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;
    private readonly IAuthorizationService _authorizationService;

    public OrdersController(IOrderService orderService, IAuthorizationService authorizationService)
    {
        _orderService = orderService;
        _authorizationService = authorizationService;
    }

    private int CurrentUserId =>
        int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : 0;

    private bool IsAdmin => User.IsInRole(AppRoles.Admin);
    
    private async Task<bool> CanAccessAsync(OrderDto order)
    {
        var result = await _authorizationService.AuthorizeAsync(
            User, order, Policies.OrderOwnerOrAdmin);
        return result.Succeeded;
    }
    
    [Authorize(Policy = Policies.AdminOnly)]
    [HttpGet]
    public async Task<IActionResult> GetAll(
        int page = 1, int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var (items, total) = await _orderService.GetAllAsync(
            page, pageSize, cancellationToken);

        Response.Headers["X-Total-Count"] = total.ToString();
        return Ok(items);
    }
    
    [HttpGet("my")]
    public async Task<IActionResult> GetMy(CancellationToken cancellationToken)
    {
        var orders = await _orderService.GetByUserAsync(CurrentUserId, cancellationToken);
        return Ok(orders);
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var order = await _orderService.GetByIdAsync(id, cancellationToken);
        if (order is null) return NotFound();
        if (!await CanAccessAsync(order)) return Forbid();
        return Ok(order);
    }
    
    [HttpPost]
    public async Task<IActionResult> Create(CreateOrderDto dto, CancellationToken cancellationToken)
    {
        if (!IsAdmin && dto.UserId != CurrentUserId) return Forbid();

        try
        {
            var order = await _orderService.CreateAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
    
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        int id, UpdateOrderDto dto, CancellationToken cancellationToken)
    {
        var existing = await _orderService.GetByIdAsync(id, cancellationToken);
        if (existing is null) return NotFound();
        if (!await CanAccessAsync(existing)) return Forbid();
        if (!IsAdmin && dto.UserId != CurrentUserId) return Forbid();

        try
        {
            var order = await _orderService.UpdateAsync(id, dto, cancellationToken);
            if (order is null) return NotFound();
            return Ok(order);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
    
    [HttpPatch("{id}")]
    public async Task<IActionResult> Patch(
        int id, PatchOrderDto dto, CancellationToken cancellationToken)
    {
        var existing = await _orderService.GetByIdAsync(id, cancellationToken);
        if (existing is null) return NotFound();
        if (!await CanAccessAsync(existing)) return Forbid();
        if (!IsAdmin && dto.UserId is not null && dto.UserId != CurrentUserId) return Forbid();

        try
        {
            var order = await _orderService.PatchAsync(id, dto, cancellationToken);
            if (order is null) return NotFound();
            return Ok(order);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var existing = await _orderService.GetByIdAsync(id, cancellationToken);
        if (existing is null) return NotFound();
        if (!await CanAccessAsync(existing)) return Forbid();

        var deleted = await _orderService.DeleteAsync(id, cancellationToken);
        if (!deleted) return NotFound();
        return NoContent();
    }
}
