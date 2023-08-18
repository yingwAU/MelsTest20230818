#define WIDE_AREA
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using System.Xml.Linq;
using System.Diagnostics;
#if server
using FarmAC.Controls;
#endif
namespace FarmGHGcalc
{
    class model
    {
        //args[0] and args[1] are farm number and scenario number respectively
        public void run(string[] args)
        {
            string dir = Directory.GetCurrentDirectory();
            /* All parameter xml files can exist in two forms. The first form has the name yyyy.xml, where yyyy is the name of the parameter file containing default data.
             * These files must exist. The second form has the name yyyyAlternative.xml and these files are optional. If one or more files with these names exist
             * the data in them will overwrite the corresponding data in the default files. The Alternative files are used to enable parameters to be made case-specific
             */
            /*
             * The system file (there is usually only one - the Alternative option is not used) contains the list of farms and scenarios that will
             * be simulated, and the general parameters concerning the whole simulation
             */
            string[] system = new string[2];

#if server
            dbInterface db = new dbInterface();
            db.GetConnectionString("xmlFile");
            system[0] = db.GetConnectionString("xmlFile")+"system.xml";
            system[1] =db.GetConnectionString("xmlFile")+ "systemAlternative.xml";
#else
            system[0] = "system.xml";
            system[1] = null;
#endif
            // The FileInformation class is used to read the xml files
            FileInformation settings = new FileInformation(system);
            //First read the variables that are relevant for all farms
            settings.setPath("CommonSettings(-1)");
            bool logFile = settings.getItemBool("logFile", false);  //if true, will create and write to a log file
            bool logScreen = settings.getItemBool("logScreen", false); //if true, will write the log information to the console
            int verbocity = settings.getItemInt("verbosityLevel", false); //An integer that determines the detail with which log information will be reported

            bool outputxls = settings.getItemBool("outputxls", false); //if true, a tab-separated output file will be created
            bool Debug = settings.getItemBool("Debug", false); //if true, a file will be created to which information useful for debugging will be created
            bool livestock = settings.getItemBool("livestock", false); //if true
            bool Plant = settings.getItemBool("Plant", false); //if true

            if (verbocity == -1)
                verbocity = 5;
            int minSetting = 99999999, maxSetting = 0;
            //the setPath function is used to find the relevant part of the system.xml from which to read data
            settings.setPath("settings");
            //get the number of the first and last farm or farm scenario 
            settings.getSectionNumber(ref minSetting, ref maxSetting);
            string outputDir;
            //loop through each farm or farm scenario
            for (int settingsID = minSetting; settingsID <= maxSetting; settingsID++)
            {
                //GlobalVars allows data to be share across different classes (i.e. does not comply with the object oriented paradym). Nasty but useful. 
                GlobalVars.Instance.reset(settingsID.ToString());
                GlobalVars.Instance.logFile = logFile;
                GlobalVars.Instance.logScreen = logScreen;
                GlobalVars.Instance.verbocity = verbocity;

                GlobalVars.Instance.Writeoutputxls = outputxls;
                GlobalVars.Instance.WriteDebug = Debug;
                GlobalVars.Instance.Writelivestock = livestock;
                GlobalVars.Instance.WritePlant = Plant;
                //report error and exit, if there are an incorrect number of arguments
                if (args.Length != 2 && args.Length != 0)
                {
                    GlobalVars.Instance.log("missing arguments in arg list", 5);
                    Environment.Exit(0);
                }
                GlobalVars.Instance.setRunFullModel(true);
                if (settings.Identity.Count == 1)
                        settings.Identity.RemoveAt(0);
                if (settings.Identity.Count == 1)
                    settings.Identity.RemoveAt(0);
                if (settings.doesIDExist(settingsID))
                {
                    settings.setPath("settings(" + settingsID.ToString() + ")");
                    if (!Directory.Exists(settings.getItemString("outputDir")))
                    {
                        Directory.CreateDirectory(settings.getItemString("outputDir"));
                    }

                    if (args.Length != 0 && args[0].CompareTo("-1") != 0)
                        GlobalVars.Instance.seterrorFileName(settings.getItemString("outputDir") + "error" + "_" + args[0] + "_" + args[1] + ".txt");
                    else
                        GlobalVars.Instance.seterrorFileName(settings.getItemString("outputDir") + "error.txt");

                    outputDir = settings.getItemString("outputDir");
                    if (settingsID == minSetting)
                    {
                        string VMP3farmfileName = "FarmDataOut";
                        string VMP3fieldfileName = "FieldDataOut";
                        VMP3.Instance.openVMP3(outputDir,VMP3farmfileName,VMP3fieldfileName);
                        VMP3.Instance.WriteFarmHeader();
                        VMP3.Instance.WriteFieldHeader();
                    }
                    if (GlobalVars.Instance.logFile == true)
                    {
                        if (args.Count() == 0)
                            GlobalVars.Instance.logFileStream = new System.IO.StreamWriter(outputDir + "\\log.txt");
                        else
                            GlobalVars.Instance.logFileStream = new System.IO.StreamWriter(outputDir + "\\log" + args[0] + "_" + args[1] + ".txt");
                    }
                    //Set the path for the file constants.xml
                    string[] file = fileName(args, settings, "constants");
                    GlobalVars.Instance.setConstantFilePath(file);

                    string farmName = settings.getItemString("farm");
                    //this code section expects the 'farm' tag in system.xml to consist of a full file name (path + filename.xxx). 
                    //The model will look for input in path + filename + "_" + args[0] + "_" + args[1].xxx
                    //note that the farm file in system.xml must be called "farm.xml" for this to work
                    if (args.Length != 0 && args[0].CompareTo("-1") != 0)
                    {
                        string[] names = farmName.Split('.');

                        farmName = getPath(farmName) + "_" + args[0] + "_" + args[1] + "." + names[names.Count() - 1];

                    }
                    //Alternative
                    string[] farmNames = new string[2];
                    farmNames[0] = farmName;
                    farmNames[1] = null; 
                    GlobalVars.Instance.setFarmtFilePath(farmNames);
                    if (GlobalVars.Instance.logFile == true)
                    {
                        GlobalVars.Instance.log("Begin simulation of:", 5);
                        GlobalVars.Instance.log(farmName, 5);
                    }
                    //Set path for file containing the database of feed items
                    file = fileName(args, settings, "feedItems");
                    GlobalVars.Instance.setFeeditemFilePath(file);
                    //Set path for file containing the general parameters
                    file = fileName(args, settings, "parameters");
                    GlobalVars.Instance.setParamFilePath(file);
                    //Set path for file containing the details of the composition of manure and synthetic fertilisers
                    file = fileName(args, settings, "fertAndManure");
                    GlobalVars.Instance.setfertManFilePath(file);
                    //If true, this will wait for a character to be input from the keyboard before exiting the program
                    GlobalVars.Instance.setPauseBeforeExit(Convert.ToBoolean(settings.getItemString("pauseBeforeExit")));
                    //If true, this will stop the simulation if an error is encountered. If false, it will continue to the next farm/scenario
                    GlobalVars.Instance.setstopOnError(Convert.ToBoolean(settings.getItemString("stopOnError")));
                    try //reading the constants file
                    {
                        GlobalVars.Instance.readGlobalConstants();
                    }
                    catch (Exception e)
                    {
                        GlobalVars.Instance.Error("constant file not found " + e.Message, "program" + e.Message, false);
                    }
                    string tmps = Directory.GetCurrentDirectory();
                    FileInformation farmInformation = new FileInformation(GlobalVars.Instance.getFarmFilePath());
                    farmInformation.setPath("Farm");
                    int min = 99999999, max = 0;

                    farmInformation.getSectionNumber(ref min, ref max);
                    string VMP3ID = "No value";
                    
                    for (int farmNr = min; farmNr <= max; farmNr++)
                    {
                        //Stuff from files
                        try
                        {
                            if (farmInformation.doesIDExist(farmNr))
                            {
                                string newPath = "Farm(" + farmNr.ToString() + ")";
                                farmInformation.setPath(newPath);
                                int zoneNr = farmInformation.getItemInt("AgroEcologicalZone");
                                GlobalVars.Instance.SetZone(zoneNr);
                                int FarmTyp = farmInformation.getItemInt("FarmType");
                                GlobalVars.Instance.theZoneData.readZoneSpecificData(zoneNr, FarmTyp);
                                double Ndep = farmInformation.getItemDouble("Value", newPath + ".NDepositionRate(-1)");
                                GlobalVars.Instance.theZoneData.SetNdeposition(Ndep);
                                newPath = newPath + ".SelectedScenario";
                                farmInformation.setPath(newPath);
                                int minScenario = 99, maxScenario = 0;
                                farmInformation.getSectionNumber(ref minScenario, ref maxScenario);
                                for (int settingsnr = minScenario; settingsnr <= maxScenario; settingsnr++)
                                {
                                    int InventorySystem = 0;
                                    int energySystem = 0;
                                    double areaWeightedDuration = 0;
                                    double farmArea = 0;

                                    int ScenarioNr = 0;
                                    if (args.Length == 0)
                                    {
                                        //Console.WriteLine(GlobalVars.Instance.getFarmFilePath());
                                        string[] tmp = GlobalVars.Instance.getFarmFilePath()[0].Split('_');
                                        ScenarioNr = Convert.ToInt32(tmp[tmp.Length - 1].Split('.')[0]);
                                    }
                                    else
                                        ScenarioNr = Convert.ToInt32(args[1]);
                                    int soilTypeCount = 0;

                                    if (farmInformation.doesIDExist(settingsnr))
                                    {
                                        string outputName;
                                        if ((GlobalVars.Instance.reuseCtoolData == -1) && (args.Length == 0))
                                            outputName = settings.getItemString("outputDir") + "outputFarm" + farmNr.ToString() + "BaselineScenarioNr" + ScenarioNr.ToString() + ".xml";
                                        else
                                            outputName = settings.getItemString("outputDir") + "outputFarm" + farmNr.ToString() + "ScenarioNr" + ScenarioNr.ToString() + ".xml";

                                        GlobalVars.Instance.initialiseExcretaExchange();
                                        GlobalVars.Instance.initialiseFeedAndProductLists();
                                        string ScenarioPath = newPath + "(" + ScenarioNr.ToString() + ")";
                                        farmInformation.setPath(ScenarioPath);
                                        farmInformation.Identity.Add(-1);
                                        if (GlobalVars.Instance.getcurrentInventorySystem() == 0)
                                        {
                                            farmInformation.PathNames.Add("InventorySystem");
                                            InventorySystem = farmInformation.getItemInt("Value");
                                            GlobalVars.Instance.setcurrentInventorySystem(InventorySystem);
                                        }
                                        farmInformation.PathNames.Add("EnergySystem");
                                        energySystem = farmInformation.getItemInt("Value");
                                        GlobalVars.Instance.setcurrentEnergySystem(energySystem);
                                        GlobalVars.Instance.OpenCtoolFile();
                                        List<CropSequenceClass> rotationList = new List<CropSequenceClass>();
                                        int minRotation = 99, maxRotation = 0;
                                        //do cropped area first
                                            string RotationPath = newPath + "(" + ScenarioNr.ToString() + ").Rotation";
                                            farmInformation.setPath(RotationPath);

                                            farmInformation.getSectionNumber(ref minRotation, ref maxRotation);
                                            for (int rotationID = minRotation; rotationID <= maxRotation; rotationID++)
                                            {
                                                if (farmInformation.doesIDExist(rotationID))
                                                {
                                                    CropSequenceClass anExample = new CropSequenceClass(RotationPath, rotationID, zoneNr, FarmTyp, "farmnr" + farmNr.ToString() + "_ScenarioNr" + ScenarioNr.ToString() + "_CropSequenceClass" + rotationID.ToString(), soilTypeCount);                                                 
                                                    farmArea += anExample.getArea();
                                                    
                                                    rotationList.Add(anExample);
                                                }
                                            }
                                       
                                        // temporary location
                                        List<manureStore> listOfManurestores = new List<manureStore>();
                                        List<housing> listOfHousing = new List<housing>();

                                        //start of livestock section
                                        string LivestockPath = newPath + "(" + ScenarioNr.ToString() + ").Livestock";
                                        farmInformation.setPath(LivestockPath);

                                        List<livestock> listOfLivestock = new List<livestock>();
                                        //read the livestock details from file
                                        int minLivestock = 99, maxLivestock = 0;

                                        farmInformation.getSectionNumber(ref minLivestock, ref maxLivestock);
                                        int animalNr = 0;
                                        for (int LiveStockID = minLivestock; LiveStockID <= maxLivestock; LiveStockID++)
                                        {

                                            if (farmInformation.doesIDExist(LiveStockID))
                                            {
                                                livestock anAnimal = new livestock(LivestockPath, LiveStockID, zoneNr, animalNr, "farmnr" + farmNr.ToString() + "_ScenarioNr" + ScenarioNr.ToString() + "_livestock" + LiveStockID.ToString());
#if WIDE_AREA
#else
                                                anAnimal.GetAllFeedItemsUsed();
#endif
                                                listOfLivestock.Add(anAnimal);
                                                animalNr++;
                                            }
                                        }

                                        //calculate composition of bedding material
#if WIDE_AREA
#else
                                        GlobalVars.Instance.CalcbeddingMaterial(rotationList);
#endif
                                        //read details of any manure stores that do not receive manure from livestock on the farm
                                        string ManureStoragePath = newPath + "(" + ScenarioNr.ToString() + ").ManureStorage";
                                        farmInformation.setPath(ManureStoragePath);
                                        //
                                        int minManureStorage = 99, maxManureStorage = 0;

                                        farmInformation.getSectionNumber(ref minManureStorage, ref maxManureStorage);
                                        for (int ManureStorageID = minManureStorage; ManureStorageID <= maxManureStorage; ManureStorageID++)
                                        {
                                            if (farmInformation.doesIDExist(ManureStorageID))
                                            {
                                                manureStore amanurestore = new manureStore(ManureStoragePath, ManureStorageID, zoneNr, "farmnr" + farmNr.ToString() + "_ScenarioNr" + ScenarioNr.ToString() + "_manureStore" + ManureStorageID.ToString());
                                                listOfManurestores.Add(amanurestore);
                                            }
                                        }

                                        //get details of animal housing (for each livestock category)
                                        for (int i = 0; i < listOfLivestock.Count(); i++)
                                        {
                                            livestock anAnimalCategory = listOfLivestock[i];
                                            for (int j = 0; j < anAnimalCategory.GethousingDetails().Count(); j++)
                                            {
                                                int housingType = anAnimalCategory.GethousingDetails()[j].GetHousingType();
                                                double proportionOfTime = anAnimalCategory.GethousingDetails()[j].GetpropTime();
                                                housing aHouse = new housing(housingType, anAnimalCategory, j, zoneNr, "farmnr" + farmNr.ToString() + "_ScenarioNr" + ScenarioNr.ToString() + "_housingi" + i.ToString() + "_housingj" + j.ToString());
                                                listOfHousing.Add(aHouse);
                                                //storage for manure produced in housing is initiated in the housing module
                                                for (int k = 0; k < aHouse.GetmanurestoreDetails().Count; k++)
                                                {
                                                    manureStore aManureStore = aHouse.GetmanurestoreDetails()[k].GettheStore();
                                                    aManureStore.SettheHousing(aHouse);
                                                    listOfManurestores.Add(aManureStore);
                                                }
                                            }
                                        }
                                        for (int i = 0; i < listOfLivestock.Count; i++)
                                        {
                                            livestock anAnimal = listOfLivestock[i];
#if WIDE_AREA
                                            anAnimal.DoLivestockIPCC();
#else
                                            
#endif
                                        }
                                        for (int i = 0; i < listOfHousing.Count; i++)
                                        {
                                            housing ahouse = listOfHousing[i];
                                            ahouse.DoHousing();
                                        }

                                        GlobalVars.Instance.theManureExchange = new GlobalVars.theManureExchangeClass();
                                        for (int i = 0; i < listOfManurestores.Count; i++)
                                        {
                                            manureStore amanurestore2 = listOfManurestores[i];
                                            amanurestore2.DoManurestore();
                                        }

#if WIDE_AREA
                                        for (int rotationID = 0; rotationID < rotationList.Count; rotationID++)
                                        {
                                            rotationList[rotationID].CalculateNbudget();
                                            
                                        }
                                        GlobalVars.Instance.CloseLogfile();
#else
#endif
                                        farmBalanceClass theBalances = new farmBalanceClass("farmnr" + farmNr.ToString() + "_ScenarioNr" + ScenarioNr.ToString() + "FarmBalance_1");
                                        theBalances.DoFarmBalances(FarmTyp,farmArea,rotationList, listOfLivestock, listOfHousing, listOfManurestores);
                                    }//end of scenario exists
                                    long ticks = DateTime.UtcNow.Ticks;
                                    //System.IO.File.WriteAllText(outputDir + "done" + farmNr.ToString() + "ScenarioNr" + ScenarioNr.ToString() + ".txt", ticks.ToString());
                                }//end of scenario
                            }//end of farm exists
                        }
                        catch (Exception e)
                        {

                            GlobalVars.Instance.Error(e.Message, e.StackTrace, false);
                            VMP3.Instance.closeVMP3files();
                        }
                    }
                    Console.WriteLine("Finished running farm " + (settingsID + 1).ToString() + " VMP3ID " + VMP3ID);
                }
            }
            VMP3.Instance.closeVMP3files();

        }
        static string getPath(string oldSPath)
        {
            string[] oldSPathSub = oldSPath.Split('.');
            string returnValue = "";
            for (int i = 0; i < oldSPathSub.Count() - 1; i++)
            {
                returnValue += oldSPathSub[i];
                if (i < (oldSPathSub.Count() - 2))
                {
                    returnValue += ".";
                }
            }
            return returnValue;
        }
        static string[] fileName(string[] args, FileInformation settings, string file)
        {
            string[] names = new string[2];
            if (args.Length != 0)
            {
                List<string> tmpPath = new List<string>(settings.PathNames);
                List<int> tmpID = new List<int>(settings.Identity);
                settings.Identity.Clear();
                settings.PathNames.Clear();
                settings.setPath("CommonSettings(-1)");
                string alternativePath = settings.getItemString("alternativePath");
                settings.PathNames = tmpPath;
                settings.Identity = tmpID;
                string constants = settings.getItemString(file);
                string[] constantsList = constants.Split('\\');
                string fileName = constantsList[constantsList.Length - 1];
                if (args[0] == "-1")
                    alternativePath += "\\" + fileName;
                else
                    alternativePath += "\\" + args[0] + "\\" + fileName;
                if (File.Exists(alternativePath))
                {
                    names[0] = alternativePath;
                }
                else
                {
                    names[0] = constants;
                }
                alternativePath = getPath(alternativePath) + "Alternative.xml";
                if (File.Exists(alternativePath))
                {
                    names[1] = alternativePath;
                }
                else
                {
                    names[1] = getPath(constants) + "Alternative.xml";
                }
            }
            else
            {
                names[0] = settings.getItemString(file);
                names[1] = null;
            }
            return names;


        }

        //code not used?
       
        static string[] argsthread;
      
    }
}
