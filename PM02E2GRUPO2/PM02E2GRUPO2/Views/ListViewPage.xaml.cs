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
using Plugin.AudioRecorder;

namespace PM02E2GRUPO2.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ListViewPage : ContentPage
    {

        private readonly AudioPlayer audioPlayer = new AudioPlayer();

        string txtDescripcionSeleccionada;
        double dbLatitud, dbLongitud;

        List<Models.SitiosListModel> lista = new List<Models.SitiosListModel>();

        Object objSitioGlobal = null;
        string idGlobal = "";
        string sitioGlobal = "";
        string latitud = "";
        string longitud = "";

        public ListViewPage()
        {
            InitializeComponent();
            lista.Clear();
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
            var valores = e.SelectedItem as SitiosListModel;
            txtDescripcionSeleccionada = valores.Descripcion;
            dbLatitud = Convert.ToDouble(valores.Latitud);
            dbLongitud = Convert.ToDouble(valores.Longitud);

            idGlobal = null;

            idGlobal = valores.Id;
            sitioGlobal = valores.Descripcion;
            latitud = valores.Latitud;
            longitud = valores.Longitud;

            string audio64 = valores.Audio.ToString();
            byte[] decodedString = Base64.Decode(audio64, Base64Flags.Default);

            objSitioGlobal = new
            {
                id = valores.Id,
                latitud = valores.Latitud,
                longitud = valores.Longitud,
                descripcion = valores.Descripcion,
                imagen = ImageSource.FromStream(() => new MemoryStream(Convert.FromBase64String(valores.Foto))),
                audio = decodedString
            };


        }

        private async void btnEliminar_Clicked(object sender, EventArgs e)
        {

            if (!string.IsNullOrEmpty(idGlobal) || !string.IsNullOrEmpty(sitioGlobal))
            {
                bool res = await DisplayAlert("Notificación", $"¿Esta seguro de eliminar el sitio {sitioGlobal}?", "Sí", "Cancelar");

                if (res)
                {

                    object sitio = new
                    {
                        Id = idGlobal
                    };

                    Uri RequestUri = new Uri("https://webfacturacesar.000webhostapp.com/pm02exa/methods/sitios/del.php");
                    var client = new HttpClient();
                    var json = JsonConvert.SerializeObject(sitio);

                    HttpRequestMessage request = new HttpRequestMessage
                    {
                        Content = new StringContent(json, Encoding.UTF8, "application/json"),
                        Method = HttpMethod.Post,
                        RequestUri = RequestUri
                    };

                    HttpResponseMessage response = await client.SendAsync(request);

                    if (response.IsSuccessStatusCode)
                    {
                        await DisplayAlert("Notificación", $"Registro eliminado con éxito", "Ok");
                        lista.Clear();
                        GetSitiosList();
                    }
                    else
                    {
                        await DisplayAlert("Notificación", $"Ha ocurrido un error", "Ok");

                    }

                }
            }
            else
            {
                await DisplayAlert("Notificación", $"Por favor, seleccione un registro", "Ok");

            }

        }

        private async void btnActualizar_Clicked(object sender, EventArgs e)
        {
            if (objSitioGlobal != null)
            {
                var detalle = new ActualizarUbicacionPage();
                detalle.BindingContext = objSitioGlobal;
                await Navigation.PushAsync(detalle);
            }
            else
            {
                await DisplayAlert("Notificación", $"Por favor, seleccione un registro", "Ok");
            }
        }

        private async void btnVerMapa_Clicked(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(txtDescripcionSeleccionada))
            {
                await DisplayAlert("Mensaje", "Debe seleccionar ubicación.", "OK");
            }
            else
            {
                var openXamarinMap = new MapaPage("Ubicacion", txtDescripcionSeleccionada, dbLongitud, dbLatitud);
                await Navigation.PushAsync(openXamarinMap);
            }

        }

        private void btnescucharaudio_Clicked(object sender, EventArgs e)
        {
            Models.Sitios item = new Models.Sitios();

            var uri = item.Audio;
            audioPlayer.Play(uri);
        }
    }
}