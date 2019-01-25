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
using Android.Support.V7.App;
using Android.Graphics;
using Newtonsoft.Json;
using Android.Telephony;

namespace ICC
{
    [Activity(Label = "Inicio", Theme = "@style/MyTheme")]
    public class InicioActivity : AppCompatActivity
    {
        IccSql lObjIcc = new IccSql();
        ProgressDialog cProc = null;
        EditText lEdU;
        EditText lEdC;
        Button lBtnInicio;
        Button lBtnSalir;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Inicio);
            
            var toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbarInicio);
            toolbar.SetTitleTextColor(Color.White);
            SetSupportActionBar(toolbar);
            SupportActionBar.Title = "ICC - Inicio";

            toolbar.InflateMenu(Resource.Menu.ConfMenu);
            toolbar.MenuItemClick += Toolbar_MenuItemClick;

            lEdU = this.FindViewById<EditText>(Resource.Id.TELoginU);
            lEdC = this.FindViewById<EditText>(Resource.Id.TELoginC);
            lBtnInicio = this.FindViewById<Button>(Resource.Id.btnLogin);
            lBtnSalir = this.FindViewById<Button>(Resource.Id.btnSalir);
            lBtnInicio.Click += LBtnInicio_Click;
            lBtnSalir.Click += LBtnSalir_Click;

            cProc = new ProgressDialog(this);

            if (!lObjIcc.FncPermiteTransaccion())
                lBtnInicio.Enabled = false;

            SubInicioAutomatico();
        }

        public override void OnBackPressed()
        {
            return;
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.ConfMenu, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        private void Toolbar_MenuItemClick(object sender, Android.Support.V7.Widget.Toolbar.MenuItemClickEventArgs e)
        {
            switch (e.Item.ItemId)
            {
                case Resource.Id.menu_share:
                    SubActualizarTransaccionesLocales();
                    break;
                case Resource.Id.salir_share:
                    FinishAffinity();
                    break;
            }
        }

        private void SubActualizarTransaccionesLocales()
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
                if (lObjIcc.FncPermiteTransaccion())
                    lBtnInicio.Enabled = true;
                cProc.Hide();
            }
            catch (Exception ex)
            {
                cProc.Hide();
                Toast.MakeText(ApplicationContext, ex.Message, ToastLength.Long).Show();
            }
        }

        private void LBtnInicio_Click(object sender, EventArgs e)
        {
            int lintTipoUsuario = lObjIcc.FncValidarUsuario(lEdU.Text, lEdC.Text);
            if (lintTipoUsuario > -1)
            {
                cProc = new ProgressDialog(this);
                cProc.SetCancelable(false);
                cProc.SetProgressStyle(ProgressDialogStyle.Spinner);
                cProc.SetMessage("Validando datos de inicio.");
                cProc.Show();
                TelephonyManager lObjInfoTel = (TelephonyManager)GetSystemService(TelephonyService);
                cObjInicio.NumeroTelefono = lObjInfoTel.Line1Number;
                cObjInicio.Imei = lObjInfoTel.DeviceId;
                cObjInicio.cUsuario = lEdU.Text;
                cObjInicio.cTipoUsuario = lintTipoUsuario;
                lObjIcc.SubGuardarMovil(cObjInicio.cUsuario, cObjInicio.cTipoUsuario);
                cProc.Hide();
                Intent lObjIntent = new Intent(this, typeof(MainActivity));
                StartActivity(lObjIntent);
            }
            else
            {
                Toast.MakeText(ApplicationContext, "Los datos de inicio no son válidos, favor de revisar usuario u contraseña.", ToastLength.Long).Show();
            }
        }

        private void LBtnSalir_Click(object sender, EventArgs e)
        {
            this.Finish();
        }

        private void SubInicioAutomatico()
        {
            if(lObjIcc.FncPermiteTransaccion())
            {
                CAT_Movil lObjMovil = lObjIcc.FncBuscarMovil();
                if (lObjMovil != null)
                {
                    TelephonyManager lObjInfoTel = (TelephonyManager)GetSystemService(TelephonyService);
                    cObjInicio.NumeroTelefono = lObjInfoTel.Line1Number;
                    cObjInicio.Imei = lObjInfoTel.DeviceId;
                    cObjInicio.cUsuario = lObjMovil.Usuario;
                    cObjInicio.cTipoUsuario = lObjMovil.TipoUsuario;
                    Intent lObjIntent = new Intent(this, typeof(MainActivity));
                    StartActivity(lObjIntent);
                }
            }
        }    

    }
}