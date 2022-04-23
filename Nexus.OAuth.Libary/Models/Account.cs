using Nexus.OAuth.Libary.Models.Api.Result.Enums;

namespace Nexus.OAuth.Libary.Models;
public class Account
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public DateTime Created { get; set; }
    public string Phone { get; set; }
    public ValidationStatus ValidationStatus { get; set; }

    internal Account(AccountResult account)
    {
        Id = account.Id;
        Name = account.Name;
        Email = account.Email;
        Created = account.Created;
        Phone = account.Phone;
        ValidationStatus = account.ConfirmationStatus;
    }
}

