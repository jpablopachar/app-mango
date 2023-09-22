namespace coupon_service.Dtos
{
    public class CouponResponseDto
    {
        public object? Result { get; set; }
        public bool Success { get; set; }
        public string? Message { get; set; }
    }
}