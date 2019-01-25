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

namespace ICC.Clases
{
    public class YoReporto
    {
        public Guid Codigo { get; set; }
        public DateTime FechaHoraInicial { get; set; } 
        public string Usuario { get; set; }
        public string Nombre { get; set; }
        public string Telefono { get; set; }
        public string Correo { get; set; }
        public string TipoReporte { get; set; }
        public string Departamento { get; set; }
        public string Municipio { get; set; }
        public string Barrio { get; set; }
        public string Direccion { get; set; }
        public string Comentarios { get; set; }
        public double GpsLatitud { get; set; }
        public double GpsLongitud { get; set; }
        public byte[] Imagen { get; set; }
        public string Emei { get; set; }
    }
}