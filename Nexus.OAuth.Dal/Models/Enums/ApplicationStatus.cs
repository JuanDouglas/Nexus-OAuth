namespace Nexus.OAuth.Dal.Models.Enums;

/// <summary>
/// Application Status
/// </summary>
public enum ApplicationStatus : sbyte
{
    /// <summary>
    /// Application disabled status
    /// </summary>
    Disabled = 0,
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

