using System;
using System.Configuration;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

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
        static string sede  = string.Empty;
        static string profesionalesXX  = string.Empty;
        static string _path = "json\\profesionales.json";
        static Profesional profesionalSeleccionado = new Profesional();
        static List<Profesional> profesionalesAll= new List<Profesional>();
        static List<Profesional> profesionalesFilter= new List<Profesional>();
        static List<IGrouping<string, Profesional>> profesionalesAgrupados = new List<IGrouping<string, Profesional>>();

        static void Main(string[] args)
        {
            init();

            // Obtengo todos los profesionales
            string profesionalesString = GetProfesionalesJsonFromFile();
            profesionalesAll = DeserializeJson(profesionalesString);

            // Filtro por sede
            profesionalesAgrupados = profesionalesAll
                        .AsEnumerable()
                        .Where(x => x.sede == sede)
                        .OrderBy(d => d.especialidad)
                        .ThenBy(x => x.nombre)
                        .GroupBy(d => d.especialidad)
                        .ToList();

            profesionalesFilter = profesionalesAgrupados.SelectMany(group => group).ToList();

            ValidacionArchivos(pathPDF, msg1, complemento);
            sign = new Firma();
            sign.Sign = new String[4];

            //Validaciones();
            showMenuMiddle();
        }

        static void init(){
            IConfigurationBuilder builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appSettings.json", true,true);

            IConfigurationRoot config = builder.Build();
            appName = config["ConnectionString"];

            //Sede
            sede = config["Sede"];

            //var profesionales = config.GetValue<>("Profesionales");
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
        
        static bool showMenuMiddle(){
            bool resp = false;
            try
            {
                 string opcion = showMenu();
                //string opcion = "3";
                Console.WriteLine("La opcion seleccionada es: ", opcion);

                // Evalua que exista la plantilla PDF

                CTextSharp cTextSharp = new CTextSharp(pathPDF, pathPDF2);
                resp = cTextSharp.Firmar(profesionalSeleccionado);
                if(resp){
                    showMenuMiddle();
                }
            }
            catch (System.Exception)
            {
                showMenuMiddle();
            }
            return resp;
        }

        static string showMenu(){
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("                             ┌──────────────────────────────────────┐");
            Console.WriteLine("                             │          REGIONAL BOLIVAR            │");
            Console.WriteLine("                             │  Sistema para firmas de actas de     │");
            Console.WriteLine("                             │ juntas de profesionales de la salud  │");
            Console.WriteLine("                             │          MIPRES NO PBSUPC            │");
            Console.WriteLine("                             └──────────────────────────────────────┘");

            
            Console.WriteLine("                    ┌────────────────┐  ┌──────────────────────────────────────┐");
            Console.WriteLine("                    │    OPCIONES    │  │  FIRMA PROFESIONALES REGIONAL CSC    │");
            Console.WriteLine("                    └────────────────┘  └──────────────────────────────────────┘");
            foreach (var profresionalesEspecialidad in profesionalesAgrupados)
            {
                Console.WriteLine("                    ┌────────────────┐  ┌──────────────────────────────────────┐");
                Console.WriteLine("                    ├  ESPECIALIDAD  ┤  ├  {0}                       ┤", profresionalesEspecialidad.Key);
                Console.WriteLine("                    ├────────────────┤  ├──────────────────────────────────────┤");

                Console.WriteLine("                    ├ SELECCIONE ID  ┤  ├                                      ┤");
                Console.WriteLine("                    │                │  │                                      │");

                foreach (var item in profresionalesEspecialidad)
                {
                    Console.WriteLine("                    │  {0}]            │  │ {1}                        │", item.id, item.nombre);
                }
                Console.WriteLine("                    └────────────────┘  └──────────────────────────────────────┘");
            }

            Console.WriteLine("                                                                                ");
            Console.WriteLine("                      Seleccione un ID por favor... pulse 0 para terminar");
        
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

                int profesionalSeleccionadoValidar = profesionalesFilter.Where(x => x.id == Convert.ToInt32(opcion)).Count();
                if (profesionalSeleccionadoValidar==0)
                {
                    Console.WriteLine("      Seleccione un id valido por favor...");
                    sw = 0;
                }
                else
                {
                    profesionalSeleccionado = profesionalesFilter.Where(x => x.id == Convert.ToInt32(opcion)).First();
                    profesionalSeleccionado.signPath = @Path.GetFullPath(profesionalSeleccionado.sign);
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
                Console.WriteLine("Revisar que exista este elemento " + complemen);
                Console.WriteLine("Pulsar cualquier tecla para salir. ");
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

        static void AddProfesional(Profesional profesional){
            profesionalesAll.Add(profesional);
            SaveProfesional(profesionalesAll);
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
    
        #region Writing Json
        static void SaveProfesional(List<Profesional> profesionales){
            string profesionalJson = JsonConvert.SerializeObject(profesionales);
            File.WriteAllText(_path, profesionalJson);
        }
        #endregion

        #region GetProfesionalesJsonFromFile
        static string GetProfesionalesJsonFromFile()
        {
            string profesionalesJsonFromFile = string.Empty;

            using(var reader = new StreamReader(_path))
            {
                profesionalesJsonFromFile = reader.ReadToEnd();
            }

            return profesionalesJsonFromFile;
        }
        #endregion

        #region DeserializeJson
        static List<Profesional> DeserializeJson(string json){
            return JsonConvert.DeserializeObject<List<Profesional>>(json);
        }
        #endregion
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

public class Profesional{
    public int id { get; set; }
    public string nombre { get; set; }
    public int xposition { get; set; }
    public int yposition { get; set; }
    public string especialidad { get; set; }
    public string sede { get; set; }
    public string sign {get; set;}
    public string signPath { get; set; }
    public int SignWidth { get; set; }
}
