using AutoMapper;
using coupon_service.Dtos;
using coupon_service.Models;

namespace coupon_service.Profiles
{
    public class CouponProfile : Profile
    {
        public CouponProfile()
        {
            CreateMap<Coupon, CouponResponseDto>();
            CreateMap<CouponRequestDto, Coupon>();
        }
    }
}