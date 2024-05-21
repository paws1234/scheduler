using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sched
{
    [Activity(Label = "Login")]
    public class LoginActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.LoginActivity);

            Button loginButton = FindViewById<Button>(Resource.Id.loginButton);
            loginButton.Click += LoginButton_Click;
        }

        private void LoginButton_Click(object sender, EventArgs e)
        {
            EditText usernameEditText = FindViewById<EditText>(Resource.Id.usernameEditText);
            EditText passwordEditText = FindViewById<EditText>(Resource.Id.passwordEditText);

            string username = usernameEditText.Text;
            string password = passwordEditText.Text;

          
            var user = DatabaseHelper.Instance.GetAllUsers().FirstOrDefault(u => u.Username == username);

            if (user != null && user.Password == password)
            {
                
                Intent intent = new Intent(this, typeof(SchedulerActivity));
                StartActivity(intent);


                Finish();
            }
            else
            {
                Toast.MakeText(this, "Incorrect username or password", ToastLength.Short).Show();
            }
        }
    }
}
