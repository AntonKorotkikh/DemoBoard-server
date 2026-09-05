using DemoBoard_server.Security;

namespace DemoBoard_server.DTOs;

public class AccountPasswordUpdateDTO : IUserOwnedResource
{
    public required string UserName { get; set; }
    public required string NewPassword { get; set; }
    public required string CurrentPassword { get; set; }
}