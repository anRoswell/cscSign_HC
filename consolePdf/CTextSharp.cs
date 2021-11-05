using System;
using System.IO;
using System.Configuration;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

public class CTextSharp{
    //Objeto para leer el pdf original
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

    string dirPruebas = string.Empty;
    string ficPruebas = string.Empty;

    public CTextSharp(string pathPDF, string pathPDF2){
        this.pathPDF = pathPDF;
        this.pathPDF2 = pathPDF2;

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
    

    public bool Firmar(List<Profesional> profesionales, int option){
        //Objeto para leer el pdf original
        oReader = new PdfReader(this.pathPDF);

        //Objeto que tiene el tamaño de nuestro documento
        oSize = oReader.GetPageSizeWithRotation(1);

        //documento de itextsharp para realizar el trabajo asignandole el tamaño del original
        oDocument = new Document(oSize);

        try
        {
            FileStream oFS = new FileStream(this.pathPDF2, FileMode.Create, FileAccess.Write);
            PdfWriter oWriter = PdfWriter.GetInstance(oDocument, oFS);
            oDocument.Open();

            // Creamos la imagen y le ajustamos el tamaño
            iTextSharp.text.Image imagen1 = CreateImagenToPdf(profesionales[option].sign, profesionales[option].SignWidth);

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

            oDocument.Close();

            oFS.Close();
            oWriter.Close();
            oReader.Close();
            System.Diagnostics.Process proc = new System.Diagnostics.Process();
            
            Console.WriteLine("                               *** Firmado satisfactoriamente !!!!! ***");
            Console.WriteLine("                            *** presione cualquier tecla para terminar *** ");
            Console.ReadLine();

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