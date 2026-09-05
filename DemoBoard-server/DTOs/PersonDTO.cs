using DemoBoard_server.Models;

namespace DemoBoard_server.DTOs;

public class PersonDTO
{
    public long Id { get; set; }
    public required AccountResponseDTO Account { get; set; }
}