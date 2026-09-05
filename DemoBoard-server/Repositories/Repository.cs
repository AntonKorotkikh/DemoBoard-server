using DemoBoard_server.Data;
using DemoBoard_server.Exceptions;
using DemoBoard_server.Models;
using Microsoft.EntityFrameworkCore;

namespace DemoBoard_server.Repositories;

public abstract class Repository<T> : IRepository<T> where T : class, IEntity
{
    protected readonly DatabaseContext context;

    protected Repository(DatabaseContext context)
    {
        this.context = context;
    }
    
    public async Task<T> GetByIdAsync(long id)
    {
        var item = await context.Set<T>().FindAsync(id);
        if (item == null)
            throw new ItemNotFoundException<T>();

        return item;
    }
    
    public async Task<long> AddAsync(T item)
    {
        context.Set<T>().Add(item);
        await context.SaveChangesAsync();

        return item.Id;
    }

    public async Task DeleteAsync(long id)
    {
        var item = await context.Set<T>().FindAsync(id);
        if (item == null)
            throw new ItemNotFoundException<T>();

        context.Set<T>().Remove(item);
        await context.SaveChangesAsync();
    }

    public abstract Task UpdateAsync(T item);
}