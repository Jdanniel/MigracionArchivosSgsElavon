using MigracionArchivosSgsElavon.DAL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace MigracionArchivosSgsElavon
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var fotoatt = new FotoAttachDal();
            var listaFotos = fotoatt.GetList();

            for (int i = 0; i < listaFotos.Count; i++)
            {
                Console.WriteLine(".............................................");
                await SendFiles(listaFotos[i].archivo,listaFotos[i].noar,listaFotos[i].idar,listaFotos[i].idfotoar);
                Console.WriteLine("...................................");
            }
        }

        static async Task SendFiles(string name, string noar, int idar, int id_foto_ar)
        {
            var foto = new FotoAttachDal();

            try
            {
                HttpClient client = new HttpClient();
                MultipartFormDataContent form = new MultipartFormDataContent();
                HttpContent content = new StringContent("fileToUpload");
                HttpContent contentNoar = new StringContent(noar);
                form.Add(content, "fileToUpload");
                form.Add(contentNoar, "noar");
                Console.WriteLine("Tomando Imagen: " + name);
                var stream = new FileStream("C:\\inetpub\\wwwroot\\MIC3\\UPLOADER\\ARCHIVOS\\" + name, FileMode.Open);
                content = new StreamContent(stream);
                content.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                {
                    Name = "archivos",
                    FileName = name
                };

                form.Add(content);

                HttpResponseMessage response = null;
                Console.WriteLine("Iniciando Envio de Imagen: " + name);
                response = (await client.PostAsync("http://sgse.microformas.com.mx:8093/api/files/ODT", form));
                var k = response.Content.ReadAsStringAsync().Result;

                if(response.StatusCode != HttpStatusCode.OK)
                {
                    foto.insertDatos(name,noar,idar,k,id_foto_ar,0);
                }

                foto.insertDatos(name, noar, idar, k, id_foto_ar,1);
            }
            catch (Exception ex)
            {
                foto.insertDatos(name, noar, idar, "A OCURRIDO UN ERROR INTERNO EN EL PROGRAMA MigracionArchivosSgsElavon AL ENVIAR EL ARCHIVO  " + ex.StackTrace, id_foto_ar,0);
            }
        }
    }
}
