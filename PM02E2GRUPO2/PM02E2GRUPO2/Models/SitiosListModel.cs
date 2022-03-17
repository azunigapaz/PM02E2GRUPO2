using System;
using System.Collections.Generic;
using System.Text;

namespace PM02E2GRUPO2.Models
{
    class SitiosListModel
    {
        public SitiosListModel(string Id, string Descripcion, string Latitud, string Longitud, string Foto, string Audio)
        {
            this.Id = Id;
            this.Descripcion = Descripcion;
            this.Latitud = Latitud;
            this.Longitud = Longitud;
            this.Foto = Foto;
            this.Audio = Audio;
        }

        public string Id { get; set; }
        public string Descripcion { get; set; }
        public string Latitud { get; set; }
        public string Longitud { get; set; }
        public string Foto { get; set; }
        public string Audio { get; set; }

    }
}
