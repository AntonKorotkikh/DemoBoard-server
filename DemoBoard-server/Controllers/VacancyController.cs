using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DemoBoard_server.Data;
using DemoBoard_server.DTOs;
using DemoBoard_server.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DemoBoard_server.Models;
using DemoBoard_server.Repositories;
using DemoBoard_server.Services;
using Microsoft.AspNetCore.Authorization;

namespace DemoBoard_server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VacancyController : ControllerBase
    {
        private readonly VacancyService service;
        private readonly ILogger<VacancyController> logger;
        private readonly IAuthorizationService authorizationService;

        public VacancyController(VacancyService service, ILogger<VacancyController> logger, IAuthorizationService authorizationService)
        {
            this.service = service;
            this.logger = logger;
            this.authorizationService = authorizationService;
        }

        // GET: api/Vacancy/recent
        [AllowAnonymous]
        [HttpGet("recent")]
        public async Task<ActionResult<IEnumerable<VacancyDTO>>> GetRecentVacancies()
        {
            try
            {
                var recentVacancies = await service.GetRecentAsync();
                return Ok(recentVacancies);
            }
            catch (Exception e)
            {
                logger.LogError(e, "GET api/Vacancy/recent: unexpected failure");
                return StatusCode(500, "Fetching recent vacancies failed unexpectedly");
            }
        }

        // GET: api/Vacancy/5
        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ActionResult<VacancyDTO>> GetVacancy(long id)
        {
            try
            {
                var vacancy = await service.GetByIdAsync(id);
                return vacancy;
            }
            catch (ItemNotFoundException<Vacancy>)
            {
                logger.LogWarning("GET api/Vacancy/{RouteId}: vacancy with this ID does not exist", id);
                return NotFound();                
            }
            catch (Exception e)
            {
                logger.LogError(e, "GET api/Vacancy/{RouteId}: unexpected failure", id);
                return StatusCode(500, "Fetching the vacancy failed unexpectedly");
            }
        }

        // PUT: api/Vacancy/5
        [Authorize(Roles = "Admin, Company")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutVacancy(long id, VacancyDTO vacancy)
        {
            var authorizationResult = await authorizationService.AuthorizeAsync(User, vacancy, "AccessPolicy");
            if (!authorizationResult.Succeeded)
            {
                logger.LogInformation("PUT api/Vacancy/{id}): denied access attempt by account {UserName}", id, User.Identity.Name);
                return Forbid();
            }
            
            if (id != vacancy.Id)
            {
                logger.LogWarning("PUT api/Vacancy/{RouteId}: route ID does not match vacancy ID {VacancyId}", id, vacancy.Id);
                return BadRequest();
            }

            try
            {
                await service.UpdateAsync(vacancy);
                logger.LogInformation("PUT api/Vacancy/{RouteId}: updated ok", id);
                return NoContent();
            }
            catch (DbUpdateConcurrencyException e)
            {
                logger.LogWarning(e, "PUT api/Vacancy/{RouteId}: concurrency conflict", id);
                return Conflict();      // error message here?
            }
            catch (Exception e) 
            {
                logger.LogError(e, "PUT api/Vacancy/{RouteId}: unexpected failure", id);
                return StatusCode(500, "Updating the vacancy failed unexpectedly");
            }
        }

        // POST: api/Vacancy
        [Authorize(Roles = "Admin, Company")]
        [HttpPost]
        public async Task<ActionResult<VacancyDTO>> PostVacancy(VacancyDTO vacancy)     // no way for the company to know the company id at the moment
        {
            var authorizationResult = await authorizationService.AuthorizeAsync(User, vacancy, "AccessPolicy");
            if (!authorizationResult.Succeeded)
            {
                logger.LogInformation("POST api/Vacancy): denied access attempt by account {UserName}", User.Identity.Name);
                return Forbid();
            }
            
            try
            {
                var addedVacancyId = await service.AddAsync(vacancy);
                logger.LogInformation("POST api/Vacancy: added new vacancy ok (ID: {id})", addedVacancyId);
                return CreatedAtAction(nameof(GetVacancy), new { id = addedVacancyId }, vacancy);
            }
            catch (ItemNotFoundException<Company> e)
            {
                logger.LogWarning("POST api/Vacancy: company with the specified ID ({CompanyId}) does not exist", vacancy.Company.Id);
                return BadRequest();
            }
            catch (Exception e)
            {
                logger.LogError(e, "POST api/Vacancy: unexpected failure");
                return StatusCode(500, "Adding a new vacancy failed unexpectedly");
            }
        }

        // DELETE: api/Vacancy/5
        [Authorize(Roles = "Admin, Company")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVacancy(long id)
        {
            VacancyDTO vacancy;
            try
            {
                vacancy = await service.GetByIdAsync(id);
            }
            catch (ItemNotFoundException<Vacancy>)
            {
                logger.LogWarning("DELETE api/Vacancy/{RouteId}: vacancy with this ID does not exist", id);
                return BadRequest();
            }
            catch (Exception e)
            {
                logger.LogError(e, "DELETE api/Vacancy/{RouteId}: unexpected failure", id);
                return StatusCode(500, "Deleting the vacancy failed unexpectedly");
            }
            
            var authorizationResult = await authorizationService.AuthorizeAsync(User, vacancy, "AccessPolicy");
            if (!authorizationResult.Succeeded)
            {
                logger.LogInformation("DELETE api/Vacancy/{id}): denied access attempt by account {UserName}", id, User.Identity.Name);
                return Forbid();
            }
            
            try
            {
                await service.DeleteAsync(id);
                return NoContent();
            }
            catch (ItemNotFoundException<Vacancy>)
            {
                logger.LogError("DELETE api/Vacancy/{RouteId}: vacancy with this ID does not exist DESPITE having found it moments ago during authorization", id);
                return StatusCode(500, "Deleting the vacancy failed unexpectedly");
            }
            catch (Exception e)
            {
                logger.LogError(e, "DELETE api/Vacancy/{RouteId}: unexpected failure", id);
                return StatusCode(500, "Deleting the vacancy failed unexpectedly");
            }
        }
    }
}
