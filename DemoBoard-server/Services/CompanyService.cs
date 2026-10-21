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

    public async Task<CompanyResponseDTO> GetByIdAsync(long id)
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

    public async Task UpdateDataAsync(CompanyUpdateDTO companyUpdateDTO)
    {
        var existingCompany = await repository.GetByAccountNameAsync(companyUpdateDTO.CompanyAccountName);

        var account = await userManager.FindByIdAsync(existingCompany.Account.Id);
        if (account == null)
            throw new ItemNotFoundException<IdentityUser>();
        
        var updatedCompany = new Company()
        {
            Id = existingCompany.Id,
            Account = account,
            LogoPath = companyUpdateDTO.LogoPath ?? existingCompany.LogoPath,
            BusinessName = companyUpdateDTO.BusinessName
        };
        
        await repository.UpdateAsync(updatedCompany);
    }
    
    private CompanyResponseDTO ToDTO(Company company)
    {
        return new()
        {
            Id = company.Id,
            CompanyAccountName = company.Account.UserName!,
            LogoPath = company.LogoPath,
            BusinessName = company.BusinessName
        };
    }
}