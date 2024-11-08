namespace API.Services;

public interface IService<T>
{
    public Task<T?> Create(T entity);
    public Task<T?> GetOne(int id);
    public Task<List<T>> GetAll();
    public Task<T?> Update(T entity);
    public Task Delete(int id);
}