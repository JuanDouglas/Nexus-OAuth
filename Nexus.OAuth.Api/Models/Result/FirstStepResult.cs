namespace Nexus.OAuth.Api.Models.Result;

public class FirstStepResult
{
    public int Id { get; set; }
    /// <summary>
    /// Date for first step token attemp
    /// </summary>
    public DateTime Date { get; set; }
    /// <summary>
    /// User-Agent Http Header
    /// </summary>
    public string UserAgent { get; set; }
    /// <summary>
    /// First Step Token
    /// </summary>
    public string Token { get; set; }
    /// <summary>
    /// Max use time for First Step token (Time in miliseconds)
    /// </summary>
    public double ExpiresIn { get; set; }

    public FirstStepResult(FirstStep firstStep, string token, double expiresIn)
    {
        Id = firstStep.Id;
        Date = firstStep.Date;
        ExpiresIn = expiresIn;
        Token = token;
        UserAgent = firstStep.UserAgent;
    }
}

