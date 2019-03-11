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
            proj.Model.CreateMaterial(new SCIA.OpenAPI.StructureModelDefinition.Material(comatid, "conc", 0, "S 350"));
            Guid stmatid = Guid.NewGuid();
            proj.Model.CreateMaterial(new SCIA.OpenAPI.StructureModelDefinition.Material(stmatid, "steel", 1, "C30/37"));
            Guid timatid = Guid.NewGuid();
            proj.Model.CreateMaterial(new SCIA.OpenAPI.StructureModelDefinition.Material(timatid, "timber", 2, "D20"));
            Guid alumatid = Guid.NewGuid();
            proj.Model.CreateMaterial(new SCIA.OpenAPI.StructureModelDefinition.Material(alumatid, "alu", 3, "EN-AW5083"));
            Guid mamatid = Guid.NewGuid();
            //proj.Model.CreateMaterial(new SCIA.OpenAPI.StructureModelDefinition.Material(mamatid, "masonry", 4, 3e4, 1e4, 0.3, 0, 2500));
            Guid omatid = Guid.NewGuid();
            //proj.Model.CreateMaterial(new SCIA.OpenAPI.StructureModelDefinition.Material(omatid, "other", 5, 3e4, 1e4, 0.3, 0, 2500));
            Console.WriteLine($"Materials created in ADM");

            //proj.Model.CreateCrossSection(new SCIA.OpenAPI.CrossSection(Guid.NewGuid(), "conc.rect", stmatid, "HEA260", 6, 0));
            Guid css_steel = Guid.NewGuid();
            proj.Model.CreateCrossSection(new SCIA.OpenAPI.StructureModelDefinition.CrossSectionManufactored(css_steel, "steel.HEA", stmatid, "HEA260", 1, 0, 0.2, 0.2));
            Console.WriteLine($"CSSs created in ADM");


            double a = 6;
            double b = 8;
            double c = 3;

            Guid n1 = Guid.NewGuid();
            Guid n2 = Guid.NewGuid();
            Guid n3 = Guid.NewGuid();
            Guid n4 = Guid.NewGuid();
            Guid n5 = Guid.NewGuid();
            Guid n6 = Guid.NewGuid();
            Guid n7 = Guid.NewGuid();
            Guid n8 = Guid.NewGuid();
            proj.Model.CreateNode(new SCIA.OpenAPI.StructureModelDefinition.StructNode(n1, "n1", 0, 0, 0));
            proj.Model.CreateNode(new SCIA.OpenAPI.StructureModelDefinition.StructNode(n2, "n2", a, 0, 0));
            proj.Model.CreateNode(new SCIA.OpenAPI.StructureModelDefinition.StructNode(n3, "n3", a, b, 0));
            proj.Model.CreateNode(new SCIA.OpenAPI.StructureModelDefinition.StructNode(n4, "n4", 0, b, 0));
            proj.Model.CreateNode(new SCIA.OpenAPI.StructureModelDefinition.StructNode(n5, "n5", 0, 0, c));
            proj.Model.CreateNode(new SCIA.OpenAPI.StructureModelDefinition.StructNode(n6, "n6", a, 0, c));
            proj.Model.CreateNode(new SCIA.OpenAPI.StructureModelDefinition.StructNode(n7, "n7", a, b, c));
            proj.Model.CreateNode(new SCIA.OpenAPI.StructureModelDefinition.StructNode(n8, "n8", 0, b, c));

            Guid b1 = Guid.NewGuid();
            Guid b2 = Guid.NewGuid();
            Guid b3 = Guid.NewGuid();
            Guid b4 = Guid.NewGuid();
            proj.Model.CreateBeam(new SCIA.OpenAPI.StructureModelDefinition.Beam(b1, "b1", css_steel, new Guid[2] { n1, n5 }));
            proj.Model.CreateBeam(new SCIA.OpenAPI.StructureModelDefinition.Beam(b2, "b2", css_steel, new Guid[2] { n2, n6 }));
            proj.Model.CreateBeam(new SCIA.OpenAPI.StructureModelDefinition.Beam(b3, "b3", css_steel, new Guid[2] { n3, n7 }));
            proj.Model.CreateBeam(new SCIA.OpenAPI.StructureModelDefinition.Beam(b4, "b4", css_steel, new Guid[2] { n4, n8 }));
            

            Guid s1 = Guid.NewGuid();
            Guid[] nodes = new Guid[4]{n5, n6, n7, n8};
            proj.Model.CreateSlab(new SCIA.OpenAPI.StructureModelDefinition.Slab(s1, "s1", 0, comatid, 0.15, nodes));

            Guid lg1 = Guid.NewGuid();
            proj.Model.CreateLoadGroup(new SCIA.OpenAPI.StructureModelDefinition.LoadGroup(lg1, "lg1", 0));

            Guid lc1 = Guid.NewGuid();
            proj.Model.CreateLoadCase(new SCIA.OpenAPI.StructureModelDefinition.LoadCase(lc1, "lc1", 0, lg1, 1));

            Guid sf1 = Guid.NewGuid();
            proj.Model.CreateSurfaceLoad(new SCIA.OpenAPI.StructureModelDefinition.SurfaceLoad(sf1, "sf1", -12500, lc1, s1, 2));


            proj.Model.RefreshModel_ToSCIAEngineer();
            Console.WriteLine($"My model sent to SEn");

            Console.WriteLine($"Press key to exit");
            Console.ReadKey();

            proj.CloseProject(EnvESA80.TEnvESAApp_SaveChanges.eEPSaveChangesNo);

        }
    }
}
