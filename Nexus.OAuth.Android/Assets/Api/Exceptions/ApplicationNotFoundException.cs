using System;

namespace Nexus.OAuth.Android.Assets.Api.Exceptions
{
    [Serializable]
    public class ApplicationNotFoundException : Exception
    {
        private const string message = "";
        public ApplicationNotFoundException() : base(message) { }
        public ApplicationNotFoundException(Exception inner) : base(message, inner) { }
        protected ApplicationNotFoundException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}