namespace Nexus.OAuth.Server.Models.Result;

public class FirstStepResult
{
    public int Id { get; set; }
    /// <summary>
    /// Date for first step token creation
    /// </summary>
    public DateTime Date { get; set; }
    public string Token { get; set; }
    /// <summary>
    /// Max use time for token (Time in miliseconds)
    /// </summary>
    public double ExpiresIn { get; set; }

    public FirstStepResult(FirstStep firstStep, string token, double expiresIn)
    {
        Id = firstStep.Id;
        Date = firstStep.Date;
        ExpiresIn = expiresIn;
        Token = token;
    }
}

