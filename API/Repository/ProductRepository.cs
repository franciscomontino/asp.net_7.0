using Product_API.Data;
using Product_API.Models;
using Product_API.Repository.IRepository;

namespace Product_API.Repository
{
  public class ProductRepository : Repository<Product>, IProductRepository
  {
    private readonly AppDbContext _db;

    public ProductRepository(AppDbContext db) :base(db)
    {
      _db = db;
    }

    public async Task<Product> Update(Product entity)
    {
      entity.Update = DateTime.Now;
      _db.Products.Update(entity);
      await _db.SaveChangesAsync();
      return entity;
    }
  }
}