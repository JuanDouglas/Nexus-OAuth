using BenjaminAbt.HCaptcha;
using Nexus.OAuth.Api.Properties;
using Nexus.OAuth.Domain.Messages;
using System.Globalization;

namespace Nexus.OAuth.Api.Controllers.Base;
public class AuthenticationsController : ApiController
{
    protected internal string ntConn;
    public const double FirsStepMaxTime = 150000; // Milliseconds time
    public const int MinKeyLength = 32;
    public const int MaxKeyLength = 256;
    public const int AuthenticationTokenSize = 96;
    public const int RefreshTokenSize = 128;
    public const string From = "security@mail.nexus-company.net";
    private const string nameKey = "{name}";
    private const string ipKey = "{ip}";
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
    protected internal async Task SendSecurityNotificationAsync(string conn, Account account, Authentication authentication)
    {
        MongoContext context = new(conn);
        CultureInfo culture = new(account.Culture);
        string name = account.Name.Split(' ').First();
        string ip = new IPAddress(authentication.Ip).MapToIPv4().ToString();

        Notifications.Culture = culture;
        string title = Notifications.TitleLogin.Replace(nameKey, name);
        string description = Notifications.DescriptionLogin
            .Replace(ipKey, ip)
            .Replace("{date}", authentication.Date.ToString("F", culture));

        string htmlEmail = Resources.authentication_alert
            .Replace(nameKey, name)
            .Replace(ipKey, ip);

        await EmailSender.SendEmailAsync(htmlEmail, title, account.Email, From);

        await context
            .SendNotificationAsync(account.Id, title, description, Notification.Channels.Security, Notification.Categories.LoginSuccess);
    }
}