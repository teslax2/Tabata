using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Content;

namespace Tabata.Droid
{
    [Activity(Label = "Tabata", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        public static MainActivity Instance { get; private set; }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());
            Instance = this;
        }

        public override void OnBackPressed()
        {
            Android.Content.IDialogInterfaceOnClickListener handler = null;
            var dialog = new AlertDialog.Builder(this)
                .SetTitle("Really Exit?")
                .SetMessage("Do you want to exit?")
                .SetNegativeButton("no", handler)
                .SetPositiveButton("yes", handlerYes)
                .Show();
        }

        private void handlerYes(object sender, DialogClickEventArgs e)
        {
            base.OnBackPressed();
        }
    }
}