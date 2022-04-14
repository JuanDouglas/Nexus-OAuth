namespace Nexus.OAuth.Libary.Exceptions
{
    [Serializable]
    public class UnauthorizedException : Exception
    {
        private const string ErrorMessage = "You are not authorized to access this feature or the \"Authorization\" header is invalidates or is incorrect!";
        public UnauthorizedException() : base(ErrorMessage) { }
        public UnauthorizedException(Exception inner) : base(ErrorMessage, inner) { }
        protected UnauthorizedException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
