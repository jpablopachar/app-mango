namespace coupon_service.Dtos
{
    public class CouponResponseDto
    {
        public IReadOnlyList<CouponDto>? Result { get; set; }
        public bool Success { get; set; }
        public string? Message { get; set; }
    }
}