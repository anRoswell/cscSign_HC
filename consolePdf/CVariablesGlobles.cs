
using Microsoft.Extensions.Configuration;
using System.IO;

public static class VariablesGlobales{

  public static string complemento = "junta.pdf";
  public static string msg1 = "ERROR!!! No existe archivo ";
  public static string msg2 = "ERROR!!! No existe firma ";
  
 public static void init(){

   IConfigurationBuilder builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appSettings.json", true,true);

  IConfigurationRoot config = builder.Build();
  string appName = config["ConnectionString"];   
 }
}