using Microsoft.AspNetCore.Authorization;

namespace DemoBoard_server.Security;

public class SameAccountRequirement : IAuthorizationRequirement
{
}