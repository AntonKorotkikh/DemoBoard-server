namespace DemoBoard_server.Security;

public interface IUserOwnedResource
{
    string UserName { get; set; }
}