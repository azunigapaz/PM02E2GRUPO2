using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace PM02E2GRUPO2.Models
{
    class ListarSitios
    {
        public ListarSitios(string id, string descripcion, string longitud, string lalitud, ImageSource fotografia,
                            string img64
                            )
        {
            this.id = id;
            this.descripcion = descripcion;
            this.longitud = longitud;
            this.lalitud = lalitud;
            this.fotografia = fotografia;
            this.img64 = img64;

        }

        public string id { get; set; }
        public string descripcion { get; set; }
        public string longitud { get; set; }
        public string lalitud { get; set; }
        public ImageSource fotografia { get; set; }
        public string img64 { get; set; }
    }
}
