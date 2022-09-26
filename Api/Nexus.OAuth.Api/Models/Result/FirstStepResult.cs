using File = Nexus.OAuth.Dal.Models.File;

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
    /// <summary>
    /// User Name for Account
    /// </summary>
    public string UserName { get; set; }
    /// <summary>
    /// User Profile Image
    /// </summary>
    public FileResult ProfileImage { get; set; }

    public FirstStepResult(Account account, FirstStep firstStep, string token, double expiresIn)
    {
        string[] names = account.Name.Split(' ');
        Id = firstStep.Id;
        Date = firstStep.Date;
        ExpiresIn = expiresIn;
        Token = token;
        UserAgent = firstStep.UserAgent;
        UserName = $"{names[0]} {names[1]}";
        ProfileImage = (account.ProfileImage != null) ? new(account.ProfileImage, ResourceType.AccountProfile) : new(AccountResult.DefaultProfile);
    }
}

