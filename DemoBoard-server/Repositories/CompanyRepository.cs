using DemoBoard_server.Data;
using DemoBoard_server.Exceptions;
using DemoBoard_server.Models;
using Microsoft.EntityFrameworkCore;

namespace DemoBoard_server.Repositories;

public class CompanyRepository : Repository<Company>
{
    public CompanyRepository(DatabaseContext context) : base(context)
    {
    }
    
    public async Task<Company> GetByAccountName(string accountName)
    {
        var companyInDB = await context.Companies.FirstOrDefaultAsync(company => company.Account.UserName == accountName);
        
        return companyInDB ?? throw new ItemNotFoundException<Company>();
    }

    public override async Task UpdateAsync(Company companyUpdate)
    {
        var companyInDB = await context.Companies.FindAsync(companyUpdate.Id);
        if (companyInDB == null)
            throw new ItemNotFoundException<Company>();
        
        companyInDB.LogoPath = companyUpdate.LogoPath;
        companyInDB.BusinessName = companyUpdate.BusinessName;

        await context.SaveChangesAsync();
    }
}