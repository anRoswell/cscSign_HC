using System;
using System.Configuration;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using consolePdf.model;
using consolePdf.Clases;
using System.Threading.Tasks;
using consolePdf.DTO;

namespace consolePdf
{
    //Variables para albergar imágenes de firmas.
    
    class Program
    {
        static string msg1 = "ERROR!!! No existe archivo ";
        static string msg2 = "ERROR!!! No existe firma ";
        static string complemento = "junta.pdf";
        static Root profesionalesApi;
        static Firma sign;
        static string[] signPath = new String[6];
        static string pathPDF = string.Empty;
        static string SedeDescription  = string.Empty;
        static string _pathProfesionales = "json\\profesionales.json";
        static string _pathProfesionalesTxt = "json\\profesionales.txt";
        static Parameter parameter = new Parameter ();
        static Profesional profesionalSeleccionado = new Profesional();
        static Body profSeleccionado = new Body();
        static List<Profesional> profesionalesAll = new List<Profesional>();
        static List<Profesional> profesionalesFilter = new List<Profesional>();
        static List<Body> profFilter = new List<Body>();
        static List<IGrouping<string, Profesional>> profesionalesAgrupados = new List<IGrouping<string, Profesional>>();
        static List<IGrouping<string, Body>> profAgr = new List<IGrouping<string, Body>>();

        static async Task Main(string[] args)
        {
            try
            {
                await InitAsync();


                // Filtro por sede
                //profesionalesAgrupados = profesionales
                //            .Where(x => x.sede == parameter.Sede)
                //            .OrderBy(d => d.especialidad)
                //            .ThenBy(x => x.nombre)
                //            .GroupBy(d => d.especialidad)
                //            .ToList();

                // Filtro por sede
                profAgr = profesionalesApi.body
                        .Where(x => x.idSede == parameter.SedeId)
                        .OrderBy(d => d.idProfesion)
                        .ThenBy(x => x.Nombre)
                        .GroupBy(d => d.Profesion)
                        .ToList();

                //profesionalesFilter = profesionalesAgrupados.SelectMany(group => group).ToList();
                profFilter = profAgr.SelectMany(group => group).ToList();

                pathPDF = parameter.Pdf.NameJunta;
                ValidacionArchivos(pathPDF, msg1, complemento);

                sign = new Firma
                {
                    Sign = new String[4]
                };

                //Validaciones();
                showMenuMiddle();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }            
        }

        static async Task InitAsync(){
            //profesionalesAll = GetProfesionaleAll();
            profesionalesApi = await GetProfesionalsApiAsync();
            parameter = GetParameters(); 
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
                Console.WriteLine("La opcion seleccionada es: {0}", opcion);

                // Evalua que exista la plantilla PDF

                CTextSharp cTextSharp = new(pathPDF, parameter);
                //resp = cTextSharp.Firmar(profSeleccionado);
                resp = cTextSharp.Firmar(profSeleccionado);
                if(resp){
                    showMenuMiddle();
                }
            }
            catch (System.Exception e)
            {
                Console.WriteLine("Se ha presentado un error: {0}", e.Message);
                showMenuMiddle();
            }
            return resp;
        }

        static string showMenu(){
            SedeDescription = parameter.SedeDescription;
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("                             ┌──────────────────────────────────────┐");
            Console.WriteLine("                             │       {0}          │", SedeDescription);
            Console.WriteLine("                             │  Sistema para firmas de actas de     │");
            Console.WriteLine("                             │ juntas de profesionales de la salud  │");
            Console.WriteLine("                             │          MIPRES NO PBSUPC            │");
            Console.WriteLine("                             └──────────────────────────────────────┘");

            
            Console.WriteLine("                    ┌────────────────┐  ┌──────────────────────────────────────┐");
            Console.WriteLine("                    │    OPCIONES    │  │  FIRMA PROFESIONALES REGIONAL CSC    │");
            Console.WriteLine("                    └────────────────┘  └──────────────────────────────────────┘");
            foreach (var profresionalesEspecialidad in profAgr)
            {
                Console.WriteLine("                    ┌────────────────┐  ┌──────────────────────────────────────┐");
                Console.WriteLine("                    ├  ESPECIALIDAD  ┤  ├  {0}                       ┤", profresionalesEspecialidad.Key);
                Console.WriteLine("                    ├────────────────┤  ├──────────────────────────────────────┤");
                Console.WriteLine("                    ├ SELECCIONE ID  ┤  ├                                      ┤");
                Console.WriteLine("                    │                │  │                                      │");

                foreach (var item in profresionalesEspecialidad)
                {
                    Console.WriteLine("                    │  {0}]           │  │ {1}                        │", item.id, item.Nombre);
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

                int profesionalSeleccionadoValidar = profFilter.Where(x => x.id == Convert.ToInt32(opcion)).Count();

                if (profesionalSeleccionadoValidar==0)
                {
                    Console.WriteLine("      Seleccione un id valido por favor...");
                    sw = 0;
                }
                else
                {
                    //profesionalSeleccionado = profesionalesFilter.Where(x => x.id == Convert.ToInt32(opcion)).First();
                    profSeleccionado = profFilter.Where(x => x.id == Convert.ToInt32(opcion)).First();
                    //profesionalSeleccionado.signPath = @Path.GetFullPath(profesionalSeleccionado.sign);
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
            File.WriteAllText(_pathProfesionales, profesionalJson);
        }
        #endregion

        #region GetProfesionalesJsonFromFile
        static string GetProfesionalesJsonFromFile()
        {
            string profesionalesJsonFromFile = string.Empty;

            using(var reader = new StreamReader(_pathProfesionales))
            {
                profesionalesJsonFromFile = reader.ReadToEnd();
            }

            return profesionalesJsonFromFile;
        }

        static string GetProfesionalesTxtFromFile()
        {
            string profesionalesTxtFromFile = string.Empty;

            using(var reader = new StreamReader(_pathProfesionalesTxt))
            {
                profesionalesTxtFromFile = reader.ReadToEnd();
            }

            return profesionalesTxtFromFile;
        }
        #endregion

        #region DeserializeJson
        static List<Profesional> DeserializeJson(string json){
            return JsonConvert.DeserializeObject<List<Profesional>>(json);
        }
        #endregion
    
        /// <summary>
        /// Obtenemos listado de todos los profesionales
        /// </summary>
        /// <returns>Objeto</returns>
        static List<Profesional> GetProfesionaleAll()
        {
            string fileName = "json/profesionales.json";
            string path = @Path.GetFullPath(fileName);
            string jsonString = string.Empty;
            using (StreamReader r = new(path))
            {
                jsonString = r.ReadToEnd();
            }          
            return JsonConvert.DeserializeObject<List<Profesional>>(jsonString);
        }

        static async Task<Root> GetProfesionalsApiAsync()
        {
            Root profesionales = await HttpHelper.Get<Root>("profesionales", parameter.SedeId);
            return profesionales;
        }

        static Parameter GetParameters()
        {
            string fileName2 = "appSettings.json";
            string path2 = @Path.GetFullPath(fileName2);
            string pathPDF = string.Empty;
            using (StreamReader r2 = new(path2))
            {
                pathPDF = r2.ReadToEnd();
            }            
            return JsonConvert.DeserializeObject<Parameter>(pathPDF);
        }
    }
}


public class Firma{
    public string[] Sign { get; set; }
    public int SignWidth { get; set; }
}
