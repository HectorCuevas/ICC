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
using GR.Net.Maroulis.Library;
using Geolocator.Plugin;

namespace ICC
{
    [Activity(Label = "SiAgua", Icon = "@drawable/icon", MainLauncher = true, Theme = "@style/Theme.AppCompat.Light.NoActionBar")]
    public class SplashScreenActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SubConfDb();
            var config = new EasySplashScreen(this)
                .WithFullScreen()
                .WithBackgroundColor(Android.Graphics.Color.White)
                .WithTargetActivity(Java.Lang.Class.FromType(typeof(InicioActivity)))
                .WithSplashTimeOut(1000)
                .WithLogo(Resource.Drawable.Logo)
                .WithFooterText(String.Format("Prosisco Copyright {0}", DateTime.Now.Year));
            View view = config.Create();
            SetContentView(view);
        }

        private void SubConfDb()
        {
            IccSql lObjIcc = new IccSql();
            lObjIcc.SubCrearDbIcc();
        }

    }
}