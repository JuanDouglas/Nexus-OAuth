namespace Nexus.OAuth.Server.Controllers.Base;

/// <summary>
/// Base Application Controller
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ApiController : ControllerBase
{
    protected internal readonly OAuthContext db = new();
    public string UserAgent { get => Request.Headers.UserAgent.ToString(); }
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
    /// <returns>New token with size value.</returns>
    [NonAction]
    public static string GenerateToken(int size)
    {
        string result = string.Empty;
        for (int i = 0; i < size / 32; i++)
        {
            result += Guid.NewGuid().ToString();
        }

        result = result.Replace("-", string.Empty);
        return result.Remove(size, result.Length - size);
    }
}

