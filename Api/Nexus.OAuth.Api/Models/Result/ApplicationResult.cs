﻿namespace Nexus.OAuth.Api.Models.Result;

public class ApplicationResult
{
    public const string DefaultLogo = "applications.png";
    /// <summary>
    /// Application Id
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Application Name
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Application Secret
    /// </summary>
    public string? Secret { get; set; }

    /// <summary>
    /// Application unique identify key 
    /// </summary>
    public string Key { get; set; }

    /// <summary>
    /// Appplication Redirect Login URL
    /// </summary>
    public string RedirectLogin { get; set; }

    /// <summary>
    /// Application Authorize Login redirect URL
    /// </summary>
    public string RedirectAuthorize { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Application owner website
    /// </summary>
    public string Site { get; set; }

    /// <summary>
    /// Application work Status
    /// </summary>
    public ApplicationStatus Status { get; set; }

    /// <summary>
    /// Application Logo
    /// </summary>
    public FileResult Logo { get; set; }

    /// <summary>
    /// Min authorize confirm status
    /// </summary>
    public ConfirmationStatus? MinConfirmationStatus { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public bool? Authorized => _authorized;
    private bool _authorized;

    /// <summary>
    /// Indicates if and an internal Nexus Company application
    /// </summary>
    public bool Internal => _isInternal;
    private readonly bool _isInternal;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="application"></param>
    /// <param name="isInternal">Defines if this application is created by Nexus Company</param>
    public ApplicationResult(Application application, bool isInternal, Dal.Models.File? file, bool authorized = false)
    {
        Name = application.Name;
        Id = application.Id;
        Secret = application.Secret;
        Key = application.Key;
        Status = application.Status ?? ApplicationStatus.Disabled;
        RedirectLogin = application.RedirectLogin;
        RedirectAuthorize = application.RedirectAuthorize;
        Description = application.Description;
        Site = application.Site;
        Logo = new(DefaultLogo);
        _isInternal = isInternal;
        _authorized = authorized;
        MinConfirmationStatus = application.MinConfirmationStatus;
    }
}

