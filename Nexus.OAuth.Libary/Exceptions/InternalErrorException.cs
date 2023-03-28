namespace Nexus.OAuth.Libary.Exceptions;

[Serializable]
public class InternalErrorException : Exception
{
    private const string ErrorMessage = "An internal error occurred on the server and we were unable to respond to this request.";
    public InternalErrorException() : base(ErrorMessage) { }
    public InternalErrorException(Exception inner) : base(ErrorMessage, inner) { }
    protected InternalErrorException(
      System.Runtime.Serialization.SerializationInfo info,
      System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}