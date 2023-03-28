using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Hardware.Camera2;
using Android.OS;
using Android.Runtime;
using Android.Views;
using AndroidX.Core.Content;
using Nexus.OAuth.Android.Assets.Api.Base;
using Nexus.OAuth.Android.Assets.Callbacks;
using Nexus.OAuth.Android.Base;
using System;

namespace Nexus.OAuth.Android
{
    [Activity(Name = "com.nexus.oauth.QrCodeActivity", Label = "@string/app_name", ScreenOrientation = ScreenOrientation.Portrait, Theme = "@style/AppTheme.Translucent", Exported = true, MainLauncher = IsDebug)]
    [IntentFilter(new string[] { Intent.ActionSend, Intent.ActionView }, Categories = new string[] { Intent.CategoryDefault, Intent.CategoryBrowsable }, DataScheme = "https", DataHost = BaseApiController.ApiHost, DataPathPrefix = "/api/Authentications/QrCode/Authorize")]
    public class QrCodeActivity : AuthenticatedActivity
    {
        const int requestCamera = 1234;
        public SurfaceView CameraOutPut { get; set; }
        CameraManager cameraManager;
        CaptureStateCallback cameraStateCallback;

        SelectedCamera camera;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_qrcode);

            CameraOutPut = FindViewById<SurfaceView>(Resource.Id.imgCameraOutPut);
            cameraManager = (CameraManager)GetSystemService(CameraService);
            CameraOutPut.KeepScreenOn = true;

            if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.Camera) != Permission.Granted)
            {
                RequestPermissions(new string[] { Manifest.Permission.Camera }, requestCamera);
                return;
            }

            foreach (var item in cameraManager.GetCameraIdList())
            {
                CameraCharacteristics characteristics = cameraManager.GetCameraCharacteristics(item);

                LensFacing lensFacing = (LensFacing)Convert.ToInt32(characteristics.Get(CameraCharacteristics.LensFacing));
                if (lensFacing == LensFacing.Back)
                {
                    camera = new SelectedCamera(item, characteristics);
                    var orientation = characteristics.Get(CameraCharacteristics.SensorOrientation);
                    break;
                }
            }

            cameraStateCallback = new CaptureStateCallback(CameraOutPut, cameraManager.GetCameraCharacteristics(camera.Id));
            cameraManager.OpenCamera(camera.Id, cameraStateCallback, null);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            switch (requestCode)
            {
                case requestCamera:
                    if (grantResults[0] == Permission.Denied)
                        RequestPermissions(new string[] { Manifest.Permission.Camera }, requestCamera);
                    else
                    {
                        StartActivity(new Intent(this, typeof(QrCodeActivity)));
                    }
                    break;
                default:
                    break;
            }

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
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