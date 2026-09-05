using DemoBoard_server.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace DemoBoard_server.Security;

public class VacancyAuthorizationHandler : AuthorizationHandler<SameAccountRequirement, VacancyDTO>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, SameAccountRequirement requirement, VacancyDTO resource)
    {
        if (context.User.Identity?.Name == resource.Company.Account.UserName)
            context.Succeed(requirement);

        return Task.CompletedTask;
    }
}