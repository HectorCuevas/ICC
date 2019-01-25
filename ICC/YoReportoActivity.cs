using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Android.Graphics;
using Android.Support.V7.App;
using Geolocator.Plugin;
using ICC.Clases;
using Android.Provider;
using System.IO;
using Newtonsoft.Json;

namespace ICC
{
    [Activity(Label = "YoReportoActivity", Theme = "@style/MyTheme")]
    public class YoReportoActivity : AppCompatActivity
    {
        ProgressDialog cProc = null;
        EditText EdNombre = null;
        EditText EdTelefono = null;
        EditText EdCorreo = null;
        Spinner SpTipoReporte = null;
        Spinner SpDepartamento = null;
        Spinner SpMinicipio = null;
        EditText EdBarrio = null;
        EditText EdDireccion = null;
        EditText EdComentario = null;
        CheckBox CkFotografia = null;
        Button btnEnviar = null;
        Button btnCancelar = null;
        Bitmap lObjBitmap = null;
        TextView TwYoReportoNombre;
        TextView TwYoReportoTelefono;
        TextView TwYoReportoCorreo;
        TextView TwYoReportoDepartamento;
        TextView TwYoReportoMunicipio;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.YoReporto);
            SubConfToolbar();
            SubConfControles();
            SubConfControlesVista();
            SubConfDatos();
        }

        public override void OnBackPressed()
        {
            return;
        }

        private void SubConfToolbar()
        {
           var toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbarYoReporto);
           toolbar.SetTitleTextColor(Color.White);
           SetSupportActionBar(toolbar);
           SupportActionBar.Title = "ICC - Yo Reporto";
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

        private void SubConfControles()
        {
            TwYoReportoNombre = this.FindViewById<TextView>(Resource.Id.TwYoReportoNombre);
            EdNombre = this.FindViewById<EditText>(Resource.Id.EdYoReportoNombre);
            TwYoReportoTelefono = this.FindViewById<TextView>(Resource.Id.TwYoReportoTelefono);
            EdTelefono = this.FindViewById<EditText>(Resource.Id.EdYoReportoTelefono);
            TwYoReportoCorreo = this.FindViewById<TextView>(Resource.Id.TwYoReportoCorreo);
            EdCorreo = this.FindViewById<EditText>(Resource.Id.EdYoReportoCorreo);
            SpTipoReporte = this.FindViewById<Spinner>(Resource.Id.SpYoReportoTipoReporte);
            TwYoReportoDepartamento = this.FindViewById<TextView>(Resource.Id.TwYoReportoDepartamento);
            SpDepartamento = this.FindViewById<Spinner>(Resource.Id.SpYoReportoDepartamento);
            TwYoReportoMunicipio = this.FindViewById<TextView>(Resource.Id.TwYoReportoMunicipio);
            SpMinicipio = this.FindViewById<Spinner>(Resource.Id.SpYoReportoMunicipio);
            EdBarrio = this.FindViewById<EditText>(Resource.Id.EdYoReportoBarrio);
            EdDireccion = this.FindViewById<EditText>(Resource.Id.EdYoReportoDireccion);
            EdComentario = this.FindViewById<EditText>(Resource.Id.EdYoReportoComentario);
            CkFotografia = this.FindViewById<CheckBox>(Resource.Id.CkYoReportoFoto);
            btnEnviar = this.FindViewById<Button>(Resource.Id.btnYoReportoGuardarReporte);
            btnCancelar = this.FindViewById<Button>(Resource.Id.btnYoReportoCancelar);
            SpDepartamento.ItemSelected += SpDepartamento_ItemSelected;
            CkFotografia.CheckedChange += CkFotografia_CheckedChange;
            btnEnviar.Click += BtnEnviar_Click;
            btnCancelar.Click += BtnCancelar_Click;
        }

        private void SubConfControlesVista()
        {
            TwYoReportoNombre.Visibility = ViewStates.Gone;
            EdNombre.Visibility = ViewStates.Gone;
            TwYoReportoTelefono.Visibility = ViewStates.Gone;
            EdTelefono.Visibility = ViewStates.Gone;
            TwYoReportoCorreo.Visibility = ViewStates.Gone;
            EdCorreo.Visibility = ViewStates.Gone;
            TwYoReportoDepartamento.Visibility = ViewStates.Gone;
            SpDepartamento.Visibility = ViewStates.Gone;
            TwYoReportoMunicipio.Visibility = ViewStates.Gone;
            SpMinicipio.Visibility = ViewStates.Gone;
        }

        private void SubConfDatos()
        {
            SubCargarTipoReporte();
            SubCargarDepartamento();
        }

        private void SubCargarTipoReporte()
        {
            List<string> lObjItems = new List<string>();
            lObjItems = FncLeerDb("TipoReporte", string.Empty);
            ArrayAdapter lObjAdapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerDropDownItem, lObjItems);
            SpTipoReporte.Adapter = lObjAdapter;
        }

        private void SubCargarDepartamento()
        {
            List<string> lObjItems = new List<string>();
            lObjItems = FncLeerDb("Departamento", string.Empty);
            ArrayAdapter lObjAdapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerDropDownItem, lObjItems);
            SpDepartamento.Adapter = lObjAdapter;
        }

        private void SubCargarMunicipio(string lStrDepartamento)
        {
            List<string> lObjItems = new List<string>();
            lObjItems = FncLeerDb("Municipio", lStrDepartamento);
            ArrayAdapter lObjAdapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerDropDownItem, lObjItems);
            SpMinicipio.Adapter = lObjAdapter;
        }

        private List<String> FncLeerDb(string pTabla, string pPrefijo)
        {
            List<String> lObjString = new List<String>();
            IccSql lObjIcc = new IccSql();
            if (pPrefijo.Length > 0)
            {
                lObjString = lObjIcc.FncLeerTablaIcc(pTabla, pPrefijo);
            }
            else
            {
                lObjString = lObjIcc.FncLeerTablaIcc(pTabla);
            }
            return lObjString;
        }

        private void SpDepartamento_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            SubCargarMunicipio(((Spinner)sender).GetItemAtPosition(e.Position).ToString());
        }

        private void CkFotografia_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            if(e.IsChecked)
            {
                Intent lObjIntent = new Intent(MediaStore.ActionImageCapture);
                StartActivityForResult(lObjIntent,0);
            }
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (data != null)
            {
                lObjBitmap = (Bitmap)data.Extras.Get("data");                
            }
            else
            {
                lObjBitmap = null;
                CkFotografia.Checked = false;
            }
        }

        private void BtnEnviar_Click(object sender, EventArgs e)
        {
            try
            {
                cProc = new ProgressDialog(this);
                cProc.SetMessage("Buscando coordenadas GPS.");
                cProc.SetProgressStyle(ProgressDialogStyle.Spinner);
                cProc.Show();
                SubConfGps();
            }
            catch (Exception ex)
            {
                Toast.MakeText(ApplicationContext, ex.Message, ToastLength.Long).Show();
            }
        }

        private void BtnCancelar_Click(object sender, EventArgs e)
        {
            Intent lObjIntent = new Intent(this, typeof(MainActivity));
            StartActivity(lObjIntent);
        }

        private async void SubConfGps()
        {
            var lObjGps = CrossGeolocator.Current;
            lObjGps.DesiredAccuracy = 100;
            var lObjPosition = await lObjGps.GetPositionAsync();
            SubYoReporto(lObjPosition.Latitude, lObjPosition.Longitude);
        }

        private void SubYoReporto(double lGpsLat, double lGpsLong)
        {
            try
            {
                if (lObjBitmap == null)
                    throw new Exception("Fotografía es obligatoria, para reportar.");
                YoReporto lObjYoReporto = new YoReporto();
                lObjYoReporto.Codigo = Guid.NewGuid();
                lObjYoReporto.FechaHoraInicial = DateTime.Now;
                lObjYoReporto.Usuario = cObjInicio.cUsuario;
                lObjYoReporto.Nombre = cObjInicio.cUsuario;
                lObjYoReporto.Telefono = cObjInicio.NumeroTelefono;
                lObjYoReporto.Correo = cObjInicio.cUsuario;
                lObjYoReporto.TipoReporte = SpTipoReporte.GetItemAtPosition(SpTipoReporte.SelectedItemPosition).ToString();
                lObjYoReporto.Departamento = "NA";//SpDepartamento.GetItemAtPosition(SpDepartamento.SelectedItemPosition).ToString();
                lObjYoReporto.Municipio = "NA";//SpMinicipio.GetItemAtPosition(SpMinicipio.SelectedItemPosition).ToString();
                lObjYoReporto.Barrio = EdBarrio.Text;
                lObjYoReporto.Direccion = EdDireccion.Text;
                lObjYoReporto.Comentarios = EdComentario.Text;
                lObjYoReporto.GpsLatitud = lGpsLat;
                lObjYoReporto.GpsLongitud = lGpsLong;
                lObjYoReporto.Imagen = FncObtenerImagen(lObjBitmap);
                lObjYoReporto.Emei = cObjInicio.Imei;
                IccSql lObjSql = new IccSql();
                IccReporte lIccTran = new IccReporte();
                lIccTran.Codigo = 0;
                lIccTran.JsonTran = JsonConvert.SerializeObject(lObjYoReporto);
                lObjSql.SubCrearTransaccionYoReporto(lIccTran);
                cProc.Hide();
                Intent lObjIntent = new Intent(this, typeof(MainActivity));
                StartActivity(lObjIntent);
            }
            catch(Exception ex)
            {
                Toast.MakeText(ApplicationContext, ex.Message, ToastLength.Long).Show();
            }
        }

        private byte[] FncObtenerImagen(Bitmap lObjBitmap)
        {
            MemoryStream lObjStream = new MemoryStream();
            lObjBitmap.Compress(Bitmap.CompressFormat.Png, 25, lObjStream);
            byte[] lObjImg = lObjStream.ToArray();
            return lObjImg;
        }
    }
}