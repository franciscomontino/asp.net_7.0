using Product_API.Data;
using Product_API.Models;
using Product_API.Repository.IRepository;

namespace Product_API.Repository
{
  public class KindOfProductRepository : Repository<KindOfProduct>, IKindOfProductRepository
  {
    private readonly AppDbContext _db;

    public KindOfProductRepository(AppDbContext db) : base(db)
    {
      _db = db;
    }

    public async Task<KindOfProduct> Update(KindOfProduct entity)
    {
      entity.Update = DateTime.Now;
      _db.KindOfProducts.Update(entity);
      await _db.SaveChangesAsync();
      return entity;
    }
  }
}