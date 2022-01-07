using QRCoder;
using System.Web;
using Nexus.OAuth.Api.Controllers.Base;
using System.ComponentModel;

namespace Nexus.OAuth.Api.Controllers;


[AllowAnonymous]
[Route("Authentications/QrCode")]
public class AuthenticationsQrCodeController : ApiController
{
    public const double MaxQrCodeAge = AuthenticationsController.FirsStepMaxTime;
    public const int MaxPixeisPerModuleQrCode = 150;
    public const int MinKeyLength = AuthenticationsController.MinKeyLength;
    public const int MaxKeyLength = AuthenticationsController.MaxKeyLength;
    public const int AuthenticationTokenSize = AuthenticationsController.AuthenticationTokenSize;
    /// <summary>
    /// 
    /// </summary>
    /// <param name="client_key">Client unique key</param>
    /// <param name="user_agent">Client user agent</param>
    /// <param name="pixeis_per_module">Pixeis per module (<c>Default: 5</c>, <c>Max: 50</c>)</param>
    /// <returns></returns>
    [HttpGet]
    [Route("Generate")]
    [ProducesResponseType(typeof(byte[]), (int)HttpStatusCode.OK, "image/png")]
    public async Task<IActionResult> GetQrCodeAsync([FromHeader(Name = ClientKeyHeader)] string client_key, [FromHeader(Name = UserAgentHeader)] string user_agent, Theme theme = Theme.Dark, bool transparent = true, int? pixeis_per_module = 5)
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

        Response.Headers["X-Code"] = code;
        Response.Headers["X-Validation"] = validationToken;

        return File(bytes, "image/png");
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="registor_key">Client key of registor</param>
    /// <param name="code"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("Authorize")]
    [RequireAuthentication(ShowView = true)]
    public async Task<IActionResult> AuthorizeCodeAsync(string registor_key, string code)
    {
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


        throw new NotImplementedException();
    }

    [HttpGet]
    [Route("CheckStatus")]
    public async Task<IActionResult> CheckQrCodeStatusAsync(string code, string validation)
    {
        throw new NotImplementedException();
    }

    private static byte[] GetPrimaryColor(Theme theme) => theme switch
    {
        Theme.Light => new byte[] { 190, 190, 190 },
        _ => new byte[3]
    };
}

