using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Nexus.OAuth.Web.Controllers.Base;
using Nexus.OAuth.Web.Models;
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
        if (XssValidation(ref after))
            return XssError();

        ViewBag.RedirectTo = after ?? DefaultRedirect;
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