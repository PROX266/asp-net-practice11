using WebApplication1.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.DTOs;
using WebApplication1.Services;

namespace WebApplication1.Controllers.Api;

[Route("api/[controller]")]
[ApiController]
[IgnoreAntiforgeryToken]
[Authorize(Policy = Policies.AdminOnly)]
public class ReservationsController : ControllerBase
{
    private readonly IProductReservationService _reservationService;

    public ReservationsController(IProductReservationService reservationService)
    {
        _reservationService = reservationService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        int page = 1, int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var (items, total) = await _reservationService.GetAllAsync(
            page, pageSize, cancellationToken);

        Response.Headers["X-Total-Count"] = total.ToString();
        return Ok(items);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var reservation = await _reservationService.GetByIdAsync(id, cancellationToken);
        if (reservation is null) return NotFound();
        return Ok(reservation);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        CreateProductReservationDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var reservation = await _reservationService.CreateAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = reservation.Id }, reservation);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("{id}/release")]
    public async Task<IActionResult> Release(int id, CancellationToken cancellationToken)
    {
        var reservation = await _reservationService.ReleaseAsync(id, cancellationToken);
        if (reservation is null) return NotFound();
        return Ok(reservation);
    }
}
