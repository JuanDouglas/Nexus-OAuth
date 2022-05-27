using Nexus.OAuth.Android.Assets.Api.Models.Enums;
using System;

namespace Nexus.OAuth.Android.Assets.Api.Models.Result
{
    internal class AccountResult
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Culture { get; set; }
        public DateTime DateOfBirth { get; set; }
        public DateTime Created { get; set; }
        public ConfirmationStatus ConfirmationStatus { get; set; }
    }
}