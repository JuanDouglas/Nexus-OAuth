namespace Nexus.OAuth.Api.Controllers.Results;
internal class QrCodeResult : FileContentResult
{
    public QrCodeReference Reference { get; set; }
    public string ValidationToken { get; set; }
    public QrCodeResult(QrCodeReference reference, string validationToken, byte[] array, string contentType) : base(array, contentType)
    {
        Reference = reference;
        ValidationToken = validationToken;
    }

    public override async Task ExecuteResultAsync(ActionContext context)
    {
        context.HttpContext.Response.Headers["X-Code"] = Reference.Code;
        context.HttpContext.Response.Headers["X-Validation"] = ValidationToken;
        context.HttpContext.Response.Headers["X-Code-Id"] = Reference.Id.ToString();
        context.HttpContext.Response.Headers["Access-Control-Expose-Headers"] = "X-Code,X-Validation,X-Code-Id";

        await base.ExecuteResultAsync(context);
    }
}

