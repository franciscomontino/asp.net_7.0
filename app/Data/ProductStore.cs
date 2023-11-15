using ProductDto_API.Models.Product;

namespace Product_API.Data
{
  public static class ProductStore
  {
    public static List<ProductDto> productList = new List<ProductDto> {
      new ProductDto {Id=1, Name="Demo 1", Detail="detail 1", Price=50},
      new ProductDto {Id=2, Name="Demo 2 store", Detail="detail 1", Price=80}
    };
  }
}