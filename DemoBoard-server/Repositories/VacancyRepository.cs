using DemoBoard_server.Data;
using DemoBoard_server.Exceptions;
using DemoBoard_server.Models;
using Microsoft.EntityFrameworkCore;

namespace DemoBoard_server.Repositories;

public class VacancyRepository : Repository<Vacancy>
{
    public VacancyRepository(DatabaseContext context) : base(context)
    {
    }
    
    public override async Task UpdateAsync(Vacancy vacancy)
    {
        var vacancyInDB = await context.Vacancies.FindAsync(vacancy.Id);
        if (vacancyInDB == null)
            throw new ItemNotFoundException<Vacancy>();
        
        vacancyInDB.Title = vacancy.Title;
        vacancyInDB.Description = vacancy.Description;
        vacancyInDB.TimeAdded = vacancy.TimeAdded;
        vacancyInDB.Company = vacancy.Company;

        await context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Vacancy>> GetRecentAsync()
    {
        return await context.Vacancies
            .OrderByDescending(x => x.TimeAdded)
            .Take(20)
            .ToListAsync();
    }
}