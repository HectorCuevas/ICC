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
using Newtonsoft.Json;
using Android.Graphics;
using Android.Support.V7.App;
using Java.IO;

using Android.Content.PM;
using Android.Provider;
using Environment = Android.OS.Environment;
using Uri = Android.Net.Uri;

namespace ICC
{
    [Activity(Label = "ResumenActivity", Theme = "@style/MyTheme")]
    public class ResumenActivity : AppCompatActivity
    {
        
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Resumen);
            SubConfForma();
            SubConfToolbar();
        }

        public override void OnBackPressed()
        {
            return;
        }

        private void SubConfForma()
        {
            TextView TwResumenCuenca = this.FindViewById<TextView>(Resource.Id.TwResumenCuenca);
            TextView TwResumenSubCuenca = this.FindViewById<TextView>(Resource.Id.TwResumenSubCuenca);
            TextView TwResumenPuntoMonitoreo = this.FindViewById<TextView>(Resource.Id.TwResumenPuntoMonitoreo);
            TextView TwResumenFechaInicial = this.FindViewById<TextView>(Resource.Id.TwResumenFechaInicial);
            TextView TwResumenFechaFinal = this.FindViewById<TextView>(Resource.Id.TwResumenFechaFinal);
            TextView TwResumenComentario = this.FindViewById<TextView>(Resource.Id.TwResumenComentario);
            TextView TwResumenTipologia = this.FindViewById<TextView>(Resource.Id.TwResumenTipologia);
            ImageView ImgResumenImagen = this.FindViewById<ImageView>(Resource.Id.ImgResumenImagen);
            TextView TwResumenCaudal = this.FindViewById<TextView>(Resource.Id.TwResumenCaudal);
            Button btnMenuPrincipal = this.FindViewById<Button>(Resource.Id.btnMenuPrincipal);
            btnMenuPrincipal.Click += BtnMenuPrincipal_Click;
            TwResumenCuenca.Text = "Cuenca: " + cObjInicio.cTran.Cuenca;
            TwResumenSubCuenca.Text = "SubCuenca: " + cObjInicio.cTran.SubCuenca;
            TwResumenPuntoMonitoreo.Text = "Punto Monitoreo: " + cObjInicio.cTran.PuntoMonitoreo;
            TwResumenFechaInicial.Text = "Fecha Inicial: " + cObjInicio.cTran.FechaHoraInicial.ToString();
            TwResumenFechaFinal.Text = "Fecha Final: " + cObjInicio.cTran.FechaHoraFinal.ToString();
            TwResumenComentario.Text = "Comentario: " + cObjInicio.cTran.Comentario;
            TwResumenTipologia.Text = "Tipología: " + cObjInicio.cTran.Tipologia;
            TwResumenCaudal.Text = "Caudal: " + cObjInicio.cTran.Caudal + " m3/s";
            if (cObjInicio.cTran.Caudal == 0)
            {
                TwResumenCaudal.Visibility = ViewStates.Gone;
            }
            else
            {
                TwResumenTipologia.Visibility = ViewStates.Gone;
            }
            ImgResumenImagen.SetImageBitmap(BitmapFactory.DecodeByteArray(cObjInicio.cTran.ImagenNorte, 0, cObjInicio.cTran.ImagenNorte.Length));
        }

        private void SubConfToolbar()
        {
            var toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbarResumen);
            toolbar.SetTitleTextColor(Color.White);
            SetSupportActionBar(toolbar);
            SupportActionBar.Title = "ICC - Resumen";
            toolbar.InflateMenu(Resource.Menu.ConfMenuGeneral);
            toolbar.MenuItemClick += Toolbar_MenuItemClick;
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.ConfMenuGeneral, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        private void Toolbar_MenuItemClick(object sender, Android.Support.V7.Widget.Toolbar.MenuItemClickEventArgs e)
        {
            Android.App.AlertDialog.Builder lObjDialog = new Android.App.AlertDialog.Builder(this);
            lObjDialog.SetTitle("ICC");
            switch (e.Item.ItemId)
            {
                case Resource.Id.cambiar_share_general:
                    lObjDialog.SetMessage("¿Esta seguro que desea cambiar de usuario?");
                    break;
                case Resource.Id.salir_share_general:
                    lObjDialog.SetMessage("¿Esta seguro que desea salir del sistema?");
                    break;
            }
            lObjDialog.SetPositiveButton("Si", delegate
            {
                switch (e.Item.ItemId)
                {
                    case Resource.Id.cambiar_share_general:
                        IccSql lObjSql = new IccSql();
                        lObjSql.EliminarInicioAutomatico();
                        Intent lObjIntent = new Intent(this, typeof(InicioActivity));
                        StartActivity(lObjIntent);
                        break;
                    case Resource.Id.salir_share_general:
                        FinishAffinity();
                        break;
                }
            });
            lObjDialog.SetNegativeButton("No", delegate {
                return;
            });
            lObjDialog.Show();
        }

        private void BtnMenuPrincipal_Click(object sender, EventArgs e)
        {
            SubCrearJpgAforo(cObjInicio.cTran.Cuenca);
            var lObjIntent = new Intent(this, typeof(MainActivity));
            StartActivity(lObjIntent);
        }

        public void SubCrearJpgAforo(string lStrCuenca)
        {
            File lObjDirectorio = new File(Environment.GetExternalStoragePublicDirectory(Environment.DirectoryPictures), "ICC");
            if (!lObjDirectorio.Exists())
                lObjDirectorio.Mkdirs();
            File lObjArchivo = new File(lObjDirectorio, string.Format("{0}{1}.jpg", lStrCuenca, cObjInicio.cTran.Codigo));
            View view = Window.DecorView;
            var wasDrawingCacheEnabled = view.DrawingCacheEnabled;
            view.DrawingCacheEnabled = true;
            view.BuildDrawingCache(true);
            var lObjBitmap = Bitmap.CreateBitmap(view.GetDrawingCache(true));
            view.DrawingCacheEnabled = wasDrawingCacheEnabled;
            using (var lBjFileStream = new System.IO.FileStream(lObjArchivo.AbsolutePath, System.IO.FileMode.CreateNew))
            {
                lObjBitmap.Compress(Bitmap.CompressFormat.Jpeg, 95, lBjFileStream);
            }
            Uri.FromFile(lObjArchivo);
            Intent mediaScanIntent = new Intent(Intent.ActionMediaScannerScanFile);
            Uri contentUri = Uri.FromFile(lObjArchivo);
            mediaScanIntent.SetData(contentUri);
            SendBroadcast(mediaScanIntent);
        }
    }


}