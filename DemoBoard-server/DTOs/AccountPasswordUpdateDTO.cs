using DemoBoard_server.Security;

namespace DemoBoard_server.DTOs;

public class AccountPasswordUpdateDTO
{
    public required string NewPassword { get; set; }
    public required string CurrentPassword { get; set; }
}