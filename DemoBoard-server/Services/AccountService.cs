using DemoBoard_server.DTOs;
using DemoBoard_server.Exceptions;
using DemoBoard_server.Models;
using DemoBoard_server.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace DemoBoard_server.Services;

public class AccountService
{
    private readonly IAuthorizationService authorizationService;
    private readonly UserManager<IdentityUser> userManager;
    private readonly SignInManager<IdentityUser> signInManager;
    private readonly PersonService personService;
    private readonly CompanyService companyService;

    public AccountService(IAuthorizationService authorizationService, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, PersonService personService, CompanyService companyService)
    {
        this.userManager = userManager;
        this.signInManager = signInManager;
        this.personService = personService;
        this.companyService = companyService;
    }
    
    public async Task<AccountResponseDTO> GetByEmailAsync(string email)
    {
        var account = await userManager.FindByEmailAsync(email);
        if (account == null)
            throw new ItemNotFoundException<IdentityUser>();
        
        return ToResponseDTO(account);
    }

    public async Task<string> AddPersonAccountAsync(PersonAccountCreationDTO account)
    {
        if (await userManager.FindByEmailAsync(account.Email) != null)
            throw new DuplicateItemException();
        
        var user = new IdentityUser { UserName = account.UserName, Email = account.Email };
        var result = await userManager.CreateAsync(user, account.Password);
        
        if (!result.Succeeded)
            throw new IdentityUserCreationFailedException(result.Errors);

        await userManager.AddToRoleAsync(user, "Person");

        try
        {
            await personService.AddAsync(user);
        }
        catch (Exception e)     // rollback. not an ideal implementation
        {
            await userManager.DeleteAsync(user);
            throw;
        }

        return user.Id;
    }
    
    public async Task<string> AddCompanyAccountAsync(CompanyAccountCreationDTO account)
    {
        if (await userManager.FindByEmailAsync(account.Email) != null)
            throw new DuplicateItemException();
        
        var user = new IdentityUser { UserName = account.UserName, Email = account.Email };
        var result = await userManager.CreateAsync(user, account.Password);

        if (!result.Succeeded)
            throw new IdentityUserCreationFailedException(result.Errors);
        
        await userManager.AddToRoleAsync(user, "Company");

        try
        {
            await companyService.AddAsync(user, account);
        }
        catch (Exception e)     // rollback. not an ideal implementation
        {
            await userManager.DeleteAsync(user);
            throw;
        }
        
        return user.Id;
    }

    public async Task Login(AccountLoginDTO account)
    {
        var result = await signInManager.PasswordSignInAsync(account.UserName, account.Password, isPersistent: true, lockoutOnFailure: false);

        if (!result.Succeeded)
            throw new LoginUnsuccessfulException();
    }

    public async Task DeleteAsync(string userName)
    {
        var user = await userManager.FindByNameAsync(userName);
        if (user == null)
            throw new ItemNotFoundException<IdentityUser>();
        
        await userManager.DeleteAsync(user);
    }

    public async Task UpdateDataAsync(AccountUpdateDTO accountUpdate)
    {
        var existingAccount = await userManager.FindByIdAsync(accountUpdate.Id);
        if (existingAccount == null)
            throw new ItemNotFoundException<IdentityUser>();

        var updatedName = accountUpdate.UserName ?? existingAccount.UserName;
        var updatedEmail = accountUpdate.Email ?? existingAccount.Email;
        
        var updatedAccount = new IdentityUser()
        {
            Id = accountUpdate.Id,
            UserName = updatedName,
            Email = updatedEmail
        };
        
        await userManager.UpdateAsync(updatedAccount);
    }

    public async Task UpdatePasswordAsync(AccountPasswordUpdateDTO passwordUpdate)
    {
        var existingAccount = await userManager.FindByNameAsync(passwordUpdate.UserName);
        if (existingAccount == null)
            throw new ItemNotFoundException<IdentityUser>();

        if (! await userManager.CheckPasswordAsync(existingAccount, passwordUpdate.CurrentPassword))
            throw new IncorrectCurrentPasswordException();

        var result = await userManager.ChangePasswordAsync(existingAccount, passwordUpdate.CurrentPassword, passwordUpdate.NewPassword);
        if (!result.Succeeded)
            throw new InvalidNewPasswordException(result.Errors);
    }
    
    private AccountResponseDTO ToResponseDTO(IdentityUser account) =>
        new()
        {
            Id = account.Id,
            UserName = account.UserName!,
            Email = account.Email!
        };
}