using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Nexus.OAuth.Web.Controllers.Base;
using Nexus.OAuth.Web.Models;
using Nexus.OAuth.Web.Models.Enums;
using System.Net;

namespace Nexus.OAuth.Web.Controllers;

public class AccountController : BaseController
{
    private readonly ILogger<AccountController> _logger;

    public AccountController(ILogger<AccountController> logger)
    {
        _logger = logger;
    }

    public IActionResult ConfirmationModal()
        => View();
    public IActionResult Register(string? after)
    {
        after ??= DefaultRedirect;

        if (XssValidation(ref after))
            return XssError();

        ViewBag.RedirectTo = after;

        return View(new Account()
        {
            Culture = Thread.CurrentThread.CurrentUICulture.Name
        });
    }
    public async Task<IActionResult> RegisterChat(string? input, RegisterStep step)
    {
        if (string.IsNullOrEmpty(input) && step != RegisterStep.Welcome)
            return Text(HttpStatusCode.BadRequest, "O valor de entrada não pode ser nulo!");

        switch (step)
        {
            case RegisterStep.Welcome:
                return Text(text: "Seja bem-vindo a Nexus Company, para continuar digite seu nome completo: ");
            default:
                break;
        }

        throw new NotImplementedException();
    }

    public enum RegisterStep
    {
        Welcome
    }

    public IActionResult Recovery(string? after)
    {
        after ??= DefaultRedirect;

        if (XssValidation(ref after))
            return XssError();

        ViewBag.RedirectTo = after;
        return View();
    }
    public IActionResult Confirm(ConfirmationType type, string token)
    {
        token ??= string.Empty;
        if (XssValidation(ref token))
            return XssError();

        ViewBag.Token = token;
        ViewBag.Type = Enum.GetName(type);
        return View();
    }

    [HttpPost]
    public IActionResult Register(Account account)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        return Ok(new { valid = true });
    }

    internal class BadRequestResponse
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public string Type { get; set; }
        public string Title { get; set; }
        public HttpStatusCode Status { get; set; }
        public string TraceId { get; set; }
        public JObject Errors { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    }

    public class AjaxResponse
    {
        public int Status { get; set; }
        public object? Object { get; set; }

        public AjaxResponse(int status, object? @object)
        {
            Status = status;
            Object = @object;
        }
    }

    private IActionResult Text(HttpStatusCode status = HttpStatusCode.OK, string? text = null)
        => Ok(new AjaxResponse((int)status, text));
}