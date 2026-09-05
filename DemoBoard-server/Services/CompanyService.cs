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

    public async Task<CompanyDTO> GetByIdAsync(long id)
    {
        var company = await repository.GetByIdAsync(id);
        
        return ToDTO(company);
    }

    public async Task<long> AddAsync(IdentityUser account, CompanyAccountCreationDTO company)
    {
        var companyToAdd = new Company()
        {
            Account = account,
            LogoPath = "",          // not implemented
            BusinessName = company.BusinessName
        };

        var addedCompanyId = await repository.AddAsync(companyToAdd);

        return addedCompanyId;
    }

    public async Task DeleteAsync(long id)
    {
        await repository.DeleteAsync(id);
    }

    public async Task UpdateDataAsync(CompanyDTO companyDTO)
    {
        var existingCompany = await GetByIdAsync(companyDTO.Id);

        var account = await userManager.FindByIdAsync(companyDTO.Account.Id);
        if (account == null)
            throw new ItemNotFoundException<IdentityUser>();
        
        var updatedCompany = new Company()
        {
            Id = existingCompany.Id,
            Account = account,
            LogoPath = companyDTO.LogoPath,
            BusinessName = companyDTO.BusinessName
        };
        
        await repository.UpdateAsync(updatedCompany);
    }
    
    private CompanyDTO ToDTO(Company company)
    {
        var account = new AccountResponseDTO()
        {
            Id = company.Account.Id,
            UserName = company.Account.UserName!,
            Email = company.Account.Email!
        };
        
        return new()
        {
            Id = company.Id,
            Account = account,
            LogoPath = company.LogoPath,
            BusinessName = company.BusinessName
        };
    }
}