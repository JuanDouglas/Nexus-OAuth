namespace Nexus.OAuth.Libary.Models.Api;

public class ApiAuthorization
{
    public TokenType TokenType { get; set; }
    public string? FirstStepToken { get; set; }
    public string? AuthorizationToken { get; set; }
    public string? Authorization { private get; set; }
    public string? ClientKey { get; set; }

    public ApiAuthorization(string auth)
    {
        string[] splited = auth.Split(' ');
        if (splited.Length != 2)
        {
            throw new ArgumentException("Header is invalid");
        }

        _ = Enum.TryParse(splited[0], out TokenType type);
        splited = splited[1].Split('.');

        if (splited.Length == 1)
        {

        }
    }
    public ApiAuthorization(string clientKey, string fsToken, string authToken, TokenType type)
    {
        TokenType = type;
        FirstStepToken = fsToken;
        AuthorizationToken = authToken;
        ClientKey = clientKey;
    }
    public ApiAuthorization(string authorization, TokenType type)
    {
        TokenType = type;
        Authorization = authorization;
    }

    public override string ToString()
    {
        return $"{Enum.GetName(TokenType)} {((!string.IsNullOrEmpty(Authorization)) ? Authorization : $"{AuthorizationToken}.{FirstStepToken}")}";
    }
}