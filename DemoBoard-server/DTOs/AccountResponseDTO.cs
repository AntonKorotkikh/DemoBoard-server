using DemoBoard_server.Security;

namespace DemoBoard_server.DTOs;

public class AccountResponseDTO : IUserOwnedResource
{
    public required string Id { get; set; }
    public required string UserName { get; set; }
    public required string Email { get; set; }
}