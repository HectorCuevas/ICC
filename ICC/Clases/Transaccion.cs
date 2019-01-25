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
    public class Transaccion
    {
        public Guid Codigo { get; set; }
        public DateTime FechaHoraInicial { get; set; }
        public DateTime FechaHoraFinal { get; set; }
        public string Cuenca { get; set; }
        public string SubCuenca { get; set; }
        public string PuntoMonitoreo { get; set; }
        public string ModeloMolinete { get; set; }
        public string IntervaloSector { get; set; }
        public string TipoMedicion { get; set; }
        public string Tipologia { get; set; }
        public double GpsLatitud { get; set; }
        public double GpsLongitud { get; set; }
        public byte[] ImagenNorte { get; set; }
        public byte[] ImagenSur { get; set; }
        public string Comentario { get; set; }
        public string Estado { get; set; }
        public string Usuario { get; set; }
        public double Caudal { get; set; }
    }
}