using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SQLite;

namespace ICC
{
    public class CAT_Plantilla_Movil
    {
        [PrimaryKey, AutoIncrement, Column("_Codigo")]
        public int Codigo { get; set; }
        public string Tabla { get; set; }
        public string Prefijo { get; set; }
        public string Descripcion { get; set; }
        public int Orden { get; set; }
    }
}