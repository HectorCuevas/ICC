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

namespace ICC
{
    class ListViewCaudalAdapter : BaseAdapter<TransaccionDet>
    {
        public List<TransaccionDet> mItems;
        private Context mContext;
        private bool mblnTipoMedicionCaudal;

        public ListViewCaudalAdapter(Context Context, List<TransaccionDet> Items, bool blnTipoMedicionCaudal) : base()
        {
            mItems = Items;
            mContext = Context;
            mblnTipoMedicionCaudal = blnTipoMedicionCaudal;
        }

        public override int Count
        {
            get { return mItems.Count; }
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override TransaccionDet this[int position]
        {
            get { return mItems[position]; }
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            TransaccionDet lObjDet = mItems[position];
            View row = convertView;
            if (row == null)
                row = LayoutInflater.From(mContext).Inflate(Resource.Layout.Listado, null, false);
            TextView TwLBaseA = row.FindViewById<TextView>(Resource.Id.TwLBaseA);
            TextView TwLBaseB = row.FindViewById<TextView>(Resource.Id.TwLBaseB);
            TextView TwLSector = row.FindViewById<TextView>(Resource.Id.TwLSector);
            TextView TwLRevoluciones = row.FindViewById<TextView>(Resource.Id.TwLRevoluciones);
            TextView TwLVelocidad = row.FindViewById<TextView>(Resource.Id.TwLVelocidad);
            TextView TwLCaudal = row.FindViewById<TextView>(Resource.Id.TwLCaudal);
            if(mblnTipoMedicionCaudal)
                TwLVelocidad.Visibility = ViewStates.Gone;       
            else
                TwLRevoluciones.Visibility = ViewStates.Gone;
            TwLBaseA.Text = lObjDet.MedicionBaseInicial.ToString();
            TwLBaseB.Text = lObjDet.MedicionBaseFinal.ToString();
            TwLSector.Text = lObjDet.SectorMetros.ToString();
            TwLRevoluciones.Text = lObjDet.Revoluciones.ToString();
            TwLVelocidad.Text = lObjDet.Velocidad.ToString();
            TwLCaudal.Text = lObjDet.Caudal.ToString();
            return row;
        }
    }
}