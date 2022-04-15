using Authorization = Nexus.OAuth.Dal.Models.Authorization;

namespace Nexus.OAuth.Api.Models.Result;

public class AuthorizeResult
{
    public AuthorizeResult(Authorization auth, Application app)
    {
        Code = auth.Code;
        State = auth.State;
    }

    public AuthorizeResult(string error, string error_description)
    {
        Code = string.Empty;
        Error = error;
        ErrorDescription = error_description;
    }

    public string Code { get; set; }
    public string? State { get; set; }
    public string? Error { get; set; }
    public string? ErrorDescription { get; set; }
}

