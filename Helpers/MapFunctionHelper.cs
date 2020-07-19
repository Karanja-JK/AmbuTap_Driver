using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Com.Google.Maps.Android;
using Java.Util;
using Newtonsoft.Json;
using Ambu.Helpers;

namespace AmbuTap_Driver.Helpers
{
    public class MapFunctionHelper
    {
        string mapkey;
        GoogleMap mainMap;
        public Marker destinationMarker;
        public Marker positionMarker;
        public Marker pickupMarker;

        // Flag
        bool isRequestingDirection;


        public MapFunctionHelper(string mMapkey, GoogleMap mmap)
        {
            mapkey = mMapkey;
            mainMap = mmap;
        }

        // make asyncronous web request to the url using http client
        public async Task<string> GetGeoJsonAsync(string url)
        {

            var handler = new HttpClientHandler();
            HttpClient client = new HttpClient(handler); // Pass system uri objects instead of strings
            string result = await client.GetStringAsync(url);// Pass system uri objects instead of strings
            return result;
        }



        // direction and polylines 
        public async Task<string> GetDirectionJsonAsync(LatLng location, LatLng destination)
        {
            // pickup location of trip
            string str_origin = "origin=" + location.Latitude + "," + location.Longitude;

            // destination
            string str_destination = "destination=" + destination.Latitude + "," + destination.Longitude;

            // mode
            string mode = "mode=driving";

            // webservice parameters
            string parameters = str_origin + "&" + str_destination + "&" + "&" + mode + "&key=";

            // output format
            string output = "json";

            string key = mapkey;

            // final url string
            string url = "https://maps.googleapis.com/maps/api/directions/" + output + "?" + parameters + key;

            string json = "";
            json = await GetGeoJsonAsync(url);

            return json;
        }


        // Draw polylines
        public void DrawTripOnMap(string json)
        {
            Android.Gms.Maps.Model.Polyline mPolyline;
           // Marker pickupMarker;

            var directionData = JsonConvert.DeserializeObject<DirectionParser>(json);

            // Pickup Position
            var pointCode = directionData.routes[0].overview_polyline.points;
            var line = PolyUtil.Decode(pointCode);
            LatLng firstpoint = line[0];
            LatLng lastpoint = line[line.Count - 1];           


            // Take off position - Driver's current position
            MarkerOptions markerOptions = new MarkerOptions(); 
            markerOptions.SetPosition(firstpoint);
            markerOptions.SetIcon(BitmapDescriptorFactory.DefaultMarker(BitmapDescriptorFactory.HueRed));
            pickupMarker = mainMap.AddMarker(markerOptions);


            // constantly change current location
            MarkerOptions positionMarkerOption = new MarkerOptions();
            positionMarkerOption.SetPosition(firstpoint);
            positionMarkerOption.SetTitle("Current Location");
            positionMarkerOption.SetIcon(BitmapDescriptorFactory.FromResource(Resource.Drawable.position));
            positionMarker = mainMap.AddMarker(positionMarkerOption);


            // pick up position
            MarkerOptions markerOptions1 = new MarkerOptions();
            markerOptions1.SetPosition(lastpoint);
            markerOptions1.SetTitle("Pickup Location");
            markerOptions1.SetIcon(BitmapDescriptorFactory.DefaultMarker(BitmapDescriptorFactory.HueGreen));
            destinationMarker = mainMap.AddMarker(markerOptions1);


            // Array List
            // From the current position to pick up location

            ArrayList routeList = new ArrayList();
            int locationCount = 0;
            foreach (LatLng item in line)
            {
                routeList.Add(item);
                locationCount++;
                Console.WriteLine("Position " + locationCount.ToString() + " = " + item.Latitude.ToString() + " , " + item.Longitude.ToString());
            }


            // Polylines in map
            // specifications of the polylines
            PolylineOptions polylineOptions = new PolylineOptions()
                .AddAll(routeList)
                .InvokeWidth(10)
                .InvokeColor(Color.Teal)
                .InvokeStartCap(new SquareCap())
                .InvokeEndCap(new SquareCap())
                .InvokeJointType(JointType.Round)
                .Geodesic(true);


            // draw the polyline
            mPolyline = mainMap.AddPolyline(polylineOptions);
            mainMap.UiSettings.ZoomControlsEnabled = true;
            mainMap.AnimateCamera(CameraUpdateFactory.NewLatLngZoom(firstpoint, 15));
            pickupMarker.ShowInfoWindow(); 

        }



        public void DrawTripToDestination(string json)
        {
            Android.Gms.Maps.Model.Polyline mPolyline;
            //Marker pickupMarker;

            var directionData = JsonConvert.DeserializeObject<DirectionParser>(json);

            var pointCode = directionData.routes[0].overview_polyline.points;
            var line = PolyUtil.Decode(pointCode);
            LatLng firstpoint = line[0];
            LatLng lastpoint = line[line.Count - 1];



            // Take off position
            MarkerOptions markerOptions = new MarkerOptions();
            markerOptions.SetPosition(firstpoint);
            markerOptions.SetIcon(BitmapDescriptorFactory.DefaultMarker(BitmapDescriptorFactory.HueGreen));
            markerOptions.SetTitle("Pickup Location");
            pickupMarker = mainMap.AddMarker(markerOptions);


            // constantly change current location
            MarkerOptions positionMarkerOption = new MarkerOptions();
            positionMarkerOption.SetPosition(firstpoint);
            positionMarkerOption.SetTitle("Current Location");
            positionMarkerOption.SetIcon(BitmapDescriptorFactory.FromResource(Resource.Drawable.position));
            positionMarker = mainMap.AddMarker(positionMarkerOption);


            // pick up position
            MarkerOptions markerOptions1 = new MarkerOptions();
            markerOptions1.SetPosition(lastpoint);
            markerOptions1.SetTitle("Destination");
            markerOptions1.SetIcon(BitmapDescriptorFactory.DefaultMarker(BitmapDescriptorFactory.HueRed));
            destinationMarker = mainMap.AddMarker(markerOptions1);

            // Array List
            // From the current position to pick up location

            ArrayList routeList = new ArrayList();
            int locationCount = 0;
            foreach (LatLng item in line)
            {
                routeList.Add(item);
                locationCount++;
                Console.WriteLine("Position" + locationCount.ToString() + " = " + item.Latitude.ToString() + " , " + item.Longitude.ToString());
            }


            // Polylines in map
            // specifications of the polylines
            PolylineOptions polylineOptions = new PolylineOptions()
                .AddAll(routeList)
                .InvokeWidth(10)
                .InvokeColor(Color.Teal)
                .InvokeStartCap(new SquareCap())
                .InvokeEndCap(new SquareCap())
                .InvokeJointType(JointType.Round)
                .Geodesic(true);


            // draw the polyline
            mPolyline = mainMap.AddPolyline(polylineOptions);
            mainMap.UiSettings.ZoomControlsEnabled = true;
            mainMap.TrafficEnabled = true;

            LatLng southwest = new LatLng(directionData.routes[0].bounds.southwest.lat, directionData.routes[0].bounds.southwest.lng);
            LatLng northeast = new LatLng(directionData.routes[0].bounds.northeast.lat, directionData.routes[0].bounds.northeast.lng);

            LatLngBounds tripBounds = new LatLngBounds(southwest, northeast);
            mainMap.AnimateCamera(CameraUpdateFactory.NewLatLngBounds(tripBounds, 100));

            destinationMarker.ShowInfoWindow();

        }



         
        // Update driver movement
        public async void UpdateMovement(LatLng myposition, LatLng destination, string whereto)
        {
            positionMarker.Visible = true;
            positionMarker.Position = myposition;

            if(!isRequestingDirection)
            {
                isRequestingDirection = true;
                string json = await GetDirectionJsonAsync(myposition, destination);
                var directionData = JsonConvert.DeserializeObject<DirectionParser>(json);
                string duration = directionData.routes[0].legs[0].duration.text;
                positionMarker.Title = "Current Location";
                positionMarker.Snippet = duration + "Away from" + whereto;
                positionMarker.ShowInfoWindow();
                isRequestingDirection = false;
            }
        }

        // Calculate Fare
        public async Task<double>CalculateFares(LatLng firstpoint, LatLng lastpoint)
        {
            string directionJson = await GetDirectionJsonAsync(firstpoint, lastpoint);
            var directionData = JsonConvert.DeserializeObject<DirectionParser>(directionJson);

            double distanceValue = directionData.routes[0].legs[0].distance.value;
            double durationValue = directionData.routes[0].legs[0].duration.value;

            double basefare = 200;
            double distancefare = 15;
            double timeFare = 5;

            double kmfare = (distanceValue / 1000) * distancefare;
            double minsfare = (durationValue / 60) * timeFare;
            double amount = basefare + kmfare + minsfare;
            double fare = Math.Floor(amount / 10) * 10;
            return fare;
        }
    }
}