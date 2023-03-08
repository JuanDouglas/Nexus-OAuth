namespace Nexus.OAuth.Web.Models.Enums;

public enum TwoFactorType : byte
{
    /// <summary>
    /// Owner Email Message
    /// </summary>
    Email,
    /// <summary>
    ///  Sms in owner Phone
    /// </summary>
    Phone,
    /// <summary>
    /// OAuth Mobile Application
    /// </summary>
    App,
    /// <summary>
    /// Whatsapp
    /// </summary>
    Wpp
}