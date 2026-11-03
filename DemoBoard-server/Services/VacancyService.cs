using DemoBoard_server.DTOs;
using DemoBoard_server.Exceptions;
using DemoBoard_server.Models;
using DemoBoard_server.Repositories;
using Microsoft.AspNetCore.Identity;

namespace DemoBoard_server.Services;

public class VacancyService
{
    private readonly VacancyRepository repository;
    private readonly CompanyRepository companyRepository;
    private readonly UserManager<IdentityUser> userManager;

    public VacancyService(VacancyRepository repository, CompanyRepository companyRepository, UserManager<IdentityUser> userManager)
    {
        this.repository = repository;
        this.companyRepository = companyRepository;
        this.userManager = userManager;
    }

    public async Task<Vacancy> GetByIdAsync(long id)
    {
        return await repository.GetByIdAsync(id);
    }

    public async Task<long> AddAsync(string userId, VacancyCreationDTO vacancyCreation)
    {
        var account = await userManager.FindByIdAsync(userId);
        if (account == null)
            throw new ItemNotFoundException<IdentityUser>();
        
        var company = await companyRepository.GetByAccountNameAsync(account.UserName!);

        var vacancyToAdd = new Vacancy
        {
            Title = vacancyCreation.Title,
            Description = vacancyCreation.Description,
            TimeAdded = DateTime.Now,
            Company = company,
            Owner = account
        };

        var addedVacancyId = await repository.AddAsync(vacancyToAdd);

        return addedVacancyId;
    }

    public async Task DeleteAsync(long id)
    {
        await repository.DeleteAsync(id);
    }
    
    public async Task<IEnumerable<Vacancy>> GetRecentAsync()
    {
        return await repository.GetRecentAsync();
    }

    public async Task UpdateAsync(long id, VacancyUpdateDTO vacancyUpdate)
    {
        var vacancy = await repository.GetByIdAsync(id);
        
        var updatedVacancy = new Vacancy()
        {
            Id = id,
            Title = vacancyUpdate.Title ?? vacancy.Title,
            Description = vacancyUpdate.Description ?? vacancy.Description,
            TimeAdded = vacancy.TimeAdded,
            Company = vacancy.Company,
            Owner = vacancy.Owner
        };
        
        await repository.UpdateAsync(updatedVacancy);
    }
}