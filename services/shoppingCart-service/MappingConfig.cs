using AutoMapper;
using shoppingCart_service.Dtos;
using shoppingCart_service.Models;

namespace shoppingCart_service
{
    public class MappingConfig
    {
        /// <summary>Creates and configures a mapper configuration for mapping
        /// between different types.</summary>
        /// <returns>MapperConfiguration object.</returns>
        public static MapperConfiguration RegisterMaps()
        {
            return new MapperConfiguration(config =>
            {
                config.CreateMap<CartHeader, CartHeaderDto>().ReverseMap();
                config.CreateMap<CartDetails, CartDetailsDto>().ReverseMap();
            });
        }
    }
}