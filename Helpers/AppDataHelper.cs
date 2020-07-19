using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Firebase;
using Firebase.Auth;
using Firebase.Database;

namespace AmbuTap_Driver.Helpers
{

    public static class AppDataHelper
    {
        static ISharedPreferences pref = Application.Context.GetSharedPreferences("userinfo", FileCreationMode.Private);
        public static FirebaseDatabase GetDatabase()
        {
            var app = FirebaseApp.InitializeApp(Application.Context);
            FirebaseDatabase database;
            if (app == null)
            {

                var options = new FirebaseOptions.Builder()

                    .SetApplicationId("ambutap - fd06c")
                    .SetApiKey("AIzaSyB130IeurG8F7hM-ky1S8J2-7q6lUlTkQQ")
                    .SetDatabaseUrl("https://ambutap-fd06c.firebaseio.com")
                    .SetStorageBucket("ambutap-fd06c.appspot.com")
                    .Build();

                app = FirebaseApp.InitializeApp(Application.Context, options);
                database = FirebaseDatabase.GetInstance(app);   // reads data to the database     
            }
            else
            {
                database = FirebaseDatabase.GetInstance(app);
            }
            return database;
        }


        // Authentication of users
        public static FirebaseAuth GetFirebaseAuth()
        {
            var app = FirebaseApp.InitializeApp(Application.Context);
            FirebaseAuth mAuth;
            if (app == null)
            {

                var options = new FirebaseOptions.Builder()

                    .SetApplicationId("ambutap - fd06c")
                    .SetApiKey("AIzaSyB130IeurG8F7hM-ky1S8J2-7q6lUlTkQQ")
                    .SetDatabaseUrl("https://ambutap-fd06c.firebaseio.com")
                    .SetStorageBucket("ambutap-fd06c.appspot.com")
                    .Build();

                app = FirebaseApp.InitializeApp(Application.Context, options);
                mAuth = FirebaseAuth.Instance;   // reads data to the database     
            }
            else
            {
                // if account exists, return the exisiting account
                mAuth = FirebaseAuth.Instance;
            }

            return mAuth;
        }


        // grab instance of current users
        // same as authentication
        // just need to add instnce of the firebase user
        public static FirebaseUser GetCurrentUser()
        {
            var app = FirebaseApp.InitializeApp(Application.Context);
            FirebaseAuth mAuth;
            FirebaseUser mUser;
            if (app == null)
            {

                var options = new FirebaseOptions.Builder()

                    .SetApplicationId("ambutap - fd06c")
                    .SetApiKey("AIzaSyB130IeurG8F7hM-ky1S8J2-7q6lUlTkQQ")
                    .SetDatabaseUrl("https://ambutap-fd06c.firebaseio.com")
                    .SetStorageBucket("ambutap-fd06c.appspot.com")
                    .Build();

                app = FirebaseApp.InitializeApp(Application.Context, options);
                mAuth = FirebaseAuth.Instance;   // reads data to the database
                mUser = mAuth.CurrentUser;
            }
            else
            {
                // if account exists, return the exisiting account
                mAuth = FirebaseAuth.Instance;
                mUser = mAuth.CurrentUser;
            }

            return mUser;
        }

        // want to be able to retrieve data at any given point
        // method to get our full name from the local storage = shared preferences
        public static string GetFullName()
        {
            string fullname = pref.GetString("fullname", "");
            return fullname;
        }

        // get email
        public static string GetEmail()
        {
            string email = pref.GetString("email", "");
            return email;
        }

        // get phone number
        public static string GetPhone()
        {
            string phone = pref.GetString("phone", "");
            return phone;
        }
    }
}