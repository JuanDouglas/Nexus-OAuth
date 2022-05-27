using Nexus.OAuth.Android.Assets.Api.Models.Enums;
using System;

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