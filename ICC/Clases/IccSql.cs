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
using System.IO;
using ICC.Clases;

namespace ICC
{
    public class IccSql
    {

        public void SubCrearDbIcc()
        {
            string dbPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "IccDb.db3");
            var db = new SQLiteConnection(dbPath);
            db.CreateTable<CAT_Plantilla_Movil>();
            db.CreateTable<CAT_Movil>();            
            db.CreateTable<IccTran>();
            db.CreateTable<IccReporte>();            
        }

        public bool FncPermiteTransaccion()
        {
            bool blnPermiteTransaccion = false;
            string dbPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "IccDb.db3");
            var db = new SQLiteConnection(dbPath);
            if (db.Table<CAT_Plantilla_Movil>().Count() > 0)
                blnPermiteTransaccion = true;
            return blnPermiteTransaccion;
        }

        public bool FncEnviaTransaccion()
        {
            bool blnPermiteTransaccion = false;
            string dbPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "IccDb.db3");
            var db = new SQLiteConnection(dbPath);
            if (db.Table<IccTran>().Count() > 0)
                blnPermiteTransaccion = true;
            return blnPermiteTransaccion;
        }

        public bool FncEnviaTransaccionYoReporto()
        {
            bool blnPermiteTransaccion = false;
            string dbPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "IccDb.db3");
            var db = new SQLiteConnection(dbPath);
            if (db.Table<IccReporte>().Count() > 0)
                blnPermiteTransaccion = true;
            return blnPermiteTransaccion;
        }

        public List<string> FncLeerTablaIcc(string pTabla)
        {
            List<string> lObjFilas = new List<string>();
            string dbPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "IccDb.db3");
            var db = new SQLiteConnection(dbPath);
            var lObjTabla = db.Query<CAT_Plantilla_Movil>("SELECT * FROM CAT_Plantilla_Movil WHERE Tabla = ?", pTabla);
            foreach (var lObjFila in lObjTabla)
            {
                lObjFilas.Add(lObjFila.Descripcion);
            }
            return lObjFilas;
        }

        public List<string> FncLeerTablaIcc(string pTabla, string pPrefijo)
        {
            List<string> lObjFilas = new List<string>();
            string dbPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "IccDb.db3");
            var db = new SQLiteConnection(dbPath);
            var lObjTabla = db.Query<CAT_Plantilla_Movil>("SELECT * FROM CAT_Plantilla_Movil WHERE Tabla = ? AND Prefijo = ?", pTabla, pPrefijo);
            foreach (var lObjFila in lObjTabla)
            {
                lObjFilas.Add(lObjFila.Descripcion);
            }
            return lObjFilas;
        }

        public int FncValidarUsuario(string lStrUsuario, string lStrContrasena)
        {
            string dbPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "IccDb.db3");
            var db = new SQLiteConnection(dbPath);
            CAT_Plantilla_Movil lObjTabla = new CAT_Plantilla_Movil();
            lObjTabla = db.Query<CAT_Plantilla_Movil>("SELECT * FROM CAT_Plantilla_Movil WHERE Tabla = 'LOGIN' AND Prefijo = ? AND Descripcion = ?",
                lStrUsuario, lStrContrasena).FirstOrDefault();
            if(lObjTabla == null)
            {
                lObjTabla = new CAT_Plantilla_Movil();
                lObjTabla.Orden = -1;
            }
            return Convert.ToInt32(lObjTabla.Orden);
        }

        public CAT_Movil FncBuscarMovil()
        {
            string dbPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "IccDb.db3");
            var db = new SQLiteConnection(dbPath);
            return db.Table<CAT_Movil>().FirstOrDefault();
        }

        public void SubGuardarMovil(string lStrUsuario, int TipoUsuario)
        {
            string dbPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "IccDb.db3");
            var db = new SQLiteConnection(dbPath);
            CAT_Movil lObjMovil = new CAT_Movil();
            lObjMovil.Usuario = lStrUsuario;
            lObjMovil.TipoUsuario = TipoUsuario;
            db.DeleteAll<CAT_Movil>();
            db.Insert(lObjMovil);
        }

        public List<IccTran> FncLeerTransaccion()
        {
            string dbPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "IccDb.db3");
            var db = new SQLiteConnection(dbPath);
            return db.Table<IccTran>().ToList();
        }

        public List<IccReporte> FncLeerTransaccionYoReporto()
        {
            string dbPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "IccDb.db3");
            var db = new SQLiteConnection(dbPath);
            return db.Table<IccReporte>().ToList();
        }

        public void SubCrearTransaccion(IccTran lObjTran)
        {
            string dbPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "IccDb.db3");
            var db = new SQLiteConnection(dbPath);
            db.Insert(lObjTran);
        }

        public void SubCrearTransaccionYoReporto(IccReporte lObjTran)
        {
            string dbPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "IccDb.db3");
            var db = new SQLiteConnection(dbPath);
            db.Insert(lObjTran);
        }

        public void SubEliminarTransaccion()
        {
            string dbPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "IccDb.db3");
            var db = new SQLiteConnection(dbPath);
            db.DeleteAll<IccTran>();
        }

        public void SubEliminarTransaccionYoReporto()
        {
            string dbPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "IccDb.db3");
            var db = new SQLiteConnection(dbPath);
            db.DeleteAll<IccReporte>();
        }

        public void SubEliminarCatalogos()
        {
            string dbPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "IccDb.db3");
            var db = new SQLiteConnection(dbPath);
            db.DeleteAll<CAT_Plantilla_Movil>();
        }

        public void SubAgregarCatalogo(CAT_Plantilla_Movil lObjMovil)
        {
            string dbPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "IccDb.db3");
            var db = new SQLiteConnection(dbPath);
            db.Insert(lObjMovil);
        }

        public void EliminarInicioAutomatico()
        {
            string dbPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "IccDb.db3");
            var db = new SQLiteConnection(dbPath);
            db.DeleteAll<CAT_Movil>();
        }
    }
}