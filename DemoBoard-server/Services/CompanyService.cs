using DemoBoard_server.DTOs;
using DemoBoard_server.Exceptions;
using DemoBoard_server.Models;
using DemoBoard_server.Repositories;
using Microsoft.AspNetCore.Identity;

namespace DemoBoard_server.Services;

public class CompanyService
{
    private readonly CompanyRepository repository;
    private readonly UserManager<IdentityUser> userManager;

    public CompanyService(CompanyRepository repository, UserManager<IdentityUser> userManager)
    {
        this.repository = repository;
        this.userManager = userManager;
    }

    public async Task<Company> GetByIdAsync(long id)
    {
        return await repository.GetByIdAsync(id);
    }

    public async Task<long> AddAsync(IdentityUser account, CompanyAccountCreationDTO companyAccountCreation)
    {
        var companyToAdd = new Company()
        {
            Account = account,
            LogoPath = "",          // not implemented
            BusinessName = companyAccountCreation.BusinessName,
            Owner = account
        };

        var addedCompanyId = await repository.AddAsync(companyToAdd);

        return addedCompanyId;
    }

    public async Task DeleteAsync(long id)
    {
        await repository.DeleteAsync(id);
    }

    public async Task UpdateDataAsync(string userId, CompanyUpdateDTO companyUpdate)
    {
        var account = await userManager.FindByIdAsync(userId);
        if (account == null)
            throw new ItemNotFoundException<IdentityUser>();
        
        var company = await repository.GetByAccountNameAsync(account.UserName!);
        
        var updatedCompany = new Company()
        {
            Id = company.Id,
            Account = account,
            LogoPath = companyUpdate.LogoPath ?? company.LogoPath,
            BusinessName = companyUpdate.BusinessName ?? company.BusinessName,
            Owner = company.Owner
        };
        
        await repository.UpdateAsync(updatedCompany);
    }
}