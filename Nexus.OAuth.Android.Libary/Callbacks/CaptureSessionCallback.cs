using Android.Hardware.Camera2;
using Android.Views;
using System;

namespace Nexus.OAuth.Android.Libary.Callbacks
{
    internal class CaptureSessionCallback : CameraCaptureSession.StateCallback
    {
        public CaptureSessionCallback(Surface surface)
        {
            Surface = surface ?? throw new ArgumentNullException(nameof(surface));
        }
        public Surface Surface { get; set; }
        public override void OnConfigured(CameraCaptureSession session)
        {
            throw new NotImplementedException();
        }

        public override void OnConfigureFailed(CameraCaptureSession session)
        {
            throw new NotImplementedException();
        }
    }
}