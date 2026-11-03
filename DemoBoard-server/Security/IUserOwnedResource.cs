using Microsoft.AspNetCore.Identity;

namespace DemoBoard_server.Security;

public interface IUserOwnedResource
{
    IdentityUser Owner { get; set; }
}