using System;
using System.Configuration;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.Extensions.Configuration;

namespace consolePdf
{
    //Variables para albergar imágenes de firmas.
    
    class Program
    {        
        static PdfReader oReader = null;
        //Objeto que tiene el tamaño de nuestro documento
        static Rectangle oSize = null;
        //documento de itextsharp para realizar el trabajo asignandole el tamaño del original
        static Document oDocument = null;
        static string msg1 = "ERROR!!! No existe archivo ";
        static string msg2 = "ERROR!!! No existe firma ";
        static string complemento = "junta.pdf";

            //rutas de firmas
    static string[] firmasSearch;

        static void Main(string[] args)
        {
            Console.Clear();
            IConfigurationBuilder builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appSettings.json", true,true);

            IConfigurationRoot config = builder.Build();
            string appName = config["ConnectionString"];
            IConfigurationSection pdf = config.GetSection(nameof(Pdf));
            IConfigurationSection dimensionesImagenes = config.GetSection(nameof(DimensionesImagenes));


            string GetUrlApiRestWebConfig = ConfigurationManager.AppSettings["MySetting"];

            //rutas de nuestros pdf
            string pathPDF = @Path.GetFullPath(pdf["nameJunta"]);
            string pathPDF2 = @Path.GetFullPath(pdf["nameJuntaFirmada"]);

            ValidacionArchivos(pathPDF, msg1, complemento);

            //rutas de firmas
            firmasSearch[0] = @Path.GetFullPath("miembros\\elizabethChacon.png");
            firmasSearch[1] = @Path.GetFullPath("miembros\\enalbaDavila.png");
            firmasSearch[2] = @Path.GetFullPath("miembros\\giskarMiranda.png");
            firmasSearch[3] = @Path.GetFullPath("miembros\\robertoTipon.png");
            firmasSearch[4] = @Path.GetFullPath("miembros\\danielaMurillo.png");
            firmasSearch[5] = @Path.GetFullPath("miembros\\sandraOspina.png");

            //Evalua que existan las firmas
            ValidacionArchivos(firmasSearch[0], msg2, Reversar(firmasSearch[0]));
            ValidacionArchivos(firmasSearch[1], msg2, Reversar(firmasSearch[1]));
            ValidacionArchivos(firmasSearch[2], msg2, Reversar(firmasSearch[2]));
            ValidacionArchivos(firmasSearch[3], msg2, Reversar(firmasSearch[3]));
            ValidacionArchivos(firmasSearch[4], msg2, Reversar(firmasSearch[4]));
            ValidacionArchivos(firmasSearch[5], msg2, Reversar(firmasSearch[5]));

            string opcion = showMenu();
                                
            // Evalua que exista la plantilla PDF
            CTextSharp cTextSharp = new CTextSharp();
            cTextSharp.Firmar(opcion, firmasSearch);
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
    public string Sign { get; set; }
    public int SignWidth { get; set; }
}

