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
public class RolesController : ControllerBase
{
    private readonly IRoleService _roleService;

    public RolesController(IRoleService roleService)
    {
        _roleService = roleService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        int page = 1, int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var (items, total) = await _roleService.GetAllAsync(
            page, pageSize, cancellationToken);

        Response.Headers["X-Total-Count"] = total.ToString();
        return Ok(items);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var role = await _roleService.GetByIdAsync(id, cancellationToken);
        if (role is null) return NotFound();
        return Ok(role);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateRoleDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var role = await _roleService.CreateAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = role.Id }, role);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        int id, UpdateRoleDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var role = await _roleService.UpdateAsync(id, dto, cancellationToken);
            if (role is null) return NotFound();
            return Ok(role);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> Patch(
        int id, PatchRoleDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var role = await _roleService.PatchAsync(id, dto, cancellationToken);
            if (role is null) return NotFound();
            return Ok(role);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var deleted = await _roleService.DeleteAsync(id, cancellationToken);
        if (!deleted) return NotFound();
        return NoContent();
    }
}
