using System.Runtime.Serialization;

namespace Nexus.OAuth.Domain.Authentication.Exceptions;

[Serializable]
public class AuthenticationException : Exception
{
    public string Header { get; set; }
    public AuthenticationException(string message, string header) : base(message)
    {
        Header = header;
    }
    public AuthenticationException(string message, string header, Exception inner) : base(message, inner)
    {
        Header = header;
    }
    protected AuthenticationException(
      SerializationInfo info,
     StreamingContext context) : base(info, context) { }
}

