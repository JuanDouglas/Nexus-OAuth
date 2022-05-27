using System;

namespace Nexus.OAuth.Android.Assets.Api.Models.Result
{
    internal class FirstStepResult
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string UserAgent { get; set; }
        public string Token { get; set; }
        public double? Expires { get; set; }
    }
}