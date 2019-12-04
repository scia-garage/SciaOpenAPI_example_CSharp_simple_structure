using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SCIA.OpenAPI.StructureModelDefinition;
using SCIA.OpenAPI.Results;
using SCIA.OpenAPI.OpenAPIEnums;
using Results64Enums;
using System.IO;
using System.Reflection;

namespace SciaOpenAPI_example_simple_structure
{
    class Program
    {
        private static string GetAppPath()
        {
            //var directory = new DirectoryInfo(Environment.CurrentDirectory);
            //return directory.Parent.FullName;
            return @"c:\WORK\SCIA-ENGINEER\TESTING-VERSIONS\Full_19.1.2010.32_rel_19.1_patch_2_x86\"; // SEn application installation folder, don't forget run "EP_regsvr32 esa.exe" from commandline with Admin rights
        }

        /// <summary>
        /// Path to Scia engineer
        /// </summary>
        static private string SciaEngineerFullPath => GetAppPath();


        /// <sumamary>
        /// Path to SCIA Engineer temp
        /// </sumamary>
        static private string SciaEngineerTempPath => GetTempPath();

        private static string GetTempPath()
        {
            return @"c:\WORK\SCIA-ENGINEER\TESTING-VERSIONS\Full_19.1.2010.32_rel_19.1_patch_2_x86\Temp\"; // Must be SEn application temp path, run SEn and go to menu: Setup -> Options -> Directories -> Temporary files
        }

        static private string SciaEngineerProjecTemplate => GetTemplatePath();

        private static string GetTemplatePath()
        {
            //Open project in SCIA Engineer on specified path
            string MyAppPath = AppDomain.CurrentDomain.BaseDirectory;
            return Path.Combine(MyAppPath, @"..\..\..\..\res\OpenAPIEmptyProject.esa");//path to teh empty SCIA Engineer project

        }

        static private string AppLogPath => GetThisAppLogPath();    

         static private string GetThisAppLogPath() {
             return @"c:\TEMP\OpenAPI\MyLogsTemp"; // Folder for storing of log files for this console application
         }

        private static void DeleteTemp()
        {

            if (Directory.Exists(SciaEngineerTempPath)){
                Directory.Delete(SciaEngineerTempPath, true);
            }

        }

        /// <summary>
        /// Assembly resolve method has to be call here
        /// </summary>
    private static void SciaOpenApiAssemblyResolve()
        {
            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
            {
                string dllName = args.Name.Substring(0, args.Name.IndexOf(",")) + ".dll";
                string dllFullPath = Path.Combine(SciaEngineerFullPath, dllName);
                if (!File.Exists(dllFullPath))
                {
                    //return null;
                    dllFullPath = Path.Combine(SciaEngineerFullPath, "OpenAPI_dll", dllName);
                }
                if (!File.Exists(dllFullPath))
                {
                    return null;
                }
                return Assembly.LoadFrom(dllFullPath);
            };
        }

        static private void RunSCIAOpenAPI()
        {

            //Initialization of OpenAPI environment
           using (SCIA.OpenAPI.Environment env = new SCIA.OpenAPI.Environment(SciaEngineerFullPath, AppLogPath, "1.0.0.0"))// path to the location of your installation and temp path for logs)
            {
                //Run SCIA Engineer application
                bool openedSE = env.RunSCIAEngineer(SCIA.OpenAPI.Environment.GuiMode.ShowWindowShow);
                if (!openedSE)
                {
                    return;
                }
                Console.WriteLine($"SEn opened");
               SCIA.OpenAPI.EsaProject proj = env.OpenProject(SciaEngineerProjecTemplate);
                if (proj == null)
                {
                    return;
                }
                Console.WriteLine($"Proj opened");
                //Create materials in local ADM
                Console.WriteLine($"Set grade for concrete material: ");
                string conMatGrade = Console.ReadLine();
                Guid comatid = Guid.NewGuid();
                proj.Model.CreateMaterial(new Material(comatid, "conc", 0, conMatGrade));
                //Guid test = proj.Model.FindMaterialGuid(conMatGrade);
                Console.WriteLine($"Set grade for steel material: ");
                string steelMatGrade = Console.ReadLine();
                Guid stmatid = Guid.NewGuid();
                proj.Model.CreateMaterial(new Material(stmatid, "steel", 1, steelMatGrade));
                Console.WriteLine($"Materials created in ADM");
                //Create cross-sections in local ADM
                //proj.Model.CreateCrossSection(new CrossSectionParametric(Guid.NewGuid(), "conc.rect", comatid, 1, new double[] { 0.2, 0.4 }));//example of parametric CSS - rectangle
                Guid css_steel = Guid.NewGuid();
                Console.WriteLine($"Set steel profile: ");
                string steelProfile = Console.ReadLine();
                proj.Model.CreateCrossSection(new CrossSectionManufactured(css_steel, "steel.HEA", stmatid, steelProfile, 1, 0));
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
                proj.Model.CreateNode(new StructNode(n1, "n1", 0, 0, 0));
                proj.Model.CreateNode(new StructNode(n2, "n2", a, 0, 0));
                proj.Model.CreateNode(new StructNode(n3, "n3", a, b, 0));
                proj.Model.CreateNode(new StructNode(n4, "n4", 0, b, 0));
                proj.Model.CreateNode(new StructNode(n5, "n5", 0, 0, c));
                proj.Model.CreateNode(new StructNode(n6, "n6", a, 0, c));
                proj.Model.CreateNode(new StructNode(n7, "n7", a, b, c));
                proj.Model.CreateNode(new StructNode(n8, "n8", 0, b, c));

                Guid b1 = Guid.NewGuid();
                Guid b2 = Guid.NewGuid();
                Guid b3 = Guid.NewGuid();
                Guid b4 = Guid.NewGuid();
                //Create beams in local ADM
                proj.Model.CreateBeam(new Beam(b1, "b1", css_steel, new Guid[2] { n1, n5 }));
                proj.Model.CreateBeam(new Beam(b2, "b2", css_steel, new Guid[2] { n2, n6 }));
                proj.Model.CreateBeam(new Beam(b3, "b3", css_steel, new Guid[2] { n3, n7 }));
                proj.Model.CreateBeam(new Beam(b4, "b4", css_steel, new Guid[2] { n4, n8 }));
                //Create fix nodal support in local ADM
                var Su1 = new PointSupport(Guid.NewGuid(), "Su1", n1)
                {
                    ConstraintRx = eConstraintType.Free,
                    ConstraintRy = eConstraintType.Free,
                    ConstraintRz = eConstraintType.Free
                };
                proj.Model.CreatePointSupport(Su1);
                proj.Model.CreatePointSupport(new PointSupport(Guid.NewGuid(), "Su2", n2));
                proj.Model.CreatePointSupport(new PointSupport(Guid.NewGuid(), "Su3", n3));
                proj.Model.CreatePointSupport(new PointSupport(Guid.NewGuid(), "Su4", n4));

                Guid s1 = Guid.NewGuid();
                Guid[] nodes = new Guid[4] { n5, n6, n7, n8 };
                Console.WriteLine($"Set thickness of the slab: ");
                double thickness = Convert.ToDouble(Console.ReadLine());
                //Create flat slab in local ADM
                string slabName = "SLAB_1";
                proj.Model.CreateSlab(new Slab(s1, slabName, 0, comatid, thickness, nodes));



                Guid lg1 = Guid.NewGuid();
                //Create load group in local ADM
                proj.Model.CreateLoadGroup(new LoadGroup(lg1, "lg1", 0));

                Guid lc1 = Guid.NewGuid();
                //Create load case in local ADM
                proj.Model.CreateLoadCase(new LoadCase(lc1, "lc1", 0, lg1, 1));

                //Combination
                CombinationItem[] combinationItems = new CombinationItem[1] { new CombinationItem(lc1, 1.5) };
                Combination C1 = new Combination(Guid.NewGuid(), "C1", combinationItems)
                {
                    Category = eLoadCaseCombinationCategory.AccordingNationalStandard,
                    NationalStandard = eLoadCaseCombinationStandard.EnUlsSetC
                };
                proj.Model.CreateCombination(C1);

                Guid sf1 = Guid.NewGuid();
                Console.WriteLine($"Set value of surface load on the slab: ");
                double loadValue = Convert.ToDouble(Console.ReadLine());
                //Create surface load on slab in local ADM
                proj.Model.CreateSurfaceLoad(new SurfaceLoad(sf1, "sf1", loadValue, lc1, s1, 2));
                // line support on B1
                var lineSupport = new LineSupport(Guid.NewGuid(), "lineSupport", b1)
                {
                    Member = b1,
                    ConstraintRx = eConstraintType.Free,
                    ConstraintRy = eConstraintType.Free,
                    ConstraintRz = eConstraintType.Free
                };
                proj.Model.CreateLineSupport(lineSupport);
                //line load on B1
                var lineLoad = new LineLoadOnBeam(Guid.NewGuid(), "lineLoad")
                {
                    Member = b1,
                    LoadCase = lc1,
                    Value1 = -12500,
                    Value2 = -12500,
                    Direction = eDirection.X
                };
                proj.Model.CreateLineLoad(lineLoad);

                //Refresh model in SCIA Engineer from local ADM
                proj.Model.RefreshModel_ToSCIAEngineer();
                Console.WriteLine($"My model sent to SEn");


                // Run calculation
                proj.RunCalculation();
                Console.WriteLine($"My model calculate");

                //Initialize Results API
                ResultsAPI rapi = proj.Model.InitializeResultsAPI();
                if (rapi != null)
                {
                    //Create container for 1D results
                    Result IntFor1Db1 = new Result();
                    //Results key for internal forces on beam 1
                    ResultKey keyIntFor1Db1 = new ResultKey
                    {
                        CaseType = eDsElementType.eDsElementType_LoadCase,
                        CaseId = lc1,
                        EntityType = eDsElementType.eDsElementType_Beam,
                        EntityName = "b1",
                        Dimension = eDimension.eDim_1D,
                        ResultType = eResultType.eFemBeamInnerForces,
                        CoordSystem = eCoordSystem.eCoordSys_Local
                    };
                    //Load 1D results based on results key
                    IntFor1Db1 = rapi.LoadResult(keyIntFor1Db1);
                    if (IntFor1Db1 != null)
                    {
                        Console.WriteLine(IntFor1Db1.GetTextOutput());
                    }
                    //combination
                    //Create container for 1D results
                    Result IntFor1Db1Combi = new Result();
                    //Results key for internal forces on beam 1
                    ResultKey keyIntFor1Db1Combi = new ResultKey
                    {
                        EntityType = eDsElementType.eDsElementType_Beam,
                        EntityName = "b1",
                        CaseType = eDsElementType.eDsElementType_Combination,
                        CaseId = C1.Id,
                        Dimension = eDimension.eDim_1D,
                        ResultType = eResultType.eFemBeamInnerForces,
                        CoordSystem = eCoordSystem.eCoordSys_Local
                    };
                    // Load 1D results based on results key
                    IntFor1Db1Combi = rapi.LoadResult(keyIntFor1Db1Combi);
                    if (IntFor1Db1Combi != null)
                    {
                        Console.WriteLine(IntFor1Db1Combi.GetTextOutput());
                    }
                    ResultKey keyReactionsSu1 = new ResultKey
                    {
                        CaseType = eDsElementType.eDsElementType_LoadCase,
                        CaseId = lc1,
                        EntityType = eDsElementType.eDsElementType_Node,
                        EntityName = "n1",
                        Dimension = eDimension.eDim_reactionsPoint,
                        ResultType = eResultType.eReactionsNodes,
                        CoordSystem = eCoordSystem.eCoordSys_Global
                    };
                    Result reactionsSu1 = new Result();
                    reactionsSu1 = rapi.LoadResult(keyReactionsSu1);
                    if (reactionsSu1 != null)
                    {
                        Console.WriteLine(reactionsSu1.GetTextOutput());
                    }


                    Result Def2Ds1 = new Result();
                    //Results key for internal forces on slab
                    ResultKey keySlab = new ResultKey
                    {
                        EntityType = eDsElementType.eDsElementType_Slab,
                        EntityName = slabName,
                        CaseType = eDsElementType.eDsElementType_LoadCase,
                        CaseId = lc1,
                        Dimension = eDimension.eDim_2D,
                        ResultType = eResultType.eFemDeformations,
                        CoordSystem = eCoordSystem.eCoordSys_Local
                    };


                    Def2Ds1 = rapi.LoadResult(keySlab);
                    if (Def2Ds1 != null)
                    {
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
                    }

                }

                Console.WriteLine($"Press key to exit");
                Console.ReadKey();

                proj.CloseProject(SCIA.OpenAPI.SaveMode.SaveChangesNo);
                env.Dispose();
            }

        }

        static void Main()
        {
            SciaOpenApiAssemblyResolve();
            DeleteTemp();
            RunSCIAOpenAPI();
        }
    }
}


