using DemoBoard_server.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace DemoBoard_server.Security;

public class CompanyUpdateAuthorizationHandler : AuthorizationHandler<SameAccountRequirement, CompanyUpdateDTO>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, SameAccountRequirement requirement, CompanyUpdateDTO resource)
    {
        if (context.User.Identity?.Name == resource.CompanyAccountName)
            context.Succeed(requirement);

        return Task.CompletedTask;
    }
}