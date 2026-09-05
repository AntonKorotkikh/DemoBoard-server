using DemoBoard_server.Security;

namespace DemoBoard_server.DTOs;

public class AccountUpdateDTO : IUserOwnedResource
{
    public required string Id { get; set; }
    public string? UserName { get; set; }       // should be separated into current username and new username
    public string? Email { get; set; }
}