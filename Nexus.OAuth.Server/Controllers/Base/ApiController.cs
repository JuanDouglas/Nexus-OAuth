using System.Text;

namespace Nexus.OAuth.Server.Controllers.Base;

/// <summary>
/// Base Application Controller
/// </summary>
[RequireHttps]
[ApiController]
[RequireAuthentication]
[Route("api/[controller]")]
public class ApiController : ControllerBase
{
    protected internal readonly OAuthContext db = new();

    /// <summary>
    /// Client User-Agent
    /// </summary>
    public string UserAgent { get => Request.Headers.UserAgent.ToString(); }

    /// <summary>
    /// Client Remote Ip Adress
    /// </summary>
    public IPAddress? RemoteIpAdress { get => HttpContext.Connection.RemoteIpAddress; }

    /// <summary>
    /// Transform string password in string hash 
    /// </summary>
    /// <param name="password">String password</param>
    /// <returns>New hash by password</returns>
    [NonAction]
    public static string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    /// <summary>
    /// Valid password by password hash.
    /// </summary>
    /// <param name="password">Password</param>
    /// <param name="hash">Password hash.</param>
    /// <returns>Password is valid</returns>
    [NonAction]
    public static bool ValidPassword(string password, string hash)
    {
        return BCrypt.Net.BCrypt.Verify(password, hash);
    }

    /// <summary>
    /// Generate Tokens with specific length
    /// </summary>
    /// <param name="size">Token Size</param>
    /// <param name="lower">Use lowercase characters.</param>
    /// <param name="upper">Use uppercase characters.</param>
    /// <returns>New token with size value.</returns>
    [NonAction]
    public static string GenerateToken(int size, bool upper = true, bool lower = true)
    {
        byte[] lowers = new byte[] { 97, 123 };
        byte[] uppers = new byte[] { 65, 91 };
        byte[] numbers = new byte[] { 48, 58 };

        Random random = new();
        string result = string.Empty;

        for (int i = 0; i < size; i++)
        {
            int type = random.Next(0, lower ? 3 : 2);

            byte[] possibles = type switch
            {
                1 => upper ? uppers : numbers,
                2 => lowers,
                _ => numbers
            };

            int selected = random.Next(possibles[0], possibles[1]);
            char character = (char)selected;

            result += character;
        }

        return result;
    }
}

