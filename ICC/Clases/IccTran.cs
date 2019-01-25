using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SQLite;

namespace ICC
{
    public class IccTran
    {
        [PrimaryKey, AutoIncrement, Column("_Codigo")]
        public int Codigo { get; set; }
        public string JsonTran { get; set; }        
    }
}