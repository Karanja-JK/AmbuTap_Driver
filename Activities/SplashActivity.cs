﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using AmbuTap_Driver.Helpers;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Firebase.Auth;

namespace AmbuTap_Driver.Activities
{
    [Activity(Label = "AT-Driver", Theme = "@style/MyTheme.Splash", MainLauncher = true)]
    public class SplashActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Thread.Sleep(100);

            // Create your application here
        }

        protected override void OnResume()
        {
            base.OnResume();

            // set user sepcifications
            FirebaseUser currentUser = AppDataHelper.GetCurrentUser();

            if (currentUser == null)
            {
                StartActivity(typeof(LoginActivity));
            }
            else
            {
                StartActivity(typeof(RegistrationActivity));
            }
        }
    }
}

