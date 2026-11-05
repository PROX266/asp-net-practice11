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
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        int page = 1, int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var (items, total) = await _userService.GetAllAsync(
            page, pageSize, cancellationToken);

        Response.Headers["X-Total-Count"] = total.ToString();
        return Ok(items);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var user = await _userService.GetByIdAsync(id, cancellationToken);
        if (user is null) return NotFound();
        return Ok(user);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateUserDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _userService.CreateAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        int id, UpdateUserDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _userService.UpdateAsync(id, dto, cancellationToken);
            if (user is null) return NotFound();
            return Ok(user);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> Patch(
        int id, PatchUserDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _userService.PatchAsync(id, dto, cancellationToken);
            if (user is null) return NotFound();
            return Ok(user);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var deleted = await _userService.DeleteAsync(id, cancellationToken);
        if (!deleted) return NotFound();
        return NoContent();
    }
}
