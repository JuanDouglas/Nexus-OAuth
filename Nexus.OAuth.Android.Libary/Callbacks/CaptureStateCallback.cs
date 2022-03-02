using Android.Graphics;
using Android.Hardware.Camera2;
using Android.Hardware.Camera2.Params;
using Android.Media;
using Android.Runtime;
using Android.Util;
using Android.Views;
using System.Collections.Generic;

namespace Nexus.OAuth.Android.Libary.Callbacks
{
    internal class CameraStateCallback : CameraDevice.StateCallback, ImageReader.IOnImageAvailableListener
    {
        public CameraCharacteristics Characteristics { get; set; }
        public SurfaceView SurfaceView { get; set; }
        public CameraStateCallback(SurfaceView surfaceView, CameraCharacteristics characteristics)
        {
            Characteristics = characteristics;
            SurfaceView = surfaceView;
        }

        public override void OnDisconnected(CameraDevice camera)
        {

        }

        public override void OnError(CameraDevice camera, [GeneratedEnum] CameraError error)
        {

        }
        public override void OnClosed(CameraDevice camera)
        {
            base.OnClosed(camera);
        }

        public void OnImageAvailable(ImageReader reader)
        {
            using (reader)
            {
                var img = reader.AcquireNextImage();
                var plane = img.GetPlanes()[0];

                img.Close();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="camera"></param>
        public override void OnOpened(CameraDevice camera)
        {
            var streamConfiguration = (StreamConfigurationMap)Characteristics.Get(CameraCharacteristics.ScalerStreamConfigurationMap);

            Size[] output = streamConfiguration.GetHighSpeedVideoSizes();

            var reader = ImageReader.NewInstance(output[0].Width, output[0].Height, ImageFormatType.Yuv420888, 3);

            reader.SetOnImageAvailableListener(this, null);

            var surfaces = new List<Surface>() { reader.Surface, SurfaceView.Holder.Surface };

            CaptureSessionCallback captureSessionCallback = new CaptureSessionCallback(surfaces, camera);

            camera.CreateCaptureSession(surfaces, captureSessionCallback, null);
        }

        public static Bitmap RotateBitmap(Bitmap source, float angle)
        {
            Matrix matrix = new Matrix();
            matrix.PostRotate(angle);
            return Bitmap.CreateBitmap(source, 0, 0, source.Width, source.Height, matrix, true);
        }
    }
}