using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace consolePdf.DTO
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class Body
    {
        public int id { get; set; }
        public int idEmpresa { get; set; }
        public string empresa { get; set; }
        public int idSede { get; set; }
        public string Sede { get; set; }
        public int idProfesion { get; set; }
        public string Profesion { get; set; }
        public string Identificacion { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Nombre_Completo { get; set; }
        public string rutaAbsoluta { get; set; }
        public string rutaRelativa { get; set; }
        public DateTime createAtDate { get; set; }
        public bool estado { get; set; }
        public int xposition { get; set; }
        public int yposition { get; set; }
    }

    public class Root
    {
        public bool error { get; set; }
        public int status { get; set; }
        public List<Body> body { get; set; }
        public string action { get; set; }
    }


}
