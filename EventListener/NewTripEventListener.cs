using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AmbuTap_Driver.Helpers;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using  Firebase.Database;

namespace AmbuTap_Driver.EventListener
{
    public class NewTripEventListener : Java.Lang.Object, IValueEventListener
    {
        string mRideID;
        Android.Locations.Location mLastLocation;
        FirebaseDatabase database;
        DatabaseReference tripRef;

        // Flag
        bool isAccepted;


        public NewTripEventListener(string ride_id, Android.Locations.Location lastlocation)
        {
            mRideID = ride_id;
            mLastLocation = lastlocation;
            database = AppDataHelper.GetDatabase();
        }

        public void OnCancelled(DatabaseError error)
        {
            
        }


        // on the trip being accepted
        public void OnDataChange(DataSnapshot snapshot)
        {
            if(snapshot.Value != null)
            {
                if(!isAccepted)
                {
                    isAccepted = true;
                    Accept();
                }
            }
        }


        // initialize trip event listener
        public void Create()
        {
            tripRef = database.GetReference("AmbulanceRequest/" + mRideID);
            tripRef.AddValueEventListener(this);
        }


        // on the trip being accepted 
        void Accept()
        {
            tripRef.Child("status").SetValue("accepted");
            tripRef.Child("driver_name").SetValue(AppDataHelper.GetFullName());
            tripRef.Child("driver_phone").SetValue(AppDataHelper.GetPhone());
            tripRef.Child("driver_location").Child("latitude").SetValue(mLastLocation.Latitude);
            tripRef.Child("driver_location").Child("longitude").SetValue(mLastLocation.Longitude);
            tripRef.Child("driver_id").SetValue(AppDataHelper.GetCurrentUser().Uid);
        }

        // update location
        public void UpdateLocation(Android.Locations.Location lastlocation)
        {
            mLastLocation = lastlocation;
            tripRef.Child("driver_location").Child("latitude").SetValue(mLastLocation.Latitude);
            tripRef.Child("driver_location").Child("longitude").SetValue(mLastLocation.Longitude);
        }

        // Update rider driver coming
        public void UpdateStatus(string status)
        {
            tripRef.Child("status").SetValue(status);
        }

        // End trip
        public void EndTrip (double fares)
        {
            if(tripRef != null)
            {
                tripRef.Child("fares").SetValue(fares);
                tripRef.Child("status").SetValue("ended");
                tripRef.RemoveEventListener(this);
                tripRef = null;
            }
        }
    }
}