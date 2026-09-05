namespace DemoBoard_server.Repositories;

public interface IRepository<T>
{
    Task<T> GetByIdAsync(long id);
    Task<long> AddAsync(T vacancy);
    Task DeleteAsync(long id);
    Task UpdateAsync(T item);
}