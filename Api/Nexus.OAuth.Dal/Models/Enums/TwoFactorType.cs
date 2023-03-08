﻿namespace Nexus.OAuth.Dal.Models.Enums;

/// <summary>
/// Two Factor Authentication Type
/// </summary>
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