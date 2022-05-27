using System;

namespace Nexus.OAuth.Android.Assets.Api.Exceptions
{

    [Serializable]
    public class UserNotFoundException : Exception
    {
        private const string message = "";
        public UserNotFoundException() : base(message) { }
        public UserNotFoundException(Exception inner) : base(message, inner) { }
        protected UserNotFoundException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}