using AutoMapper;
using Product_API.Models;
using ProductDto_API.Models.Product;
using ProductDto_API.Models.KindOfProduct;
using Product_API.Models.Dto;

namespace Product_API
{
  public class MappingConfig : Profile
  {
    public MappingConfig()
    {
      // Reverse Map avoids to writing two create map for each dto 
      // Product, ProductDto - ProductDto - Product
      CreateMap<Product, ProductDto>().ReverseMap();
      CreateMap<Product, ProductCreateDto>().ReverseMap();
      CreateMap<Product, ProductUpdateDto>().ReverseMap();
      // KindOfProduct
      CreateMap<KindOfProduct, KindOfProductDto>().ReverseMap();
      CreateMap<KindOfProduct, KindOfProductCreateDto>().ReverseMap();
      CreateMap<KindOfProduct, KindOfProductUpdateDto>().ReverseMap();
      //User
      CreateMap<UserApplication, UserDto>().ReverseMap();
    }
  }
}