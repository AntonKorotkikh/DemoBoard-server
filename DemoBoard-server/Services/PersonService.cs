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
            Account = account
        };

        var addedPersonId = await repository.AddAsync(personToAdd);

        return addedPersonId;
    }

    public async Task<PersonDTO> GetByIdAsync(long id)
    {
        var person = await repository.GetByIdAsync(id);
        if (person == null)
            throw new ItemNotFoundException<Person>();

        return ToDTO(person);
    }

    public async Task DeleteAsync(long id)
    {
        await repository.DeleteAsync(id);
    }
    
    private PersonDTO ToDTO(Person person)
    {
        var accountResponseDTO = new AccountResponseDTO()
        {
            Id = person.Account.Id,
            UserName = person.Account.UserName!,
            Email = person.Account.Email!
        };
        
        return new()
        {
            Id = person.Id,
            Account = accountResponseDTO
        };
    }
}