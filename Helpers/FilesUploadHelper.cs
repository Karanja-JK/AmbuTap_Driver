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

namespace AmbuTap_Driver.Helpers
{
    public class Upload
    {
        public String url;

        // Default constructor required for calls to
        // DataSnapshot.getValue(User.class)
        public Upload()
        {
        }

        public Upload(String url)
        {
            this.url = url;
        }

        public String getUrl()
        {
            return url;
        }
    }
}