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

            SCIA.OpenAPI.EsaProject proj = env.OpenProject(@"c:\SCIA\GIThub\SciaOpenAPI_example_CSS.mat\res\empty.esa");
            Console.WriteLine($"Proj opened");

            Guid comatid = Guid.NewGuid();
            proj.Model.CreateMaterial(new SCIA.OpenAPI.Material(comatid, "conc", 0, "C30/37"));
            Guid stmatid = Guid.NewGuid();
            proj.Model.CreateMaterial(new SCIA.OpenAPI.Material(stmatid, "steel", 1, "S355"));
            Guid timatid = Guid.NewGuid();
            proj.Model.CreateMaterial(new SCIA.OpenAPI.Material(timatid, "timber", 2, "D22"));
            Guid alumatid = Guid.NewGuid();
            proj.Model.CreateMaterial(new SCIA.OpenAPI.Material(alumatid, "alu", 3, "EN-AW3003"));
            Guid mamatid = Guid.NewGuid();
            proj.Model.CreateMaterial(new SCIA.OpenAPI.Material(mamatid, "masonry", 4, "M1"));
            Guid omatid = Guid.NewGuid();
            proj.Model.CreateMaterial(new SCIA.OpenAPI.Material(omatid, "other", 5, "O2"));
            Console.WriteLine($"Materials created in ADM");

            proj.Model.CreateCrossSection(new SCIA.OpenAPI.CrossSection(Guid.NewGuid(), "conc.rect", stmatid, "HEA260", 6, 0));
            proj.Model.CreateCrossSection(new SCIA.OpenAPI.CrossSection(Guid.NewGuid(), "steel.HEA", stmatid, "HEA260", 1, 0));
            Console.WriteLine($"CSSs created in ADM");

            proj.Model.RefreshModel_ToSCIAEngineer();
            Console.WriteLine($"My model sent to SEn");

            Console.WriteLine($"Press key to exit");
            Console.ReadKey();

            proj.CloseProject(EnvESA80.TEnvESAApp_SaveChanges.eEPSaveChangesNo);

        }
    }
}
