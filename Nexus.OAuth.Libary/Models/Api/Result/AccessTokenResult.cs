namespace Nexus.OAuth.Libary.Models
{
    internal class AccessTokenResult
    {
        public bool isValid { get; set; }
        public string Token { get; set; }
        public DateTime Date { get; set; }
        public double? ExpiresIn { get; set; }
        public string? RefreshToken { get; set; }
        public TokenType TokenType { get; set; }
    }
}
