using DemoBoard_server.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace DemoBoard_server.Security;

public class PersonAuthorizationHandler : AuthorizationHandler<SameAccountRequirement, PersonDTO>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, SameAccountRequirement requirement, PersonDTO resource)
    {
        if (context.User.Identity?.Name == resource.Account.UserName)
            context.Succeed(requirement);

        return Task.CompletedTask;
    }
}