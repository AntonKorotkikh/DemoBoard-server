using DemoBoard_server.DTOs;
using DemoBoard_server.Exceptions;
using DemoBoard_server.Models;
using DemoBoard_server.Repositories;
using DemoBoard_server.Security;
using DemoBoard_server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DemoBoard_server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CompanyController : ControllerBase
{
    private readonly CompanyService service;
    private readonly ILogger<CompanyController> logger;
    private readonly IAuthorizationService authorizationService;

    public CompanyController(CompanyService service, ILogger<CompanyController> logger, IAuthorizationService authorizationService)
    {
        this.service = service;
        this.logger = logger;
        this.authorizationService = authorizationService;
    }
    
    // GET: api/Company/5
    [Authorize(Roles = "Admin, Company")]
    [HttpGet("{id}")]
    public async Task<ActionResult<CompanyDTO>> GetCompany(long id)
    {
        CompanyDTO company;
        try
        {
            company = await service.GetByIdAsync(id);
        }
        catch (ItemNotFoundException<Person>)
        {
            logger.LogWarning("GET api/Company/{RouteId}: company with this ID does not exist", id);
            return NotFound();
        }
        catch (Exception e)
        {
            logger.LogError(e, "GET api/Company/{RouteId}: unexpected failure", id);
            return StatusCode(500, "Fetching the company failed unexpectedly");
        }
        
        var authorizationResult = await authorizationService.AuthorizeAsync(User, company, "AccessPolicy");
        if (!authorizationResult.Succeeded)
        {
            logger.LogInformation("GET api/Company/{id}): denied access attempt by account {UserName}", id, User.Identity.Name);
            return Forbid();
        }
        
        return company;
    }
}