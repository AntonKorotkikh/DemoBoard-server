using DemoBoard_server.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace DemoBoard_server.Security;

public class CompanyAuthorizationHandler : AuthorizationHandler<SameAccountRequirement, CompanyDTO>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, SameAccountRequirement requirement, CompanyDTO resource)
    {
        if (context.User.Identity?.Name == resource.Account.UserName)
            context.Succeed(requirement);

        return Task.CompletedTask;
    }
}