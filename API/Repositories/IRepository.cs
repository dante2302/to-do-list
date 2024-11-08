using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories;

public interface IRepository<T>
{
    public Task Create(T entity);
    public Task<T?> GetOne(int id);
    public Task<List<T>> GetAll();
    public Task<List<T>> GetAllByFilter(Expression<Func<T, bool>> expression);
    public Task Update(T entityBefore, T entityAfter);
    public Task Delete(T entity);
}