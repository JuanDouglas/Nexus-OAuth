using Android.Hardware.Camera2;
using Android.Views;
using System;
using System.Collections.Generic;

namespace Nexus.OAuth.Android.Libary.Callbacks
{

    internal class CaptureSessionCallback : CameraCaptureSession.StateCallback
    {
        public List<Surface> Surfaces { get; set; }
        public CameraDevice CameraDevice { get; set; }

        public CaptureSessionCallback(List<Surface> surfaces, CameraDevice cameraDevice)
        {
            Surfaces = surfaces ?? throw new ArgumentNullException(nameof(surfaces));
            CameraDevice = cameraDevice ?? throw new ArgumentNullException(nameof(cameraDevice));
        }
        public override void OnConfigured(CameraCaptureSession session)
        {
            CaptureRequest.Builder builder = CameraDevice.CreateCaptureRequest(CameraTemplate.Record);

            foreach (var surface in Surfaces)
            {
                builder.AddTarget(surface);
            }

            session.SetRepeatingRequest(builder.Build(), new CaptureCallback(), null);
        }

        public override void OnConfigureFailed(CameraCaptureSession session)
        {
            throw new NotImplementedException();
        }
    }
}