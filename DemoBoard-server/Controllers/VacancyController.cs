using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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
                var recentVacanciesDTOs = recentVacancies.Select(x => ToDTO(x));
                return Ok(recentVacanciesDTOs);
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
                return ToDTO(vacancy);
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
        public async Task<IActionResult> PutVacancy(long id, VacancyUpdateDTO vacancyUpdate)
        {
            Vacancy vacancy;
            try
            {
                vacancy = await service.GetByIdAsync(id);
            }
            catch (ItemNotFoundException<Vacancy>)
            {
                logger.LogWarning("PUT api/Vacancy/{RouteId}: vacancy with this ID does not exist", id);
                return BadRequest();
            }
            catch (Exception e)
            {
                logger.LogError(e, "PUT api/Vacancy/{RouteId}: unexpected failure during vacancy fetch", id);
                return StatusCode(500, "Updating the vacancy failed unexpectedly");
            }
            
            var authorizationResult = await authorizationService.AuthorizeAsync(User, vacancy, "AccessPolicy");
            if (!authorizationResult.Succeeded)
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                logger.LogInformation("PUT api/Vacancy/{id}): denied access attempt by account {UserId}", id, userId);
                return Forbid();
            }

            try
            {
                await service.UpdateAsync(id, vacancyUpdate);
                logger.LogInformation("PUT api/Vacancy/{RouteId}: updated ok", id);
                return NoContent();
            }
            catch (ItemNotFoundException<Vacancy>)
            {
                logger.LogWarning("PUT api/Vacancy/{RouteId}: vacancy with this ID does not exist", id);
                return NotFound();
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
        public async Task<ActionResult<VacancyDTO>> PostVacancy(VacancyCreationDTO vacancy)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var addedVacancyId = await service.AddAsync(userId, vacancy);
                logger.LogInformation("POST api/Vacancy: added new vacancy ok (ID: {id})", addedVacancyId);
                return CreatedAtAction(nameof(GetVacancy), new { id = addedVacancyId }, vacancy);
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
            Vacancy vacancy;
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
                logger.LogError(e, "DELETE api/Vacancy/{RouteId}: unexpected failure during vacancy fetch", id);
                return StatusCode(500, "Deleting the vacancy failed unexpectedly");
            }
            
            var authorizationResult = await authorizationService.AuthorizeAsync(User, vacancy, "AccessPolicy");
            if (!authorizationResult.Succeeded)
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                logger.LogInformation("DELETE api/Vacancy/{id}): denied access attempt by account {UserId}", id, userId);
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
        
        private VacancyDTO ToDTO(Vacancy vacancy)
        {
            var companyDTO = new CompanyResponseDTO()
            {
                Id = vacancy.Company.Id,
                LogoPath = vacancy.Company.LogoPath,
                BusinessName = vacancy.Company.BusinessName
            };

            return new()
            {
                Id = vacancy.Id,
                Title = vacancy.Title,
                Description = vacancy.Description,
                TimeAdded = vacancy.TimeAdded,
                Company = companyDTO
            };
        }
    }
}
