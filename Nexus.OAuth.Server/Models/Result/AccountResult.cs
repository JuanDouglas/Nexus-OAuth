
namespace Nexus.OAuth.Api.Models.Result;
public class AccountResult
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public DateTime Created { get; set; }
    public ConfirmationStatus ConfirmationStatus { get; set; }

    public AccountResult(Account account)
    {
        Id = account.Id;
        Name = account.Name;
        Email = account.Email;
        Phone = account.Phone;
        Created = account.Created;
        ConfirmationStatus = account.ConfirmationStatus;
    }
}

