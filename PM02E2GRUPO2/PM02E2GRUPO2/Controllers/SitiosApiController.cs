using System;
using System.Collections.Generic;
using System.Text;
using PM02E2GRUPO2.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace PM02E2GRUPO2.Controllers
{
    public static class SitiosApiController
    {

        public async static Task<List<Sitios>> getSitios()
        {
            List<Sitios> listasitios = new List<Sitios>();

            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetAsync("https://webfacturacesar.000webhostapp.com/pm02exa/methods/sitios/index.php");

                if (response.IsSuccessStatusCode)
                {
                    var Contenido = response.Content.ReadAsStringAsync().Result;

                    listasitios = JsonConvert.DeserializeObject<List<Sitios>>(Contenido);
                }
            }

            return listasitios;
        }


    }
}
