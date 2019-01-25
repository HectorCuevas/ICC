using Android.App;
using Android.Widget;
using Android.OS;
using Android.Content;
using System.Collections.Generic;
using Geolocator.Plugin;
using System;
using Newtonsoft.Json;
using Android.Views;
using ICC.Clases;
using Android.Support.V7.App;
using Android.Graphics;
using System.Net.NetworkInformation;

namespace ICC
{
    [Activity(Label = "ICC", Theme = "@style/MyTheme")]
    public class MainActivity : AppCompatActivity
    {
        ProgressDialog cProc = null;
        IccSql lObjSql;
        string[] lMenus = new string[0];
        int[] lLogos = new int[0];

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);            
            SubConfMenu();
        }

        public override void OnBackPressed()
        {
            return;
        }

        private void SubConfMenu()
        {
            var toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            toolbar.SetTitleTextColor(Color.White);
            SetSupportActionBar(toolbar);
            SupportActionBar.Title = "ICC - Menú Principal";
            SubConfMenu(ref lMenus, ref lLogos);
            GridView gridView = FindViewById<GridView>(Resource.Id.grid_view_image_text);
            gridView.SetBackgroundColor(Color.SkyBlue);
            CustomGridViewAdapter lObjMenuCol;
            if (cObjInicio.cTipoUsuario != 2)
            {
                lObjMenuCol = new CustomGridViewAdapter(this, lMenus, lLogos, 2);
                gridView.NumColumns = 2;
            }
            else
            {
                lObjMenuCol = new CustomGridViewAdapter(this, lMenus, lLogos, 3);
                gridView.NumColumns = 3;
            }
            gridView.Adapter = lObjMenuCol;
            gridView.ItemClick += GridView_ItemClick;
            toolbar.InflateMenu(Resource.Menu.ConfMenuGeneral);
            toolbar.MenuItemClick += Toolbar_MenuItemClick;
            gridView.Enabled = true;
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
            lObjDialog.SetNegativeButton("No",delegate {
                return;
            });
            lObjDialog.Show();            
        }

        private void SubConfMenu(ref string[] lMenus, ref int[] lLogos)
        {
            IccSql lObjSql = new IccSql();
            bool blnEnvio = lObjSql.FncEnviaTransaccion();
            bool blnEnvioReporte = lObjSql.FncEnviaTransaccionYoReporto();
            cObjInicio.cTipoUsuario = 1;
            switch (cObjInicio.cTipoUsuario)
            {
                case 1: //TECNICO
                    lMenus = new string[] {
                        "Aforo","No-aforo", "Enviar","Recibir","Yo reporto","Enviar Rpt"};
                    lLogos = new int[]{
                               Resource.Drawable.aforo,Resource.Drawable.sinaforo,Resource.Drawable.enviarinfo,                        
                                Resource.Drawable.recibirinfo,Resource.Drawable.yoreporto,Resource.Drawable.yoreportoenvio };
                    if(!blnEnvio)
                        lLogos[2] = Resource.Drawable.enviarinfo2;
                    if (!blnEnvioReporte)
                        lLogos[5] = Resource.Drawable.yoreportoenvio2;
                    break;
                case 2: //ORGANIZACION
                    lMenus = new string[] {
                        "Yo reporto","Enviar Rpt","Recibir","Boletines","Boletin Gnl","Info Monitoreo","ICC","SIAgua","Redmet"};
                    lLogos = new int[]{
                               Resource.Drawable.yoreporto,Resource.Drawable.yoreportoenvio,
                                Resource.Drawable.recibirinfo,Resource.Drawable.boletines,
                                Resource.Drawable.boletingral,Resource.Drawable.infomonitoreo,Resource.Drawable.ICC,
                                Resource.Drawable.sismasur,Resource.Drawable.redmet};
                    if (!blnEnvioReporte)
                        lLogos[1] = Resource.Drawable.yoreportoenvio2;
                    break;
                case 3: //INVITADO
                    lMenus = new string[] {
                        "Yo reporto","Enviar Rpt","Boletines","Recibir","ICC","Redmet" };
                    lLogos = new int[]{
                               Resource.Drawable.yoreporto,Resource.Drawable.yoreportoenvio, Resource.Drawable.boletines,
                                Resource.Drawable.recibirinfo,Resource.Drawable.ICC,Resource.Drawable.redmet };
                    if (!blnEnvioReporte)
                        lLogos[1] = Resource.Drawable.yoreportoenvio2;
                    break;
            }
        }

        private void GridView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {            
            try
            {
                if(cProc != null)
                    return;
                switch (lMenus[e.Position])
                {
                    case "Aforo":
                        Intent lObjIntentA = new Intent(this, typeof(MedicionActivity));
                        cObjInicio.cBlnTipoMedidicion = true;
                        StartActivity(lObjIntentA);
                        break;
                    case "No-aforo":
                        Intent lObjIntentNA = new Intent(this, typeof(MedicionActivity));
                        cObjInicio.cBlnTipoMedidicion = false;
                        StartActivity(lObjIntentNA);
                        break;
                    case "Yo reporto":
                        Intent lObjIntentReporte = new Intent(this, typeof(YoReportoActivity));
                        StartActivity(lObjIntentReporte);
                        break;
                    case "Recibir":
                        SubActualizarInformacion();
                        break;
                    case "Enviar":
                        SubEnviarInformacion();
                        break;
                    case "Enviar Rpt":
                        SubEnviarInformacionYoReporto();
                        break;
                    case "Boletines":
                        SubAbrirReporte("http://138.128.150.200/ICC/Catalogo/BoletinOtrosPdf.aspx");
                        break;
                    case "Boletin Gnl":
                        SubAbrirReporte("http://138.128.150.200/ICC/Catalogo/BoletinGeneral.aspx");
                        break;
                    case "Info Monitoreo":
                        SubAbrirReporte("http://138.128.150.200/ICC/Catalogo/BoletinInformeMonitoreo.aspx");
                        break;
                    case "ICC":
                        SubAbrirReporte("http://icc.org.gt/");
                        break;
                    case "SIAgua":
                        SubAbrirReporte("http://redmet.icc.org.gt");
                        break;
                    case "Redmet":
                        SubAbrirReporte("http://redmet.icc.org.gt");
                        break;
                    case "Salir":
                        break;
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(ApplicationContext, ex.Message, ToastLength.Long).Show();
            }
        }

        private void SubActualizarInformacion()
        {
            cProc = new ProgressDialog(this);
            cProc.SetProgressStyle(ProgressDialogStyle.Spinner);
            cProc.SetMessage("Procesando información en servidor central.");
            cProc.Show();
            wsIcc.WsSincronizacion lObjIcc = new wsIcc.WsSincronizacion();
            lObjIcc.FncObtenerDatosCompleted += LObjIcc_FncObtenerDatosCompleted;
            lObjIcc.FncObtenerDatosAsync();
        }

        private void LObjIcc_FncObtenerDatosCompleted(object sender, wsIcc.FncObtenerDatosCompletedEventArgs e)
        {
            try
            {
                if (e.Error != null)
                    throw new Exception(e.Error.Message);
                List<CAT_Plantilla_Movil> lObjMoviles = new List<CAT_Plantilla_Movil>();
                lObjMoviles = JsonConvert.DeserializeObject<List<CAT_Plantilla_Movil>>(e.Result);
                IccSql lObjSql = new IccSql();
                lObjSql.SubEliminarCatalogos();
                foreach (CAT_Plantilla_Movil lObjMovil in lObjMoviles)
                {
                    lObjSql.SubAgregarCatalogo(lObjMovil);
                }
                cProc.Hide();
            }
            catch (Exception ex)
            {
                cProc.Hide();
                Toast.MakeText(ApplicationContext, ex.Message, ToastLength.Long).Show();
            }
        }

        private void SubEnviarInformacion()
        {
            lObjSql = new IccSql();
            cProc = new ProgressDialog(this);
            cProc.SetCancelable(false);
            cProc.SetProgressStyle(ProgressDialogStyle.Spinner);
            cProc.SetMessage("Procesando información en servidor central.");
            cProc.Show();
            List<IccTran> lObjEnvios = lObjSql.FncLeerTransaccion();
            if(lObjEnvios.Count > 0)
            {
                string lJsonEnvio = string.Empty;
                var lObjJsonBit = System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(lObjEnvios));
                lJsonEnvio = System.Convert.ToBase64String(lObjJsonBit);
                wsIcc.WsSincronizacion lObjIcc = new wsIcc.WsSincronizacion();
                lObjIcc.SubGuardarInformacionCompleted += LObjIcc_SubGuardarInformacionCompleted;
                lObjIcc.SubGuardarInformacionAsync(lJsonEnvio);
            }
            else
            {
                cProc.Hide();
                cProc = null;
                SubConfMenu();
            }
        }

        private void LObjIcc_SubGuardarInformacionCompleted(object sender, wsIcc.SubGuardarInformacionCompletedEventArgs e)
        {
            if (e.Error == null && e.Result.Length == 0)
            {
                lObjSql.SubEliminarTransaccion();
                cProc.Hide();
                cProc = null;
            }
            else if (e.Error != null)
            {
                cProc.Hide();
                cProc = null;
                Toast.MakeText(ApplicationContext, e.Error.Message, ToastLength.Long).Show();
            }
            else if (e.Result.Length > 0)
            {
                cProc.Hide();
                cProc = null;
                Toast.MakeText(ApplicationContext, e.Result, ToastLength.Long).Show();
            }
            SubConfMenu();
        }

        private void SubEnviarInformacionYoReporto()
        {
            lObjSql = new IccSql();            
            cProc = new ProgressDialog(this);
            cProc.SetCancelable(false);
            cProc.SetProgressStyle(ProgressDialogStyle.Spinner);
            cProc.SetMessage("Procesando información en servidor central.");
            cProc.Show();
            List<IccReporte> cObjIccTranYoReporto = lObjSql.FncLeerTransaccionYoReporto();
            if (cObjIccTranYoReporto.Count > 0)
            {
                string lJsonEnvio = string.Empty;
                var lObjJsonBit = System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(cObjIccTranYoReporto));
                lJsonEnvio = System.Convert.ToBase64String(lObjJsonBit);
                wsIcc.WsSincronizacion lObjIcc = new wsIcc.WsSincronizacion();
                lObjIcc.SubGuardarInformacionYoReportoCompleted += LObjIcc_SubGuardarInformacionYoReportoCompleted;
                lObjIcc.SubGuardarInformacionYoReportoAsync(lJsonEnvio);
            }
            else
            {
                cProc.Hide();
                cProc = null;
                SubConfMenu();
            }
        }

        private void LObjIcc_SubGuardarInformacionYoReportoCompleted(object sender, wsIcc.SubGuardarInformacionYoReportoCompletedEventArgs e)
        {
            if (e.Error == null && e.Result.Length == 0)
            {
                lObjSql.SubEliminarTransaccionYoReporto();
                cProc.Hide();
                cProc = null;
            }
            else if (e.Error != null)
            {
                cProc.Hide();
                cProc = null;
                Toast.MakeText(ApplicationContext, e.Error.Message, ToastLength.Long).Show();
            }
            else if (e.Result.Length > 0)
            {
                cProc.Hide();
                cProc = null;
                Toast.MakeText(ApplicationContext, e.Result, ToastLength.Long).Show();
            }
            SubConfMenu();
        }

        private void SubAbrirReporte(string lStrUri)
        {
            var uri = Android.Net.Uri.Parse(lStrUri);
            var intent = new Intent(Intent.ActionView, uri);
            StartActivity(intent);
        }

    }
}

