using System.ComponentModel.DataAnnotations;

namespace Nexus.OAuth.Web.Models;

public class Login
{
    [Display(Name = "User email")]
    public string User { get; set; }
    public string Password { get; set; }
}