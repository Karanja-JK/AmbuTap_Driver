using System;
using System.Collections.Generic;
using System.Linq;
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

namespace AmbuTap_Driver.Activities
{
    [Activity(Label = "LoginActivity", Theme = "@style/AppTheme", MainLauncher =false)]
    public class LoginActivity : AppCompatActivity
    {
        TextInputLayout textInputEmail;
        TextInputLayout textInputPassword;
        Button loginButton;
        CoordinatorLayout rootView; // instantiate the coordinator layout from the login.xml file

        // registerText variable
        TextView registerText;

        // create instance of our authentication
        FirebaseDatabase database;
        FirebaseAuth mAuth;
        FirebaseUser currentUser;

        // loading progress
        // goto method showprogress() below 
        Android.Support.V7.App.AlertDialog.Builder alert;
        Android.Support.V7.App.AlertDialog alertDialog;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.login);
            ConnectViews();
            InitializeFirebase();
        }



        // method to get database values
        void InitializeFirebase()
        {
            database = AppDataHelper.GetDatabase();
            mAuth = AppDataHelper.GetFirebaseAuth();
            currentUser = AppDataHelper.GetCurrentUser();
        }



        // method to refrence views
        void ConnectViews()
        {
            // reference the information from login.xml
            textInputEmail = (TextInputLayout)FindViewById(Resource.Id.emailText);
            textInputPassword = (TextInputLayout)FindViewById(Resource.Id.passwordText);
            rootView = (CoordinatorLayout)FindViewById(Resource.Id.rootView);
            loginButton = (Button)FindViewById(Resource.Id.loginButton);
            registerText = (TextView)FindViewById(Resource.Id.registerText);

            // login button event handler
            loginButton.Click += LoginButton_Click;
            registerText.Click += RegisterText_Click;
        }

        private void RegisterText_Click(object sender, EventArgs e)
        {
            StartActivity(typeof(RegistrationActivity));
        }

        private void LoginButton_Click(object sender, EventArgs e)
        {
            string email, password;
            email = textInputEmail.EditText.Text;
            password = textInputPassword.EditText.Text;

            // show loader
            ShowProgressDialogue();

            // instance of task completion listener
            TaskCompletionListener taskCompletionListener = new TaskCompletionListener();
            taskCompletionListener.Success += TaskCompletionListener_Success;
            taskCompletionListener.Failure += TaskCompletionListener_Failure;

            // sign-in user
            mAuth.SignInWithEmailAndPassword(email, password)
                .AddOnSuccessListener(this, taskCompletionListener)
                .AddOnFailureListener(this, taskCompletionListener);
        }

        private void TaskCompletionListener_Failure(object sender, EventArgs e)
        {
            CloseProgressDialogue();
            Snackbar.Make(rootView, "Login failed", Snackbar.LengthShort).Show();
        }

        private void TaskCompletionListener_Success(object sender, EventArgs e)
        {
            CloseProgressDialogue();
            StartActivity(typeof(MainActivity));
        }

        // load progress
        void ShowProgressDialogue()
        {
            alert = new Android.Support.V7.App.AlertDialog.Builder(this);
            alert.SetView(Resource.Layout.progress);
            alert.SetCancelable(false);
            alertDialog = alert.Show();
        }

        void CloseProgressDialogue()
        {
            if(alert != null)
            {
                alertDialog.Dismiss();
                alertDialog = null;
                alert = null;
            }
        }
    }
}