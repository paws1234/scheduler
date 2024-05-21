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

    [Activity(Label = "Register")]
    public class RegistrationActivity : Activity
    {
        private EditText usernameEditText;
        private EditText passwordEditText;
        private EditText emailEditText;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
           
            SetContentView(Resource.Layout.RegistrationActivity);

            usernameEditText = FindViewById<EditText>(Resource.Id.usernameEditText);
            passwordEditText = FindViewById<EditText>(Resource.Id.passwordEditText);
            emailEditText = FindViewById<EditText>(Resource.Id.emailEditText);

            FindViewById<Button>(Resource.Id.registerButton).Click += async (sender, e) =>
            {
                if (string.IsNullOrEmpty(usernameEditText.Text) ||
                    string.IsNullOrEmpty(passwordEditText.Text) ||
                    string.IsNullOrEmpty(emailEditText.Text))
                {
                    Toast.MakeText(this, "Please enter all fields.", ToastLength.Short).Show();
                    return;
                }

                var user = new User
                {
                    Username = usernameEditText.Text,
                    Password = passwordEditText.Text,
                    Email = emailEditText.Text
                };

                try
                {
                    using (var db = DatabaseHelper.Instance.GetConnection())
                    {
                        db.Insert("User", null, user.ToContentValues());

                        Toast.MakeText(this, "Registration successful.", ToastLength.Short).Show();

                        Finish();
                    }
                }
                catch (Exception ex)
                {
                    Toast.MakeText(this, $"Error: {ex.Message}", ToastLength.Short).Show();
                }
            };
        }
    }
}