using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AmbuTap_Driver.Helpers;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Firebase.Auth;
using Firebase.Database;

namespace AmbuTap_Driver.Fragments
{
    public class AccountFragment : Android.Support.V4.App.Fragment
    {
        public static TextView mTxtEmail, mTxtName, mTxtPhone;
        private Button mBtnSignOut;
        private FirebaseUser firebaseUser;
        private FirebaseAuth mAuth;
        private FirebaseDatabase database;
        private FirebaseUser currentUser;

        ISharedPreferences preferences = Application.Context.GetSharedPreferences("userinfo", FileCreationMode.Private);
        ISharedPreferencesEditor editor;  // enables us to write new data to the shared preferences


        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            View view = inflater.Inflate(Resource.Layout.Account, container, false);

            mTxtEmail = (TextView)view.FindViewById(Resource.Id.profileEmail);
            mTxtName = (TextView)view.FindViewById(Resource.Id.profileName);
            mTxtPhone = (TextView)view.FindViewById(Resource.Id.profileNumber);
            mBtnSignOut = (Button)view.FindViewById(Resource.Id.dashboardLogout);



            if (AppDataHelper.GetCurrentUser() != null)
            {
                var user = AppDataHelper.GetCurrentUser();
                mTxtEmail.Text = user.Email;

            };

            database = AppDataHelper.GetDatabase();
            mAuth = AppDataHelper.GetFirebaseAuth();
            currentUser = AppDataHelper.GetCurrentUser();

            string driverID = AppDataHelper.GetCurrentUser().Uid;
            FirebaseDatabase.Instance.Reference.Child("drivers/" + driverID)
                .AddListenerForSingleValueEvent(new DataValueEventListener());

            mBtnSignOut.Click += MBtnSignOut_Click;

            return view;
        }

        private void MBtnSignOut_Click(object sender, EventArgs e)
        {
            // sign-out user
            mAuth.SignOut();
            Activity.FinishAffinity();
            var intent = new Intent(Activity, typeof(Activities.LoginActivity));
            StartActivity(intent);
        }

        public static string fullname, email, phone;
        class DataValueEventListener : Java.Lang.Object, IValueEventListener
        {
            public void OnCancelled(DatabaseError error)
            {

            }

            public void OnDataChange(DataSnapshot snapshot)
            {
                if (snapshot.Exists())
                {
                    fullname = (snapshot.Child("fullname") != null) ? snapshot.Child("fullname").Value.ToString() : "";
                    email = (snapshot.Child("email") != null) ? snapshot.Child("email").Value.ToString() : "";
                    phone = (snapshot.Child("phone") != null) ? snapshot.Child("phone").Value.ToString() : "";

                    mTxtName.Text = fullname;
                    mTxtPhone.Text = phone;
                }
                   
            }
        }
    }
}