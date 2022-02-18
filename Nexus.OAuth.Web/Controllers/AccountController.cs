using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Nexus.OAuth.Web.Controllers.Base;
using Nexus.OAuth.Web.Models;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Tasks;

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
        if (XssValidation(after))
            return XssError();

        ViewBag.RedirectTo = after ?? "../Home";
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(Account account)
    {
        if (!ModelState.IsValid)
            return View(account);

        HttpContent content = new StringContent(JsonConvert.SerializeObject(account),
                Encoding.UTF8,
                "application/json");

        HttpResponseMessage response = await ApiClient.PutAsync("Accounts/Register", content);

        string responseStr = await response.Content.ReadAsStringAsync();

        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            BadRequestResponse? badRequest = JsonConvert.DeserializeObject<BadRequestResponse>(responseStr);


        }

        return Ok();
    }

    internal class BadRequestResponse
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public string Type { get; set; }
        public string Title { get; set; }
        public HttpStatusCode Status { get; set; }
        public string TraceId { get; set; }
        public object Errors { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public class Error
        {
            public int Field { get; set; }
        }
    }
}