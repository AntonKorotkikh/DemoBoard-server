using DemoBoard_server.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace DemoBoard_server.Security;

public class VacancyCreationAuthorizationHandler : AuthorizationHandler<SameAccountRequirement, VacancyCreationDTO>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, SameAccountRequirement requirement, VacancyCreationDTO resource)
    {
        if (context.User.Identity?.Name == resource.CompanyAccountName)
            context.Succeed(requirement);

        return Task.CompletedTask;
    }
}