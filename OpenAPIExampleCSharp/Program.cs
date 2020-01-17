﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SCIA.OpenAPI.Utils;
using SCIA.OpenAPI.StructureModelDefinition;
using SCIA.OpenAPI.Results;
using SCIA.OpenAPI.OpenAPIEnums;
using Results64Enums;
using System.IO;
using System.Reflection;
using SCIA.OpenAPI;

namespace SciaOpenAPI_example_simple_structure
{

    class Program
    {
        private static Guid Lc1Id { get; } = Guid.NewGuid();
        private static Guid C1Id { get; } = Guid.NewGuid();
        private static string SlabName { get; } = "S1";

        private static string GetAppPath()
        {
            // SEn application installation folder, don't forget run "esa.exe /regserver" from commandline with Admin rights
            return @"C:\Program Files (x86)\SCIA\Engineer19.1\";
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
            // Must be SEn application temp path, run SEn and go to menu: Setup -> Options -> Directories -> Temporary files
            return @"C:\Users\jbroz\ESA19.1\Temp\"; 
        }

        static private string SciaEngineerProjecTemplate => GetTemplatePath();

        private static string GetTemplatePath()
        {
            //path to teh empty SCIA Engineer project
            return @"C:\WORK\SourceCodes\SciaOpenAPI_example_CSharp_simple_structure\res\OpenAPIEmptyProject.esa";
        }

        static private string AppLogPath => GetThisAppLogPath();    

        static private string GetThisAppLogPath() 
        {
            // Folder for storing of log files for this console application
            return @"c:\TEMP\OpenAPI\MyLogsTemp"; 
        }

        // before each run of the application, delete of temp folder is neccessary
        private static void DeleteTemp()
        {

            if (Directory.Exists(SciaEngineerTempPath)){
                Directory.Delete(SciaEngineerTempPath, true);
            }

        }

        /// <summary>
        /// Assembly resolve method has to be call here
        /// This is needed due to not copy dll into folder where exe file of this application is placed
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

        /// <summary>
        ///Method create model in proj environment
        /// </summary>
        private static void CreateModel(Structure model)
        {
            //Create materials in local ADM
            Console.WriteLine($"Set grade for concrete material: ");
            string conMatGrade = Console.ReadLine();
            Guid comatid = Guid.NewGuid();
            model.CreateMaterial(new Material(comatid, "conc", 0, conMatGrade));
            //Guid test = model.FindMaterialGuid(conMatGrade);
            Console.WriteLine($"Set grade for steel material: ");
            string steelMatGrade = Console.ReadLine();
            Guid stmatid = Guid.NewGuid();
            model.CreateMaterial(new Material(stmatid, "steel", 1, steelMatGrade));
            Console.WriteLine($"Materials created in ADM");
            Guid css_steel = Guid.NewGuid();
            Console.WriteLine($"Set steel profile: ");
            string steelProfile = Console.ReadLine();
            model.CreateCrossSection(new CrossSectionManufactured(css_steel, "steel.HEA", stmatid, steelProfile, 1, 0));
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
            model.CreateNode(new StructNode(n1, "n1", 0, 0, 0));
            model.CreateNode(new StructNode(n2, "n2", a, 0, 0));
            model.CreateNode(new StructNode(n3, "n3", a, b, 0));
            model.CreateNode(new StructNode(n4, "n4", 0, b, 0));
            model.CreateNode(new StructNode(n5, "n5", 0, 0, c));
            model.CreateNode(new StructNode(n6, "n6", a, 0, c));
            model.CreateNode(new StructNode(n7, "n7", a, b, c));
            model.CreateNode(new StructNode(n8, "n8", 0, b, c));

            Guid b1 = Guid.NewGuid();
            Guid b2 = Guid.NewGuid();
            Guid b3 = Guid.NewGuid();
            Guid b4 = Guid.NewGuid();
            //Create beams in local ADM
            model.CreateBeam(new Beam(b1, "b1", css_steel, new Guid[2] { n1, n5 }));
            model.CreateBeam(new Beam(b2, "b2", css_steel, new Guid[2] { n2, n6 }));
            model.CreateBeam(new Beam(b3, "b3", css_steel, new Guid[2] { n3, n7 }));
            model.CreateBeam(new Beam(b4, "b4", css_steel, new Guid[2] { n4, n8 }));
            //Create fix nodal support in local ADM
            var Su1 = new PointSupport(Guid.NewGuid(), "Su1", n1)
            {
                ConstraintRx = eConstraintType.Free,
                ConstraintRy = eConstraintType.Free,
                ConstraintRz = eConstraintType.Free
            };
            model.CreatePointSupport(Su1);
            model.CreatePointSupport(new PointSupport(Guid.NewGuid(), "Su2", n2));
            model.CreatePointSupport(new PointSupport(Guid.NewGuid(), "Su3", n3));
            model.CreatePointSupport(new PointSupport(Guid.NewGuid(), "Su4", n4));

            Guid s1 = Guid.NewGuid();
            Guid[] nodes = new Guid[4] { n5, n6, n7, n8 };
            Console.WriteLine($"Set thickness of the slab: ");
            double thickness = Convert.ToDouble(Console.ReadLine());
            //Create flat slab in local ADM
            model.CreateSlab(new Slab(s1, SlabName, 0, comatid, thickness, nodes));



            Guid lg1 = Guid.NewGuid();
            //Create load group in local ADM
            model.CreateLoadGroup(new LoadGroup(lg1, "lg1", 0));

            //Create load case in local ADM
            model.CreateLoadCase(new LoadCase(Lc1Id, "lc1", 0, lg1, 1));

            //Combination
            CombinationItem[] combinationItems = new CombinationItem[1] { new CombinationItem(Lc1Id, 1.5) };
            Combination C1 = new Combination(C1Id, "C1", combinationItems)
            {
                Category = eLoadCaseCombinationCategory.AccordingNationalStandard,
                NationalStandard = eLoadCaseCombinationStandard.EnUlsSetC
            };
            model.CreateCombination(C1);

            Guid sf1 = Guid.NewGuid();
            Console.WriteLine($"Set value of surface load on the slab: ");
            double loadValue = Convert.ToDouble(Console.ReadLine());
            //Create surface load on slab in local ADM
            model.CreateSurfaceLoad(new SurfaceLoad(sf1, "sf1", loadValue, Lc1Id, s1, 2));
            // line support on B1
            var lineSupport = new LineSupport(Guid.NewGuid(), "lineSupport", b1)
            {
                Member = b1,
                ConstraintRx = eConstraintType.Free,
                ConstraintRy = eConstraintType.Free,
                ConstraintRz = eConstraintType.Free
            };
            model.CreateLineSupport(lineSupport);
            //line load on B1
            var lineLoad = new LineLoadOnBeam(Guid.NewGuid(), "lineLoad")
            {
                Member = b1,
                LoadCase = Lc1Id,
                Value1 = -12500,
                Value2 = -12500,
                Direction = eDirection.X
            };
            model.CreateLineLoad(lineLoad);

        }

        /// <summary>
        /// This method represented simple use of OpenAPI, initialization of environment, creation of model, linear calculation and reading results
        /// </summary>
        static private void RunSCIAOpenAPI_simple()
        {
           //Initialization of OpenAPI environment
           using (SCIA.OpenAPI.Environment env = new SCIA.OpenAPI.Environment(SciaEngineerFullPath, AppLogPath, "1.0.0.0"))// path to the location of your installation and temp path for logs)
            {
                //Run SCIA Engineer application
                bool openedSE = env.RunSCIAEngineer(SCIA.OpenAPI.Environment.GuiMode.ShowWindowShow);
                if (!openedSE)
                {
                    throw new InvalidOperationException($"Cannot run SCIA Engineer");
                }
                //Open project
                SCIA.OpenAPI.EsaProject proj = env.OpenProject(SciaEngineerProjecTemplate);
                if (proj == null)
                {
                    throw new InvalidOperationException($"Cannot open project");
                }

                //method which create model
                CreateModel(proj.Model);

                //Refresh model in SCIA Engineer from local ADM
                proj.Model.RefreshModel_ToSCIAEngineer();
             
                // Run calculation
                proj.RunCalculation();
               

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
                        CaseId = Lc1Id,
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
                        CaseId = C1Id,
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
                        CaseId = Lc1Id,
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
                        EntityName = SlabName,
                        CaseType = eDsElementType.eDsElementType_LoadCase,
                        CaseId = Lc1Id,
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
                else {
                    throw new Exception("No results accessible");
                }

                Console.WriteLine($"Press key to exit");
                Console.ReadKey();

                proj.CloseProject(SCIA.OpenAPI.SaveMode.SaveChangesNo);
                env.Dispose();
            }

        }

        /// <summary>
        /// This method represented advance use of OpenAPI, initialization of environment is done on background thanks to Context, EmptyFile is generated on backround and used than follows creation of model, linear calculation and reading results
        /// </summary>
        private static object SCIAOpenAPIWorker(SCIA.OpenAPI.Environment env)
        {
            //Run SCIA Engineer application
            bool openedSE = env.RunSCIAEngineer(SCIA.OpenAPI.Environment.GuiMode.ShowWindowShow);
            if (!openedSE)
            {
                throw new InvalidOperationException($"Cannot run SCIA Engineer");
            }
            SciaFileGetter fileGetter = new SciaFileGetter();
            var EsaFile = fileGetter.PrepareBasicEmptyFile(@"C:/TEMP/");//path where the template file "template.esa" is created
            if (!File.Exists(EsaFile))
            {
                throw new InvalidOperationException($"File from manifest resource is not created ! Temp: {env.AppTempPath}");
            }

            SCIA.OpenAPI.EsaProject proj = env.OpenProject(EsaFile);
            if (proj == null)
            {
                throw new InvalidOperationException($"File from manifest resource is not opened ! Temp: {env.AppTempPath}");
            }
            CreateModel(proj.Model);
            //Refresh model in SCIA Engineer from local ADM
            proj.Model.RefreshModel_ToSCIAEngineer();


            // Run calculation
            proj.RunCalculation();
            Console.WriteLine($"My model calculate");
            OpenApiE2EResults storage = new OpenApiE2EResults();

            //Initialize Results API
            ResultsAPI resultsApi = proj.Model.InitializeResultsAPI();
            if (resultsApi == null)
            {
                return storage;
            }
            {
                OpenApiE2EResult beamB1InnerForLc = new OpenApiE2EResult("beamB1InnerForcesLC1")
                {
                    ResultKey = new ResultKey
                    {
                        EntityType = eDsElementType.eDsElementType_Beam,
                        EntityName = "b1",
                        CaseType = eDsElementType.eDsElementType_LoadCase,
                        CaseId = Lc1Id,
                        Dimension = eDimension.eDim_1D,
                        ResultType = eResultType.eFemBeamInnerForces,
                        CoordSystem = eCoordSystem.eCoordSys_Local
                    }
                };
                beamB1InnerForLc.Result = resultsApi.LoadResult(beamB1InnerForLc.ResultKey);
                storage.SetResult(beamB1InnerForLc);
            }
            { 
            OpenApiE2EResult beamInnerForcesCombi = new OpenApiE2EResult("beamInnerForcesCombi")
            {
                ResultKey = new ResultKey
                {
                    EntityType = eDsElementType.eDsElementType_Beam,
                    EntityName = "b1",
                    CaseType = eDsElementType.eDsElementType_Combination,
                    CaseId = C1Id,
                    Dimension = eDimension.eDim_1D,
                    ResultType = eResultType.eFemBeamInnerForces,
                    CoordSystem = eCoordSystem.eCoordSys_Local
                }
            };
            beamInnerForcesCombi.Result = resultsApi.LoadResult(beamInnerForcesCombi.ResultKey);
            storage.SetResult(beamInnerForcesCombi);
            }
            {
                OpenApiE2EResult slabInnerForces = new OpenApiE2EResult("slabInnerForces")
                {
                    ResultKey = new ResultKey
                    {
                        EntityType = eDsElementType.eDsElementType_Slab,
                        EntityName = SlabName,
                        CaseType = eDsElementType.eDsElementType_LoadCase,
                        CaseId = Lc1Id,
                        Dimension = eDimension.eDim_2D,
                        ResultType = eResultType.eFemInnerForces,
                        CoordSystem = eCoordSystem.eCoordSys_Local
                    }
                };
                slabInnerForces.Result = resultsApi.LoadResult(slabInnerForces.ResultKey);
                storage.SetResult(slabInnerForces);
            }
            {
                OpenApiE2EResult slabDeformations = new OpenApiE2EResult("slabDeformations")
                {
                    ResultKey = new ResultKey
                    {
                        EntityType = eDsElementType.eDsElementType_Slab,
                        EntityName = SlabName,
                        CaseType = eDsElementType.eDsElementType_LoadCase,
                        CaseId = Lc1Id,
                        Dimension = eDimension.eDim_2D,
                        ResultType = eResultType.eFemDeformations,
                        CoordSystem = eCoordSystem.eCoordSys_Local
                    }
                };
                slabDeformations.Result = resultsApi.LoadResult(slabDeformations.ResultKey);
                storage.SetResult(slabDeformations);
            }
            {
                OpenApiE2EResult reactions = new OpenApiE2EResult("Reactions")
                {
                    ResultKey = new ResultKey
                    {
                        CaseType = eDsElementType.eDsElementType_LoadCase,
                        CaseId = Lc1Id,
                        EntityType = eDsElementType.eDsElementType_Node,
                        EntityName = "n1",
                        Dimension = eDimension.eDim_reactionsPoint,
                        ResultType = eResultType.eReactionsNodes,
                        CoordSystem = eCoordSystem.eCoordSys_Global
                    }
                };
                reactions.Result = resultsApi.LoadResult(reactions.ResultKey);
                storage.SetResult(reactions);
            }
           return storage;
        }

       //method prepared for patch3
        private static void RunSCIAOpenAPI_advance()
        {
            //Context for OpenAPI
            var Context = new SciaOpenApiContext(SciaEngineerFullPath, SCIAOpenAPIWorker);
            //Context.YourAppTempFullPath = @"C:\WORK\SourceCodes\SciaOpenAPI_example_CSharp_simple_structure\OpenAPIExampleCSharp\bin\Debug\";
            //Run code which perform operations in OpenAPI - create model, linear calculation and write results to Context
            SciaOpenApiUtils.RunSciaOpenApi(Context);
            if (Context.Exception != null)
            {
                Console.WriteLine(Context.Exception.Message);
                return;
            }
            if (!(Context.Data is OpenApiE2EResults data))
            {
                Console.WriteLine("SOMETHING IS WRONG NO Results DATA !");
                return;
            }
            //TextBlockOpenApi.Text = "RESULTS";
            //foreach (var item in data.GetAll())
            //{
            //    Console.WriteLine(item.Value.Result.GetTextOutput());             
            //}
            var slabDef = data.GetResult("slabDeformations").Result;
            if (slabDef != null)
            {
                double maxvalue = 0;
                double pivot;
                for (int i = 0; i < slabDef.GetMeshElementCount(); i++)
                {
                    pivot = slabDef.GetValue(2, i);
                    if (System.Math.Abs(pivot) > System.Math.Abs(maxvalue))
                    {
                        maxvalue = pivot;

                    }
                }
                Console.WriteLine("Maximum deformation on slab:");
                Console.WriteLine(maxvalue);
            }
            Console.WriteLine($"Press key to exit");
            Console.ReadKey();
        }

        static void Main(string[] args)
        {
            SciaOpenApiAssemblyResolve();
            DeleteTemp();
            RunSCIAOpenAPI_simple();
        }

       
    }

       
}



