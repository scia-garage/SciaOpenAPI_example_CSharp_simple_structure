using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SciaOpenAPI_example_simple_structure
{
    class Program
    {
        static void Main(string[] args)
        {
            //Initialization of OpenAPI environment
            using (SCIA.OpenAPI.Environment env = new SCIA.OpenAPI.Environment(@"c:\Program Files (x86)\SCIA\Engineer19.0\", @".\SCIATemp", "1.0.0.0"))//path to the location of your installation and temp path for logs)
            {
                //Run SCIA Engineer application
                bool openedSE = env.RunSCIAEngineer(SCIA.OpenAPI.Environment.GuiMode.ShowWindowShow);
                if (!openedSE)
                {
                    return;
                }
                Console.WriteLine($"SEn opened");
                //Open project in SCIA Engineer on specified path
                string MyAppPath = AppDomain.CurrentDomain.BaseDirectory;
                string templatePath = System.IO.Path.Combine(MyAppPath, @"..\..\..\..\res\OpenAPIEmptyProject.esa");//path to teh empty SCIA Engineer project
                SCIA.OpenAPI.EsaProject proj = env.OpenProject(templatePath);
                if (proj == null)
                {
                    return;
                }
                Console.WriteLine($"Proj opened");
                //Create materials in local ADM
                Console.WriteLine($"Set grade for concrete material: ");
                string conMatGrade = Console.ReadLine();
                Guid comatid = Guid.NewGuid();
                proj.Model.CreateMaterial(new SCIA.OpenAPI.StructureModelDefinition.Material(comatid, "conc", 0, conMatGrade));
                Console.WriteLine($"Set grade for steel material: ");
                string steelMatGrade = Console.ReadLine();
                Guid stmatid = Guid.NewGuid();
                proj.Model.CreateMaterial(new SCIA.OpenAPI.StructureModelDefinition.Material(stmatid, "steel", 1, steelMatGrade));
                Console.WriteLine($"Materials created in ADM");
                //Create cross-sections in local ADM
                //proj.Model.CreateCrossSection(new SCIA.OpenAPI.StructureModelDefinition.CrossSectionParametric(Guid.NewGuid(), "conc.rect", comatid, 1, new double[] { 0.2, 0.4 }));//example of parametric CSS - rectangle
                Guid css_steel = Guid.NewGuid();
                Console.WriteLine($"Set steel profile: ");
                string steelProfile = Console.ReadLine();
                proj.Model.CreateCrossSection(new SCIA.OpenAPI.StructureModelDefinition.CrossSectionManufactured(css_steel, "steel.HEA", stmatid, steelProfile, 1, 0));
                Console.WriteLine($"CSSs created in ADM");

                Console.WriteLine($"Set parameter a: ");
                double a = Convert.ToDouble(Console.ReadLine());
                Console.WriteLine($"Set parameter b: ");
                double b = Convert.ToDouble(Console.ReadLine());
                Console.WriteLine($"Set parameter c: ");
                double c = Convert.ToDouble(Console.ReadLine());

                Guid n1 = Guid.NewGuid();
                Guid n2 = Guid.NewGuid();
                Guid n3 = Guid.NewGuid();
                Guid n4 = Guid.NewGuid();
                Guid n5 = Guid.NewGuid();
                Guid n6 = Guid.NewGuid();
                Guid n7 = Guid.NewGuid();
                Guid n8 = Guid.NewGuid();
                //Create structural nodes in local ADM
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
                //Create beams in local ADM
                proj.Model.CreateBeam(new SCIA.OpenAPI.StructureModelDefinition.Beam(b1, "b1", css_steel, new Guid[2] { n1, n5 }));
                proj.Model.CreateBeam(new SCIA.OpenAPI.StructureModelDefinition.Beam(b2, "b2", css_steel, new Guid[2] { n2, n6 }));
                proj.Model.CreateBeam(new SCIA.OpenAPI.StructureModelDefinition.Beam(b3, "b3", css_steel, new Guid[2] { n3, n7 }));
                proj.Model.CreateBeam(new SCIA.OpenAPI.StructureModelDefinition.Beam(b4, "b4", css_steel, new Guid[2] { n4, n8 }));
                //Create fix nodal support in local ADM
                proj.Model.CreatePointSupport(new SCIA.OpenAPI.StructureModelDefinition.PointSupport(Guid.NewGuid(), "Su1", n1));
                proj.Model.CreatePointSupport(new SCIA.OpenAPI.StructureModelDefinition.PointSupport(Guid.NewGuid(), "Su2", n2));
                proj.Model.CreatePointSupport(new SCIA.OpenAPI.StructureModelDefinition.PointSupport(Guid.NewGuid(), "Su3", n3));
                proj.Model.CreatePointSupport(new SCIA.OpenAPI.StructureModelDefinition.PointSupport(Guid.NewGuid(), "Su4", n4));


                Guid s1 = Guid.NewGuid();
                Guid[] nodes = new Guid[4] { n5, n6, n7, n8 };
                Console.WriteLine($"Set thickness of the slab: ");
                double thickness = Convert.ToDouble(Console.ReadLine());
                //Create flat slab in local ADM
                proj.Model.CreateSlab(new SCIA.OpenAPI.StructureModelDefinition.Slab(s1, "s1", 0, comatid, thickness, nodes));

                Guid lg1 = Guid.NewGuid();
                //Create load group in local ADM
                proj.Model.CreateLoadGroup(new SCIA.OpenAPI.StructureModelDefinition.LoadGroup(lg1, "lg1", 0));

                Guid lc1 = Guid.NewGuid();
                //Create load case in local ADM
                proj.Model.CreateLoadCase(new SCIA.OpenAPI.StructureModelDefinition.LoadCase(lc1, "lc1", 0, lg1, 1));

                Guid sf1 = Guid.NewGuid();
                Console.WriteLine($"Set value of surface load on the slab: ");
                double loadValue = Convert.ToDouble(Console.ReadLine());
                //Create surface load on slab in local ADM
                proj.Model.CreateSurfaceLoad(new SCIA.OpenAPI.StructureModelDefinition.SurfaceLoad(sf1, "sf1", loadValue, lc1, s1, 2));

                //Refresh model in SCIA Engineer from local ADM
                proj.Model.RefreshModel_ToSCIAEngineer();
                Console.WriteLine($"My model sent to SEn");


                //proj.CreateMesh(); //needs dialogue click
                // Run calculation
                proj.RunCalculation();
                Console.WriteLine($"My model calculate");

                //Initialize Results API
                SCIA.OpenAPI.Results.ResultsAPI rapi = proj.Model.InitializeResultsAPI();
                //Create container for 1D results
                SCIA.OpenAPI.Results.Result IntFor1Db1 = new SCIA.OpenAPI.Results.Result();
                //Results key for internal forces on beam 1
                SCIA.OpenAPI.Results.ResultKey keyIntFor1Db1 = new SCIA.OpenAPI.Results.ResultKey
                {
                    CaseType = Results64Enums.eDsElementType.eDsElementType_LoadCase,
                    CaseId = lc1,
                    EntityType = Results64Enums.eDsElementType.eDsElementType_Beam,
                    EntityName = "b1",
                    Dimension = Results64Enums.eDimension.eDim_1D,
                    ResultType = Results64Enums.eResultType.eFemBeamInnerForces,
                    CoordSystem = Results64Enums.eCoordSystem.eCoordSys_Local
                };
                //Load 1D results based on results key
                IntFor1Db1 = rapi.LoadResult(keyIntFor1Db1);
                Console.WriteLine(IntFor1Db1.GetTextOutput());
                var N = IntFor1Db1.GetMagnitudeName(0);
                var Nvalue = IntFor1Db1.GetValue(0, 0);
                Console.WriteLine(N);
                Console.WriteLine(Nvalue);
                //Results key for reaction on node 1
                SCIA.OpenAPI.Results.ResultKey keyReactionsSu1 = new SCIA.OpenAPI.Results.ResultKey
                {
                    CaseType = Results64Enums.eDsElementType.eDsElementType_LoadCase,
                    CaseId = lc1,
                    EntityType = Results64Enums.eDsElementType.eDsElementType_Node,
                    EntityName = "n1",

                    Dimension = Results64Enums.eDimension.eDim_reactionsPoint,
                    ResultType = Results64Enums.eResultType.eReactionsNodes,
                    CoordSystem = Results64Enums.eCoordSystem.eCoordSys_Global
                };
                SCIA.OpenAPI.Results.Result reactionsSu1 = new SCIA.OpenAPI.Results.Result();
                reactionsSu1 = rapi.LoadResult(keyReactionsSu1);
                Console.WriteLine(reactionsSu1.GetTextOutput());



                SCIA.OpenAPI.Results.Result Def2Ds1 = new SCIA.OpenAPI.Results.Result();
                //Results key for internal forces on slab
                SCIA.OpenAPI.Results.ResultKey keyDef2Ds1 = new SCIA.OpenAPI.Results.ResultKey
                {
                    CaseType = Results64Enums.eDsElementType.eDsElementType_LoadCase,
                    CaseId = lc1,
                    EntityType = Results64Enums.eDsElementType.eDsElementType_Slab,
                    EntityName = "s1",
                    Dimension = Results64Enums.eDimension.eDim_2D,
                    ResultType = Results64Enums.eResultType.eFemDeformations,
                    CoordSystem = Results64Enums.eCoordSystem.eCoordSys_Local
                };

                Def2Ds1 = rapi.LoadResult(keyDef2Ds1);
                Console.WriteLine(Def2Ds1.GetTextOutput());

                double maxvalue = 0;
                double pivot;
                for (int i = 0; i < Def2Ds1.GetMeshElementCount(); i++)
                {
                    pivot = Def2Ds1.GetValue(2, i);
                    if (System.Math.Abs(pivot) > System.Math.Abs(maxvalue))
                    {
                        maxvalue = pivot;

                    };
                };
                Console.WriteLine("Maximum deformation on slab:");
                Console.WriteLine(maxvalue);




                Console.WriteLine($"Press key to exit");
                Console.ReadKey();

                proj.CloseProject(SCIA.OpenAPI.SaveMode.SaveChangesNo);
            }
        }
    }
}

