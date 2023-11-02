namespace client.Utilities
{
    public class SD
    {
        public const string ROLE_ADMIN = "Admin";
        public const string ROLE_CUSTOMER = "Customer";
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