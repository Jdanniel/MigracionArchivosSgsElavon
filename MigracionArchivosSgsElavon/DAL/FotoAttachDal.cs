using MigracionArchivosSgsElavon.Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MigracionArchivosSgsElavon.DAL
{
    public class FotoAttachDal
    {
        private string _connectionstring = "Server=192.168.5.68;Persist Security Info=True;Database=MIC;User Id=sa;Password=micr0f0rmas;";

        public List<FotoAttachModel> GetList()
        {
            var lista = new List<FotoAttachModel>();

            try
            {
                using (SqlConnection con = new SqlConnection(_connectionstring))
                {
                    SqlCommand cmd = new SqlCommand("SELECT BD_FOTO_AR.ID_AR, " +
                        "BD_AR.NO_AR, BD_ATTACH.SYSTEM_FILENAME, BD_FOTO_AR.ID_FOTO_AR " +
                        "FROM BD_FOTO_AR " +
                        "INNER JOIN BD_AR ON BD_AR.ID_AR = BD_FOTO_AR.ID_AR " +
                        "INNER JOIN BD_ATTACH ON BD_FOTO_AR.ID_ATTACH = BD_ATTACH.ID_ATTACH " +
                        "WHERE BD_AR.ID_CLIENTE=43 AND STATUS = 'PROCESADO' AND ID_STATUS_AR <> 1 " +
                        "AND BD_FOTO_AR.ID_FOTO_AR NOT IN(SELECT B.ID_FOTO_AR FROM BD_ENVIO_IMAGENES_APLICACION AS B " +
                        "WHERE B.IS_ENVIO = 1) " +
                        "AND BD_ATTACH.FEC_ALTA > '01/01/2019 00:00:00'"
                        , con);
                    con.Open();

                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        lista.Add(new FotoAttachModel
                        {
                            idar = Convert.ToInt32(rdr[0]),
                            noar = rdr[1].ToString(),
                            archivo = rdr[2].ToString(),
                            idfotoar = Convert.ToInt32(rdr[3])
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return lista;
        }
        public void insertDatos(string archivo, string noar, int idar, string msg, int idfotoar, int value)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(_connectionstring))
                {
                    SqlCommand cmd = new SqlCommand("INSERT INTO BD_ENVIO_IMAGENES_APLICACION " +
                        "VALUES(" + idar + ",'" + noar + "','" + archivo + "',GETDATE(),'" + msg + "',"+idfotoar+","+value+")", con);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
