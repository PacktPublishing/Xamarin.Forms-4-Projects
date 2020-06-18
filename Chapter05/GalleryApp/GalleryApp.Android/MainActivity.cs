using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace GalleryApp.Droid
{
    [Activity(Label = "GalleryApp", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        public static MainActivity Current { get; private set; }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            Current = this;

            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            _ = new Bootstrapper();

            global::Xamarin.Forms.Forms.SetFlags("CarouselView_Experimental", "IndicatorView_Experimental");

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);

            LoadApplication(new App());
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            if (requestCode == 33)
            {
                var importer = (PhotoImporter)Resolver.Resolve<IPhotoImporter>();
                importer.ContinueWithPermission(grantResults[0] == Permission.Granted);
            }

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}
