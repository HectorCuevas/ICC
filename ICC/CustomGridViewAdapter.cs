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
    public class CustomGridViewAdapter : BaseAdapter
    {

        private Context context;
        private string[] gridViewString;
        private int[] gridViewImage;
        private int NoColumnas;

        public CustomGridViewAdapter(Context context, string[] gridViewstr, int[] gridViewImage, int NoColumnas)
        {
            this.context = context;
            gridViewString = gridViewstr;
            this.gridViewImage = gridViewImage;
            this.NoColumnas = NoColumnas;
        }
        public override int Count
        {
            get
            {
                return gridViewString.Length;
            }
        }

        public override Java.Lang.Object GetItem(int position)
        {
            return null;
        }

        public override long GetItemId(int position)
        {
            return 0;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View view;
            LayoutInflater inflater = (LayoutInflater)context.GetSystemService(Context.LayoutInflaterService);
            if (convertView == null)
            {
                view = new View(context);
                view = inflater.Inflate(Resource.Layout.ListadoMenu, null);
                TextView txtView = view.FindViewById<TextView>(Resource.Id.TwGrid);
                ImageView imgView = view.FindViewById<ImageView>(Resource.Id.ImgGrid);
                txtView.Text = gridViewString[position];
                imgView.SetImageResource(gridViewImage[position]);
                if (NoColumnas == 2)
                {
                    imgView.LayoutParameters.Width = 150;
                    imgView.LayoutParameters.Height = 150;
                }
            }
            else
            {
                view = (View)convertView;
            }
            return view;
        }
    }
}