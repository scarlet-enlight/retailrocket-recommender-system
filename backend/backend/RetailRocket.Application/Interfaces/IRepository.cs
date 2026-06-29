namespace RetailRocket.Application.Interfaces;

public interface IRepository<T, TKey>
{
    Task<T?> GetByIdAsync(TKey id);
    Task AddAsync(T element);
    Task DeleteAsync(TKey id);
}