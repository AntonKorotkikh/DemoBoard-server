using DemoBoard_server.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace DemoBoard_server.Security;

public class UserOwnedResourceAuthorizationHandler : AuthorizationHandler<SameAccountRequirement, IUserOwnedResource>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, SameAccountRequirement requirement, IUserOwnedResource resource)
    {
        if (context.User.Identity?.Name == resource.Owner.UserName)
            context.Succeed(requirement);

        return Task.CompletedTask;
    }
}