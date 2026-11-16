using DemoBoard_server.DTOs;
using DemoBoard_server.Exceptions;
using DemoBoard_server.Models;
using DemoBoard_server.Repositories;
using Microsoft.AspNetCore.Identity;

namespace DemoBoard_server.Services;

public class PersonService
{
    private readonly PersonRepository repository;

    public PersonService(PersonRepository repository)
    {
        this.repository = repository;
    }

    public async Task<long> AddAsync(IdentityUser account)
    {
        var personToAdd = new Person()
        {
            Owner = account
        };

        var addedPersonId = await repository.AddAsync(personToAdd);

        return addedPersonId;
    }

    public async Task<Person> GetByIdAsync(long id)
    {
        return await repository.GetByIdAsync(id);
    }

    public async Task DeleteAsync(long id)
    {
        await repository.DeleteAsync(id);
    }
}