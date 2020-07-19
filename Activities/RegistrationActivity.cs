using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using AmbuTap_Driver.EventListener;
using AmbuTap_Driver.Helpers;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Firebase.Auth;
using Firebase.Database;
using Java.Util;

namespace AmbuTap_Driver.Activities
{
    [Activity(Label = "RegistrationActivity", MainLauncher =false, Theme = "@style/AppTheme")]
    public class RegistrationActivity : AppCompatActivity
    {

        // reference our reigister button here
        TextInputLayout fullNameText;
        TextInputLayout phoneText;
        TextInputLayout emailText;
        TextInputLayout passwordText;
        Button registerButton;
        TextView loginText;
        CoordinatorLayout rootView; // call the rootview from register.xml
        


        // instance of firebase
        FirebaseDatabase database;
        FirebaseAuth mAuth;
        FirebaseUser currentUser;


        // call the task listener
        TaskCompletionListener taskCompletionListener = new TaskCompletionListener();


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Create your application here

            SetContentView(Resource.Layout.register);
            ConnectViews();
            SetupFirebase();
        }



        // method to call database values
        void SetupFirebase()
        {
            database = AppDataHelper.GetDatabase();
            mAuth = AppDataHelper.GetFirebaseAuth();
            currentUser = AppDataHelper.GetCurrentUser();
        }



        // method to refrence views
        void ConnectViews()
        {
            fullNameText = (TextInputLayout)FindViewById(Resource.Id.fullNameText);
            phoneText = (TextInputLayout)FindViewById(Resource.Id.phoneText);
            emailText = (TextInputLayout)FindViewById(Resource.Id.emailText);
            loginText = (TextView)FindViewById(Resource.Id.loginText);
            //Console.WriteLine(emailText);
            passwordText = (TextInputLayout)FindViewById(Resource.Id.passwordText);
            rootView = (CoordinatorLayout)FindViewById(Resource.Id.rootView);
            registerButton = (Button)FindViewById(Resource.Id.registerButton);

            // register button event handler
            registerButton.Click += RegisterButton_Click;
            loginText.Click += LoginText_Click;
        }

        private void LoginText_Click(object sender, EventArgs e)
        {
            StartActivity(typeof(LoginActivity));
        }

        // register button event handler 
        private void RegisterButton_Click(object sender, EventArgs e)
        {
            string fullname, email, phone, password;

            // holders for the information provided by the user
            fullname = fullNameText.EditText.Text;
            phone = phoneText.EditText.Text;
            email = emailText.EditText.Text;
            password = passwordText.EditText.Text;


            // validation of user details
            if (fullname.Length < 3)
            {
                Snackbar.Make(rootView, "Please enter a valid name", Snackbar.LengthShort).Show();
                return;
            }
            else if (phone.Length < 10)
            {
                Snackbar.Make(rootView, "Please enter a valid phone number", Snackbar.LengthShort).Show();
                return;
            }
            else if (!email.Contains("@gmail.com, @uonbi.ac.ke, @students.uonbi.ac.ke"))
            {
                try
                {
                    MailAddress m = new MailAddress(email);
                    
                }
                catch (FormatException)
                {
                    Snackbar.Make(rootView, "Please enter a valid email address", Snackbar.LengthShort).Show();
                    return;
                }

            }
            else if (password.Length < 8)
            {
                Snackbar.Make(rootView, "Please enter a password of upto 8 characters", Snackbar.LengthShort).Show();
                return;
            }

            // register driver
            mAuth.CreateUserWithEmailAndPassword(email, password)
                .AddOnSuccessListener(this, taskCompletionListener)
                .AddOnFailureListener(this, taskCompletionListener);

            // using lamba expressions
            // on successful registration
            taskCompletionListener.Success += (o, g) =>
            {
                DatabaseReference newDriverRef = database.GetReference("drivers/" + mAuth.CurrentUser.Uid);
                // key value object used to send data to our database
                HashMap map = new HashMap();

                map.Put("fullname", fullname);
                map.Put("email", email);
                map.Put("phone", phone);
                map.Put("created_at", DateTime.Now.ToString());

                newDriverRef.SetValue(map);

                //post status to db
                DatabaseReference DriverStatusRef = database.GetReference("driverApplications/" + mAuth.CurrentUser.Uid);
                HashMap mapStatus = new HashMap();
                mapStatus.Put("status", "pending");

                DriverStatusRef.SetValue(mapStatus);

                Snackbar.Make(rootView, "Driver was registered successfully", Snackbar.LengthShort).Show();
                StartActivity(typeof(UploadsActivity));
            };

            // on failure
            taskCompletionListener.Failure += (w, r) =>
            {
                Snackbar.Make(rootView, "Driver registration failed", Snackbar.LengthShort).Show();
            };
        }
    } 
}