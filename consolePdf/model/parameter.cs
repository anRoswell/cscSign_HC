using System;
using System.Collections.Generic;
using System.Text;

namespace consolePdf.model
{
    public class Parameter
    {
        public MassTransit MassTransit { get; set; }
        public string ConnectionString { get; set; }
        public string _pathProfesionales { get; set; }
        public Smtp Smtp { get; set; }
        public Pdf Pdf { get; set; }
        public string Sede { get; set; }
        public string SedeDescription { get; set; }
        public DimensionesImagenes DimensionesImagenes { get; set; }
    }

    public class DimensionesImagenes
    {
        public int SWidth { get; set; }
        public int SHeight { get; set; }
        public int XAbsolutePosition { get; set; }
    }

    public class MassTransit
    {
        public string Host { get; set; }
        public string Queue { get; set; }
    }

    public class Pdf
    {
        public string NameJunta { get; set; }
        public string NameJunta2 { get; set; }
        public string NameJuntaFirmada { get; set; }
    }

    public class Smtp
    {
        public string Host { get; set; }
        public string Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string From { get; set; }
    }
}
