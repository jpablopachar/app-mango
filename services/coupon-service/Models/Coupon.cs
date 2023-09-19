using System.ComponentModel.DataAnnotations;

namespace coupon_service.Models
{
    public class Coupon
    {
        [Key]
        public int CouponId { get; set; }
        [Required]
        public string? CouponCode { get; set; }
        [Required]
        public double DiscountAmount { get; set; }
        public int MinAmount { get; set; }
    }
}