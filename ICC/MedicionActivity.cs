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
using System.Xml;
using Android.Media;
using Newtonsoft.Json;
using Geolocator.Plugin;
using Android.Support.V7.App;
using Android.Graphics;
using Android;

namespace ICC
{
    [Activity(Label = "ICC", Theme = "@style/MyTheme")]
    public class MedicionActivity : AppCompatActivity
    {
        ProgressDialog cProc = null;

        Spinner SpCuena = null;
        Spinner SpSubCuena = null;
        Spinner SpPuntoMonitoreo = null;
        Spinner SpTipoMedicion = null;        
        Spinner SpModeloMolinete = null;
        Spinner SpTipologia = null;
        TextView TwModeloMolinete = null;
        TextView TwTipologia = null;
        Button btnIniciarMedicion = null;
        Button btnCancelarMedicion = null;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Medicion);
            SubConfToolbar();
            SubConfControles();
            SubConfForma();
            SubCargarDatos();
        }

        public override void OnBackPressed()
        {
            return;
        }

        private void SubConfToolbar()
        {
            var toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbarMedicion);
            toolbar.SetTitleTextColor(Color.White);
            SetSupportActionBar(toolbar);
            if(cObjInicio.cBlnTipoMedidicion)
                SupportActionBar.Title = "ICC - Medición Aforo";
            else
                SupportActionBar.Title = "ICC - Medición Sin Aforo";
            toolbar.InflateMenu(Resource.Menu.ConfMenuGeneral);
            toolbar.MenuItemClick += Toolbar_MenuItemClick;
        }

        private void SubConfForma()
        {            
            if (cObjInicio.cBlnTipoMedidicion)
            {
                TwTipologia.Visibility = ViewStates.Gone;
                SpTipologia.Visibility = ViewStates.Gone;
            }
            else
            {
                TwModeloMolinete.Visibility = ViewStates.Gone;
                SpModeloMolinete.Visibility = ViewStates.Gone;
            }
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
            SpCuena = this.FindViewById<Spinner>(Resource.Id.SpCuenca);
            SpSubCuena = this.FindViewById<Spinner>(Resource.Id.SpSubCuenca);
            SpPuntoMonitoreo = this.FindViewById<Spinner>(Resource.Id.SpPuntoMonitoreo);
            SpModeloMolinete = this.FindViewById<Spinner>(Resource.Id.SpModeloMolinete);            
            SpTipoMedicion = this.FindViewById<Spinner>(Resource.Id.SpTipoMedicion);
            SpTipologia = this.FindViewById<Spinner>(Resource.Id.SpTipologia);
            TwModeloMolinete = this.FindViewById<TextView>(Resource.Id.TwModeloMolinete);
            TwTipologia = this.FindViewById<TextView>(Resource.Id.TwTipologia);
            btnIniciarMedicion = this.FindViewById<Button>(Resource.Id.btnIniciarMedicion);
            btnCancelarMedicion = this.FindViewById<Button>(Resource.Id.btnCancelarMedicion);
            btnCancelarMedicion.Click += BtnCancelarMedicion_Click;
            btnIniciarMedicion.Click += BtnIniciarMedicion_Click;
            SpCuena.ItemSelected += SpCuena_ItemSelected;
            SpSubCuena.ItemSelected += SpSubCuena_ItemSelected;
        }

        //Eventos

        private void BtnCancelarMedicion_Click(object sender, EventArgs e)
        {
            var lObjIntent = new Intent(this, typeof(MainActivity));
            StartActivity(lObjIntent);
        }

        private void BtnIniciarMedicion_Click(object sender, EventArgs e)
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

        private void SpCuena_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            SubLeerSubCuenca(((Spinner)sender).GetItemAtPosition(e.Position).ToString());
        }

        private void SpSubCuena_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            SubLeerPuntoMonitoreo(((Spinner)sender).GetItemAtPosition(e.Position).ToString());
        }

        //Metodos 
        private void SubCargarDatos()
        {
            SubLeerCuenca();
            SubLeerModeloMolinete();
            SubLeerTipoMedicion();
            SubLeerTipologian();
        }

        private void SubLeerCuenca()
        {
            List<string> lObjItems = new List<string>();
            lObjItems = FncLeerDb("Cuenca",string.Empty);
            ArrayAdapter lObjAdapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerDropDownItem, lObjItems);
            SpCuena.Adapter = lObjAdapter;           
        }

        private void SubLeerSubCuenca(string pStrCuenca)
        {
            List<string> lObjItems = new List<string>();
            lObjItems = FncLeerDb("SubCuenca", pStrCuenca);
            ArrayAdapter lObjAdapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerDropDownItem, lObjItems);
            SpSubCuena.Adapter = lObjAdapter;
        }

        private void SubLeerPuntoMonitoreo(string pStrSubCuenca)
        {
            List<string> lObjItems = new List<string>();
            lObjItems = FncLeerDb("PuntoMonitoreo", pStrSubCuenca);
            ArrayAdapter lObjAdapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerDropDownItem, lObjItems);
            SpPuntoMonitoreo.Adapter = lObjAdapter;
        }

        private void SubLeerModeloMolinete()
        {
            List<string> lObjItems = new List<string>();
            lObjItems = FncLeerDb("ModeloMolinete", string.Empty);
            ArrayAdapter lObjAdapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerDropDownItem, lObjItems);
            SpModeloMolinete.Adapter = lObjAdapter;
        }

        private void SubLeerTipoMedicion()
        {
            List<string> lObjItems = new List<string>();
            lObjItems = FncLeerDb("TipoMedicion", string.Empty);
            ArrayAdapter lObjAdapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerDropDownItem, lObjItems);
            SpTipoMedicion.Adapter = lObjAdapter;
        }

        private void SubLeerTipologian()
        {
            List<string> lObjItems = new List<string>();
            lObjItems = FncLeerDb("Tipologia", string.Empty);
            ArrayAdapter lObjAdapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerDropDownItem, lObjItems);
            SpTipologia.Adapter = lObjAdapter;
        }

        private List<String> FncLeerDb(string pTabla, string pPrefijo)
        {
            List<String> lObjString = new List<String>();
            IccSql lObjIcc = new IccSql();
            if(pPrefijo.Length > 0 )
            {
                lObjString = lObjIcc.FncLeerTablaIcc(pTabla, pPrefijo);
            }
            else
            {
                lObjString = lObjIcc.FncLeerTablaIcc(pTabla);
            }
            return lObjString;
        }

        //Inicializar Transaccion
        private async void SubConfGps()
        {
            var lObjGps = CrossGeolocator.Current;
            lObjGps.DesiredAccuracy = 100;
            var lObjPosition = await lObjGps.GetPositionAsync();
            SubFormaCaudalComentarios(lObjPosition.Latitude, lObjPosition.Longitude);
        }

        private void SubFormaCaudalComentarios(double lGpsLat,double lGpsLong)
        {
            cObjInicio.cTran = new Transaccion();
            cObjInicio.cTranDet = new List<TransaccionDet>();
            cObjInicio.cTran.Codigo = Guid.NewGuid();
            cObjInicio.cTran.Cuenca = SpCuena.GetItemAtPosition(SpCuena.SelectedItemPosition).ToString();
            cObjInicio.cTran.SubCuenca = SpSubCuena.GetItemAtPosition(SpSubCuena.SelectedItemPosition).ToString();
            cObjInicio.cTran.PuntoMonitoreo = SpPuntoMonitoreo.GetItemAtPosition(SpPuntoMonitoreo.SelectedItemPosition).ToString();
            cObjInicio.cTran.ModeloMolinete = SpModeloMolinete.GetItemAtPosition(SpModeloMolinete.SelectedItemPosition).ToString();
            cObjInicio.cTran.TipoMedicion = SpTipoMedicion.GetItemAtPosition(SpTipoMedicion.SelectedItemPosition).ToString();
            cObjInicio.cTran.Tipologia = SpTipologia.GetItemAtPosition(SpTipologia.SelectedItemPosition).ToString();
            cObjInicio.cTran.FechaHoraInicial = DateTime.Now;
            cObjInicio.cTran.FechaHoraFinal = DateTime.Now;
            cObjInicio.cTran.GpsLatitud = lGpsLat;
            cObjInicio.cTran.GpsLongitud = lGpsLong;
            cObjInicio.cTran.Estado = "Solicitud Pendiente de Revisar";
            cObjInicio.cTran.Usuario = cObjInicio.cUsuario;
            cProc.Hide();
            Intent lObjIntent = null;
            if (cObjInicio.cBlnTipoMedidicion)            
                lObjIntent = new Intent(this, typeof(ListadoCaudalActivity));
            else
                lObjIntent = new Intent(this, typeof(ComentarioActivity));
            StartActivity(lObjIntent);
        }

    }
}