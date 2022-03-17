using Plugin.Media;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Plugin.AudioRecorder;
using PM02E2GRUPO2.Views;

namespace PM02E2GRUPO2
{
    public partial class MainPage : ContentPage
    {

        public string AudioPath, fileName;

        private readonly AudioRecorderService audioRecorderService = new AudioRecorderService
        {
            StopRecordingOnSilence = true, //will stop recording after 2 seconds (default)
            StopRecordingAfterTimeout = true,  //stop recording after a max timeout (defined below)
            TotalAudioTimeout = TimeSpan.FromSeconds(180) //audio will stop recording after 3 minutes
        };

        private readonly AudioPlayer audioPlayer = new AudioPlayer();

        public MainPage()
        {
            InitializeComponent();
            obtenerCoordenadas();
            descripcion_entry.Text = "";
            imgubicacionactual.Source = null;
        }

        byte[] imageToSave;

        private void btnguardarubicacion_Clicked(object sender, EventArgs e)
        {

        }

        private async void btnlistviewubicaciones_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ListViewPage());
        }

        private async void btngrabaraudio_Clicked(object sender, EventArgs e)
        {

            var status = await Permissions.RequestAsync<Permissions.Microphone>();

            if (status != PermissionStatus.Granted)
                return;

            if (audioRecorderService.IsRecording)
            {
                await audioRecorderService.StopRecording();
                //audioPlayer.Play(audioRecorderService.GetAudioFilePath());
                //await DisplayAlert("Alerta", audioRecorderService.TotalAudioTimeout.ToString(), "cancel");
                getRecord();
            }
            else
            {
                await audioRecorderService.StartRecording();
            }

        }

        private async void btntomarphoto_Clicked(object sender, EventArgs e)
        {

            try
            {
                //Code to execute on tapped event

                var takepic = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
                {
                    Directory = "PhotoApp",
                    Name = DateTime.Now.ToString() + "_Pic.jpg",
                    SaveToAlbum = true
                });

                await DisplayAlert("Ubicacion de la foto: ", takepic.Path, "Ok");

                if (takepic != null)
                {
                    imageToSave = null;
                    MemoryStream memoryStream = new MemoryStream();

                    takepic.GetStream().CopyTo(memoryStream);
                    imageToSave = memoryStream.ToArray();

                    imgubicacionactual.Source = ImageSource.FromStream(() => { return takepic.GetStream(); });
                }

                //await DisplayAlert("Aviso", "Ha dado click en la imagen ", "Ok");
                obtenerCoordenadas();
                descripcion_entry.Focus();
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }

        public async void obtenerCoordenadas()
        {
            try
            {
                var georequest = new GeolocationRequest(GeolocationAccuracy.Best, TimeSpan.FromSeconds(10));

                var tokendecancelacion = new CancellationTokenSource();

                var localizacion = await Geolocation.GetLocationAsync(georequest, tokendecancelacion.Token);


                if (localizacion != null)
                {
                    latitud_entry.Text = localizacion.Latitude.ToString();
                    longitud_entry.Text = localizacion.Longitude.ToString();
                }
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                await DisplayAlert("Advertencia", "Este dispositivo no soporta GPS", "Ok");
            }
            catch (FeatureNotEnabledException fneEx)
            {
                await DisplayAlert("Advertencia", "Error de Dispositivo", "Ok");
            }
            catch (PermissionException pEx)
            {
                await DisplayAlert("Advertencia", "Sin Permisos de Geolocalizacion", "Ok");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Advertencia", "Sin Ubicacion", "Ok");
            }
        }

        private async void getRecord()
        {

            //var audioFile = await recorder;
            if (audioRecorderService.FilePath != null) //non-null audioFile indicates audio was successfully recorded
            {
                //do something with the file
                //var path = audioRecorderService.FilePath;
                //await CrossMediaManager.Current.Play("file://" + path);

                var stream = audioRecorderService.GetAudioFileStream();

                //string fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), DateTime.Now.ToString("dd_MM_yyyy_mm_ss") + "_sample.wav");
                fileName = Path.Combine(FileSystem.CacheDirectory, DateTime.Now.ToString("ddMMyyyymmss") + "_VoiceNote.wav");

                using (var fileStream = new FileStream(fileName, FileMode.Create, FileAccess.Write))
                {
                    stream.CopyTo(fileStream);
                }

                await DisplayAlert("Alerta", fileName, "cancel");

                AudioPath = fileName;

            }

        }


    }
}
