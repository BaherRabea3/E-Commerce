
namespace Application.Common.DTOs.AccountDTOs
{
    public class AuthResponseDto
    {
        public string Email { get; set; } = string.Empty;
        public int UserId { get; set; }
        public bool IsAuthenticated { get; set; } = false;
        public string Token { get; set; } = string.Empty;
        public DateTime TokenExpiration { get; set; }
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime RefreshTokenExpiration { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
