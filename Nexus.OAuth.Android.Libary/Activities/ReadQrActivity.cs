using Android;
using Android.App;
using Android.Content.PM;
using Android.Hardware.Camera2;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.Core.Content;
using Nexus.OAuth.Android.Libary.Callbacks;
using System;

namespace Nexus.OAuth.Android.Libary.Activities
{
    [Activity(Label = "@string/read_qr_activity", Theme = "@style/NexusOAuth")]
    public class ReadQrActivity : AppCompatActivity
    {
        const int RequestCamera = 1234;

        CameraManager cameraManager;
        CameraStateCallback cameraStateCallback;
        SurfaceView cameraOutPut;
        SelectedCamera camera;
        SurfaceOrientation displayOrientation;
        ImageView btnFlash;
        bool flashing;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.layout_qr_code_bottom_bar);

            cameraOutPut = FindViewById<SurfaceView>(Resource.Id.imgCameraOutPut);
            btnFlash = FindViewById<ImageView>(Resource.Id.btnFlashLight);

            btnFlash.Click += FlashClick;

            displayOrientation = WindowManager.DefaultDisplay.Rotation;

            bool cameraPermited = ContextCompat.CheckSelfPermission(this, Manifest.Permission.Camera) == Permission.Granted;
            if (!cameraPermited)
            {
                RequestPermissions(new string[] {
                    Manifest.Permission.Camera
                }, RequestCamera);
            }

            cameraManager = (CameraManager)GetSystemService(CameraService);
            foreach (var item in cameraManager.GetCameraIdList())
            {
                CameraCharacteristics characteristics = cameraManager.GetCameraCharacteristics(item);

                LensFacing lensFacing = (LensFacing)Convert.ToInt32(characteristics.Get(CameraCharacteristics.LensFacing));
                if (lensFacing == LensFacing.Back)
                {
                    camera = new SelectedCamera(item, characteristics);
                    var oreitnation = characteristics.Get(CameraCharacteristics.SensorOrientation);
                    break;
                }
            }

            if (cameraPermited)
            {
                cameraStateCallback = new CameraStateCallback(cameraOutPut, cameraManager.GetCameraCharacteristics(camera.Id));
                cameraManager.OpenCamera(camera.Id, cameraStateCallback, null);
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            switch (requestCode)
            {

                case RequestCamera:
                    if (grantResults[0] == Permission.Denied)
                    {
                        RequestPermissions(new string[] {
                    Manifest.Permission.Camera
                }, RequestCamera);
                    }
                    break;
                default:
                    break;
            }

            OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        private void FlashClick(object sender, EventArgs args)
        {

            flashing = !flashing;
        }
    }

    public class SelectedCamera
    {
        public string Id { get; set; }
        public CameraCharacteristics Characteristics { get; set; }

        public SelectedCamera(string id, CameraCharacteristics characteristics)
        {
            Id = id;
            Characteristics = characteristics;
        }
    }
}