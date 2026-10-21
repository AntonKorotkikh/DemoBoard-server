using DemoBoard_server.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace DemoBoard_server.Security;

public class VacancyUpdateAuthorizationHandler : AuthorizationHandler<SameAccountRequirement, VacancyUpdateDTO>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, SameAccountRequirement requirement, VacancyUpdateDTO resource)
    {
        if (context.User.Identity?.Name == resource.CompanyAccountName)
            context.Succeed(requirement);

        return Task.CompletedTask;
    }
}