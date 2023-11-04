namespace auth_service.Dtos
{
    public class LoginRequestDto
    {
        public string? UserName { get; set; }
        public string? Password { get; set; }
    }
}