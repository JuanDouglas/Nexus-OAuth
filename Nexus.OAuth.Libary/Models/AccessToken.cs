namespace Nexus.OAuth.Libary.Models
{
    public class AccessToken
    {
        public bool isValid { get; set; }
        public string Token { get; set; }
        public DateTime Date { get; set; }
        public double? ExpiresIn { get; set; }
        public string? RefreshToken { get; set; }
        public TokenType TokenType { get; set; }
        internal AccessToken(AccessTokenResult accessToken)
        {
            Token = accessToken.Token;
            ExpiresIn = accessToken.ExpiresIn;
            RefreshToken = accessToken.RefreshToken;
            Token = accessToken.Token;
            Date = accessToken.Date;
            isValid = accessToken.isValid;
            TokenType = accessToken.TokenType;
        }
    }
}
