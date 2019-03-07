using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SciaOpenAPI_example_CSS.mat
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine($"Hello!");

            SCIA.OpenAPI.Environment env = new SCIA.OpenAPI.Environment(@"c:\scia\GIT\sen\A\Bin\release32", @".\Temp");

            env.RunSCIAEngineer(EnvESA80.TEnvESAApp_ShowWindow.eEPShowWindowShow);
            Console.WriteLine($"SEn opened");

            SCIA.OpenAPI.EsaProject proj = env.OpenProject(@"C:\Users\adamp\Desktop\temp\emptyECEN.esa");
            Console.WriteLine($"Proj opened");

            Guid mat_id;

            Console.WriteLine($"Press key to exit");
            Console.ReadKey();

        }
    }
}
