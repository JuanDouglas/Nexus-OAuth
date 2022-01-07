using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Hardware.Camera2;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nexus.OAuth.Android.Libary.Callbacks
{
    internal class CameraStateCallback : CameraDevice.StateCallback, ImageReader.IOnImageAvailableListener
    {
        public CameraCharacteristics Characteristics { get; set; }
        public View ResultView { get; set; }
        public CameraStateCallback(View resultView, CameraCharacteristics characteristics)
        {
            ResultView = resultView;
            Characteristics = characteristics;
        }

        public override void OnDisconnected(CameraDevice camera)
        {
            throw new NotImplementedException();
        }

        public override void OnError(CameraDevice camera, [GeneratedEnum] CameraError error)
        {
            throw new NotImplementedException();
        }

        public void OnImageAvailable(ImageReader reader)
        {

        }

        public override void OnOpened(CameraDevice camera)
        {
            var streamConfiguration = Characteristics.Get(CameraCharacteristics.ScalerStreamConfigurationMap);

            var reader = ImageReader.NewInstance(0, 0, ImageFormatType.Jpeg, 120);

            reader.SetOnImageAvailableListener(this, null);

            CaptureSessionCallback captureSessionCallback = new CaptureSessionCallback(reader.Surface);

            camera.CreateCaptureSession(new List<Surface>() { reader.Surface }, captureSessionCallback, null);
        }
    }
}