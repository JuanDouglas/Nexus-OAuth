namespace Nexus.OAuth.Server.Models.Result;

public class AuthenticationResult
{
    public AuthenticationResult(Authentication authentication,string refreshToken)
    {
        IsValid = authentication.IsValid;
        Token = authentication.Token;
        Date = authentication.Date;
        ExpiresIn = authentication.ExpiresIn;
        TokenType = authentication.TokenType;
        RefreshToken = refreshToken;
    }

    public bool IsValid { get; set; }
    public string Token { get; set; }
    public DateTime Date { get; set; }
    public double? ExpiresIn { get; set; }
    public string RefreshToken { get; set; }
    public TokenType TokenType { get; set; }

}

