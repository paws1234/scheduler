using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Views;
using AndroidX.AppCompat.App;
using Google.Android.Material.Button;
using System;

namespace sched
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

       
            FindViewById<MaterialButton>(Resource.Id.registerButton).Click += StartRegistrationActivity;
            FindViewById<MaterialButton>(Resource.Id.loginButton).Click += StartLoginActivity;
        }

        public void StartRegistrationActivity(object sender, EventArgs e)
        {
            StartActivity(typeof(RegistrationActivity));
        }

        public void StartLoginActivity(object sender, EventArgs e)
        {
            StartActivity(typeof(LoginActivity));
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}
