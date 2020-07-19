using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Firebase.Annotations;

namespace AmbuTap_Driver.Fragments
{
    public class NewRequestFragment : Android.Support.V4.App.DialogFragment
    {
        // Events
        // used to communicate the buttons click events to main activity
        public event EventHandler RideAccepted;
        public event EventHandler RideRejected;


        // Views
        RelativeLayout acceptRideButton;
        RelativeLayout rejectRideButton;
        TextView pickupAddressText;
        TextView destinationAddressText;

        string mPickupAddress;
        string mDestinationAddress;

 
        // constructor
        public NewRequestFragment(string PickupAddress, string DestinationAddress)
        {
            mPickupAddress = PickupAddress;
            mDestinationAddress = DestinationAddress;
        }


        public override void OnCreate(Bundle savedInstanceState)
        {

            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            View view = inflater.Inflate(Resource.Layout.newrequest_dialogue, container, false);

            pickupAddressText = (TextView)view.FindViewById(Resource.Id.newRidePickupText);
            destinationAddressText = (TextView)view.FindViewById(Resource.Id.newRideDestinationText);

            pickupAddressText.Text = mPickupAddress;
            destinationAddressText.Text = mDestinationAddress;

            acceptRideButton = (RelativeLayout)view.FindViewById(Resource.Id.acceptRideButton);
            rejectRideButton = (RelativeLayout)view.FindViewById(Resource.Id.rejectRideButton);

            // click event handlers for the buttons
            acceptRideButton.Click += AcceptRideButton_Click;
            rejectRideButton.Click += RejectRideButton_Click;

            return view;
        }


        private void RejectRideButton_Click(object sender, EventArgs e)
        {
            // invoke the ride rejected event
            RideRejected?.Invoke(this, new EventArgs());

        }

        private void AcceptRideButton_Click(object sender, EventArgs e)
        {
            // invoke the ride accepted event
            RideAccepted?.Invoke(this, new EventArgs());
        }
    }
}