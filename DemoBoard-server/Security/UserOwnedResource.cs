namespace DemoBoard_server.Security;

public class UserOwnedResource : IUserOwnedResource
{
    public required string UserName { get; set; }
}