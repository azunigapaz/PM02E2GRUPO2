using Plugin.Geolocator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;

namespace PM02E2GRUPO2.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MapaPage : ContentPage
    {
        string mapEtiqueta, mapDireccion;
        double mapLongitud, mapLatitud;

        public MapaPage(String mapEtiquetaCt, String mapDireccionCt, double mapLongitudCt, double mapLatitudCt)
        {
            InitializeComponent();
            mapEtiqueta = mapEtiquetaCt;
            mapDireccion = mapDireccionCt;
            mapLongitud = mapLongitudCt;
            mapLatitud = mapLatitudCt;
        }

        private async void btnRuta_Clicked(object sender, EventArgs e)
        {
            var options = new MapLaunchOptions { NavigationMode = NavigationMode.Driving };            var location = new Location(mapLatitud, mapLongitud);            await Xamarin.Essentials.Map.OpenAsync(location, options);
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            Pin ubicacion = new Pin();
            ubicacion.Label = mapEtiqueta;
            ubicacion.Address = mapDireccion;
            ubicacion.Type = PinType.Place;
            ubicacion.Position = new Position(mapLatitud, mapLongitud);
            mpsitios.Pins.Add(ubicacion);

            mpsitios.Pins.Add(ubicacion);

            mpsitios.MoveToRegion(new MapSpan(new Position(mapLatitud, mapLongitud), 0.05, 0.05));

            var localizacion = CrossGeolocator.Current;

            if(localizacion != null)
            {
                localizacion.PositionChanged += Localizacion_positionChanged;

                if (!localizacion.IsListening)
                {
                    await localizacion.StartListeningAsync(TimeSpan.FromSeconds(10), 100);
                }
            }
        }

        private void Localizacion_positionChanged(object sender, Plugin.Geolocator.Abstractions.PositionEventArgs e)
        {
            var posicion_mapa = new Position(e.Position.Latitude, e.Position.Longitude);
            mpsitios.MoveToRegion(new MapSpan(posicion_mapa, 1, 1));
        }
    }
}