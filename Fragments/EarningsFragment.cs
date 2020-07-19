using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using Firebase.Auth;
using Firebase.Database;

namespace AmbuTap_Driver.Fragments
{
    public class EarningsFragment : Android.Support.V4.App.Fragment
    {
        RecyclerView mRecyclerView;
        RecyclerView.LayoutManager mLayoutManager;

        private TextView mTxtAddNot;
        private DatabaseReference dataRef;
        private FirebaseAuth mAuth;
        private RecyclerView recyclerView;
        private Query mQuery;

        //private ImageButton imgBtnSort;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            View view = inflater.Inflate(Resource.Layout.earnings, container, false);

            return view;
        }
    }
}