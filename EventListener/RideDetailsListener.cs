using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AmbuTap_Driver.DataModels;
using AmbuTap_Driver.Helpers;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Firebase.Database;

namespace AmbuTap_Driver.EventListener
{
    public  class RideDetailsListener : Java.Lang.Object, IValueEventListener
    {

        // Eventhandlers
        public class RideDetailsEventArgs : EventArgs
        {
            public RideDetails RideDetails { get; set; }
        }
        public event EventHandler<RideDetailsEventArgs> RideDetailsFound;
        public event EventHandler RideDetailsNotFound;



        public void OnCancelled(DatabaseError error) 
        {
            
        }


        /*
        public void OnDataChange(DataSnapshot snapshot)
        {
            if(snapshot.Value != null)
            {
                RideDetails rideDetails = new RideDetails();
                rideDetails.DestinationAddress = snapshot.Child("destination_address").Value.ToString();
                rideDetails.DestinationLat = (double)snapshot.Child("destination").Child("latitude").Value;
                rideDetails.DestinationLng = (double)snapshot.Child("destination").Child("longitude").Value;

                //(double)snapshot.Child("destination").Child("latitude").Value;
                //double.Parse(snapshot.Child("destination").Child("longitude").Value.ToString());
                //double.Parse(snapshot.Child("destination").Child("latitude").Value.ToString());
                //double.Parse(snapshot.Child("destination").Child("longitude").Value.ToString());

                rideDetails.PickupAddress = snapshot.Child("pickup_address").Value.ToString();
                rideDetails.PickupLat = (double)snapshot.Child("location").Child("latitude").Value;
                rideDetails.PickupLng = (double)snapshot.Child("location").Child("longitude").Value;

                //(double)snapshot.Child("location").Child("latitude").Value;
                //double.Parse(snapshot.Child("location").Child("latitude").Value.ToString());

                rideDetails.RideID = snapshot.Key;
                rideDetails.RiderName = snapshot.Child("rider_name").Value.ToString();
                rideDetails.RiderPhone = snapshot.Child("rider_phone").Value.ToString();
                RideDetailsFound?.Invoke(this, new RideDetailsEventArgs { RideDetails = rideDetails }); // invoke in Main Acivity

            }
            else
            {
                RideDetailsNotFound?.Invoke(this, new EventArgs());
            }
        }
        */

        public void OnDataChange(DataSnapshot snapshot)
        {
            if (snapshot.Value != null)
            {
                RideDetails rideDetails = new RideDetails();
                if (snapshot.Child("destination").Child("latitude").Value != null)
                {
                    rideDetails.DestinationLat = double.Parse(snapshot.Child("destination").Child("latitude").Value.ToString());
                }
                if (snapshot.Child("destination").Child("longitude").Value != null)
                {
                    rideDetails.DestinationLng = double.Parse(snapshot.Child("destination").Child("longitude").Value.ToString());
                }

                rideDetails.DestinationAddress = snapshot.Child("destination_address").Value.ToString();



                if (snapshot.Child("location").Child("latitude").Value != null)
                {
                    rideDetails.PickupLat = double.Parse(snapshot.Child("location").Child("latitude").Value.ToString());
                }
                if (snapshot.Child("location").Child("longitude").Value != null)
                {
                    rideDetails.PickupLng = double.Parse(snapshot.Child("location").Child("longitude").Value.ToString());
                }

                rideDetails.PickupAddress = snapshot.Child("pickup_address").Value.ToString();


                rideDetails.RideID = snapshot.Key;
                rideDetails.RiderName = snapshot.Child("rider_name").Value.ToString();
                rideDetails.RiderPhone = snapshot.Child("rider_phone").Value.ToString();
                RideDetailsFound?.Invoke(this, new RideDetailsEventArgs { RideDetails = rideDetails }); // invoke in Main Acivity

            }
            else
            {
                RideDetailsNotFound?.Invoke(this, new EventArgs());
            }
        }


        public void Create(string ride_id)
        {
            FirebaseDatabase database = AppDataHelper.GetDatabase();
            DatabaseReference rideDetailsRef = database.GetReference("AmbulanceRequest/" + ride_id);
            rideDetailsRef.AddListenerForSingleValueEvent(this);
        }
    }
}