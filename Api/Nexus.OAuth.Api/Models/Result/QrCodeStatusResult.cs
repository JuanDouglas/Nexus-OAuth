using Nexus.OAuth.Api.Controllers;

namespace Nexus.OAuth.Api.Models.Result
{
    public class QrCodeStatusResult
    {
        public TimeSpan RemaingTime { get; set; }
        public bool Authorized { get; set; }
        public string? Token { get; set; }
        public QrCodeStatusResult(QrCodeReference reference, QrCodeAuthorization? authorization)
        {
            RemaingTime = TimeSpan.FromMilliseconds(AuthenticationsQrCodeController.MaxQrCodeAge) -
                          (DateTime.UtcNow - reference.Create);

            Authorized = authorization != null;
            if (Authorized)
            {
                Token = authorization.Token;
            }
        }
    }
}
