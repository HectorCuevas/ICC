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
using Android.Provider;
using Android.Graphics;
using System.IO;
using Android.Support.V7.App;

namespace ICC
{
    [Activity(Label = "ICC", Theme = "@style/MyTheme")]
    public class ComentarioActivity : AppCompatActivity
    {
        EditText EdComentario = null;
        ImageView ImgNorte = null;
        Bitmap lObjBitmapNorte = null;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Comentario);
            SubConfToolbar();
            Button btnImagenNorte = this.FindViewById<Button>(Resource.Id.btnImgNorte);
            Button btnGuardarComentarios = this.FindViewById<Button>(Resource.Id.btnGuardarComentarios);
            btnImagenNorte.Click += BtnImagenNorte_Click;
            btnGuardarComentarios.Click += BtnGuardarComentarios_Click;
            EdComentario = this.FindViewById<EditText>(Resource.Id.EdComentario);
            ImgNorte = this.FindViewById<ImageView>(Resource.Id.ImgNorte);
        }

        public override void OnBackPressed()
        {
            return;
        }

        private void SubConfToolbar()
        {
            var toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbarComentarios);
            toolbar.SetTitleTextColor(Color.White);
            SetSupportActionBar(toolbar);
            SupportActionBar.Title = "ICC - Comentarios";
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

        private void BtnGuardarComentarios_Click(object sender, EventArgs e)
        {
            try
            {
                SubGuardarComentarios();
                SubGuardarDb();
                var lObjIntent = new Intent(this, typeof(ResumenActivity));
                StartActivity(lObjIntent);
            }
            catch (Exception ex)
            {
                Toast.MakeText(ApplicationContext, ex.Message, ToastLength.Long).Show();
            }
        }

        private void BtnImagenNorte_Click(object sender, EventArgs e)
        {
            Intent lObjIntent = new Intent(MediaStore.ActionImageCapture);
            StartActivityForResult(lObjIntent, 1);
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if(data != null)
            {
                Bitmap lObjBitmap = (Bitmap)data.Extras.Get("data");
                lObjBitmapNorte = lObjBitmap;
                ImgNorte.SetImageBitmap(lObjBitmap);
            }
        }

        private void SubGuardarComentarios()
        {
            if (lObjBitmapNorte == null)
                throw new Exception("Imagen Norte, obligatoria");
            cObjInicio.cTran.FechaHoraFinal = DateTime.Now;
            cObjInicio.cTran.Comentario = EdComentario.Text;
            cObjInicio.cTran.ImagenNorte = FncObtenerImagen(lObjBitmapNorte);
            cObjInicio.cTran.ImagenSur = new byte[0];
        }

        private void SubGuardarDb()
        {
            IccSql lObjSql = new IccSql();
            IccTran lIccTran = new IccTran();
            IccEnvio lObjEnvio = new IccEnvio();
            lObjEnvio.lobjEnce = cObjInicio.cTran;
            lObjEnvio.lObjDet = cObjInicio.cTranDet;
            lIccTran.Codigo = 0;
            lIccTran.JsonTran = JsonConvert.SerializeObject(lObjEnvio);
            lObjSql.SubCrearTransaccion(lIccTran);
        }

        private byte[] FncObtenerImagen(Bitmap lObjBitmap)
        {
            MemoryStream lObjStream = new MemoryStream();
            lObjBitmap.Compress(Bitmap.CompressFormat.Png,25, lObjStream);
            byte[] lObjImg = lObjStream.ToArray();
            return lObjImg;
        }

    }
}