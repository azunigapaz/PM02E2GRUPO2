using Plugin.Geolocator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;

namespace PM02E2GRUPO2.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MapaPage : ContentPage
    {
        // String mapEtiqueta, mapDireccion;
        // float mapLongitud, mapLatitud;

        public MapaPage()
        {
            InitializeComponent();
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            Pin ubicacion = new Pin();
            ubicacion.Label = "San Pedro Sula";
            ubicacion.Address = "Cerca de UTH";
            ubicacion.Type = PinType.Place;
            ubicacion.Position = new Position(15.5510539, -88.0109923);
            mpsitios.Pins.Add(ubicacion);

            mpsitios.MoveToRegion(new MapSpan(new Position(15.5510539, -88.0109923), 1, 1));

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