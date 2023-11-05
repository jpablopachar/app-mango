using AutoMapper;
using product_service.Dtos;
using product_service.Models;

namespace product_service
{
    public class MappingConfig
    {
        /// <summary>Creates a mapping configuration between the "ProductDto" and "Product"
        /// classes in both directions.</summary>
        /// <returns>MapperConfiguration object.</returns>
        public static MapperConfiguration RegisterMaps()
        {
            return new MapperConfiguration(config => config.CreateMap<ProductDto, Product>().ReverseMap());
        }
    }
}