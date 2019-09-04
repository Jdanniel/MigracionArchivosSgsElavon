using MigracionArchivosSgsElavon.DAL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace MigracionArchivosSgsElavon
{
    class Program
    {
        static void Main(string[] args)
        {
            var fotoatt = new FotoAttachDal();
            var listaFotos = fotoatt.GetList();

            listaFotos.ForEach(async item =>
            {
                await SendFiles(item.archivo, item.noar, item.idar, item.idfotoar);
                //Console.WriteLine(item.archivo);
                
            });
            fotoatt.insertDatos("","",0,"La aplicacion se ejecuto.",0,2);
            //Console.ReadKey();
        }

        static async Task<bool> SendFiles(string name, string noar, int idar, int id_foto_ar)
        {
            var foto = new FotoAttachDal();
            bool respuesta = false;
            try
            {
                HttpClient client = new HttpClient();
                MultipartFormDataContent form = new MultipartFormDataContent();
                HttpContent content = new StringContent("fileToUpload");
                HttpContent contentNoar = new StringContent(noar);
                form.Add(content, "fileToUpload");
                form.Add(contentNoar, "noar");

                var stream = new FileStream("C:\\inetpub\\wwwroot\\MIC3\\UPLOADER\\ARCHIVOS\\" + name, FileMode.Open);
                content = new StreamContent(stream);
                content.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                {
                    Name = "archivos",
                    FileName = name
                };

                form.Add(content);

                HttpResponseMessage response = null;
                response = (await client.PostAsync("http://sgse.microformas.com.mx:8093/api/files/ODT", form));
                respuesta = true;
                var k = response.Content.ReadAsStringAsync().Result;
                foto.insertDatos(name, noar, idar, k, id_foto_ar,1);
                return respuesta;
            }
            catch (Exception ex)
            {
                foto.insertDatos(name, noar, idar, "A OCURRIDO UN ERROR INTERNO EN EL PROGRAMA MigracionArchivosSgsElavon AL ENVIAR EL ARCHIVO  " + ex.StackTrace, id_foto_ar,0);
                return respuesta;
            }
        }
    }
}
