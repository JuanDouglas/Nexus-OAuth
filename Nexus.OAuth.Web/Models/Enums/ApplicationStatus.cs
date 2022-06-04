namespace Nexus.OAuth.Web.Models.Enums;

public enum ApplicationStatus : sbyte
{
    /// <summary>
    /// Application disabled status
    /// </summary>
    Disabled = 1,
    /// <summary>
    /// 
    /// </summary>
    Development = 50,
    /// <summary>
    /// Application Testing status
    /// </summary>
    Testing = 100,
    /// <summary>
    /// Application active
    /// </summary>
    Active = 127
}