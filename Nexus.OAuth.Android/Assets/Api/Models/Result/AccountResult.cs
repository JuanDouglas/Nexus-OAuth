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