
namespace Application.Common.Settings
{
    public class JwtOptions
    {
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public int Expiration_Minutes {  get; set; }
        public string SecretKey { get; set; } = string.Empty;
        public int RefreshToken_Expiration_Days { get; set; }
    }
}
