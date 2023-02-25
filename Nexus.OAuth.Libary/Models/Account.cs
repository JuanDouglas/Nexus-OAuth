using Nexus.OAuth.Libary.Controllers;
using Nexus.OAuth.Libary.Models.Api.Result.Enums;
using Nexus.OAuth.Libary.Models.Enums;

namespace Nexus.OAuth.Libary.Models;

/// <summary>
/// User Account
/// </summary>
public class Account
{
    /// <summary>
    /// User Account Id
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// User name.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// User Account E-mail
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    /// User account phone number.
    /// </summary>
    public string Phone { get; set; }

    /// <summary>
    /// Account creation date
    /// </summary>
    public DateTime Created { get; set; }

    /// <summary>
    /// User date of birth
    /// </summary>
    public DateTime DateOfBirth { get; set; }

    /// <summary>
    /// User Culture 
    /// </summary>
    public string Culture { get; set; }

    /// <summary>
    ///  Account Validation status
    /// </summary>
    public ValidationStatus ValidationStatus { get; set; }

    private FileResult profileImage;
    private AccessToken access;
    private string clientKey;
    internal Account(AccountResult account, AccessToken access, string clientKey)
    {
        Id = account.Id;
        Name = account.Name;
        Email = account.Email;
        Created = account.Created;
        Phone = account.Phone;
        ValidationStatus = account.ConfirmationStatus;
        Culture = account.Culture;
        DateOfBirth = account.DateOfBirth;
        Created = account.Created;
        profileImage = account.ProfileImage;
        this.access = access;
        this.clientKey = clientKey;
    }

    /// <summary>
    /// Download image 
    /// </summary>
    /// <returns></returns>
    public async Task<byte[]> DownloadImageAsync(Extension extension = Extension.Png)
    {
        var controller = new FilesController(clientKey, access.Token, access.TokenType);
        return await controller.DownloadAsync(profileImage.Type, profileImage.FileName, profileImage.ResourceType, extension);
    }
}

