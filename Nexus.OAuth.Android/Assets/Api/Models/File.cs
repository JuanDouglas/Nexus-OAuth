using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Nexus.OAuth.Android.Assets.Api.Models.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nexus.OAuth.Android.Assets.Api.Models
{
    internal class File
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public FileType Type { get; set; }
        public long Length { get; set; }
        public ResourceType ResourceType { get; set; }

        public async Task<Drawable> DownloadAsync(Context ctx, ImageExtension extension = ImageExtension.Png)
        {
            using (var fsController = new FilesController(ctx))
            {
                byte[] file = await fsController.DownloadAsync(Type, ResourceType, FileName, extension);

                using (var ms = new MemoryStream(file))
                {
                    return new BitmapDrawable(ctx.Resources, await BitmapFactory.DecodeStreamAsync(ms));
                }
            }
        }
    }
}