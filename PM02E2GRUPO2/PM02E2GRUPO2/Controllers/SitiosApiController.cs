using System;
using System.Collections.Generic;
using System.Text;
using PM02E2GRUPO2.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.IO;
using Xamarin.Forms;
using Android.Util;

namespace PM02E2GRUPO2.Controllers
{
    
    public class SitiosApiController
    {        
        public async static Task<List<SitiosListModel>> ControllerObtenerListaSitios()
        {
            List<SitiosListModel> listasitios = new List<SitiosListModel>();

            using (HttpClient cliente = new HttpClient())
            {
                var respuesta = await cliente.GetAsync("https://webfacturacesar.000webhostapp.com/pm02exa/methods/sitios/index.php");

                if (respuesta.IsSuccessStatusCode)
                {
                    string contenido = respuesta.Content.ReadAsStringAsync().Result.ToString();

                    dynamic dyn = JsonConvert.DeserializeObject(contenido);
                    byte[] newBytes = null;


                    if (contenido.Length > 28)
                    {

                        foreach (var item in dyn.items)
                        {
                            string img64 = item.Foto.ToString();
                            newBytes = Convert.FromBase64String(img64);
                            var stream = new MemoryStream(newBytes);

                            string audio64 = item.Audio.ToString();
                            byte[] decodedString = Base64.Decode(audio64, Base64Flags.Default);

                            listasitios.Add(new SitiosListModel(
                                            item.Id.ToString(), item.Descripcion.ToString(),
                                            item.Latitud.ToString(), item.Longitud.ToString(),
                                            ImageSource.FromStream(() => stream),
                                            img64, audio64, decodedString
                                            ));
                        }
                    }
                }
            }
            return listasitios;
        }



    }
}
