namespace Nexus.OAuth.Libary.Exceptions
{
    [Serializable]
    public class AuthenticationException : Exception
    {
        private const string ErrorMessage = "Invalid or incorrect username or password!";
        public AuthenticationException() : base(ErrorMessage) { }
        public AuthenticationException(Exception inner) : base(ErrorMessage, inner) { }
        protected AuthenticationException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
