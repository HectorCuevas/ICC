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
using Android.Support.V7.App;
using Android.Graphics;

namespace ICC
{
    [Activity(Label = "ICC", Theme = "@style/MyTheme")]
    public class CaudalActivity : AppCompatActivity
    {
        TextView TwRevolucionoVelocidad = null;
        EditText EtBaseI = null;
        EditText EtBaseF = null;
        EditText EtSector = null;
        EditText EtRevolucionOVelocidad = null;
        Button btnMedirCaudal = null;
        Button btnCancelarMedicion = null;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Caudal);
            SubConfToolbar();
            SubConfControles();
            SubConfIni();
        }

        public override void OnBackPressed()
        {
            return;
        }

        private void SubConfToolbar()
        {
            var toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbarCaudal);
            toolbar.SetTitleTextColor(Color.White);
            SetSupportActionBar(toolbar);
            SupportActionBar.Title = "ICC - Agregar Sección";
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
            TwRevolucionoVelocidad = this.FindViewById<TextView>(Resource.Id.TwRevolucionoVelocidad);
            EtBaseI = this.FindViewById<EditText>(Resource.Id.EtBaseI);
            EtBaseF = this.FindViewById<EditText>(Resource.Id.EtBaseF);
            EtSector = this.FindViewById<EditText>(Resource.Id.EtSector);
            EtRevolucionOVelocidad = this.FindViewById<EditText>(Resource.Id.EtRevolucionoVelocidad);
            btnMedirCaudal = this.FindViewById<Button>(Resource.Id.btnGuardarMedicion);
            btnCancelarMedicion = this.FindViewById<Button>(Resource.Id.btnCancelarMedicion);
            btnMedirCaudal.Click += BtnMedirCaudal_Click;
            btnCancelarMedicion.Click += BtnCancelarMedicion_Click;
        }

        private void BtnMedirCaudal_Click(object sender, EventArgs e)
        {
            try
            {
                SubGuardarMedidicon();
                cObjInicio.cIndice = -1;
                EtBaseF.RequestFocus();
                SubConfIni();
                Toast.MakeText(ApplicationContext, "La medición a sido guardada, correctamente.", ToastLength.Long).Show();               
            }
            catch (Exception ex)
            {
                Toast.MakeText(ApplicationContext, ex.Message, ToastLength.Long).Show();
            }
        }

        private void BtnCancelarMedicion_Click(object sender, EventArgs e)
        {
            SubFormaCaudalComentarios();
        }

        private double FncCalcularArea(double lBaseInicial, double lBaseFinal, double lSector)
        {
            double lDecArea = 0;
            lDecArea = lSector * ((lBaseInicial + lBaseFinal) / 2);
            return lDecArea;
        }

        private double FncCalcularVelocidad(double lRevoluciones)
        {
            double lDecVelocidad = 0;
            lDecVelocidad = lRevoluciones / 30;
            if (lDecVelocidad < 1.9800)
            {
                lDecVelocidad = (1.93 + 31.17 * lDecVelocidad) / 100;
            }
            else
            {
                lDecVelocidad = (0.19 + 32.05 * lDecVelocidad) / 100;
            }
            return lDecVelocidad;
        }

        private double FncCalcularCaudal(double lDblArea, double lDblVelocidad)
        {
            return lDblArea * lDblVelocidad;
        }

        private void SubGuardarMedidicon()
        {
            double lDblBaseI = 0;
            double lDblBaseF = 0;
            double lDblSector = 0;
            double lDblRevolucion = 0;
            double lDblArea = 0;
            double lDblVelocidad = 0;
            double lDblCaudal = 0;
            if (EtBaseF.Text == "0")
                throw new Exception("Para agregar sección, de ingresar algún valor");
            if (EtSector.Text == "0")
                throw new Exception("Para agregar sección, de ingresar algún valor");
            if (EtRevolucionOVelocidad.Text == "-1")
                throw new Exception("Para agregar sección, de ingresar algún valor");
            lDblBaseI = Convert.ToDouble(EtBaseI.Text);
            lDblBaseF = Convert.ToDouble(EtBaseF.Text);
            lDblSector = Convert.ToDouble(EtSector.Text);
            lDblArea = FncCalcularArea(lDblBaseI, lDblBaseF, lDblSector);
            if (cObjInicio.cTran.ModeloMolinete != "OTT")
            {
                lDblRevolucion = Convert.ToDouble(EtRevolucionOVelocidad.Text);
                lDblVelocidad = FncCalcularVelocidad(lDblRevolucion);
                lDblCaudal = FncCalcularCaudal(lDblArea, lDblVelocidad);
            }
            else
            {
                lDblRevolucion = 0;
                lDblVelocidad = Convert.ToDouble(EtRevolucionOVelocidad.Text);
                lDblCaudal = FncCalcularCaudal(lDblArea, lDblVelocidad);
            }
            TransaccionDet lObjDet = new TransaccionDet();
            if (cObjInicio.cIndice == -1)
            {
                lObjDet.Codigo = cObjInicio.cTran.Codigo;
                lObjDet.NoCorrelativo = cObjInicio.cTranDet.Count + 1;
                lObjDet.MedicionBaseInicial = lDblBaseI;
                lObjDet.MedicionBaseFinal = lDblBaseF;
                lObjDet.SectorMetros = lDblSector;
                lObjDet.Area = lDblArea;
                lObjDet.Revoluciones = lDblRevolucion;
                lObjDet.Velocidad = lDblVelocidad;
                lObjDet.Caudal = lDblCaudal;
                cObjInicio.cTranDet.Add(lObjDet);
            }
            else
            {
                int lNoElementos = cObjInicio.cTranDet.Count -1;
                cObjInicio.cTranDet[cObjInicio.cIndice].MedicionBaseInicial = lDblBaseI;
                cObjInicio.cTranDet[cObjInicio.cIndice].MedicionBaseFinal = lDblBaseF;
                cObjInicio.cTranDet[cObjInicio.cIndice].SectorMetros = lDblSector;
                cObjInicio.cTranDet[cObjInicio.cIndice].Area = lDblArea;
                cObjInicio.cTranDet[cObjInicio.cIndice].Revoluciones = lDblRevolucion;
                cObjInicio.cTranDet[cObjInicio.cIndice].Velocidad = lDblVelocidad;
                cObjInicio.cTranDet[cObjInicio.cIndice].Caudal = lDblCaudal;
                if(cObjInicio.cIndice < lNoElementos)
                {
                    cObjInicio.cIndice += 1;
                    cObjInicio.cTranDet[cObjInicio.cIndice].MedicionBaseInicial = lDblBaseF;
                    cObjInicio.cIndice -= 1;      
                }
                else if (cObjInicio.cIndice == lNoElementos && cObjInicio.cIndice > 0)
                {
                    cObjInicio.cIndice -= 1;
                    cObjInicio.cTranDet[cObjInicio.cIndice].MedicionBaseFinal = lDblBaseI;
                    cObjInicio.cIndice += 1;
                }
            }
        }

        private void SubConfIni()
        {
            EtBaseI.Text = string.Empty;
            EtBaseF.Text = string.Empty;
            EtSector.Text = string.Empty;
            EtRevolucionOVelocidad.Text = string.Empty;
            if (cObjInicio.cTran.ModeloMolinete == "OTT")
                TwRevolucionoVelocidad.Text = "Velocidad (m/s)";
            else
                TwRevolucionoVelocidad.Text = "RPS (revoluciones)";
            if (cObjInicio.cIndice == -1)
            {
                if (cObjInicio.cTranDet.Count > 0)
                    EtBaseI.Text = Convert.ToString(cObjInicio.cTranDet.OrderByDescending(o => o.NoCorrelativo).FirstOrDefault().MedicionBaseFinal);
                else
                    EtBaseI.Text = "0";                    
                if (cObjInicio.cTranDet.Count > 0)
                    EtSector.Text = Convert.ToString(cObjInicio.cTranDet.OrderByDescending(o => o.NoCorrelativo).FirstOrDefault().SectorMetros);
            }
            else
            {
                EtBaseI.Enabled = true;
                EtBaseF.Enabled = true;
                EtBaseI.Text = cObjInicio.cTranDet[cObjInicio.cIndice].MedicionBaseInicial.ToString();
                EtBaseF.Text = cObjInicio.cTranDet[cObjInicio.cIndice].MedicionBaseFinal.ToString();
                EtSector.Text = cObjInicio.cTranDet[cObjInicio.cIndice].SectorMetros.ToString();
                if (cObjInicio.cTran.ModeloMolinete == "OTT")
                    EtRevolucionOVelocidad.Text = cObjInicio.cTranDet[cObjInicio.cIndice].Velocidad.ToString();
                else
                    EtRevolucionOVelocidad.Text = cObjInicio.cTranDet[cObjInicio.cIndice].Revoluciones.ToString();
            }
        }

        private void SubFormaCaudalComentarios()
        {
            Intent lObjIntent = new Intent(this, typeof(ListadoCaudalActivity));
            StartActivity(lObjIntent);
        }

    }
}