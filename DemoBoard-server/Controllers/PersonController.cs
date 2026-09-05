using DemoBoard_server.DTOs;
using DemoBoard_server.Exceptions;
using DemoBoard_server.Models;
using DemoBoard_server.Repositories;
using DemoBoard_server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DemoBoard_server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PersonController : ControllerBase
{
    private readonly PersonService service;
    private readonly ILogger<PersonController> logger;
    private readonly IAuthorizationService authorizationService;

    public PersonController(PersonService service, ILogger<PersonController> logger, IAuthorizationService authorizationService)
    {
        this.service = service;
        this.logger = logger;
        this.authorizationService = authorizationService;
    }
    
    // GET: api/Person/5
    [Authorize(Roles = "Admin, Person")]
    [HttpGet("{id}")]
    public async Task<ActionResult<PersonDTO>> GetPerson(long id)
    {
        PersonDTO person;
        try
        {
            person = await service.GetByIdAsync(id);
        }
        catch (ItemNotFoundException<Person>)
        {
            logger.LogWarning("GET api/Person/{RouteId}: person with this ID does not exist", id);
            return NotFound();
        }
        catch (Exception e)
        {
            logger.LogError(e, "GET api/Person/{RouteId}: unexpected failure", id);
            return StatusCode(500, "Fetching the person failed unexpectedly");
        }
        
        var authorizationResult = await authorizationService.AuthorizeAsync(User, person, "AccessPolicy");
        if (!authorizationResult.Succeeded)
        {
            logger.LogInformation("GET api/Person/{id}): denied access attempt by account {UserName}", id, User.Identity.Name);
            return Forbid();
        }
        
        return person;
    }
}