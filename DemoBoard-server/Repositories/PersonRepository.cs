using DemoBoard_server.Data;
using DemoBoard_server.Exceptions;
using DemoBoard_server.Models;

namespace DemoBoard_server.Repositories;

public class PersonRepository : Repository<Person>
{
    public PersonRepository(DatabaseContext context) : base(context)
    {
    }

    public override async Task UpdateAsync(Person person)
    {
        var personInDB = await context.People.FindAsync(person.Id);
        if (personInDB == null)
            throw new ItemNotFoundException<Person>();
        
        // no fields to update

        await context.SaveChangesAsync();
    }
}