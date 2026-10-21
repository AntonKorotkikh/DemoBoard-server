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
    [AllowAnonymous]
    [HttpGet("{id}")]
    public async Task<ActionResult<CompanyResponseDTO>> GetCompany(long id)
    {
        CompanyResponseDTO company;
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
        
        return company;
    }
}