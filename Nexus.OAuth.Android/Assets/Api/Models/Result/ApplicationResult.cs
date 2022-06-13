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
    internal class ApplicationResult
    {
        /// <summary>
        /// Application Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Application Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Application unique identify key 
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Appplication Redirect Login URL
        /// </summary>
        public string RedirectLogin { get; set; }

        /// <summary>
        /// Application Authorize Login redirect URL
        /// </summary>
        public string RedirectAuthorize { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Application owner website
        /// </summary>
        public string Site { get; set; }

        /// <summary>
        /// Application work Status
        /// </summary>
        public ApplicationStatus Status { get; set; }

        /// <summary>
        /// Application Logo
        /// </summary>
        public File Logo { get; set; }

        /// <summary>
        /// Min authorize confirm status
        /// </summary>
        public ConfirmationStatus? MinConfirmationStatus { get; set; }

        /// <summary>
        /// Indicates if and an internal Nexus Company application
        /// </summary>
        public bool Internal { get; set; }
    }
}