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
        private string _connectionstring = "Server=192.168.100.56;Persist Security Info=True;Database=MIC;User Id=sa;Password=b4ckl45h;";

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
                        "LEFT JOIN BD_ENVIO_IMAGENES_APLICACION C ON C.ID_FOTO_AR = BD_FOTO_AR.ID_FOTO_AR " +
                        "WHERE BD_AR.ID_CLIENTE=43 AND STATUS = 'PROCESADO' AND ID_STATUS_AR <> 1 " +
                        "AND BD_ATTACH.FEC_ALTA > '22/09/2019 23:59:00' " + 
                        "AND C.NOMBRE_ARCHIVO IS NULL"
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
                insertErrores(ex.ToString());
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
                insertErrores(ex.ToString());
            }
        }

        public void insertErrores(string msg){
            using(SqlConnection con = new SqlConnection(_connectionstring)){
                SqlCommand cmd = new SqlCommand("INSERT INTO BD_LOG_ENVIO_IMAGENES_APLICACION " + 
                    "VALUES ('"+msg+"',GETDATE())",con);
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}
