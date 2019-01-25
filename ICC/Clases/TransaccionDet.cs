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

namespace ICC
{
    public class TransaccionDet
    {
        public Guid Codigo { get; set; }
        public int NoCorrelativo { get; set; }
        public double MedicionBaseInicial { get; set; }
        public double MedicionBaseFinal { get; set; }
        public double SectorMetros { get; set; }
        public double Area { get; set; }
        public double Revoluciones { get; set; }
        public double Velocidad { get; set; }
        public double Caudal { get; set; }
    }
}