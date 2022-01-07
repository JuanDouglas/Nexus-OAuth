using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Hardware.Camera2;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.Camera.Lifecycle;
using AndroidX.Core.Content;
using Google.Common.Util.Concurrent;
using Nexus.OAuth.Android.Libary.Callbacks;
using System;
using System.Collections.Generic;

namespace Nexus.OAuth.Android.Libary.Activities
{
    [Activity(Label = "@string/read_qr_activity", Theme = "@style/NexusOAuth")]
    public class ReadQrActivity : AppCompatActivity
    {
        const int RequestCamera = 1234;

        CameraManager cameraManager;
        CameraStateCallback cameraStateCallback;
        View cameraOutPut;
        string camera;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_read_qr);

            if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.Camera) == Permission.Denied)
            {
                RequestPermissions(new string[] {
                    Manifest.Permission.Camera
                }, RequestCamera);
            }

            cameraManager = (CameraManager)GetSystemService(CameraService);
            foreach (var item in cameraManager.GetCameraIdList())
            {
                CameraCharacteristics characteristics = cameraManager.GetCameraCharacteristics(item);
                LensFacing lensFacing = (LensFacing)(Convert.ToInt32(characteristics.Get(CameraCharacteristics.LensFacing)));
                if (lensFacing == LensFacing.Back)
                {
                    camera = item;
                    break;
                }
            }

            cameraOutPut = FindViewById(Resource.Id.imgCameraOutPut);
            cameraStateCallback = new CameraStateCallback(cameraOutPut,cameraManager.GetCameraCharacteristics(camera));
            cameraManager.OpenCamera(camera, cameraStateCallback, null);
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
    }
}