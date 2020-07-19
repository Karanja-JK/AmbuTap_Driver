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
using Firebase.Auth;
using Firebase.Database;

namespace AmbuTap_Driver.EventListener 
{

    public class ProfileEventListener : Java.Lang.Object, IValueEventListener
    {

        ISharedPreferences preferences = Application.Context.GetSharedPreferences("userinfo", FileCreationMode.Private);
        ISharedPreferencesEditor editor;  // enables us to write new data to the shared preferences
        public void OnCancelled(DatabaseError error)
        {
       
        }

        public void OnDataChange(DataSnapshot snapshot)
        {
            if(snapshot.Value != null)
            {
                string fullname, email, phone;

                fullname = (snapshot.Child("fullname") != null) ? snapshot.Child("fullname").Value.ToString() : "";
                email = (snapshot.Child("email") != null) ? snapshot.Child("email").Value.ToString() : "";
                phone = (snapshot.Child("phone") != null) ? snapshot.Child("phone").Value.ToString() : "";

                // save the above data to shared preferences
                editor.PutString("fullname", fullname);
                editor.PutString("email", email);
                editor.PutString("phone", phone);
                editor.Apply();

            }
        }

        // on create method where we initialize the event handler
        public void Create()
        {
            editor = preferences.Edit();  // initialize the editor
            FirebaseDatabase database = AppDataHelper.GetDatabase();
            string driverID = AppDataHelper.GetCurrentUser().Uid;
            DatabaseReference driverRef = database.GetReference("drivers/" + driverID);
            driverRef.AddValueEventListener(this);
        }


    }
}