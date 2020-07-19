using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using Android.Support.V4.View;
using Com.Ittianyu.Bottomnavigationviewex;
using System;
using AmbuTap_Driver.Adapter;
using AmbuTap_Driver.Fragments;
using Android.Graphics;
using System.Drawing;
using Android;
using Android.Support.V4.App;
using AmbuTap_Driver.EventListener;
using Android.Gms.Maps.Model;
using Android.Gms.Location;
using AmbuTap_Driver.Helpers;
using static AmbuTap_Driver.Helpers.LocationCallbackHelper;
using Android.Support.V4.Content;
using AmbuTap_Driver.DataModels;
using Android.Media;
using Xamarin.Essentials;
using Android.Content;
using Firebase.Database;
using Firebase;

namespace AmbuTap_Driver
{
    [Activity(Label = "@string/app_name", Theme = "@style/AmbuTheme", MainLauncher = false)]
    public class MainActivity : AppCompatActivity
    {

        //this is the pic pdf code used in file chooser
        public static int PICK_PDF_CODE = 2342;

        //these are the views
        TextView textViewStatus;
        TextView nationalIdentificationFile;
        ProgressBar progressBar;

        //the firebase objects for storage and database
        DatabaseReference mDatabaseReference;
        


        // views
        ViewPager viewPager;
        BottomNavigationViewEx bnve;


        // Fragments
        HomeFragment homeFragment = new HomeFragment();
        //RatingFragment ratingFragment = new RatingFragment();
        AccountFragment accountFragment = new AccountFragment();
        EarningsFragment earningsFragment = new EarningsFragment();
        NewRequestFragment requestFoundDialogue;


        // PermissionsRequest
        const int RequestID = 0;
        readonly string[] permissionsGroup =
        {
            Manifest.Permission.AccessCoarseLocation,
            Manifest.Permission.AccessFineLocation,
        };


        // EventListener
        ProfileEventListener ProfileEventListener = new ProfileEventListener();
        AvailabilityListener availabilityListener;
        RideDetailsListener rideDetailsListener;
        NewTripEventListener newTripEventListener;


        // Helper
        MapFunctionHelper mapHelper;


        // Last location on Map
        Android.Locations.Location mLastLocation;
        LatLng mLastLatLng;


        // Buttons
        public static Button goOnlineButton;


        // Flags
        bool availabilityStatus;
        bool isBackground;
        bool newRideAssigned;
        string status = "NORMAL";  // request found, accepted, ontrip


        // Data Models
        RideDetails newRideDetails;


        // MediaPlayer
        MediaPlayer player;


        Android.Support.V7.App.AlertDialog.Builder alert;
        Android.Support.V7.App.AlertDialog alertDialog;


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);
            
            ConnectViews();
            CheckSpecialPermission();
            ProfileEventListener.Create();


            //crashlytics
            //Fabric.Fabric.With(this, new Crashlytics.Crashlytics());
            //Crashlytics.Crashlytics.HandleManagedExceptions();


        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }



        void ShowProgressDialogue()
        {
            alert = new Android.Support.V7.App.AlertDialog.Builder(this);
            alert.SetView(Resource.Layout.progress);
            alert.SetCancelable(false);
            alertDialog = alert.Show();
        }

        void CloseProgressDialogue()
        {
            if (alert != null)
            {
                alertDialog.Dismiss();
                alertDialog = null;
                alert = null;
            }
        }



        void ConnectViews()
        {
            goOnlineButton = (Button)FindViewById(Resource.Id.goOnlineButton);      // the go online button
            bnve = (BottomNavigationViewEx)FindViewById(Resource.Id.bnve);
            bnve.EnableItemShiftingMode(false);
            bnve.EnableShiftingMode(false);


            // ----- CHECK DRIVER APPROVAL STATUS ------
            string driverID = AppDataHelper.GetCurrentUser().Uid;
            FirebaseDatabase.Instance.Reference.Child("driverApplications/" + driverID)
               .AddValueEventListener(new DataValueEventListener());


            goOnlineButton.Click += GoOnlineButton_Click;
            bnve.NavigationItemSelected += Bnve_NavigationItemSelected;

            BnveToAccentColor(0);

            viewPager = (ViewPager)FindViewById(Resource.Id.viewPager);
            viewPager.OffscreenPageLimit = 3;   
            viewPager.BeginFakeDrag();

            SetupViewPager();


            homeFragment.CurrentLocation += HomeFragment_CurrentLocation;
            homeFragment.TripActionArrived += HomeFragment_TripActionArrived;
            homeFragment.CallRider += HomeFragment_CallRider;
            homeFragment.Navigate += HomeFragment_Navigate;
            homeFragment.TripActionStartTrip += HomeFragment_TripActionStartTrip;
            homeFragment.TripActionEndTrip += HomeFragment_TripActionEndTrip;
        }



        // Driver Registration Status
        public static string fullname, email, phone, driverStatus;
        class DataValueEventListener : Java.Lang.Object, IValueEventListener
        {
            public void OnCancelled(DatabaseError error)
            {

            }

            public void OnDataChange(DataSnapshot snapshot)
            {

                if (snapshot.Exists())
                {
                    driverStatus = (snapshot.Child("status") != null) ? snapshot.Child("status").Value.ToString() : "";

                    if (driverStatus.Equals("pending"))
                    {
                        goOnlineButton.Visibility = Android.Views.ViewStates.Gone;
                    }
                    else
                    {
                        goOnlineButton.Visibility = Android.Views.ViewStates.Visible;
                    }
                }

            }
        }



        // End Trip
        async void HomeFragment_TripActionEndTrip(object sender, EventArgs e)
        {
            // Reset App
            status = "NORMAL";
            homeFragment.ResetAfterTrip();

            ShowProgressDialogue();
            LatLng pickupLatLng = new LatLng(newRideDetails.PickupLat, newRideDetails.PickupLng);
            double fares = await mapHelper.CalculateFares(pickupLatLng, mLastLatLng);
            CloseProgressDialogue();

            newTripEventListener.EndTrip(fares);
            newTripEventListener = null;
            

            // collect payment
            CollectPaymentFragment collectPaymentFragment = new CollectPaymentFragment(fares);
            collectPaymentFragment.Cancelable = false;
            var trans = SupportFragmentManager.BeginTransaction();
            collectPaymentFragment.Show(trans, "pay"); 
            collectPaymentFragment.PaymentCollected += (o, u)   =>
            {
                collectPaymentFragment.Dismiss();
            };

            // Reactivate app once done
            availabilityListener.ReActivate();

        }



        private void HomeFragment_TripActionStartTrip(object sender, EventArgs e)
        {
            Android.Support.V7.App.AlertDialog.Builder startTripAlert = new Android.Support.V7.App.AlertDialog.Builder(this);
            startTripAlert.SetTitle("START TRIP");
            startTripAlert.SetMessage("Are you sure?");
            startTripAlert.SetPositiveButton("Continue", (senderAlert, args) =>
            {
                status = "ONTRIP";

                // Update Rider that driver has started trip
                newTripEventListener.UpdateStatus("ontrip");
            });

            startTripAlert.SetNegativeButton("Cancel", (senderAlert, args) =>
            {
                startTripAlert.Dispose();
            });

            startTripAlert.Show();
        }


        // navigate direction to destination
        private void HomeFragment_Navigate(object sender, EventArgs e)
        {
            string uriString = "";

            if(status == "accepted")
            {
                uriString = "google.navigation:q=" + newRideDetails.PickupLat.ToString() + " , " + newRideDetails.PickupLng.ToString();
            }
            else
            {
                uriString = "google.navigation:q=" + newRideDetails.DestinationLat.ToString() + " , " + newRideDetails.DestinationLng.ToString();
            }

            Android.Net.Uri googleMapIntentUri = Android.Net.Uri.Parse(uriString);
            Intent mapIntent = new Intent(Intent.ActionView, googleMapIntentUri);
            mapIntent.SetPackage("com.google.android.apps.maps");


            try
            {
                StartActivity(mapIntent);
            }
            catch
            {
                Toast.MakeText(this, "Google Map not installed on this device", ToastLength.Short).Show();
            }
        }



        // call rider on arrival
        void HomeFragment_CallRider(object sender, EventArgs e)
        {
            var uri = Android.Net.Uri.Parse("tel:" + newRideDetails.RiderPhone);
            Intent intent = new Intent(Intent.ActionDial, uri);
            StartActivity(intent);


        }


    
        // Driver Arrived
        async void HomeFragment_TripActionArrived(object sender, EventArgs e)
        {
            // Notify rider driver has arrived
            newTripEventListener.UpdateStatus("arrived");
            status = "ARRIVED";

            LatLng pickupLatLng = new LatLng(newRideDetails.PickupLat, newRideDetails.PickupLng);
            LatLng destinationLatLng = new LatLng(newRideDetails.DestinationLat, newRideDetails.DestinationLng);

            ShowProgressDialogue();
            string directionJson = await mapHelper.GetDirectionJsonAsync(pickupLatLng, destinationLatLng);
            CloseProgressDialogue();

            // Clear Map
            homeFragment.mainMap.Clear();
            mapHelper.DrawTripToDestination(directionJson);  
        }



        // Driver's current location
        private void HomeFragment_CurrentLocation(object sender, HomeFragment.OnLocationCapturedEventArgs e)
        {
            mLastLocation = e.Location;
            mLastLatLng = new LatLng(mLastLocation.Latitude, mLastLocation.Longitude);

            // check if driver is online to update his/her location
            if (availabilityListener != null)
            {
                availabilityListener.UpdateLocation(mLastLocation);
            }

            if(availabilityStatus && availabilityListener == null)
            {
                TakeDriverOnline();
            }

            // picking patient
            if (status == "ACCEPTED")
            {
                // update and animate driver movement to location
                LatLng pickupLatLng = new LatLng(newRideDetails.PickupLat, newRideDetails.PickupLng);
                mapHelper.UpdateMovement(mLastLatLng, pickupLatLng, "Rider");

                // Update location in rideRequest table so that rider can receive updates
                newTripEventListener.UpdateLocation(mLastLocation);
            }
            else if(status == "ARRIVED")
            {
                newTripEventListener.UpdateLocation(mLastLocation);
            }
            else if(status == "ONTRIP")
            {
                // Update and animate driver movement to destination
                LatLng destinationLatLng = new LatLng(newRideDetails.DestinationLat, newRideDetails.DestinationLng);
                mapHelper.UpdateMovement(mLastLatLng, destinationLatLng, "Destination");

                // Update Location in ride request Table, so that rider receives updates
                newTripEventListener.UpdateLocation(mLastLocation);
            }

        }



        // method to take driver online
        private void TakeDriverOnline()
        {
            availabilityListener = new AvailabilityListener();
            availabilityListener.Create(mLastLocation);

            // receiving ride request
            availabilityListener.RideCancelled += AvailabilityListener_RideCancelled;
            availabilityListener.RideTimedOut += AvailabilityListener_RideTimedOut;
            availabilityListener.RideAssigned += AvailabilityListener_RideAssigned;
        }



        // Ride assigned event handler
        // called from AvailabilityListener
        void AvailabilityListener_RideAssigned(object sender, AvailabilityListener.RideAssignedIDEventArgs e)
        {

            // Get rider details
            // called from ride details listener
            rideDetailsListener = new RideDetailsListener();
            rideDetailsListener.Create(e.RideId);
            rideDetailsListener.RideDetailsFound += RideDetailsListener_RideDetailsFound;
            rideDetailsListener.RideDetailsNotFound += RideDetailsListener_RideDetailsNotFound;
        }
        


        void RideDetailsListener_RideDetailsNotFound(object sender, EventArgs e)
        {
            
        }


        void RideDetailsListener_RideDetailsFound(object sender, RideDetailsListener.RideDetailsEventArgs e)
        {
            // if status not normal, it effects the code block
            if (status != "NORMAL")
            {
                return;
            }
                
            newRideDetails = e.RideDetails;

            // Alert when on background
            if (!isBackground)
            {
                CreateNewRequestDialogue();
            }
            else
            {
                newRideAssigned = true;
                NotificationHelper notificationHelper = new NotificationHelper();
                if ((int)Build.VERSION.SdkInt >= 26)
                {
                    notificationHelper.NotifyVersion26(this, Resources, (NotificationManager)GetSystemService(NotificationService));
                }
            }
        }



        // Ride Notification when on background
        void CreateNewRequestDialogue()
        {
            requestFoundDialogue = new NewRequestFragment(newRideDetails.PickupAddress, newRideDetails.DestinationAddress);
            requestFoundDialogue.Cancelable = false;
            var trans = SupportFragmentManager.BeginTransaction();
            requestFoundDialogue.Show(trans, "Request");

            // Play Alert
            player = MediaPlayer.Create(this, Resource.Raw.alert);
            player.Start();

            requestFoundDialogue.RideRejected += RequestFoundDialogue_RideRejected;
            requestFoundDialogue.RideAccepted += RequestFoundDialogue_RideAccepted;
            
        }



        async void RequestFoundDialogue_RideAccepted(object sender, EventArgs e)
        {

            newTripEventListener = new NewTripEventListener(newRideDetails.RideID, mLastLocation);
            newTripEventListener.Create();

            status = "accepted";

            // Stop Alert
            if (player != null)
            {
                player.Stop();
                player = null;
            }

            // Dismiss Dialogue
            if (requestFoundDialogue != null)
            {
                requestFoundDialogue.Dismiss();
                requestFoundDialogue = null;
            }

            homeFragment.CreateTrip(newRideDetails.RiderName);


            // polyline on map
            mapHelper = new MapFunctionHelper(Resources.GetString(Resource.String.mapkey), homeFragment.mainMap);
            LatLng pickupLatLng = new LatLng(newRideDetails.PickupLat, newRideDetails.PickupLng);

            ShowProgressDialogue();
            string directionJson = await mapHelper.GetDirectionJsonAsync(mLastLatLng, pickupLatLng);
            CloseProgressDialogue();

            mapHelper.DrawTripOnMap(directionJson);
        }



        private void RequestFoundDialogue_RideRejected(object sender, EventArgs e)
        {

            // Stop Alert
            if(player != null)
            {
                player.Stop();
                player = null;
            }

            // Dismiss Dialogue
            if(requestFoundDialogue != null)
            {
                requestFoundDialogue.Dismiss();
                requestFoundDialogue = null;
            }

            availabilityListener.ReActivate();

        }




        // Ride timed out event handler
        void AvailabilityListener_RideTimedOut(object sender, EventArgs e)
        {
            // avoid alert playing
            if(requestFoundDialogue != null)
            {
                requestFoundDialogue.Dismiss();
                requestFoundDialogue = null;
                player.Stop();
                player = null;
            }
                
            Toast.MakeText(this, "New trip Timeout", ToastLength.Short).Show();
            availabilityListener.ReActivate();
        }




        // Ride cancelled event handler
        void AvailabilityListener_RideCancelled(object sender, EventArgs e)
        {
            // avoid alert playing
            if (requestFoundDialogue != null)
            {
                requestFoundDialogue.Dismiss();
                requestFoundDialogue = null;
                player.Stop();
                player = null;
            }

            Toast.MakeText(this, "New trip was Cancelled", ToastLength.Short).Show();
            availabilityListener.ReActivate();
        }




        // method to take the driver offline
        private void TakeDriverOffline()
        {
            availabilityListener.RemoveListener();
            availabilityListener = null;
        }




        // go online button click event handler
        private void GoOnlineButton_Click(object sender, EventArgs e)
        {
            if (!CheckSpecialPermission())
            {
                return;
            }


            // to take driver offline
            if (availabilityStatus)
            {
                Android.Support.V7.App.AlertDialog.Builder alert = new Android.Support.V7.App.AlertDialog.Builder(this);
                alert.SetTitle("Go Offline");
                alert.SetMessage("You will not receive new ambulance requests");
                alert.SetPositiveButton("Continue", (senderAlert, args) =>
                {
                    homeFragment.GoOffline();
                    goOnlineButton.Text = "Go Online";
                    goOnlineButton.Background = ContextCompat.GetDrawable(this, Resource.Drawable.roundbutton_online);

                    availabilityStatus = false;
                    TakeDriverOffline();

                });


                alert.SetNegativeButton("Cancel", (senderAlert, args) =>
                {
                    alert.Dispose();

                });
                alert.Show();
                
            }
            else
            {
                availabilityStatus = true;
                goOnlineButton.Text = "Go Offline";
                homeFragment.GoOnline();
                goOnlineButton.Background = ContextCompat.GetDrawable(this, Resource.Drawable.roundbutton_offline);
            }

        }




        // method for switching pages
        private void Bnve_NavigationItemSelected(object sender, Android.Support.Design.Widget.BottomNavigationView.NavigationItemSelectedEventArgs e)
        {
           if(e.Item.ItemId == Resource.Id.action_earning)
            {
                viewPager.SetCurrentItem(1, true);
                BnveToAccentColor(1);
            }
           else if(e.Item.ItemId == Resource.Id.action_home)
            {
                viewPager.SetCurrentItem(0, true);
                BnveToAccentColor(0);
            }
           /*
            else if (e.Item.ItemId == Resource.Id.action_rating)
            {
                viewPager.SetCurrentItem(2, true);
                BnveToAccentColor(2);
            }
           */
            else if (e.Item.ItemId == Resource.Id.action_account)
            {
                viewPager.SetCurrentItem(3, true);
                BnveToAccentColor(3);
            }
        }




        // change color on selection
        void BnveToAccentColor (int index)
        {
            // set all to white
            var img = bnve.GetIconAt(1);
            var txt = bnve.GetLargeLabelAt(1);
            img.SetColorFilter(Android.Graphics.Color.Rgb(255, 255, 255));
            txt.SetTextColor(Android.Graphics.Color.Rgb(255, 255, 255));

            var img0 = bnve.GetIconAt(0);
            var txt0 = bnve.GetLargeLabelAt(0);
            img.SetColorFilter(Android.Graphics.Color.Rgb(255, 255, 255));
            txt.SetTextColor(Android.Graphics.Color.Rgb(255, 255, 255));

            //var img2 = bnve.GetIconAt(2);
            //var txt2 = bnve.GetLargeLabelAt(2);
            //img.SetColorFilter(Android.Graphics.Color.Rgb(255, 255, 255));
            //txt.SetTextColor(Android.Graphics.Color.Rgb(255, 255, 255));

            var img3 = bnve.GetIconAt(3);
            var txt3 = bnve.GetLargeLabelAt(3);
            img.SetColorFilter(Android.Graphics.Color.Rgb(255, 255, 255));
            txt.SetTextColor(Android.Graphics.Color.Rgb(255, 255, 255));


            // sets accent color
            var imgindex = bnve.GetIconAt(index);
            var textindex = bnve.GetLargeLabelAt(index);
            //imgindex.SetColorFilter(Android.Graphics.Color.Rgb(24, 191, 242));
            //textindex.SetTextColor(Android.Graphics.Color.Rgb(24, 191, 242));

        }



        private void SetupViewPager()
        {
            ViewPagerAdapter adapter = new ViewPagerAdapter(SupportFragmentManager);
            adapter.AddFragment(homeFragment, "Home");
            adapter.AddFragment(earningsFragment, "Earnings");
            //adapter.AddFragment(ratingFragment, "Ratings");
            adapter.AddFragment(accountFragment, "Account");
            viewPager.Adapter = adapter;
        }




        // check for permission
        bool CheckSpecialPermission()
        {
            bool permissionGranted = false;
            if(ActivityCompat.CheckSelfPermission(this, Manifest.Permission.AccessFineLocation) != Android.Content.PM.Permission.Granted &&
                ActivityCompat.CheckSelfPermission(this, Manifest.Permission.AccessCoarseLocation) != Android.Content.PM.Permission.Granted)
            {
                RequestPermissions(permissionsGroup, RequestID);
            }
            else
            {
                permissionGranted = true;
            }

            return permissionGranted;
        }




        // is background flags
        protected override void OnPause()
        {
            isBackground = true;
            base.OnPause();
        }

        
        protected override void OnResume()
        {
            isBackground = false;
            if(newRideAssigned)
            {
                CreateNewRequestDialogue();
                newRideAssigned = false;
            }
            base.OnResume();
        }


        // The Files Upload

    }
}

