using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Gms.Tasks;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace AmbuTap_Driver.EventListener
{
    public class TaskCompletionListener : Java.Lang.Object, IOnSuccessListener, IOnFailureListener 
    {
        public event EventHandler Success;
        public event EventHandler Failure;
        public void OnFailure(Java.Lang.Exception e)
        {
            Failure?.Invoke(this, new EventArgs()); // call the Failure event handler and notifies root class
        }

        public void OnSuccess(Java.Lang.Object result)
        {
            Success?.Invoke(this, new EventArgs()); // call the Success event handler and notifies root class
        }
    }
}