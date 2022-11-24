
using File = Nexus.OAuth.Dal.Models.File;

namespace Nexus.OAuth.Api.Models.Result;
public class AccountResult
{
    public const string DefaultProfile = "profile.png";

    public int Id { get; set; }
    public string Name { get; set; }
    public string ShortName { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public string Culture { get; set; }
    public DateTime Created { get; set; }
    public DateTime DateOfBirth { get; set; }
    public ConfirmationStatus ConfirmationStatus { get; set; }
    public FileResult ProfileImage { get; set; }
    public AccountResult(Account account)
    {
        Id = account.Id;
        Name = account.Name;
        Email = account.Email;
        Phone = account.Phone;
        Created = account.Created;
        ConfirmationStatus = account.ConfirmationStatus;
        DateOfBirth = account.DateOfBirth;
        Culture = account.Culture;
        ProfileImage = new FileResult(DefaultProfile);
        string[] names = Name.Split(' ');
        ShortName = names.Length < 2 ? names[0] : $"{names[0]} {names[1]}";
    }

    public AccountResult(Account account, File profileImage) : this(account)
    {
        ProfileImage = new(profileImage, ResourceType.AccountProfile);
    }
}

