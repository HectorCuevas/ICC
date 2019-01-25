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
using SQLite;

namespace ICC.Clases
{
    public class IccReporte
    {
        [PrimaryKey, AutoIncrement, Column("_Codigo")]
        public int Codigo { get; set; }
        public string JsonTran { get; set; }
    }
}