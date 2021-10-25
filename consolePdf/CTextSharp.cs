using System;
using System.IO;
using System.Configuration;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.Extensions.Configuration;

public class CTextSharp{
    static PdfReader oReader = null;
    //Objeto que tiene el tamaño de nuestro documento
    static Rectangle oSize = null;
    //documento de itextsharp para realizar el trabajo asignandole el tamaño del original
    static Document oDocument = null;

    // Declaramos variables globales
    int sWidth;
    int sHeight;
    int xAbsolutePosition;
    string GetUrlApiRestWebConfig = string.Empty;

    //rutas de nuestros pdf
    string pathPDF = string.Empty;
    string pathPDF2 = string.Empty;

    //Variables de validacion
    string[] firmas;


    string dirPruebas = string.Empty;
    string ficPruebas = string.Empty;

    public CTextSharp(){
        IConfigurationBuilder builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appSettings.json", true,true);

        IConfigurationRoot config = builder.Build();
        string appName = config["ConnectionString"];
        IConfigurationSection pdf = config.GetSection(nameof(Pdf));
        IConfigurationSection dimensionesImagenes = config.GetSection(nameof(DimensionesImagenes));

        // Dimensiones imágenes
        this.sWidth = Convert.ToInt32(dimensionesImagenes["sWidth"]);
        this.sHeight = Convert.ToInt32(dimensionesImagenes["sHeight"]);
        this.xAbsolutePosition = Convert.ToInt32(dimensionesImagenes["xAbsolutePosition"]);

        string GetUrlApiRestWebConfig = ConfigurationManager.AppSettings["MySetting"];

        string dirPruebas = @"E:\Pruebas3\RSA cripto";
        string ficPruebas = Path.Combine(dirPruebas, "MisClaves_CS.xml");
    }

   // Obtenemos nombres de firmantes para manejo personaliado de erroes
    

    public bool Firmar(string opcion, string[] firmasSearch){
        //Objeto para leer el pdf original
        oReader = new PdfReader(pathPDF);

        //Objeto que tiene el tamaño de nuestro documento
        oSize = oReader.GetPageSizeWithRotation(1);

        //documento de itextsharp para realizar el trabajo asignandole el tamaño del original
        oDocument = new Document(oSize);

        try
        {
            FileStream oFS = new FileStream(pathPDF2, FileMode.Create, FileAccess.Write);
            PdfWriter oWriter = PdfWriter.GetInstance(oDocument, oFS);
            oDocument.Open();

            switch (opcion)
            {
                case "1":
                    firmas[0] = firmasSearch[4];
                    firmas[1] = firmasSearch[3];
                    firmas[2] = firmasSearch[6];
                    firmas[3] = firmasSearch[1];
                    break;
                case "2":
                    firmas[0] = firmasSearch[4];
                    firmas[1] = firmasSearch[3];
                    firmas[2] = firmasSearch[2];
                    firmas[3] = firmasSearch[1];
                    break;
                case "3":
                    firmas[0] = firmasSearch[5];
                    firmas[1] = firmasSearch[6];
                    firmas[2] = firmasSearch[2];
                    firmas[3] = firmasSearch[1];
                    break;
                default:
                    Console.WriteLine("Opción inexistente");
                    break;
            }

            // Creamos la imagen y le ajustamos el tamaño
            iTextSharp.text.Image imagen1 = CreateImagenToPdf(firmas[1], 415);
            iTextSharp.text.Image imagen2 = CreateImagenToPdf(firmas[2], 375);
            iTextSharp.text.Image imagen3 = CreateImagenToPdf(firmas[3], 340);
            iTextSharp.text.Image imagen4 = CreateImagenToPdf(firmas[4], 295);

            // iTextSharp.text.Image imagen1 = iTextSharp.text.Image.GetInstance(firmas[1]);
            // imagen1.BorderWidth = 0;
            // imagen1.SetAbsolutePosition(xAbsolutePosition, 415);
            // imagen1.ScaleAbsoluteWidth(sWidth);
            // imagen1.ScaleAbsoluteHeight(sHeight);

            // iTextSharp.text.Image imagen2 = iTextSharp.text.Image.GetInstance(firmas[2]);
            // imagen2.BorderWidth = 0;
            // imagen2.SetAbsolutePosition(xAbsolutePosition, 375);
            // imagen2.ScaleAbsoluteWidth(sWidth);
            // imagen2.ScaleAbsoluteHeight(sHeight);

            // iTextSharp.text.Image imagen3 = iTextSharp.text.Image.GetInstance(firmas[3]);
            // imagen3.BorderWidth = 0;
            // imagen3.SetAbsolutePosition(xAbsolutePosition, 340);
            // imagen3.ScaleAbsoluteWidth(sWidth);
            // imagen3.ScaleAbsoluteHeight(sHeight);

            // iTextSharp.text.Image imagen4 = iTextSharp.text.Image.GetInstance(firmas[4]);
            // imagen4.BorderWidth = 0;
            // imagen4.SetAbsolutePosition(xAbsolutePosition, 295);
            // imagen4.ScaleAbsoluteWidth(sWidth);
            // imagen4.ScaleAbsoluteHeight(sHeight);

            //El contenido del pdf, aqui se hace la escritura del contenido
            PdfContentByte oPDF = oWriter.DirectContent;

            //Se abre el flujo para escribir el texto
            oPDF.BeginText();
            oPDF.EndText();

            //crea una nueva pagina y agrega el pdf original
            PdfImportedPage page = oWriter.GetImportedPage(oReader, 1);
            oPDF.AddTemplate(page, 0, 0);

            // Cerramos los objetos utilizados
            oDocument.Add(imagen1);
            oDocument.Add(imagen2);
            oDocument.Add(imagen3);
            oDocument.Add(imagen4);
            oDocument.Close();
            
            oFS.Close();
            oWriter.Close();
            oReader.Close();
            System.Diagnostics.Process proc = new System.Diagnostics.Process();
            
            Console.WriteLine("                               *** Firmado satisfactoriamente !!!!! ***");
            Console.WriteLine("                            *** presione cualquier tecla para terminar *** ");
            Console.ReadLine();
            /*proc.StartInfo.FileName = pathPDF2;
            proc.Start();
            proc.Close();*/
            return true;
        }
        catch (FileNotFoundException e)
        {
            Console.WriteLine($"The file was not found: '{e}'");
            Console.ReadLine();
            return false;
        }
        catch (DirectoryNotFoundException e)
        {
            Console.WriteLine($"The directory was not found: '{e}'");
            Console.ReadLine();
            return false;
        }
        catch (IOException e)
        {
            Console.WriteLine($"The file could not be opened: '{e}'");
            Console.ReadLine();
            return false;
        }
    }

    private iTextSharp.text.Image CreateImagenToPdf(string firma, int yAbsolutePosition, int BorderWidth = 0){
        // Creamos la imagen y le ajustamos el tamaño
        iTextSharp.text.Image imagen = iTextSharp.text.Image.GetInstance(firma);
        imagen.BorderWidth = BorderWidth;
        imagen.SetAbsolutePosition(xAbsolutePosition, yAbsolutePosition);
        imagen.ScaleAbsoluteWidth(sWidth);
        imagen.ScaleAbsoluteHeight(sHeight);

        return imagen;
    }

}