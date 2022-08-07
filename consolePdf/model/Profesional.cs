using System;
using System.Collections.Generic;
using System.Text;

namespace consolePdf.model
{
    public class Profesional
    {
        public int id { get; set; }
        public string nombre { get; set; }
        public int xposition { get; set; }
        public int yposition { get; set; }
        public string especialidad { get; set; }
        public string especialidadCodigo { get; set; }
        public string sede { get; set; }
        public string sign { get; set; }
        public string signPath { get; set; }
        public int SignWidth { get; set; }
    }
}
