using Newtonsoft.Json;
using Nexus.OAuth.Api.Controllers.Base;
using Nexus.OAuth.Api.Controllers.Results;
using QRCoder;
using SixLabors.ImageSharp;
using System.Net.WebSockets;
using System.Text;
using System.Web;

namespace Nexus.OAuth.Api.Controllers;

[AllowAnonymous]
[Route("api/Authentications/QrCode")]
public class AuthenticationsQrCodeController : ApiController
{
    public const double MaxQrCodeAge = AuthenticationsController.FirsStepMaxTime;
    public const int MinKeyLength = AuthenticationsController.MinKeyLength;
    public const int MaxKeyLength = AuthenticationsController.MaxKeyLength;
    public const int AuthenticationTokenSize = AuthenticationsController.AuthenticationTokenSize;
    public const int MaxPixeisPerModuleQrCode = 150;
    public const long RefreshStatusRate = 750;

    public AuthenticationsQrCodeController(IConfiguration configuration) : base(configuration)
    {
    }

    /// <summary>
    /// Generete a new QrCode for client autentication.
    /// </summary>
    /// <param name="client_key">Client unique key</param>
    /// <param name="user_agent">Client user agent</param>
    /// <param name="pixeis_per_module">Pixeis per module (<c>Default: 5</c>, <c>Max: 50</c>)</param>
    /// <returns></returns>
    [HttpGet]
    [Route("Generate")]
    [ProducesResponseType(typeof(byte[]), (int)HttpStatusCode.OK, "image/png")]
    public async Task<IActionResult> GetQrCodeAsync([FromHeader(Name = ClientKeyHeader)] string client_key, [FromHeader(Name = UserAgentHeader)] string user_agent, Theme theme = Theme.Dark, bool transparent = true, int? pixeis_per_module = 5, ImageExtension extension = ImageExtension.Png)
    {
        if (string.IsNullOrEmpty(client_key) ||
            string.IsNullOrEmpty(user_agent))
            return BadRequest();

        if (client_key.Length < MinKeyLength ||
            client_key.Length > MaxKeyLength)
            return BadRequest();

        if (pixeis_per_module > MaxPixeisPerModuleQrCode)
            pixeis_per_module = MaxPixeisPerModuleQrCode;

        string code = GeneralHelpers.GenerateToken(9, false, false);
        var query = HttpUtility.ParseQueryString(string.Empty);

        query["code"] = code;
        query["registor_key"] = client_key;

        UriBuilder uri = new($"https://{Request.Host}");
        uri.Path = "api/Authentications/QrCode/Authorize";
        uri.Query = query.ToString();
        string url = uri.ToString();
        string validationToken = GeneralHelpers.GenerateToken(AuthenticationTokenSize, false);

        QrCodeReference qrCodeReference = new()
        {
            ClientKey = GeneralHelpers.HashPassword(client_key),
            Code = code,
            Create = DateTime.UtcNow,
            Valid = true,
            IpAdress = RemoteIpAdress?.ToString() ?? string.Empty,
            UserAgent = user_agent,
            ValidationToken = GeneralHelpers.HashPassword(validationToken)
        };

        QRCodeGenerator qrGenerator = new();
        QRCodeData qrCodeData = qrGenerator.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q);

        var qrCode = new PngByteQRCode(qrCodeData);
        byte[] bytes = qrCode.GetGraphic(pixeis_per_module ?? 5,
            GetPrimaryColor(theme),
            new byte[] { 255, 255, 255, (byte)(transparent ? 0 : 255) });

        await db.QrCodes.AddAsync(qrCodeReference);
        await db.SaveChangesAsync();

        if (extension != ImageExtension.Png)
        {
            MemoryStream ms = new(bytes);

            bytes = await SaveImageAsync(await Image.LoadAsync(ms), extension, null);
        }

        return new QrCodeResult(qrCodeReference, validationToken, bytes, $"image/{Enum.GetName(extension)?.ToLowerInvariant()}"); ;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="registor_key">Client key of qr coder registor</param>
    /// <param name="code">QRCode code</param>
    /// <returns></returns>
    [HttpPost]
    [Route("Authorize")]
    [RequireAuthentication]
    public async Task<IActionResult> AuthorizeCodeAsync(string registor_key, string code)
    {
        Account? account = ClientAccount;

        if (account == null)
            return Unauthorized();

        QrCodeReference? codeReference = await (from qrCode in db.QrCodes
                                                where qrCode.Code == code &&
                                                      qrCode.Valid &&
                                                      !qrCode.Used
                                                select qrCode).FirstOrDefaultAsync();
        if (codeReference == null)
            return NotFound();

        if ((DateTime.UtcNow - codeReference.Create).TotalMilliseconds > MaxQrCodeAge)
        {
            codeReference.Valid = false;
            await db.SaveChangesAsync();
        }

        if (!GeneralHelpers.ValidPassword(registor_key, codeReference.ClientKey))
            return NotFound();

        codeReference.Used = true;
        codeReference.Valid = false;
        codeReference.Use = DateTime.UtcNow;

        QrCodeAuthorization codeAuthorization = new()
        {
            AccountId = account.Id,
            QrCodeReferenceId = codeReference.Id,
            AuthorizeDate = DateTime.UtcNow,
            Token = GeneralHelpers.GenerateToken(AuthenticationsController.AuthenticationTokenSize),
            Valid = true
        };

        db.QrCodeAuthorizations.Add(codeAuthorization);

        await db.SaveChangesAsync();

        return Ok();
    }

    [HttpGet]
    [Route("AwaitAuthorization")]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    public async Task AwaitAuthorizationAsync(int qr_code_id, string validation_token, string client_key)
    {
        if (HttpContext.WebSockets.IsWebSocketRequest)
        {
            DateTime minDate = DateTime.UtcNow - TimeSpan.FromMilliseconds(MaxQrCodeAge);

            QrCodeReference? reference = await (from qrRef in db.QrCodes
                                                where qr_code_id == qrRef.Id &&
                                                      qrRef.Valid &&
                                                      !qrRef.Used &&
                                                      qrRef.Create > minDate
                                                select qrRef).FirstOrDefaultAsync();

            if (reference == null)
            {
                HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                return;
            }

            if (!GeneralHelpers.ValidPassword(validation_token, reference.ValidationToken) ||
                !GeneralHelpers.ValidPassword(client_key, reference.ClientKey))
            {
                HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                return;
            }

            using var schtick = await HttpContext.WebSockets.AcceptWebSocketAsync();

            await AwaitAuthorizationTask(schtick, reference);
        }
        else
        {
            HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        }
    }

    [HttpGet]
    [Route("AccessToken")]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(AuthenticationResult), (int)HttpStatusCode.OK)]
    public async Task<ActionResult> AccessTokenAsync(string code, string validation, [FromHeader] string client_key)
    {
        throw new NotImplementedException();
    }

    private async Task AwaitAuthorizationTask(WebSocket sckt, QrCodeReference reference)
    {
        var buffer = new byte[1024 * 4];

        while (!sckt.CloseStatus.HasValue)
        {
            if (reference.Create + TimeSpan.FromMilliseconds(MaxQrCodeAge) < DateTime.UtcNow)
            {
                reference.Valid = false;
                await db.SaveChangesAsync();
                await sckt.CloseAsync(WebSocketCloseStatus.NormalClosure, "qr_code_expires", CancellationToken.None);
            }

            QrCodeAuthorization? authorization = await (from auth in db.QrCodeAuthorizations
                                                        where auth.QrCodeReferenceId == reference.Id
                                                        select auth).FirstOrDefaultAsync();

            QrCodeStatusResult status = new(reference, authorization);

            buffer = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(status));

            await sckt.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);

            if (status.Authorized)
            {
                await sckt.CloseAsync(WebSocketCloseStatus.NormalClosure, "qr_code_authorized", CancellationToken.None);
            }

            Thread.Sleep(TimeSpan.FromMilliseconds(RefreshStatusRate));
        }
    }

    private static byte[] GetPrimaryColor(Theme theme) => theme switch
    {
        Theme.Light => new byte[] { 190, 190, 190 },
        _ => new byte[3]
    };
}

