using System.Security.Claims;
using DemoBoard_server.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace DemoBoard_server.Security;

public class UserOwnedResourceAuthorizationHandler : AuthorizationHandler<SameAccountRequirement, IUserOwnedResource>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, SameAccountRequirement requirement, IUserOwnedResource resource)
    {
        if (context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value == resource.Owner.Id)
            context.Succeed(requirement);

        return Task.CompletedTask;
    }
}