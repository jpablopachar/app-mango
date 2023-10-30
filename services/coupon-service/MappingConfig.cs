using AutoMapper;
using coupon_service.Dtos;
using coupon_service.Models;

namespace coupon_service
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            MapperConfiguration config = new(cfg =>
            {
                cfg.CreateMap<CouponDto, Coupon>();
                cfg.CreateMap<Coupon, CouponDto>();
            });

            return config;
        }
    }
}