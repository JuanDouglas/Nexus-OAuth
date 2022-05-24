using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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