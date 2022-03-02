using Android.Content;
using Android.Hardware.Camera2;
using Android.Util;
using Android.Views;
using Nexus.OAuth.Android.Libary.Activities;
using System;

namespace Nexus.OAuth.Android.Libary.Comonentes
{
    internal class CameraSurface : SurfaceView
    {
        public CameraSurface(Context? context) : base(context)
        {
        }

        public CameraSurface(Context? context, IAttributeSet? attrs) : base(context, attrs)
        {
        }

        public CameraSurface(Context? context, IAttributeSet? attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
        }

        public CameraSurface(Context? context, IAttributeSet? attrs, int defStyleAttr, int defStyleRes) : base(context, attrs, defStyleAttr, defStyleRes)
        {
        }


        public SelectedCamera Camera { get; set; }
        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            int width = MeasureSpec.GetSize(widthMeasureSpec);
            int height = MeasureSpec.GetSize(heightMeasureSpec);
            float previewWidth = Width;
            float previewHeight = Height;

            //int relativeRotation = ComputeRelativeRotation(Camera.Characteristics, SurfaceRotationDegrees);

            //if (previewWidth > 0f && previewHeight > 0f)
            //{

            //    /* Scale factor required to scale the preview to its original size on the x-axis. */
            //    float scaleX = (relativeRotation % 180 == 0)
            //                   ? width / previewWidth
            //                   : width / previewHeight;

            //    /* Scale factor required to scale the preview to its original size on the y-axis. */
            //    float scaleY = (relativeRotation % 180 == 0)
            //                   ? height / previewHeight
            //                   : height / previewWidth;

            //    /* Scale factor required to fit the preview to the SurfaceView size. */
            //    float finalScale = Math.Min(scaleX, scaleY);

            //    ScaleX = 1 / scaleX * finalScale;
            //    ScaleY = 1 / scaleY * finalScale;
            //}

            //SetMeasuredDimension(width, height);
        }

        public int ComputeRelativeRotation(CameraCharacteristics characteristics, int surfaceRotationDegrees)
        {
            int sensorOrientationDegrees = Convert.ToInt32(characteristics.Get(CameraCharacteristics.SensorOrientation));
            // Reverse device orientation for back-facing cameras.
            int sign = ((LensFacing)Convert.ToInt32(characteristics.Get(CameraCharacteristics.LensFacing))) ==
                LensFacing.Front ? 1 : -1;

            // Calculate desired orientation relative to camera orientation to make
            // the image upright relative to the device orientation.
            return (sensorOrientationDegrees - surfaceRotationDegrees * sign + 360) % 360;
        }
    }
}
