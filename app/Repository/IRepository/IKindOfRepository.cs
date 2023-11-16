using Product_API.Models;

namespace Product_API.Repository.IRepository
{
  public interface IKindOfProductRepository : IRepository<KindOfProduct>
  {
    Task<KindOfProduct> Update(KindOfProduct entity);
  }
}