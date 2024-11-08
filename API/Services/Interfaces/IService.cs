namespace API.Services.Interfaces;

public interface IService<T>
{
    public Task<ServiceResult<T>> Create(T entity);
    public Task<ServiceResult<T>> GetOne(int id);
    public Task<ServiceResult<List<T>>> GetAll();
    public Task<ServiceResult<T>> Update(T entity);
    public Task<ServiceResult<string>> Delete(int id);
}