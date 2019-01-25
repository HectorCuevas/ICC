using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SQLite;

namespace ICC
{
    public class CAT_Movil
    {
        [PrimaryKey, AutoIncrement, Column("_Codigo")]
        public int Codigo { get; set; }
        public string Usuario { get; set; }
        public int TipoUsuario { get; set; }
    }
}