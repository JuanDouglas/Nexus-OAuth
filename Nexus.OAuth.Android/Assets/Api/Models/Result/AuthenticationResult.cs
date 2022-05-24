using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Nexus.OAuth.Android.Assets.Api.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nexus.OAuth.Android.Assets.Api.Models.Result
{
    internal class AuthenticationResult
    {
        public bool IsValid { get; set; }
        public string Token { get; set; }
        public DateTime Date { get; set; }
        public double? ExpiresIn { get; set; }
        public string RefreshToken { get; set; }
        public TokenType TokenType { get; set; }

    }
}