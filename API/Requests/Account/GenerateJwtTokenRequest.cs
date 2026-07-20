namespace API.Requests.Account
{
    public class GenerateJwtTokenRequest
    {
        public string Token { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }
}
