using DemoBoard_server.DTOs;
using DemoBoard_server.Models;
using DemoBoard_server.Repositories;

namespace DemoBoard_server.Services;

public class VacancyService
{
    private readonly VacancyRepository repository;
    private readonly CompanyRepository companyRepository;

    public VacancyService(VacancyRepository repository, CompanyRepository companyRepository)
    {
        this.repository = repository;
        this.companyRepository = companyRepository;
    }

    public async Task<Vacancy> GetByIdAsync(long id)
    {
        return await repository.GetByIdAsync(id);
    }

    public async Task<long> AddAsync(VacancyCreationDTO vacancyCreationDTO)
    {
        var company = await companyRepository.GetByAccountNameAsync(vacancyCreationDTO.CompanyAccountName);

        var vacancyToAdd = new Vacancy()
        {
            Title = vacancyCreationDTO.Title,
            Description = vacancyCreationDTO.Description,
            TimeAdded = DateTime.Now,
            Company = company
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

    public async Task UpdateAsync(long id, VacancyUpdateDTO vacancyUpdateDTO)
    {
        var company = await companyRepository.GetByAccountNameAsync(vacancyUpdateDTO.CompanyAccountName);
        
        var vacancyInDB = await repository.GetByIdAsync(id);
        
        var updatedVacancy = new Vacancy()
        {
            Id = id,
            Title = vacancyUpdateDTO.Title ?? vacancyInDB.Title,
            Description = vacancyUpdateDTO.Description ?? vacancyInDB.Description,
            TimeAdded = vacancyInDB.TimeAdded,
            Company = company
        };
        
        await repository.UpdateAsync(updatedVacancy);
    }
}