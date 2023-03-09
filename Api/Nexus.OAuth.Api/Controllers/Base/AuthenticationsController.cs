using BenjaminAbt.HCaptcha;
using Microsoft.AspNetCore.Mvc;
using Nexus.OAuth.Api.Properties;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace Nexus.OAuth.Api.Controllers.Base;
public class AuthenticationsController : ApiController
{
    protected internal string ntConn;
    public const double FirsStepMaxTime = 130000; // Milisecond time
    public const int MinKeyLength = 32;
    public const int MaxKeyLength = 256;
    public const int AuthenticationTokenSize = 96;
    public const int RefreshTokenSize = 128;

    public AuthenticationsController(IConfiguration config)
        : base(config)
    {
        ntConn = config.GetConnectionString("MongoDB");
    }

    public AuthenticationsController(IHCaptchaApi hCaptchaProvider, IConfiguration config)
        : base(hCaptchaProvider, config)
    {
        ntConn = config.GetConnectionString("MongoDB");
    }
    
    [NonAction]
    protected internal static async Task SendTryLoginNotificationAsync(string conn, Account account, FirstStep firstStep)
    {
        MongoContext context = new(conn);
        CultureInfo culture = new(account.Culture);

        Notifications.Culture = culture;
        string title = Notifications.TitleTryLogin.Replace("{name}", account.Name.Split(' ').First());
        string description = Notifications.DescriptionTryLogin
            .Replace("{ip}", new IPAddress(firstStep.Ip).MapToIPv4().ToString());

        await context
            .SendNotificationAsync(account.Id, title, description, Notification.Channels.Security, Notification.Categories.LoginSuccess, Notification.Activities.QrCodeActivity);
    }

    [NonAction]
    protected internal static async Task SendSecurityNotificationAsync(string conn, Account account, Authentication authentication)
    {
        MongoContext context = new(conn);
        CultureInfo culture = new(account.Culture);

        Notifications.Culture = culture;
        string title = Notifications.TitleLogin.Replace("{name}", account.Name.Split(' ').First());
        string description = Notifications.DescriptionLogin
            .Replace("{ip}", new IPAddress(authentication.Ip).MapToIPv4().ToString())
            .Replace("{date}", authentication.Date.ToString("F", culture));

        await context
            .SendNotificationAsync(account.Id, title, description, Notification.Channels.Security, Notification.Categories.LoginSuccess);
    }
}