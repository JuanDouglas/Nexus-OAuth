using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nexus.OAuth.Libary.Exceptions
{

    [Serializable]
    public class InvalidOAuthException : Exception
    {
        private const string ErrorMessage = "Unable to obtain authorization for desired action. Please check all informations!";
        public InvalidOAuthException() : base(ErrorMessage) { }
        public InvalidOAuthException(Exception inner) : base(ErrorMessage, inner) { }
        protected InvalidOAuthException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
