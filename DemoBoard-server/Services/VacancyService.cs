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

    public async Task<VacancyDTO> GetByIdAsync(long id)
    {
        var vacancy = await repository.GetByIdAsync(id);
        
        return ToDTO(vacancy);
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
    
    public async Task<IEnumerable<VacancyDTO>> GetRecentAsync()
    {
        var recentVacancies = await repository.GetRecentAsync();
        var recentVacanciesDTOs = recentVacancies.Select(x => ToDTO(x));

        return recentVacanciesDTOs;
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
    
    private VacancyDTO ToDTO(Vacancy vacancy)
    {
        var companyDTO = new CompanyResponseDTO()
        {
            Id = vacancy.Company.Id,
            CompanyAccountName = vacancy.Company.Account.UserName!,
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