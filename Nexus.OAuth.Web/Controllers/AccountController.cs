using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Nexus.OAuth.Web.Controllers.Base;
using Nexus.OAuth.Web.Models;
using System.Net;
using System.Text;

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
    [ValidateAntiForgeryToken]
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

            var errors = badRequest?.Errors.ToObject<IDictionary<string, string[]>>() ?? new Dictionary<string, string[]>();

            foreach (var error in errors)
            {
                foreach (var errorValue in error.Value)
                {
                    ModelState.AddModelError(error.Key, errorValue);
                }
            }

            return View(account);
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
        public JObject Errors { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    }
}