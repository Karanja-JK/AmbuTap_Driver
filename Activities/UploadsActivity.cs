using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AmbuTap_Driver.EventListener;
using AmbuTap_Driver.Helpers;
using Android;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Storage;
using Plugin.Media;
using Android.Gms.Common;
using Firebase.Messaging;
using Firebase.Iid;
using Android.Util;

namespace AmbuTap_Driver.Activities
{
    [Activity(Label = "UploadsActivity")]
    public class UploadsActivity : AppCompatActivity
    {
        Button btnUpload, btnChoose, proceedButton, btnCapture;
        ImageView imgView;
        Android.Net.Uri filePath;
        const int PICK_IMAGE_REQUEST = 71;
        ProgressDialog progressDialog;
        FirebaseStorage storage;
        StorageReference storageRef;

        FirebaseDatabase database;
        FirebaseAuth mAuth;
        FirebaseUser currentUser;

        TaskCompletionListener taskCompletionListener = new TaskCompletionListener();
        private byte[] imageArray;
        readonly string[] permissionGroup =
        {
            Manifest.Permission.ReadExternalStorage,
            Manifest.Permission.WriteExternalStorage,
            Manifest.Permission.Camera
        };


        static readonly string TAG = "UploadsActivity";

        internal static readonly string CHANNEL_ID = "my_notification_channel";
        internal static readonly int NOTIFICATION_ID = 100;

        TextView msgText;



        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);


            // Create your application here
            SetContentView(Resource.Layout.files_upload);
            ConnectViews();
            InitializeFirebase();

            msgText = (TextView)FindViewById(Resource.Id.msgText); 

            IsPlayServicesAvailable();

            CreateNotificationChannel();
        }


        void InitializeFirebase()
        {
            //Firebase Init 
            database = AppDataHelper.GetDatabase();
            mAuth = AppDataHelper.GetFirebaseAuth();
            currentUser = AppDataHelper.GetCurrentUser();

            FirebaseApp.InitializeApp(this);
            //storage = FirebaseStorage.Instance;
        }

        void ConnectViews()
        {
            //Init View  
            btnChoose = (Button)FindViewById(Resource.Id.btnChoose);
            btnUpload = (Button)FindViewById(Resource.Id.btnUpload);
            btnCapture = (Button)FindViewById(Resource.Id.btnCapture);
            proceedButton = (Button)FindViewById(Resource.Id.proceedButton);
            imgView = (ImageView)FindViewById(Resource.Id.imgView);

            btnChoose.Click += BtnChoose_Click;
            btnUpload.Click += BtnUpload_Click;
            btnCapture.Click += BtnCapture_Click;
            proceedButton.Click += ProceedButton_Click;
            RequestPermissions(permissionGroup, 0);
        }


        private void BtnCapture_Click(object sender, EventArgs e)
        {
            TakePhoto();
        }


        private void BtnUpload_Click(object sender, EventArgs e)
        {
            if (imageArray != null)
            {
                storageRef = FirebaseStorage.Instance.GetReference("driverImages/profileImage");
                storageRef.PutBytes(imageArray)
                    .AddOnSuccessListener(this, taskCompletionListener)
                    .AddOnFailureListener(this, taskCompletionListener);
            }
        }

        private void BtnChoose_Click(object sender, EventArgs e)
        {
            ChoosePhoto();
        }

        private void ProceedButton_Click(object sender, EventArgs e)
        {
            StartActivity(typeof(MainActivity));
            Finish();
        }

        // method to take photo using camera
        async void TakePhoto()
        {
            await CrossMedia.Current.Initialize();

            var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
            {
                PhotoSize = Plugin.Media.Abstractions.PhotoSize.Medium,
                CompressionQuality = 40,
                Name = "AmbuTapFiles.jpg",
                Directory = "sample"

            });

            if (file == null)
            {
                return;
            }

            // convert file to byte array and set the resulting bitmap to imageview
            imageArray = System.IO.File.ReadAllBytes(file.Path);
            Bitmap bitmap = BitmapFactory.DecodeByteArray(imageArray, 0, imageArray.Length);
            imgView.SetImageBitmap(bitmap);
        }


        // method to choose photo from phone
        async void ChoosePhoto()
        {
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsPickPhotoSupported)
            {
                Toast.MakeText(this, "Upload not supported on this device", ToastLength.Short).Show();
                return;
            }

            var file = await CrossMedia.Current.PickPhotoAsync(new Plugin.Media.Abstractions.PickMediaOptions
            {
                PhotoSize = Plugin.Media.Abstractions.PhotoSize.Full,
                CompressionQuality = 40
            });

            // convert file to byte array and set the resulting bitmap to imageview
            imageArray = System.IO.File.ReadAllBytes(file.Path);
            Bitmap bitmap = BitmapFactory.DecodeByteArray(imageArray, 0, imageArray.Length);
            imgView.SetImageBitmap(bitmap);

        }


        public void OnSuccess(Java.Lang.Object result)
        {
            //progressDialog.Dismiss();
            Toast.MakeText(this, "Uploaded Successful", ToastLength.Short).Show();
        }

        public void OnFailure(Java.Lang.Exception e)
        {
            //progressDialog.Dismiss();
            Toast.MakeText(this, "" + e.Message, ToastLength.Short).Show();
        }


        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }


        //called so that the Google Play Services check runs each time the app starts
        public bool IsPlayServicesAvailable()
        {
            int resultCode = GoogleApiAvailability.Instance.IsGooglePlayServicesAvailable(this);
            if (resultCode != ConnectionResult.Success)
            {
                if (GoogleApiAvailability.Instance.IsUserResolvableError(resultCode))
                    msgText.Text = GoogleApiAvailability.Instance.GetErrorString(resultCode);
                else
                {
                    msgText.Text = "This device is not supported";
                    Finish();
                }
                return false;
            }
            else
            {
                msgText.Text = "Google Play Services is available.";
                return true;
            }
        }



        // called to ensure that a notification channel exists for devices running Android 8 or higher
        void CreateNotificationChannel()
        {
            if (Build.VERSION.SdkInt < BuildVersionCodes.O)
            {
                // Notification channels are new in API 26 (and not a part of the
                // support library). There is no need to create a notification
                // channel on older versions of Android.
                return;
            }

            var channel = new NotificationChannel(CHANNEL_ID,
                                                  "FCM Notifications",
                                                  NotificationImportance.Default)
            {

                Description = "Firebase Cloud Messages appear in this channel"
            };

            var notificationManager = (NotificationManager)GetSystemService(Android.Content.Context.NotificationService);
            notificationManager.CreateNotificationChannel(channel);
        }


    }
}
