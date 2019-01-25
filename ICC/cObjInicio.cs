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
    public class cObjInicio : Application
    {
        public static string cUsuario = string.Empty;
        public static int cIndice = -1;
        public static bool cBlnTipoMedidicion = false;
        public static Transaccion cTran = new Transaccion();
        public static List<TransaccionDet> cTranDet = new List<TransaccionDet>();
        public static int cTipoUsuario = 0;
        public static string NumeroTelefono = string.Empty;
        public static string Imei = string.Empty;
    }            
}