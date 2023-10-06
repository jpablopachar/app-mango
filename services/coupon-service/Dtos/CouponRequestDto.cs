namespace coupon_service.Dtos
{
    public class CouponRequestDto
    {
        public string? CouponCode { get; set; }
        public double DiscountAmount { get; set; }
        public int MinAmount { get; set; }
    }
}