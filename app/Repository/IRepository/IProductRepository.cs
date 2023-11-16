using Product_API.Models;

namespace Product_API.Repository.IRepository
{
  public interface IProductRepository : IRepository<Product>
  {
    Task<Product> Update(Product entity);
  }
}