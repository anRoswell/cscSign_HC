using System;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.Extensions.Configuration;
using consolePdf.model;

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

    // rutas de nuestros pdf
    string pathPDF = string.Empty;
    string pathPDF2 = string.Empty;

    public CTextSharp(string pathPDF, Parameter parameters){
        this.pathPDF = parameters.Pdf.NameJunta;
        this.pathPDF2 = parameters.Pdf.NameJunta2;

        DimensionesImagenes dimensionesImagenes = parameters.DimensionesImagenes;

        // Dimensiones imágenes
        this.sWidth = dimensionesImagenes.SWidth;
        this.sHeight = dimensionesImagenes.SHeight;
    }

    public bool Firmar(Profesional profesionales){
        try
        {
            //var reader = new iTextSharp.text.pdf.PdfReader(this.pathPDF);
            using (FileStream oFS = new(this.pathPDF2, FileMode.Create, FileAccess.Write))
            {
                //Objeto para leer el pdf original
                oReader = new PdfReader(this.pathPDF);

                //Objeto que tiene el tamaño de nuestro documento
                oSize = oReader.GetPageSizeWithRotation(1);

                //documento de itextsharp para realizar el trabajo asignandole el tamaño del original
                oDocument = new Document(oSize);

                //FileStream oFS = new FileStream(this.pathPDF2, FileMode.Create, FileAccess.Write);
                PdfWriter oWriter = PdfWriter.GetInstance(oDocument, oFS);
                oDocument.Open();

                // Creamos la imagen y le ajustamos el tamaño
                Image imagen1 = CreateImagenToPdf(profesionales.sign, profesionales.xposition, profesionales.yposition);

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
            }
            
            System.Diagnostics.Process proc = new System.Diagnostics.Process();
            
            Console.WriteLine("                               *** Firmado satisfactoriamente !!!!! ***");
            Console.WriteLine("                            *** presione cualquier tecla para continuar *** ");
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

    private Image CreateImagenToPdf(string firma, int xAbsolutePosition, int yAbsolutePosition, int BorderWidth = 0){
        // Creamos la imagen y le ajustamos el tamaño
        Image imagen = Image.GetInstance(firma);
        imagen.BorderWidth = BorderWidth;
        imagen.SetAbsolutePosition(xAbsolutePosition, yAbsolutePosition);
        imagen.ScaleAbsoluteWidth(sWidth);
        imagen.ScaleAbsoluteHeight(sHeight);

        return imagen;
    }

}