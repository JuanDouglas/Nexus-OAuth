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
    public const int RefreshTokenSize = AuthenticationsController.RefreshTokenSize;
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
        string validation = GeneralHelpers.GenerateToken(AuthenticationTokenSize, false);

        QrCodeReference qrCodeReference = new()
        {
            ClientKey = GeneralHelpers.HashPassword(client_key),
            Code = code,
            Create = DateTime.UtcNow,
            Valid = true,
            Ip = RemoteIpAdress?.MapToIPv4().GetAddressBytes() ?? Array.Empty<byte>(),
            UserAgent = user_agent,
            ValidationToken = GeneralHelpers.HashPassword(validation)
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

        return new QrCodeResult(qrCodeReference, validation, bytes, $"image/{Enum.GetName(extension)?.ToLowerInvariant()}"); ;
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
        codeReference.Use = DateTime.UtcNow;

        QrCodeAuthorization codeAuthorization = new()
        {
            AccountId = account.Id,
            QrCodeReferenceId = codeReference.Id,
            AuthorizeDate = DateTime.UtcNow,
            Token = GeneralHelpers.GenerateToken(AuthenticationsController.AuthenticationTokenSize),
            IsValid = true
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

            if (!GeneralHelpers.ValidPassword(client_key, reference.ClientKey) ||
                !GeneralHelpers.ValidPassword(validation_token, reference.ValidationToken))
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
    public async Task<ActionResult> AccessTokenAsync([FromHeader(Name = ClientKeyHeader)] string client_key, int id, string validation_token, string authorization_token, TokenType tokenType = TokenType.Barear)
    {
        string adress = RemoteIpAdress?.ToString() ?? string.Empty;
        QrCodeReference qrCode = await (from qr in db.QrCodes
                                        where qr.Valid &&
                                            qr.Used &&
                                            qr.Id == id
                                        select qr).FirstOrDefaultAsync();

        if (qrCode == null)
        {
            return Unauthorized();
        }

        if (!GeneralHelpers.ValidPassword(client_key, qrCode.ClientKey) ||
            !GeneralHelpers.ValidPassword(validation_token, qrCode.ValidationToken) ||
           (DateTime.UtcNow - qrCode.Create) > TimeSpan.FromMilliseconds(MaxQrCodeAge * 2) ||
            (DateTime.UtcNow - qrCode.Use) > TimeSpan.FromMilliseconds(MaxQrCodeAge))
        {
            return Unauthorized();
        }

        QrCodeAuthorization qrAuthorization = await (from auth in db.QrCodeAuthorizations
                                                     where auth.IsValid &&
                                                           auth.QrCodeReferenceId == qrCode.Id
                                                     select auth).FirstOrDefaultAsync();

        if (qrAuthorization == null)
        {
            return Unauthorized();
        }

        string rfToken = GeneralHelpers.GenerateToken(RefreshTokenSize);

        Authentication authentication = new()
        {
            Date = DateTime.UtcNow,
            QrCodeAuthorizationId = qrAuthorization.Id,
            IsValid = true,
            Token = GeneralHelpers.GenerateToken(AuthenticationTokenSize),
            RefreshToken = GeneralHelpers.HashPassword(rfToken),
            TokenType = tokenType,
            ExpiresIn = AuthenticationsController.ExpiresAuthentication,
            Ip = RemoteIpAdress?.MapToIPv6().GetAddressBytes() ?? Array.Empty<byte>()
        };

        await db.Authentications.AddAsync(authentication);
        qrCode.Valid = false;
        await db.SaveChangesAsync();

        AuthenticationResult rst = new(authentication, rfToken);
        return Ok(rst);
    }

    private async Task AwaitAuthorizationTask(WebSocket sckt, QrCodeReference reference)
    {
        byte[] buffer;

        while (!sckt.CloseStatus.HasValue)
        {
            if (reference.Create + TimeSpan.FromMilliseconds(MaxQrCodeAge) < DateTime.UtcNow)
            {
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

