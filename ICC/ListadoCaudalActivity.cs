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
    public class ListadoCaudalActivity : AppCompatActivity
    {

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.ListadoCaudal);
            SubConfToolbar();
            Button btnIniciarMedicion = this.FindViewById<Button>(Resource.Id.btnMedirCaudal);
            Button btnCancelarMedicion = this.FindViewById<Button>(Resource.Id.btnCancelarLista);
            Button btnFinalizarCaudal = this.FindViewById<Button>(Resource.Id.btnFinalizarCaudal);            
            btnIniciarMedicion.Click += BtnIniciarMedicion_Click;
            btnCancelarMedicion.Click += BtnCancelarMedicion_Click;
            btnFinalizarCaudal.Click += BtnFinalizarCaudal_Click;
            SubConfLista();
        }

        private void SubConfToolbar()
        {
            var toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbarListadoCaudal);
            toolbar.SetTitleTextColor(Color.White);
            SetSupportActionBar(toolbar);
            SupportActionBar.Title = "ICC - Secciones";
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

        private void BtnCancelarMedicion_Click(object sender, EventArgs e)
        {
            Android.App.AlertDialog.Builder lObjDialog = new Android.App.AlertDialog.Builder(this);
            lObjDialog.SetTitle("ICC");
            lObjDialog.SetMessage("¿Esta seguro que desea cancelar la medición?");
            lObjDialog.SetPositiveButton("Si", delegate {
                Intent lObjIntent = new Intent(this, typeof(MedicionActivity));
                StartActivity(lObjIntent);
            });
            lObjDialog.SetNegativeButton("No", delegate {
                return;
            });
            lObjDialog.Show();            
        }

        private void BtnIniciarMedicion_Click(object sender, EventArgs e)
        {
            Intent lObjIntent = new Intent(this, typeof(CaudalActivity));
            cObjInicio.cIndice = -1;
            StartActivity(lObjIntent);
        }

        private void BtnFinalizarCaudal_Click(object sender, EventArgs e)
        {
            Intent lObjIntent = new Intent(this, typeof(ComentarioActivity));
            StartActivity(lObjIntent);
        }

        private void SubConfLista()
        {
            List<TransaccionDet> lObj = new List<TransaccionDet>(cObjInicio.cTranDet);
            TextView TwTitlo = this.FindViewById<TextView>(Resource.Id.TwTitulo);
            TextView TwRevoVel = this.FindViewById<TextView>(Resource.Id.TwLRevoVelL);
            TextView TwCaudal = this.FindViewById<TextView>(Resource.Id.TwSumaCaudal);
            bool blnMedicion = false;
            if (cObjInicio.cTran.ModeloMolinete != "OTT")
            {
                TwRevoVel.Text = "Rev";
                blnMedicion = true;
            }
             else
            {
                TwRevoVel.Text = "Vel";
                blnMedicion = false;
            }
            TwTitlo.Text = string.Format("Medicion: {0} {1} {2}",cObjInicio.cTran.Cuenca, cObjInicio.cTran.SubCuenca, cObjInicio.cTran.PuntoMonitoreo);
            lObj.OrderBy(lObjDet => lObjDet.NoCorrelativo);
            cObjInicio.cTran.Caudal = Math.Round(cObjInicio.cTranDet.Sum(lObjDet => lObjDet.Caudal), 2);
            TwCaudal.Text = string.Format("Total Caudal: {0} m3/s", cObjInicio.cTran.Caudal);
            ListView lObjListView = this.FindViewById<ListView>(Resource.Id.LwListaCaudal);
            lObjListView.ItemLongClick += LObjListView_ItemLongClick;
            ListViewCaudalAdapter lObjAdapter = new ListViewCaudalAdapter(this, lObj, blnMedicion);
            lObjListView.Adapter = lObjAdapter;
        }

        private void LObjListView_ItemLongClick(object sender, AdapterView.ItemLongClickEventArgs e)
        {
            if (e.Position > -1)
            {
                Intent lObjIntent = new Intent(this, typeof(CaudalActivity));
                cObjInicio.cIndice = e.Position;
                StartActivity(lObjIntent);
            }
        }

    }
}