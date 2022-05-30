using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Nexus.OAuth.Android.Assets.Api.Base;
using Nexus.OAuth.Android.Assets.Api.Models;
using Nexus.OAuth.Android.Assets.Api.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Nexus.OAuth.Android.Assets.Api
{
    internal class FilesController : BaseApiController
    {
        public Authentication? Authentication { get; set; }
        public FilesController(Context context) : base(context)
        {
        }

        public FilesController(Context context, Authentication authentication) : this(context)
        {
            Authentication = authentication;
        }

        public override string ControllerHost => $"{Host}/Files";
        public override HttpRequestMessage BaseRequest => Authentication == null ?
            base.BaseRequest :
            AuthenticatedController.AddAuthentication(base.BaseRequest, Authentication);

        public async Task<byte[]> DownloadAsync(FileType type, ResourceType resourceType, string fileName, ImageExtension extension = ImageExtension.Png)
        {
            string url = $"{ControllerHost}/{EncodeString(Enum.GetName(typeof(FileType), type))}/Download?" +
                $"fileName={EncodeString(fileName)}" +
                $"&extension={EncodeString(Enum.GetName(typeof(ImageExtension), extension))}" +
                $"&resourceType={EncodeString(Enum.GetName(typeof(ResourceType), resourceType))}";

            HttpRequestMessage request = BaseRequest;
            request.RequestUri = new Uri(url);

            var response = await SendAsync(request);

            return await response.Content.ReadAsByteArrayAsync();
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}