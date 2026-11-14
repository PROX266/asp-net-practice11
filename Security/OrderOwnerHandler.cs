using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using WebApplication1.DTOs;

namespace WebApplication1.Security;

public class OrderOwnerHandler : AuthorizationHandler<OrderOwnerRequirement, OrderDto>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        OrderOwnerRequirement requirement,
        OrderDto order)
    {
        if (context.User.IsInRole(AppRoles.Admin))
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        var idClaim = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (int.TryParse(idClaim, out var userId) && order.User.Id == userId)
            context.Succeed(requirement);

        return Task.CompletedTask;
    }
}
