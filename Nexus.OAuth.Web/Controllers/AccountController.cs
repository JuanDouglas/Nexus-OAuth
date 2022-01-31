using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Nexus.OAuth.Web.Models;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Nexus.OAuth.Web.Controllers;

public class AccountController : Controller
{
    private readonly ILogger<AccountController> _logger;

    public AccountController(ILogger<AccountController> logger)
    {
        _logger = logger;
    }

    public IActionResult Register(string? redirect)
    {
        return View();
    }

    [HttpPost]
    public IActionResult Register(Account account)
    {
        if (!ModelState.IsValid)
            return BadRequest(account);

        return Ok(JsonConvert.SerializeObject(account));
    }
}