using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using PM02E2GRUPO2.Models;
using PM02E2GRUPO2.Controllers;

using Xamarin.Essentials;
using System.Net.Http;

using Newtonsoft.Json;
using System.IO;
using Android.Util;

namespace PM02E2GRUPO2.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ListViewPage : ContentPage
    {
        List<Models.SitiosListModel> lista = new List<Models.SitiosListModel>();
        public ListViewPage()
        {
            InitializeComponent();            
            GetSitiosList();
        }

        private void listview_ubicaciones_ItemTapped(object sender, ItemTappedEventArgs e)
        {

        }

        private async void GetSitiosList()
        {
            var AccesoInternet = Connectivity.NetworkAccess;

            if (AccesoInternet == NetworkAccess.Internet)
            {
                sl.IsVisible = true;
                spinner.IsRunning = true;

                List<SitiosListModel> listapersonas = new List<SitiosListModel>();
                listapersonas = await SitiosApiController.ControllerObtenerListaSitios();

                if (listapersonas.Count > 0)
                {
                    lsSitios.ItemsSource = null;
                    lsSitios.ItemsSource = listapersonas;
                }
                else
                {
                    await DisplayAlert("Notificación", $"Lista vacía, ingrese datos", "Ok");

                }

                sl.IsVisible = false;
                spinner.IsRunning = false;
            }
        }

        private async void ObtenerListaSitios()
        {
            using (HttpClient cliente = new HttpClient())
            {
                sl.IsVisible = true;
                spinner.IsRunning = true;
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

                            lista.Add(new Models.SitiosListModel(
                                            item.Id.ToString(), item.Descripcion.ToString(),
                                            item.Latitud.ToString(), item.Longitud.ToString(),
                                            ImageSource.FromStream(() => stream),
                                            img64, audio64, decodedString
                                            ));
                        }
                    }
                    else
                    {
                        await DisplayAlert("Notificación", $"Lista vacía, ingrese datos", "Ok");

                    }

                    lsSitios.ItemsSource = null;

                    lsSitios.ItemsSource = lista;
                }
            }

            sl.IsVisible = false;
            spinner.IsRunning = false;

        }

        private void lsSitios_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {

        }

        private void btnEliminar_Clicked(object sender, EventArgs e)
        {

        }

        private void btnActualizar_Clicked(object sender, EventArgs e)
        {

        }

        private void btnVerMapa_Clicked(object sender, EventArgs e)
        {

        }
    }
}