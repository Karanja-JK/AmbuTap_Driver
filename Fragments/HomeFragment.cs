using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AmbuTap_Driver.Helpers;
using Android.App;
using Android.Content;
using Android.Gms.Location;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using static AmbuTap_Driver.Helpers.LocationCallbackHelper;

namespace AmbuTap_Driver.Fragments
{
    public class HomeFragment : Android.Support.V4.App.Fragment, IOnMapReadyCallback
    {


        // public event handler to send the location updates to main activity
        public EventHandler<OnLocationCapturedEventArgs> CurrentLocation;

        public class OnLocationCapturedEventArgs : EventArgs
        {
            public Android.Locations.Location Location { get; set; }
        }

        public GoogleMap mainMap; 


        // Marker
        ImageView centerMarker;


        // location client
        // this will involve our current and last locations and fuse that helps us change the location
        LocationRequest mLocationRequest;
        FusedLocationProviderClient locationProviderClient;
        Android.Locations.Location mLastLocation;


        // location callback that notifies this fragment of change in location
        LocationCallbackHelper mLocationCallback = new LocationCallbackHelper(); 

        static int UPDATE_INTERVAL = 5; // seconds
        static int FASTEST_INTERVAL = 5; // seconds
        static int DISPLACEMENT = 1; // meters


        // Accept Rider
        // Layout
        LinearLayout rideInfoLayout;


        // TextView
        TextView riderNameText;


        // Button
        ImageButton cancelTripButton;
        ImageButton callRiderButton;
        ImageButton navigateButton;
        Button tripButton;


        // Flag
        // check when trip is created
        bool tripCreated = false;
        bool driverArrived = false;
        bool tripStarted = false;


        // Events
        public event EventHandler TripActionArrived; 
        public event EventHandler TripActionStartTrip;
        public event EventHandler TripActionEndTrip;
        public event EventHandler CallRider;
        public event EventHandler Navigate;
 

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            CreateLocationRequest();
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
           View view = inflater.Inflate(Resource.Layout.home, container, false);
            SupportMapFragment mapFragment = (SupportMapFragment)ChildFragmentManager.FindFragmentById(Resource.Id.map);
            centerMarker = (ImageView)view.FindViewById(Resource.Id.centerMarker);
            mapFragment.GetMapAsync(this);


            // reference the accept ride views
            cancelTripButton = (ImageButton)view.FindViewById(Resource.Id.cancelTripButton);
            callRiderButton = (ImageButton)view.FindViewById(Resource.Id.callRiderButton);
            navigateButton = (ImageButton)view.FindViewById(Resource.Id.navigateButton);
            tripButton = (Button)view.FindViewById(Resource.Id.tripButton);
            riderNameText = (TextView)view.FindViewById(Resource.Id.riderNameText);
            rideInfoLayout = (LinearLayout)view.FindViewById(Resource.Id.rideInfoLayout);

            tripButton.Click += TripButton_Click;
            callRiderButton.Click += CallRiderButton_Click;
            navigateButton.Click += NavigateButton_Click;
            

            return view;
        }



        private void NavigateButton_Click(object sender, EventArgs e)
        {
            Navigate.Invoke(this, new EventArgs());
        }




        private void CallRiderButton_Click(object sender, EventArgs e)
        {
            CallRider.Invoke(this, new EventArgs());
        }




        private void TripButton_Click(object sender, EventArgs e)
        {
            if(!driverArrived && tripCreated)
            {
                driverArrived = true;
                TripActionArrived?.Invoke(this, new EventArgs());
                tripButton.Text = "Start Trip";
                return;
            }


            if(!tripStarted && driverArrived)
            {
                tripStarted = true;
                TripActionStartTrip.Invoke(this, new EventArgs());
                tripButton.Text = "End Trip";
                return;
            } 

            if(tripStarted)
            {
                TripActionEndTrip.Invoke(this, new EventArgs());
                return;
            }
        }



        public void OnMapReady(GoogleMap googleMap)
        {
            mainMap = googleMap;
        }


        // location request method
        void CreateLocationRequest()
        {
            mLocationRequest = new LocationRequest();
            mLocationRequest.SetInterval(UPDATE_INTERVAL);
            mLocationRequest.SetFastestInterval(FASTEST_INTERVAL);
            mLocationRequest.SetSmallestDisplacement(DISPLACEMENT);
            mLocationRequest.SetPriority(LocationRequest.PriorityHighAccuracy);
            mLocationCallback.MyLocation += MLocationCallback_MyLocation;
            locationProviderClient = LocationServices.GetFusedLocationProviderClient(Activity); // used activity since it is a fragment
            StartLocationUpdates();

        }

        // method to help grab the my location updates from the call back helper
        private void MLocationCallback_MyLocation(object sender, LocationCallbackHelper.OnLocationCapturedEventArgs e)
        {
            mLastLocation = e.Location;

            // update our last location on the map
            LatLng myPosition = new LatLng(mLastLocation.Latitude, mLastLocation.Longitude);
            mainMap.AnimateCamera(CameraUpdateFactory.NewLatLngZoom(myPosition, 17));

            // send location to mainactivity
            CurrentLocation?.Invoke(this, new OnLocationCapturedEventArgs { Location = e.Location });
        }


        // updates in any change in location 
        void StartLocationUpdates()
        {
            locationProviderClient.RequestLocationUpdates(mLocationRequest, mLocationCallback, null);
        }


        // stop location updates
        void StopLocationUpdates()
        {
            locationProviderClient.RemoveLocationUpdates(mLocationCallback);
        }


        // go online method
        public void GoOnline()
        {
            StartLocationUpdates();
            centerMarker.Visibility = ViewStates.Visible;
        }


        // go offline method
        public void GoOffline()
        {
            StopLocationUpdates();
            centerMarker.Visibility = ViewStates.Invisible;
        }


        // accept trip method
        public void CreateTrip(string ridername)
        {
            centerMarker.Visibility = ViewStates.Invisible;
            riderNameText.Text = ridername;
            rideInfoLayout.Visibility = ViewStates.Visible;
            tripCreated = true;
        }


        // Ending trip
        public void ResetAfterTrip()
        {
            tripButton.Text = "Arrived Pickup";
            centerMarker.Visibility = ViewStates.Visible;
            riderNameText.Text = "";
            rideInfoLayout.Visibility = ViewStates.Invisible;
            tripCreated = false;
            driverArrived = false;
            tripStarted = false;
            mainMap.Clear();
            mainMap.TrafficEnabled = false;
            mainMap.UiSettings.ZoomControlsEnabled = false;
        }
    }
}