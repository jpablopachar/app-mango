namespace client.Utilities
{
    public class SD
    {
        public static string? CouponApi { get; set; }
        public static string? ProductApi { get; set; }
        public static string? AuthApi { get; set; }
        public static string? ShoppingCartApi { get; set; }
        public static string? OrderApi { get; set; }

        public const string ROLE_ADMIN = "ADMIN";
        public const string ROLE_CUSTOMER = "CUSTOMER";
        public const string JWT_TOKEN = "JwtToken";
        public const string STATUS_PENDING = "Pending";
        public const string STATUS_READY_FOR_PICKUP = "ReadyForPickup";
        public const string STATUS_COMPLETED = "Completed";
        public const string STATUS_REFUNDED = "Refunded";
        public const string STATUS_CANCELLED = "Canceled";

        public enum ApiType {
            GET,
            POST,
            PUT,
            DELETE
        }

        public enum ContentType {
            Json,
            MultipartFormData
        }
    }
}