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

    public IActionResult Register(string? after)
    {
        after ??= DefaultRedirect;

        if (XssValidation(ref after))
            return XssError();

        ViewBag.RedirectTo = after;
        ViewBag.Culture = "pt-BR";
        return View();
    }

    public IActionResult ConfirmationModal() =>
        View();


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
}