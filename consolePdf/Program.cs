using System;
using System.Configuration;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace consolePdf
{
    //Variables para albergar imágenes de firmas.
    
    class Program
    {
        static string msg1 = "ERROR!!! No existe archivo ";
        static string msg2 = "ERROR!!! No existe firma ";
        static string complemento = "junta.pdf";
        static Firma sign;
        static string[] signPath = new String[6];
        static int[] signWidth = {415, 375, 340, 295};
        static string pathPDF = string.Empty;
        static string pathPDF2 = string.Empty;
        static string GetUrlApiRestWebConfig = string.Empty;
        static string appName  = string.Empty;
        static string profesionalesXX  = string.Empty;

        static void Main(string[] args)
        {
            //Console.Clear();
            init();

            ValidacionArchivos(pathPDF, msg1, complemento);
            sign = new Firma();
            sign.Sign = new String[4];

            GetSignFromDB();

            Validaciones();

            //string opcion = showMenu();
            string opcion = "3";
            GetSignsSelected(opcion);

            // Evalua que exista la plantilla PDF
            CTextSharp cTextSharp = new CTextSharp(pathPDF, pathPDF2);
            cTextSharp.Firmar(sign, signWidth);
        }

        static void init(){
            IConfigurationBuilder builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appSettings.json", true,true);

            IConfigurationRoot config = builder.Build();
            appName = config["ConnectionString"];
            var profesionales = config.GetValue<Profesionales>("Profesionales");
            IConfigurationSection pdf = config.GetSection(nameof(Pdf));
            

            GetUrlApiRestWebConfig = ConfigurationManager.AppSettings["MySetting"];

            //rutas de nuestros pdf
            pathPDF = @Path.GetFullPath(pdf["nameJunta"]);
            pathPDF2 = @Path.GetFullPath(pdf["nameJuntaFirmada"]);
        }

        static void Validaciones(){
            //Evalua que existan las firmas
            ValidacionArchivos(signPath[0], msg2, Reversar(signPath[0]));
            ValidacionArchivos(signPath[1], msg2, Reversar(signPath[1]));
            ValidacionArchivos(signPath[2], msg2, Reversar(signPath[2]));
            ValidacionArchivos(signPath[3], msg2, Reversar(signPath[3]));
            ValidacionArchivos(signPath[4], msg2, Reversar(signPath[4]));
            ValidacionArchivos(signPath[5], msg2, Reversar(signPath[5]));
        }
        static string showMenu(){
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("                             ┌──────────────────────────────────────┐");
            Console.WriteLine("                             │  Sistema para firmas de actas de     │");
            Console.WriteLine("                             │ juntas de profesionales de la salud  │");
            Console.WriteLine("                             │          MIPRES NO PBSUPC            │");
            Console.WriteLine("                             └──────────────────────────────────────┘");
            Console.WriteLine("                    ┌────────────────┐  ┌──────────────────────────────────────┐");
            Console.WriteLine("                    │     JUNTAS     │  │  I N T E G R A N T E S  J U N T A S  │");
            Console.WriteLine("                    ├────────────────┤  ├──────────────────────────────────────┤");
            Console.WriteLine("                    │..1] Junta 1....│  │ Tipon - Miranda - Ospina - Chacón    │");
            Console.WriteLine("                    │..2] Junta 2....│  │ Tipon - Miranda - Davila - Chacón    │");
            Console.WriteLine("                    │..3] Junta 3....│  │ Murillo - Ospina - Davila - Chacón   │");
            Console.WriteLine("                    └────────────────┘  └──────────────────────────────────────┘");
            Console.WriteLine("");
            Console.WriteLine("                      Digite una opción entre 1 y 3... pulse 0 para terminar");
        
            string opcion = "";
            int sw = 0;

            while (sw==0)
            {
                opcion = Console.ReadLine();
                if (opcion == "0")
                {
                    Salir();
                    Environment.Exit(1);
                }
                if (opcion != "1" && opcion !="2" && opcion != "3")
                {
                    Console.WriteLine("      Digite una opción entre 1 y 3...");
                    sw = 0;
                }
                else
                {
                    sw = 1;
                }
            }

            return opcion;
        }

        // Validamos existencia de archivos
        static void ValidacionArchivos(string file, string msgErr1, string complemen)
        {
            if (!File.Exists(file))
            {
                Console.WriteLine(msgErr1+complemen);
                Console.WriteLine("revisar que exista este elemento "+complemen);
                Console.WriteLine("pulsar cualquier tecla para salir. ");
                Console.ReadLine();
                Salir();
                Environment.Exit(1);
            }
        }   
        
        // Obtenemos nombres de firmantes para manejo personaliado de erroes
        static string Reversar (string cadena)
        {
            string nombre = "";
            for (int i = cadena.Length - 1; i > 0; i--)
            {
                if (cadena[i] == '\\')
                {
                    i = 0;
                }
                else
                {
                    nombre += cadena[i];
                }
            }

            string nombreInv = "";
            int a = 0;
            for (int j = nombre.Length - 1; j >= 0; j--)
            {
                a++;
                nombreInv += nombre[j];
            }
            return nombreInv;
        }

        static void GetSignsSelected(string opcion){
            switch (opcion)
            {
                case "1":
                    sign.Sign[0] = signPath[3];
                    sign.Sign[1] = signPath[2];
                    sign.Sign[2] = signPath[5];
                    sign.Sign[3] = signPath[0];
                    break;
                case "2":
                    sign.Sign[0] = signPath[3];
                    sign.Sign[1] = signPath[2];
                    sign.Sign[2] = signPath[1];
                    sign.Sign[3] = signPath[0];
                    break;
                case "3":
                    sign.Sign[0] = signPath[4];
                    sign.Sign[1] = signPath[5];
                    sign.Sign[2] = signPath[1];
                    sign.Sign[3] = signPath[0];
                    break;
                default:
                    Console.WriteLine("Opción inexistente");
                    break;
            }
        }

        static void GetSignFromDB(){
            //rutas de firmas
            signPath[0] = @Path.GetFullPath("miembros\\elizabethChacon.png");
            signPath[1] = @Path.GetFullPath("miembros\\enalbaDavila.png");
            signPath[2] = @Path.GetFullPath("miembros\\giskarMiranda.png");
            signPath[3] = @Path.GetFullPath("miembros\\robertoTipon.png");
            signPath[4] = @Path.GetFullPath("miembros\\danielaMurillo.png");
            signPath[5] = @Path.GetFullPath("miembros\\sandraOspina.png");
        }

        static void Salir()
        {
            Console.Clear();
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("                     **********************************************");
            Console.WriteLine("                     *              ingenIO Solutions             *");
            Console.WriteLine("                     **********************************************");
            Console.WriteLine("                     *                                            *");
            Console.WriteLine("                     *  > Software a la medida                    *");
            Console.WriteLine("                     *  > Asesoría y capacitaciones               *");
            Console.WriteLine("                     *  > Seguridad informática                   *");
            Console.WriteLine("                     *                                            *");
            Console.WriteLine("                     **********************************************");
            Console.WriteLine("                     *                CONTACTANOS                 *");
            Console.WriteLine("                     * charleoni@gmail.com - anroswell@gmail.com  *");
            Console.WriteLine("                     *      305 353 83 97 - 311 705 11 71         *");
            Console.WriteLine("                     **********************************************");
            Console.ReadLine();
        }
    }
}

public class Pdf{
    public string nameJunta { get; set; }
    public string nameJuntaFirmada { get; set; }
}

public class DimensionesImagenes{
    public int sWidth { get; set; }
    public int sHeight { get; set; }
    public int xAbsolutePosition { get; set; }
}

public class Firma{
    public string[] Sign { get; set; }
    public int SignWidth { get; set; }
}

public class Profesionales{
    public string nombre { get; set; }
    public int xposition { get; set; }
    public int yposition { get; set; }
    public string especialidad { get; set; }
    public string sede { get; set; }
}
