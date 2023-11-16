using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Update;

namespace Product_API.Repository.IRepository
{
  public interface IRepository<T> where T : class
  {
    Task Create(T entity);
    Task <T> Get(Expression<Func<T, bool>>? filter = null, bool tracked=true);
    Task <List<T>>GetAll(Expression<Func<T, bool>>? filter = null);
    Task Remove(T entity);
    Task Save();
  }
}