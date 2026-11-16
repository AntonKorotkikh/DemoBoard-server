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
using DemoBoard_server.Security;
using DemoBoard_server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace DemoBoard_server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly AccountService service;
        private readonly ILogger<AccountController> logger;

        public AccountController(AccountService service, ILogger<AccountController> logger)
        {
            this.service = service;
            this.logger = logger;
        }

        // GET: api/Account
        [Authorize(Roles = "Admin, Person, Company")]
        [HttpGet]
        public async Task<ActionResult<AccountResponseDTO>> GetAccount()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Forbid();
            
            IdentityUser account;
            try
            {
                account = await service.GetByIdAsync(userId);
            }
            catch (Exception e)
            {
                logger.LogError(e, "GET api/Account: unexpected failure during account {UserId} fetch", userId);
                return StatusCode(500, "Fetching the account failed unexpectedly");
            }
            
            return ToResponseDTO(account);
        }

        // PATCH: api/Account
        [Authorize(Roles = "Admin, Person, Company")]
        [HttpPatch]
        public async Task<IActionResult> PatchAccountData(AccountUpdateDTO accountUpdate)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Forbid();

            try
            {
                await service.UpdateDataAsync(userId, accountUpdate);
                logger.LogInformation("PATCH api/Account: account {UserId} updated ok", userId);
                return NoContent();
            }
            catch (DbUpdateConcurrencyException e)
            {
                logger.LogWarning(e, "PATCH api/Account: concurrency conflict during account {UserId} update", userId);
                return Conflict();      // error message here?
            }
            catch (Exception e) 
            {
                logger.LogError(e, "PATCH api/Account: unexpected failure during account {UserId} update", userId);
                return StatusCode(500, "Updating account data failed unexpectedly");
            }
        }

        // PATCH: api/Account/password
        [Authorize(Roles = "Admin, Person, Company")]
        [HttpPatch("password")]
        public async Task<IActionResult> PatchAccountPassword(AccountPasswordUpdateDTO passwordUpdate)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Forbid();

            try
            {
                await service.UpdatePasswordAsync(userId, passwordUpdate);
                logger.LogInformation("PATCH api/Account/password: updated ok (UserId: {UserId})", userId);
                return NoContent();
            }
            catch (IncorrectCurrentPasswordException)
            {
                return Ok("The current password is incorrect");
            }
            catch (InvalidNewPasswordException e)
            {
                var errors = e.Errors.Select(error => $"{error.Code}: {error.Description}").ToList();
                logger.LogInformation("PATCH api/Account/password: ChangePasswordAsync failed for user {UserId}. This is likely because the new password does not meet the requirements, but logging errors anyway in case this is something else: {errors}", userId, errors);
                return Ok("The new password does not meet the requirements");
            }
            catch (DbUpdateConcurrencyException e)
            {
                logger.LogWarning(e, "PATCH api/Account/password: concurrency conflict (UserId: {UserId})", userId);
                return Conflict();      // error message here?
            }
            catch (Exception e) 
            {
                logger.LogError(e, "PATCH api/Account/password: unexpected failure (UserId: {UserId})", userId);
                return StatusCode(500, "Updating the account password failed unexpectedly");
            }
        }

        // POST: api/Account/person
        [AllowAnonymous]
        [HttpPost("person")]
        public async Task<ActionResult<PersonAccountCreationDTO>> RegisterPersonAccount(PersonAccountCreationDTO account)
        {
            try
            {
                var addedAccountId = await service.AddPersonAccountAsync(account);
                logger.LogInformation("POST api/Account/person: added new account ok (ID: {Id})", addedAccountId);
                return CreatedAtAction(nameof(GetAccount), new { email = account.Email }, null);
            }
            catch (DuplicateItemException)
            {
                logger.LogWarning("POST api/Account/person: account with the email {Email} already exists", account.Email);
                return BadRequest();        // needs a message?
            }
            catch (IdentityUserCreationFailedException e)
            {
                var errors = e.Errors.Select(error => $"{error.Code}: {error.Description}").ToList();
                logger.LogError("POST api/Account/person: unexpected failure during IdentityUser creation. Errors: {errors}", errors);
                
                return StatusCode(500, "Adding a new account failed unexpectedly");
            }
            catch (Exception e)
            {
                logger.LogError(e, "POST api/Account/person: unexpected failure");
                return StatusCode(500, "Adding a new account failed unexpectedly");
            }
        }
        
        // POST: api/Account/company
        [AllowAnonymous]
        [HttpPost("company")]
        public async Task<ActionResult<CompanyAccountCreationDTO>> RegisterCompanyAccount(CompanyAccountCreationDTO account)
        {
            try
            {
                var addedAccountId = await service.AddCompanyAccountAsync(account);
                logger.LogInformation("POST api/Account/company: added new account ok (ID: {Id})", addedAccountId);
                return CreatedAtAction(nameof(GetAccount), new { email = account.Email }, null);
            }
            catch (DuplicateItemException)
            {
                logger.LogWarning("POST api/Account/company: account with the email {Email} already exists", account.Email);
                return BadRequest(); // needs a message?
            }
            catch (IdentityUserCreationFailedException e)
            {
                var errors = e.Errors.Select(error => $"{error.Code}: {error.Description}").ToList();
                logger.LogError("POST api/Account/company: unexpected failure during IdentityUser creation. Errors: {errors}", errors);
                
                return StatusCode(500, "Adding a new account failed unexpectedly");
            }
            catch (Exception e)
            {
                logger.LogError(e, "POST api/Account/company: unexpected failure");
                return StatusCode(500, "Adding a new account failed unexpectedly");
            }
        }
        
        // POST: api/Account/login
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(AccountLoginDTO account)
        {
            try
            {
                await service.Login(account);
                logger.LogInformation("POST api/Account/login: user logged in ok (UserName: {UserName})", account.UserName);
                return Ok();
            }
            catch (LoginUnsuccessfulException)
            {
                logger.LogInformation("POST api/Account/login: user failed to log in (UserName: {UserName})", account.UserName);
                return Unauthorized();
            }
            catch (Exception e)
            {
                logger.LogError(e, "POST api/Account/login: unexpected failure");
                return StatusCode(500, "The attempt to log in failed unexpectedly");
            }
        }

        // DELETE: api/Account
        [Authorize(Roles = "Admin, Person, Company")]
        [HttpDelete]
        public async Task<IActionResult> DeleteAccount()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Forbid();
            
            try
            {
                await service.DeleteAsync(userId);
                return NoContent();
            }
            catch (Exception e)
            {
                logger.LogError(e, "DELETE api/Account: unexpected failure during account {UserId} deletion", userId);
                return StatusCode(500, "Deleting the account failed unexpectedly");
            }
        }
        
        private AccountResponseDTO ToResponseDTO(IdentityUser account) =>
            new()
            {
                Id = account.Id,
                UserName = account.UserName!,
                Email = account.Email!
            };
    }
}
