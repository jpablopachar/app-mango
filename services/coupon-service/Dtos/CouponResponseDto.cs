namespace coupon_service.Dtos
{
    public class CouponResponseDto
    {
        public int CouponId { get; set; }
        public string? CouponCode { get; set; }
        public double DiscountAmount { get; set; }
        public int MinAmount { get; set; }
    }
}