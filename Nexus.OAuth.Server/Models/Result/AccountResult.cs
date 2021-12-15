
namespace Nexus.OAuth.Server.Models.Result;
public class AccountResult
{
    public int Id { get; set; }
    public string Email { get; set; }
    public DateTime Created { get; set; }
    public ValidationStatus ValidationStatus { get; set; }

    public AccountResult(Account account)
    {
        Id = account.Id;
        Email = account.Email;
        Created = account.Created;
        ValidationStatus = account.ValidationStatus;
    }
}

