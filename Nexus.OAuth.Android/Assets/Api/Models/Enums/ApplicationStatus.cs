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

namespace Nexus.OAuth.Android.Assets.Api.Models.Enums
{
    public enum ApplicationStatus : sbyte
    {
        /// <summary>
        /// Application disabled status
        /// </summary>
        Disabled = 1,
        /// <summary>
        /// 
        /// </summary>
        Development = 50,
        /// <summary>
        /// Application Testing status
        /// </summary>
        Testing = 100,
        /// <summary>
        /// Application active
        /// </summary>
        Active = 127
    }
}