using System;
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
using System.Diagnostics;

namespace SciaOpenAPI_example_simple_structure
{

    class Program
    {
        private static Guid Lc1Id { get; } = Guid.NewGuid();
        private static Guid C1Id { get; } = Guid.NewGuid();
        private static string SlabName { get; } = "S1";
        private static string beamName { get; } = "b1";

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

            if (Directory.Exists(SciaEngineerTempPath))
            {
                Directory.Delete(SciaEngineerTempPath, true);
            }

        }

        private static void KillAllOrphanSCIAEngineerIntances()
        {
            foreach (var process in Process.GetProcessesByName("EsaStartupScreen"))
            {
                process.Kill();
                Console.WriteLine($"Killed old EsaStartupScreen instance!");
                System.Threading.Thread.Sleep(1000);
            }
            foreach (var process in Process.GetProcessesByName("Esa"))
            {
                process.Kill();
                Console.WriteLine($"Killed old SCIA Engineer instance!");
                System.Threading.Thread.Sleep(5000);
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

        #region Class Enums
        enum Material_Type { Concrete = 0, Steel = 1, Timber = 2, Aluminium = 3, Masonry = 4, Other = 5 }
        enum Slab_Type { Plate = 0, Wall = 1, Shell = 2 }
        enum LoadCase_actionType { Permanent = 0, Variable = 1, Accidental = 2 }
        enum LoadCase_loadCaseType { SelfWeight = 0, Standard = 1, Prestress = 2, Dynamic = 3, PrimaryEffect = 4, Static = 5 }
        #endregion

        /// <summary>
        ///Method create model in proj environment
        /// </summary>
        private static void CreateModel(Structure model)
        {

            #region  ---------- Geometry -----------
            #region Create Materials
            Material concmat = new Material(Guid.NewGuid(), "conc", (int)Material_Type.Concrete, "C30/37");
            Material steelmat = new Material(Guid.NewGuid(), "steel", (int)Material_Type.Steel, "S 355");
            foreach (var x in new List<Material> { concmat, steelmat }) { model.CreateMaterial(x); }
            #endregion
            #region Create Cross Sections
            CrossSectionManufactured hea260 = new CrossSectionManufactured(Guid.NewGuid(), "steel.HEA", steelmat.Id, "HEA260", 1, 0);
            CrossSectionParametric rect300x300 = new CrossSectionParametric(Guid.NewGuid(), "r300x300", concmat.Id, 1, new double[] { 300.0, 300.0 });
            model.CreateCrossSection(hea260);
            model.CreateCrossSection(rect300x300);
            #endregion
            #region Create Nodes
            double a = 5.0;
            double b = 6.0;
            double c = 4.0;
            StructNode n1 = new StructNode(Guid.NewGuid(), "n1", 0, 0, 0);
            StructNode n2 = new StructNode(Guid.NewGuid(), "n2", a, 0, 0);
            StructNode n3 = new StructNode(Guid.NewGuid(), "n3", a, b, 0);
            StructNode n4 = new StructNode(Guid.NewGuid(), "n4", 0, b, 0);
            StructNode n5 = new StructNode(Guid.NewGuid(), "n5", 0, 0, c);
            StructNode n6 = new StructNode(Guid.NewGuid(), "n6", a, 0, c);
            StructNode n7 = new StructNode(Guid.NewGuid(), "n7", a, b, c);
            StructNode n8 = new StructNode(Guid.NewGuid(), "n8", 0, b, c);
            foreach (var x in new List<StructNode> { n1, n2, n3, n4, n5, n6, n7, n8 }) { model.CreateNode(x); }
            #endregion
            #region Create Beams
            Beam b1 = new Beam(Guid.NewGuid(), beamName, hea260.Id, new Guid[2] { n1.Id, n5.Id });
            Beam b2 = new Beam(Guid.NewGuid(), "b2", hea260.Id, new Guid[2] { n2.Id, n6.Id });
            Beam b3 = new Beam(Guid.NewGuid(), "b3", hea260.Id, new Guid[2] { n3.Id, n7.Id });
            Beam b4 = new Beam(Guid.NewGuid(), "b4", hea260.Id, new Guid[2] { n4.Id, n8.Id });
            foreach (var x in new List<Beam> { b1, b2, b3, b4 }) { model.CreateBeam(x); }
            #endregion
            #region Create Slab
            double thickness = 0.30;
            Slab s1 = new Slab(Guid.NewGuid(), SlabName, (int)Slab_Type.Plate, concmat.Id, thickness, new Guid[4] { n5.Id, n6.Id, n7.Id, n8.Id });
            model.CreateSlab(s1);
            #endregion
            #region Create Support - in Node
            PointSupport Su1 = new PointSupport(Guid.NewGuid(), "Su1", n1.Id) { ConstraintRx = eConstraintType.Free, ConstraintRy = eConstraintType.Free, ConstraintRz = eConstraintType.Free };
            PointSupport Su2 = new PointSupport(Guid.NewGuid(), "Su2", n2.Id) { ConstraintZ = eConstraintType.Flexible, StiffnessZ = 10000.0 };
            PointSupport Su3 = new PointSupport(Guid.NewGuid(), "Su3", n3.Id);
            PointSupport Su4 = new PointSupport(Guid.NewGuid(), "Su4", n4.Id);
            foreach (var x in new List<PointSupport> { Su1, Su2, Su3, Su4 }) { model.CreatePointSupport(x); }
            #endregion
            #region Create Support - on Beam & on Slab Edge
            LineSupport lineSupport_onBeam = new LineSupport(Guid.NewGuid(), "linSupBeam", b1.Id)
            {
                ConstraintRx = eConstraintType.Free,
                ConstraintRy = eConstraintType.Free,
                ConstraintRz = eConstraintType.Free,
                ConstraintX = eConstraintType.Flexible,
                StiffnessX = 10.0,
                ConstraintY = eConstraintType.Flexible,
                StiffnessY = 10.0,
                ConstraintZ = eConstraintType.Flexible,
                StiffnessZ = 10.0,
            };
            LineSupport lineSupport_onEdge = new LineSupport(Guid.NewGuid(), "linSupEdge", s1.Id)
            {
                ConstraintRx = eConstraintType.Free,
                ConstraintRy = eConstraintType.Free,
                ConstraintRz = eConstraintType.Free,
                ConstraintX = eConstraintType.Flexible,
                StiffnessX = 10.0,
                ConstraintY = eConstraintType.Flexible,
                StiffnessY = 10.0,
                ConstraintZ = eConstraintType.Flexible,
                StiffnessZ = 10.0,
                EdgeIndex = 2
            };
            foreach (var x in new List<LineSupport> { lineSupport_onBeam, lineSupport_onEdge }) { model.CreateLineSupport(x); }
            #endregion
            #endregion
            #region  ---------- Loads ---------------
            #region Create Load Group
            LoadGroup lgperm = new LoadGroup(Guid.NewGuid(), "lgperm", (int)eLoadGroup_Load.eLoadGroup_Load_Permanent);
            LoadGroup lgvar1 = new LoadGroup(Guid.NewGuid(), "lgvar1", (int)eLoadGroup_Load.eLoadGroup_Load_Variable);
            LoadGroup lgvar2 = new LoadGroup(Guid.NewGuid(), "lgvar2", (int)eLoadGroup_Load.eLoadGroup_Load_Variable);
            LoadGroup lgvar3 = new LoadGroup(Guid.NewGuid(), "lgvar3", (int)eLoadGroup_Load.eLoadGroup_Load_Variable);
            foreach (var x in new List<LoadGroup> { lgperm, lgvar1, lgvar2, lgvar3 }) { model.CreateLoadGroup(x); }
            #endregion
            #region Create Load Case
            LoadCase lc_sw = new LoadCase(Guid.NewGuid(), "lc_sw", (int)LoadCase_actionType.Permanent, lgperm.Id, (int)LoadCase_loadCaseType.SelfWeight);
            LoadCase lc_perm = new LoadCase(Lc1Id, "lc_perm", (int)LoadCase_actionType.Permanent, lgperm.Id, (int)LoadCase_loadCaseType.Standard);
            LoadCase lc_var1 = new LoadCase(Guid.NewGuid(), "lc_var1", (int)LoadCase_actionType.Variable, lgvar1.Id, (int)LoadCase_loadCaseType.Static);
            LoadCase lc_var2 = new LoadCase(Guid.NewGuid(), "lc_var2", (int)LoadCase_actionType.Variable, lgvar2.Id, (int)LoadCase_loadCaseType.Static);
            LoadCase lc_var3a = new LoadCase(Guid.NewGuid(), "lc_var3a", (int)LoadCase_actionType.Variable, lgvar3.Id, (int)LoadCase_loadCaseType.Static);
            LoadCase lc_var3b = new LoadCase(Guid.NewGuid(), "lc_var3b", (int)LoadCase_actionType.Variable, lgvar3.Id, (int)LoadCase_loadCaseType.Static);
            LoadCase lc_var3c = new LoadCase(Guid.NewGuid(), "lc_var3c", (int)LoadCase_actionType.Variable, lgvar3.Id, (int)LoadCase_loadCaseType.Static);
            foreach (var x in new List<LoadCase> { lc_sw, lc_perm }) { model.CreateLoadCase(x); }
            #endregion
            #region Create Load Combinations
            CombinationItem[] combinationItems = new CombinationItem[]
            {
                new CombinationItem(lc_sw.Id, 1.0), new CombinationItem(Lc1Id, 1.0),
               // new CombinationItem(lc_var1.Id, 1.0), new CombinationItem(lc_var2.Id, 1.0),
               // new CombinationItem(lc_var3a.Id, 1.0), new CombinationItem(lc_var3b.Id, 1.0), new CombinationItem(lc_var3c.Id, 1.0)
            };
            Combination C_EnUlsB = new Combination(C1Id, "C_EnUlsB", combinationItems)
            {
                Category = eLoadCaseCombinationCategory.AccordingNationalStandard,
                NationalStandard = eLoadCaseCombinationStandard.EnUlsSetB,
            };
            Combination C_EnUlsC = new Combination(Guid.NewGuid(), "C_EnUlsC", combinationItems)
            {
                Category = eLoadCaseCombinationCategory.AccordingNationalStandard,
                NationalStandard = eLoadCaseCombinationStandard.EnUlsSetC
            };
            Combination C_EnSlsChar = new Combination(Guid.NewGuid(), "C_EnSlsChar", combinationItems)
            {
                Category = eLoadCaseCombinationCategory.AccordingNationalStandard,
                NationalStandard = eLoadCaseCombinationStandard.EnSlsCharacteristic
            };
            Combination C_EnSlsFreq = new Combination(Guid.NewGuid(), "C_EnSlsFreq", combinationItems)
            {
                Category = eLoadCaseCombinationCategory.AccordingNationalStandard,
                NationalStandard = eLoadCaseCombinationStandard.EnSlsFrequent
            };
            Combination C_EnSlsQP = new Combination(Guid.NewGuid(), "C_EnSlsQP", combinationItems)
            {
                Category = eLoadCaseCombinationCategory.AccordingNationalStandard,
                NationalStandard = eLoadCaseCombinationStandard.EnSlsQuasiPermanent
            };
            Combination C_Acc1 = new Combination(Guid.NewGuid(), "C_Acc1", combinationItems)
            {
                //Category = eLoadCaseCombinationCategory.AccidentalLimitState,
                Category = eLoadCaseCombinationCategory.AccordingNationalStandard,
                NationalStandard = eLoadCaseCombinationStandard.EnAccidental1
            };
            Combination C_Acc2 = new Combination(Guid.NewGuid(), "C_Acc2", combinationItems)
            {
                //Category = eLoadCaseCombinationCategory.AccidentalLimitState,
                Category = eLoadCaseCombinationCategory.AccordingNationalStandard,
                NationalStandard = eLoadCaseCombinationStandard.EnAccidental2
            };
            Combination C_ULS = new Combination(Guid.NewGuid(), "C_ULS", combinationItems)
            {
                Category = eLoadCaseCombinationCategory.UltimateLimitState,
            };
            Combination C_SLS = new Combination(Guid.NewGuid(), "C_SLS", combinationItems)
            {
                Category = eLoadCaseCombinationCategory.ServiceabilityLimitState
            };
            foreach (var x in new List<Combination> { C_EnUlsB, C_EnUlsC, C_EnSlsChar, C_EnSlsFreq, C_EnSlsQP, C_Acc1, C_Acc2 }) { model.CreateCombination(x); }
            #endregion
            #region Create Load - Point Loads - in Node
            double loadValue;
            loadValue = -12500.0;
            PointLoadInNode pln1 = new PointLoadInNode(Guid.NewGuid(), "pln1", loadValue, lc_perm.Id, n4.Id, (int)eDirection.X);
            model.CreatePointLoadInNode(pln1);
            #endregion
            #region Create Load - Point Loads - Free
            loadValue = -12500.0;
            PointLoadFree plf1 = new PointLoadFree(Guid.NewGuid(), "plf1", lc_perm.Id, loadValue, a / 3.0, b / 3.0, c, (int)eDirection.Z, c - 1.0, c + 1.0);
            model.CreatePointLoadFree(plf1);
            #endregion
            #region Create Load - Surface Loads - on Slab
            loadValue = -12500.0;
            SurfaceLoad sf1 = new SurfaceLoad(Guid.NewGuid(), "sf1", loadValue, lc_perm.Id, s1.Id, (int)eDirection.Z);
            SurfaceLoad sf2 = new SurfaceLoad(Guid.NewGuid(), "sf2", loadValue, lc_var1.Id, s1.Id, (int)eDirection.Y);
            SurfaceLoad sf3 = new SurfaceLoad(Guid.NewGuid(), "sf3", loadValue, lc_var2.Id, s1.Id, (int)eDirection.X);
            SurfaceLoad sf4 = new SurfaceLoad(Guid.NewGuid(), "sf4", loadValue, lc_var3a.Id, s1.Id, (int)eDirection.X);
            SurfaceLoad sf5 = new SurfaceLoad(Guid.NewGuid(), "sf5", loadValue, lc_var3b.Id, s1.Id, (int)eDirection.Y);
            SurfaceLoad sf6 = new SurfaceLoad(Guid.NewGuid(), "sf6", loadValue, lc_var3c.Id, s1.Id, (int)eDirection.Z);
            foreach (var x in new List<SurfaceLoad> { sf1 }) { model.CreateSurfaceLoad(x); }
            #endregion
            #region Create Load - Line Load - on Beam & on Slab Edge
            var lin1 = new LineLoadOnBeam(Guid.NewGuid(), "lin1")
            {
                Member = b1.Id,
                LoadCase = lc_perm.Id,
                Distribution = eLineLoadDistribution.Trapez,
                Value1 = -12500,
                Value2 = -12500,
                CoordinateDefinition = eCoordinateDefinition.Relative,
                StartPoint = 0.01,
                EndPoint = 0.99,
                CoordinationSystem = eCoordinationSystem.GCS,
                Direction = eDirection.X,
                Origin = eLineOrigin.FromStart,
                Location = eLineLoadLocation.Length,
                EccentricityEy = 0.0,
                EccentricityEz = 0.0
            };
            var lin2 = new LineLoadOnBeam(Guid.NewGuid(), "lin2")
            {
                Member = b1.Id,
                LoadCase = lc_var1.Id,
                Distribution = eLineLoadDistribution.Trapez,
                Value1 = -12500,
                Value2 = 12500,
                CoordinateDefinition = eCoordinateDefinition.Relative,
                StartPoint = 0.01,
                EndPoint = 0.99,
                CoordinationSystem = eCoordinationSystem.GCS,
                Direction = eDirection.Y,
                Origin = eLineOrigin.FromStart,
                Location = eLineLoadLocation.Projection,
                EccentricityEy = 0.0,
                EccentricityEz = 0.0
            };
            var lin3a = new LineLoadOnSlabEdge(Guid.NewGuid(), "lin3a")
            {
                Member = s1.Id,
                LoadCase = lc_var3a.Id,
                EdgeIndex = 0,
                Distribution = eLineLoadDistribution.Trapez,
                Value1 = -12500,
                Value2 = 12500,
                CoordinateDefinition = eCoordinateDefinition.Relative,
                StartPoint = 0.01,
                EndPoint = 0.99,
                CoordinationSystem = eCoordinationSystem.GCS,
                Direction = eDirection.Z,
                Origin = eLineOrigin.FromStart,
                Location = eLineLoadLocation.Length
            };
            var lin3b = new LineLoadOnSlabEdge(Guid.NewGuid(), "lin3b")
            {
                Member = s1.Id,
                LoadCase = lc_var3b.Id,
                EdgeIndex = 1,
                Distribution = eLineLoadDistribution.Trapez,
                Value1 = -12500,
                Value2 = 12500,
                CoordinateDefinition = eCoordinateDefinition.Relative,
                StartPoint = 0.01,
                EndPoint = 0.99,
                CoordinationSystem = eCoordinationSystem.GCS,
                Direction = eDirection.Z,
                Origin = eLineOrigin.FromStart,
                Location = eLineLoadLocation.Length
            };
            var lin3c = new LineLoadOnSlabEdge(Guid.NewGuid(), "lin3c")
            {
                Member = s1.Id,
                LoadCase = lc_var3c.Id,
                EdgeIndex = 2,
                Distribution = eLineLoadDistribution.Trapez,
                Value1 = -12500,
                Value2 = 12500,
                CoordinateDefinition = eCoordinateDefinition.Relative,
                StartPoint = 0.01,
                EndPoint = 0.99,
                CoordinationSystem = eCoordinationSystem.GCS,
                Direction = eDirection.Z,
                Origin = eLineOrigin.FromStart,
                Location = eLineLoadLocation.Length
            };
            var lin3d = new LineLoadOnSlabEdge(Guid.NewGuid(), "lin3d")
            {
                Member = s1.Id,
                LoadCase = lc_perm.Id,
                EdgeIndex = 3,
                Distribution = eLineLoadDistribution.Trapez,
                Value1 = -12500,
                Value2 = 12500,
                CoordinateDefinition = eCoordinateDefinition.Relative,
                StartPoint = 0.01,
                EndPoint = 0.99,
                CoordinationSystem = eCoordinationSystem.GCS,
                Direction = eDirection.Z,
                Origin = eLineOrigin.FromStart,
                Location = eLineLoadLocation.Length
            };
            foreach (var x in new List<LineLoadOnBeam> { lin1 }) { model.CreateLineLoad(x); }
            foreach (var x in new List<LineLoadOnSlabEdge> { lin3d }) { model.CreateLineLoad(x); }
            //foreach (var x in new List<LineLoadOnBeam> { lin1, lin2 }) { model.CreateLineLoad(x); }
            //foreach (var x in new List<LineLoadOnSlabEdge> { lin3a, lin3b, lin3c, lin3d }) { model.CreateLineLoad(x); }
            #endregion
            #endregion
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
                #region  ---------- Calculation----------
                #region Send Model to SCIA Engineer
                //Refresh model in SCIA Engineer from local ADM
                proj.Model.RefreshModel_ToSCIAEngineer();
                #endregion
                #region Calculate
                // Run calculation
                proj.RunCalculation();

                #endregion
                #endregion
                #region  ---------- Results -------------

                //Initialize Results API
                using (ResultsAPI rapi = proj.Model.InitializeResultsAPI())
                {
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
                    else
                    {
                        throw new Exception("No results accessible");
                    }

                    Console.WriteLine($"Press key to exit");
                    Console.ReadKey();
                }
                #endregion
                proj.CloseProject(SCIA.OpenAPI.SaveMode.SaveChangesNo);
                env.Dispose();
            }

        }

        /// <summary>
        /// This method represented advance use of OpenAPI, initialization of environment is done on background thanks to Context, EmptyFile is generated on backround and used than follows creation of model, linear calculation and reading results
        /// </summary>
        private static object SCIAOpenAPIWorker(SCIA.OpenAPI.Environment env)
        {
            #region Run SCIA Engineer
            //Run SCIA Engineer application
            bool openedSE = env.RunSCIAEngineer(SCIA.OpenAPI.Environment.GuiMode.ShowWindowShow);
            if (!openedSE)
            {
                throw new InvalidOperationException($"Cannot run SCIA Engineer");
            }
            #endregion
            #region Open Project Template file
            SciaFileGetter fileGetter = new SciaFileGetter();
            var EsaFile = fileGetter.PrepareBasicEmptyFile(@"C:/TEMP/");//path where the template file "template.esa" is created
            if (!File.Exists(EsaFile))
            {
                throw new InvalidOperationException($"File from manifest resource is not created ! Temp: {env.AppTempPath}");
            }

            EsaProject proj = env.OpenProject(EsaFile);
            if (proj == null)
            {
                throw new InvalidOperationException($"File from manifest resource is not opened ! Temp: {env.AppTempPath}");
            }
            #endregion

            CreateModel(proj.Model);
            #region  ---------- Calculation----------
            #region Send Model to SCIA Engineer
            //Refresh model in SCIA Engineer from local ADM
            proj.Model.RefreshModel_ToSCIAEngineer();
            #endregion
            #region Calculate

            // Run calculation
            proj.RunCalculation();
            Console.WriteLine($"My model calculate");
            OpenApiE2EResults storage = new OpenApiE2EResults();
            #endregion
            #endregion
            #region  ---------- Results -------------
            //Initialize Results API
            using (ResultsAPI resultsApi = proj.Model.InitializeResultsAPI())
            {
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
                    OpenApiE2EResult beamB1IDeformationLc = new OpenApiE2EResult("beamB1DeformationsLC1")
                    {
                        ResultKey = new ResultKey
                        {
                            EntityType = eDsElementType.eDsElementType_Beam,
                            EntityName = "b1",
                            CaseType = eDsElementType.eDsElementType_LoadCase,
                            CaseId = Lc1Id,
                            Dimension = eDimension.eDim_1D,
                            ResultType = eResultType.eFemBeamDeformation,
                            CoordSystem = eCoordSystem.eCoordSys_Local
                        }
                    };
                    beamB1IDeformationLc.Result = resultsApi.LoadResult(beamB1IDeformationLc.ResultKey);
                    storage.SetResult(beamB1IDeformationLc);
                }
                {
                    OpenApiE2EResult beamB1RelIDeformationLc = new OpenApiE2EResult("beamB1RelativeDeformationsLC1")
                    {
                        ResultKey = new ResultKey
                        {
                            EntityType = eDsElementType.eDsElementType_Beam,
                            EntityName = "b1",
                            CaseType = eDsElementType.eDsElementType_LoadCase,
                            CaseId = Lc1Id,
                            Dimension = eDimension.eDim_1D,
                            ResultType = eResultType.eFemBeamRelativeDeformation,
                            CoordSystem = eCoordSystem.eCoordSys_Local
                        }
                    };
                    beamB1RelIDeformationLc.Result = resultsApi.LoadResult(beamB1RelIDeformationLc.ResultKey);
                    storage.SetResult(beamB1RelIDeformationLc);
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
                    OpenApiE2EResult slabStresses = new OpenApiE2EResult("slabStresses")
                    {
                        ResultKey = new ResultKey
                        {
                            EntityType = eDsElementType.eDsElementType_Slab,
                            EntityName = SlabName,
                            CaseType = eDsElementType.eDsElementType_LoadCase,
                            CaseId = Lc1Id,
                            Dimension = eDimension.eDim_2D,
                            ResultType = eResultType.eFemStress,
                            CoordSystem = eCoordSystem.eCoordSys_Local
                        }
                    };
                    slabStresses.Result = resultsApi.LoadResult(slabStresses.ResultKey);
                    storage.SetResult(slabStresses);
                }
                {
                    OpenApiE2EResult slabStrains = new OpenApiE2EResult("slabStrains")
                    {
                        ResultKey = new ResultKey
                        {
                            EntityType = eDsElementType.eDsElementType_Slab,
                            EntityName = SlabName,
                            CaseType = eDsElementType.eDsElementType_LoadCase,
                            CaseId = Lc1Id,
                            Dimension = eDimension.eDim_2D,
                            ResultType = eResultType.eFemStrains,
                            CoordSystem = eCoordSystem.eCoordSys_Local
                        }
                    };
                    slabStrains.Result = resultsApi.LoadResult(slabStrains.ResultKey);
                    storage.SetResult(slabStrains);
                }
                {
                    OpenApiE2EResult slabInnerForcesExtended = new OpenApiE2EResult("slabInnerForcesExtended")
                    {
                        ResultKey = new ResultKey
                        {
                            EntityType = eDsElementType.eDsElementType_Slab,
                            EntityName = SlabName,
                            CaseType = eDsElementType.eDsElementType_LoadCase,
                            CaseId = Lc1Id,
                            Dimension = eDimension.eDim_2D,
                            ResultType = eResultType.eFemInnerForces_Extended,
                            CoordSystem = eCoordSystem.eCoordSys_Local
                        }
                    };
                    slabInnerForcesExtended.Result = resultsApi.LoadResult(slabInnerForcesExtended.ResultKey);
                    storage.SetResult(slabInnerForcesExtended);
                }

                //{
                //    OpenApiE2EResult reactions = new OpenApiE2EResult("ReactionsN1")
                //    {
                //        ResultKey = new ResultKey
                //        {
                //            CaseType = eDsElementType.eDsElementType_LoadCase,
                //            CaseId = Lc1Id,
                //            EntityType = eDsElementType.eDsElementType_Node,
                //            EntityName = "n1",
                //            Dimension = eDimension.eDim_reactionsPoint,
                //            ResultType = eResultType.eReactionsNodes,
                //            CoordSystem = eCoordSystem.eCoordSys_Global
                //        }
                //    };
                //    reactions.Result = resultsApi.LoadResult(reactions.ResultKey);
                //    storage.SetResult(reactions);
                //}
                //{
                //    OpenApiE2EResult reactions = new OpenApiE2EResult("ReactionsSu1")
                //    {
                //        ResultKey = new ResultKey
                //        {
                //            CaseType = eDsElementType.eDsElementType_LoadCase,
                //            CaseId = Lc1Id,
                //            EntityType = eDsElementType.eDsElementType_PointSupportPoint,
                //            EntityName = "Su1",
                //            Dimension = eDimension.eDim_reactionsPoint,
                //            ResultType = eResultType.eResultTypeReactionsSupport0D,
                //            CoordSystem = eCoordSystem.eCoordSys_Global,

                //        }
                //    };
                //    reactions.Result = resultsApi.LoadResult(reactions.ResultKey);
                //    storage.SetResult(reactions);
                //}
                //{
                //    OpenApiE2EResult ReactionslinSupBeam = new OpenApiE2EResult("ReactionslinSupBeam")
                //    {
                //        ResultKey = new ResultKey
                //        {
                //            EntityType = eDsElementType.eDsElementType_LineSupportLine,
                //            EntityName = "linSupBeam",
                //            Dimension = eDimension.eDim_reactionsLine,
                //            CoordSystem = eCoordSystem.eCoordSys_Global,
                //            CaseType = eDsElementType.eDsElementType_LoadCase,
                //            CaseId = Lc1Id,
                //            ResultType = eResultType.eResultTypeReactionsSupport1D,

                //        }
                //    };
                //    ReactionslinSupBeam.Result = resultsApi.LoadResult(ReactionslinSupBeam.ResultKey);
                //    storage.SetResult(ReactionslinSupBeam);
                //}             

                return storage;
            }
            #endregion
        }

        private static void RunSCIAOpenAPI_advance()
        {

            //Context for OpenAPI
            var Context = new SciaOpenApiContext(SciaEngineerFullPath, SCIAOpenAPIWorker)
            {
                YourAppTempFullPath = AppLogPath,
                SciaEngineerTempFolderImputedByUser = SciaEngineerTempPath
            };

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
            //Show stored results data
            foreach (var item in data.GetAll())
            {
                Console.WriteLine(item.Value.Result.GetTextOutput());
            }

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
            KillAllOrphanSCIAEngineerIntances();
            SciaOpenApiAssemblyResolve();
            //DeleteTemp();
            //RunSCIAOpenAPI_simple();
            RunSCIAOpenAPI_advance();
        }


    }


}



