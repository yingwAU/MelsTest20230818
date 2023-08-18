#define WIDE_AREA
using System.Collections.Generic;
using System.Xml;
using System;
using System.Diagnostics;
using System.IO;
using System.Xml.Linq;

public class GlobalVars
{
     private static GlobalVars instance;
     public struct avgCFom
     {
         public string parants;
         public double amounts;
  
     }; 
    public List<avgCFom> allAvgCFom=new List<avgCFom>();
    
    public struct avgNFom
    {
        public string parants;
        public double amounts;

    };
    public List<avgNFom> allAvgNFom = new List<avgNFom>();
    

     public bool header;
     public bool headerLivestock;
     public bool Ctoolheader;
     private Stopwatch sw;
     System.IO.StreamWriter Seyda;

    private GlobalVars()
     {
         header = false;
         headerLivestock = false;
         Ctoolheader = false;
     }
     public static GlobalVars Instance
     {
         get
         {
             if (instance == null)
             {
                 instance = new GlobalVars();
                 instance.sw = new Stopwatch();
                 instance.sw.Start();
             }
             return instance;
         }
     }
    //Returns the species group that should be associated with the StorageID (from Plantedirektorats list)
     private int maxSpeciesGroupTypes = 25;
    
    //Returns the manure type that should be associated with the StorageID (from Plantedirektorats list)
    private int maxManureTypes = 25;
    
     public struct grazedItem
     {
         public double urineC;
         public double urineN;
         public double faecesC;
         public double faecesN;
         public double ruminantDMgrazed;
         public double fieldDMgrazed;
         public double fieldCH4C;
         public string name;
         public string parens;
         
     }
     public grazedItem[] grazedArray = new grazedItem[maxNumberFeedItems];
     public product[] allFeedAndProductsUsed = new product[maxNumberFeedItems];
     public product[] allFeedAndProductsProduced = new product[maxNumberFeedItems];
     public product[] allFeedAndProductsPotential = new product[maxNumberFeedItems];
     public product[] allFeedAndProductTradeBalance = new product[maxNumberFeedItems];
     public product[] allFeedAndProductFieldProduction = new product[maxNumberFeedItems];
     private product[] allUnutilisedGrazableFeed = new product[maxNumberFeedItems];

    //constants
     private double humification_const;
     private double alpha;
     private double rgas;
     private double CNhum;
     private double tor;
     private double Eapp;
     private double CO2EqCH4;
     private double CO2EqN2O;
     private double CO2EqsoilC;
     private double IndirectNH3N2OFactor;
     private double IndirectNO3N2OFactor;
     private double defaultBeddingCconc;
     private double defaultBeddingNconc;
     private double maxToleratedError; 
     private double maxToleratedErrorGrazing;
     private int maximumIterations;
     private double EFNO3_IPCC;
     private double digestEnergyToME = 0.81;
     private int minimumTimePeriod;
     private int adaptationTimePeriod;
     private List<int> theInventorySystems;
     private int currentInventorySystem;
     private int currentEnergySystem;
     bool strictGrazing;
     public bool logFile;
     public bool logScreen;
     public int verbocity;

     public bool Writeoutputxlm;
     public bool Writeoutputxls;
     public bool Writectoolxlm;
     public bool Writectoolxls;
     public bool WriteDebug;
     public bool Writelivestock;
     public bool WritePlant;
     public bool WriteSeyda;
     public int reuseCtoolData;
     bool lockSoilTypes = false;  //if true, the CTOOL pools for each crop sequence will be preserved but the areas must not change. If false, pools within a soil type will be merged and areas can change.            
     public System.IO.StreamWriter logFileStream;

    private int zoneNr;
     public double getHumification_const() { return humification_const; }
    public double getalpha() { return alpha; }
    public double getrgas() { return rgas; }
    public double getCNhum() { return CNhum; }
    public double gettor() { return tor; }    
    public double GetCO2EqCH4() { return CO2EqCH4; }
    public double GetCO2EqN2O() { return CO2EqN2O; }
    public double GetCO2EqsoilC() { return CO2EqsoilC; }
    public double GetIndirectNH3N2OFactor() { return IndirectNH3N2OFactor; }
    public double GetIndirectNO3N2OFactor() { return IndirectNO3N2OFactor; }  
    public double GetdigestEnergyToME(){return digestEnergyToME;}
    public int GetZone() { return zoneNr; }
    public void SetZone(int zone) { zoneNr=zone; }    
    public int getcurrentInventorySystem() { return currentInventorySystem; }
    public int getcurrentEnergySystem() { return currentEnergySystem; }   
    public double getmaxToleratedError() { return maxToleratedError; }   
    public void setcurrentInventorySystem(int aVal) { currentInventorySystem = aVal; }
    public void setcurrentEnergySystem(int aVal) { currentEnergySystem = aVal; }
    
    private bool stopOnError;
    private bool pauseBeforeExit;
    public void setPauseBeforeExit(bool stop) { pauseBeforeExit = stop; }
    public bool getPauseBeforeExit() { return pauseBeforeExit; }
    public void setstopOnError(bool stop) { stopOnError = stop; }
   
    static string parens;
    public void reset(string aParens) {
        instance=null;
        parens = aParens;
        FileInformation information = new FileInformation();
        information.reset();
    }
    //private int[] errorCodes = new int[100];
    private bool RunFullModel;//forces exit with error if energy requirements not met
    public void setRunFullModel(bool aVal){RunFullModel=aVal;}    
    public double GetECM(double litres, double percentFat, double percentProtein)
    {
        double retVal = litres * (0.383 * percentFat + 0.242 * percentProtein + 0.7832) / 3.1138;
        return retVal;
    }

    public struct zoneSpecificData
    {
   
        private string debugFileName;
        private System.IO.StreamWriter debugfile;
        public double[] airTemp;
        private double[] droughtIndex;
        public double[] Precipitation;
        public double[] PotentialEvapoTrans;
        public int[] rainDays;
        private int numberRainyDaysPerYear;
        private double Ndeposition;

        //data read from parameters.xml, fertilisers or manure tag
        public struct fertiliserData
        {
            public int manureType; 
            public int speciesGroup; //livestock type for this manure (not used for fertilisers)
            public double fertManNH3EmissionFactor; //NH3 emission factor for field-applied manure or fertiliser (read from EFNH3)
            public double EFNH3FieldTier2; //Tier 2 NH3 emission for fertiliser (read from EFNH3FieldTier2)
            public double fertManNH3EmissionFactorHousingRefTemperature;
            public string name;
        }        
        public struct soilData
        {
            public double N2Factor;
            public string name;
            public double maxSoilDepth;
       
        }

        public struct manureAppData
        {
            public double NH3EmissionReductionFactor;
            public string name;
        }

        public List<fertiliserData> theFertManData;
        public List<soilData> thesoilData;
        public List<manureAppData> themanureAppData;
        double urineNH3EmissionFactor;
        public double leachingFraction;
        double manureN20EmissionFactor;
        double fertiliserN20EmissionFactor;
        double residueN2OEmissionFactor;
        double burntResidueN2OEmissionFactor;
        double burntResidueNH3EmissionFactor;
        double burntResidueNOxEmissionFactor;
        double burntResidueCOEmissionFactor;
        double burntResidueBlackCEmissionFactor;
        double soilN2OEmissionFactor;
        double manureN2Factor;
        double averageAirTemperature;
        int airtemperatureOffset;
        double airtemperatureAmplitude;
        int grazingMidpoint;
        double averageYearsToSimulate;
       
        public double getmanureN20EmissionFactor(){return manureN20EmissionFactor;}
        public double getfertiliserN20EmissionFactor(){return fertiliserN20EmissionFactor;}
        public double getsoilN2OEmissionFactor() { return soilN2OEmissionFactor; }
        public double GetaverageAirTemperature() { return averageAirTemperature; }
        public void SetNdeposition(double aVal) { Ndeposition = aVal; }
        public double GetNdeposition() { return Ndeposition; }
        public void readZoneSpecificData(int zone_nr, int currentFarmType)
        {
            FileInformation AEZParamFile = new FileInformation(GlobalVars.Instance.getParamFilePath());
            //get zone-specific constants
            string basePath = "AgroecologicalZone(" + zone_nr.ToString() + ")";
            AEZParamFile.setPath(basePath);
            AEZParamFile.setPath(basePath + ".UrineNH3EF(-1)");
            urineNH3EmissionFactor = AEZParamFile.getItemDouble("Value");
            AEZParamFile.setPath(basePath + ".Manure(-1).EFN2O(-1)");
            manureN20EmissionFactor = AEZParamFile.getItemDouble("Value");
            AEZParamFile.setPath(basePath + ".Manure(-1).N2Factor(-1)");
            manureN2Factor = AEZParamFile.getItemDouble("Value");
            string tempPath = basePath + ".CropResidues(-1).EFN2O(-1)";
            residueN2OEmissionFactor = AEZParamFile.getItemDouble("Value", tempPath);
            tempPath = basePath + ".CropResidues(-1).EFN2O_burning(-1)";
            burntResidueN2OEmissionFactor = AEZParamFile.getItemDouble("Value", tempPath);
            tempPath = basePath + ".CropResidues(-1).EFNOx_burning(-1)";
            burntResidueNOxEmissionFactor = AEZParamFile.getItemDouble("Value", tempPath);
            tempPath = basePath + ".CropResidues(-1).EFNH3_burning(-1)";
            burntResidueNH3EmissionFactor = AEZParamFile.getItemDouble("Value", tempPath);
            tempPath = basePath + ".CropResidues(-1).EFBlackC_burning(-1)";
            burntResidueBlackCEmissionFactor = AEZParamFile.getItemDouble("Value", tempPath);
            tempPath = basePath + ".CropResidues(-1).EFCO_burning(-1)";
            burntResidueCOEmissionFactor = AEZParamFile.getItemDouble("Value", tempPath);
            AEZParamFile.setPath(basePath + ".MineralisedSoilN(-1).EFN2O(-1)");
            soilN2OEmissionFactor = AEZParamFile.getItemDouble("Value");
            AEZParamFile.setPath("AgroecologicalZone(" + zone_nr.ToString() + ").ManureApplicationTechnique");
            int maxApp = 0, minApp = 99;
            AEZParamFile.getSectionNumber(ref minApp, ref maxApp);
            themanureAppData = new List<manureAppData>();
            AEZParamFile.Identity.Add(-1);
            for (int j = minApp; j <= maxApp; j++)
            {
                AEZParamFile.Identity[1] = j;
                manureAppData newappData = new manureAppData();
                string RecipientPath = "AgroecologicalZone(" + zone_nr.ToString() + ").ManureApplicationTechnique" + '(' + j.ToString() + ").Name";
                newappData.name = AEZParamFile.getItemString("Name", RecipientPath);
                RecipientPath = "AgroecologicalZone(" + zone_nr.ToString() + ").ManureApplicationTechnique" + '(' + j.ToString() + ").NH3ReductionFactor(-1)";
                newappData.NH3EmissionReductionFactor = AEZParamFile.getItemDouble("Value", RecipientPath);
                themanureAppData.Add(newappData);
            }
            AEZParamFile.setPath("AgroecologicalZone(" + zone_nr.ToString() + ").SoilType");
            int maxSoil = 0, minSoil = 99;
            AEZParamFile.getSectionNumber(ref minSoil, ref maxSoil);
            thesoilData = new List<soilData>();
            AEZParamFile.Identity.Add(-1);
            for (int j = minSoil; j <= maxSoil; j++)
            {
                AEZParamFile.setPath("AgroecologicalZone(" + zone_nr.ToString() + ").SoilType");
                if (AEZParamFile.doesIDExist(j))
                {

                    soilData newsoilData = new soilData();
                    string RecipientStub = "AgroecologicalZone(" + zone_nr.ToString() + ").SoilType" + '(' + j.ToString() + ").";
                    string RecipientPath = RecipientStub;
                    newsoilData.name = AEZParamFile.getItemString("Name", RecipientPath);
                    RecipientPath = RecipientStub + "N2Factor(-1)";
                    newsoilData.N2Factor = AEZParamFile.getItemDouble("Value", RecipientPath);
                    thesoilData.Add(newsoilData);
                }
            }
            AEZParamFile.setPath("AgroecologicalZone(" + zone_nr.ToString() + ").Fertiliser(-1).EFN2O(-1)");
            fertiliserN20EmissionFactor = AEZParamFile.getItemDouble("Value");
            AEZParamFile.setPath("AgroecologicalZone(" + zone_nr.ToString() + ").Fertiliser(-1).FertiliserType");
            int maxFert = 0, minFert = 99;
            AEZParamFile.getSectionNumber(ref minFert, ref maxFert);
            theFertManData = new List<fertiliserData>();
           
            for (int j = minFert; j <= maxFert; j++)
            {
                AEZParamFile.setPath("AgroecologicalZone(" + zone_nr.ToString() + ").Fertiliser(-1).FertiliserType");
                if (AEZParamFile.doesIDExist(j))
                {
                    AEZParamFile.Identity.Add(j);
                    fertiliserData newfertData = new fertiliserData();
                    string RecipientPath = "AgroecologicalZone(" + zone_nr.ToString() + ").Fertiliser(-1).FertiliserType" + '(' + j.ToString() + ")";
                    newfertData.name = AEZParamFile.getItemString("Name", RecipientPath);
                    RecipientPath = "AgroecologicalZone(" + zone_nr.ToString() + ").Fertiliser(-1).FertiliserType" + '(' + j.ToString() + ").EFNH3(-1)";
                    newfertData.fertManNH3EmissionFactor = AEZParamFile.getItemDouble("Value", RecipientPath);
                    newfertData.fertManNH3EmissionFactorHousingRefTemperature = 0;
                    theFertManData.Add(newfertData);
                
                }
            }
            string tmpPath = "AgroecologicalZone(" + zone_nr.ToString() + ").Manure(-1).ManureType";
            AEZParamFile.setPath(tmpPath);
            int maxMan = 0, minMan= 99;
            AEZParamFile.getSectionNumber(ref minMan, ref maxMan);
            AEZParamFile.Identity.Add(-1);
            for (int j = minMan; j <= maxMan; j++)
            {
                tmpPath = "AgroecologicalZone(" + zone_nr.ToString() + ").Manure(-1).ManureType";
                AEZParamFile.setPath(tmpPath);
                if (AEZParamFile.doesIDExist(j))
                {
                tmpPath = "AgroecologicalZone(" + zone_nr.ToString() + ").Manure(-1).ManureType(-1)";
                AEZParamFile.setPath(tmpPath);
                AEZParamFile.Identity[2] = j;
                fertiliserData newfertData = new fertiliserData();
                newfertData.manureType = AEZParamFile.getItemInt("StorageType");
                newfertData.speciesGroup= AEZParamFile.getItemInt("SpeciesGroup");
                newfertData.name = AEZParamFile.getItemString("Name");
                string RecipientPath = "AgroecologicalZone(" + zone_nr.ToString() + ").Manure(-1).ManureType" + '(' + j.ToString() + ").EFNH3FieldRef(-1)";
                newfertData.fertManNH3EmissionFactor = AEZParamFile.getItemDouble("Value", RecipientPath);
                RecipientPath = "AgroecologicalZone(" + zone_nr.ToString() + ").Manure(-1).ManureType" + '(' + j.ToString() + ").EFNH3FieldRefTemperature(-1)";
                newfertData.fertManNH3EmissionFactorHousingRefTemperature = AEZParamFile.getItemDouble("Value", RecipientPath);
                RecipientPath = "AgroecologicalZone(" + zone_nr.ToString() + ").Manure(-1).ManureType" + '(' + j.ToString() + ").EFNH3FieldTier2(-1)";
                newfertData.EFNH3FieldTier2 = AEZParamFile.getItemDouble("Value", RecipientPath);
                theFertManData.Add(newfertData);
                }
            }         
        }         
    }
    public zoneSpecificData theZoneData;
    public void readGlobalConstants()
    {
        FileInformation constants = new FileInformation(GlobalVars.Instance.getConstantFilePath());
        constants.setPath("constants(0)");
        constants.Identity.Add(-1);
        constants.PathNames.Add("humification_const");
        humification_const=constants.getItemDouble("Value");
        constants.PathNames[constants.PathNames.Count - 1] = "alpha";
        alpha = constants.getItemDouble("Value");

        constants.PathNames[constants.PathNames.Count - 1] = "rgas";
        rgas = constants.getItemDouble("Value");
        constants.PathNames[constants.PathNames.Count - 1] = "CNhum";
        CNhum = constants.getItemDouble("Value");
        constants.PathNames[constants.PathNames.Count - 1] = "tor";
        tor = constants.getItemDouble("Value");
        constants.PathNames[constants.PathNames.Count - 1] = "Eapp";
        Eapp = constants.getItemDouble("Value");
        constants.PathNames[constants.PathNames.Count - 1] = "CO2EqCH4";
        CO2EqCH4 = constants.getItemDouble("Value");
        constants.PathNames[constants.PathNames.Count - 1] = "CO2EqN2O";
        CO2EqN2O = constants.getItemDouble("Value");
        constants.PathNames[constants.PathNames.Count - 1] = "CO2EqsoilC";
        CO2EqsoilC = constants.getItemDouble("Value");
        constants.PathNames[constants.PathNames.Count - 1] = "IndirectNH3N2OFactor";
        IndirectNH3N2OFactor = constants.getItemDouble("Value");
        constants.PathNames[constants.PathNames.Count - 1] = "IndirectNO3N2OFactor";
        IndirectNO3N2OFactor = constants.getItemDouble("Value");
        constants.PathNames[constants.PathNames.Count - 1] = "defaultBeddingCconc";
        defaultBeddingCconc = constants.getItemDouble("Value");
        constants.PathNames[constants.PathNames.Count - 1] = "defaultBeddingNconc";
        defaultBeddingNconc = constants.getItemDouble("Value");
        constants.PathNames[constants.PathNames.Count - 1] = "ErrorToleranceYield";
        maxToleratedError = constants.getItemDouble("Value");
        constants.PathNames[constants.PathNames.Count - 1] = "ErrorToleranceGrazing";
        maxToleratedErrorGrazing = constants.getItemDouble("Value");

        constants.PathNames[constants.PathNames.Count - 1] = "maximumIterations";
        maximumIterations = constants.getItemInt("Value");
        constants.PathNames[constants.PathNames.Count - 1] = "EFNO3_IPCC";
        EFNO3_IPCC = constants.getItemDouble("Value");
        constants.PathNames[constants.PathNames.Count - 1] = "minimumTimePeriod";
        minimumTimePeriod = constants.getItemInt("Value");
        constants.PathNames[constants.PathNames.Count - 1] = "adaptationTimePeriod";
        adaptationTimePeriod = constants.getItemInt("Value");
        constants.PathNames[constants.PathNames.Count - 1] = "lockSoilTypes";
        lockSoilTypes = constants.getItemBool("Value");

        List<int> theInventorySystems = new List<int>();
        constants.setPath("constants(0).InventorySystem");
        int maxInvSysts = 0, minInvSysts = 99;
        constants.getSectionNumber(ref minInvSysts, ref maxInvSysts);
        constants.Identity.Add(-1);
        for (int i = minInvSysts; i <= maxInvSysts; i++)
        {
            constants.Identity[constants.Identity.Count - 1] = i;
            theInventorySystems.Add(constants.getItemInt("Value"));
            //theInventorySystems.Add(i);
        }
    }

    private string[] constantFilePath;
     public void setConstantFilePath(string[] path)
     {
         constantFilePath = path;
     }
     public string[] getConstantFilePath()
     {
         return constantFilePath;
     }

     private string[] ParamFilePath;
     public void setParamFilePath(string[] path)
     {
         ParamFilePath = path;
     }
     public string[] getParamFilePath()
     {
         return ParamFilePath;
     }
     private string[] farmFilePath;
     public void setFarmtFilePath(string[] path)
     {
         farmFilePath = path;
     }
     public string[] getFarmFilePath()
     {
         return farmFilePath;
     }
     private string[] feeditemPath;
     public void setFeeditemFilePath(string[] path)
     {
         feeditemPath = path;
     }
     public string[] getfeeditemFilePath()
     {
         return feeditemPath;
     }
     private string[] fertManPath;
     public void setfertManFilePath(string[] path)
     {
         fertManPath = path;
     }
     public string[] getfertManFilePath()
     {
         return fertManPath;
     }
     private string simplesoilModel = "simplesoilModel.xml";
     
     private string writeHandOverData = "simplesoilModel.xml";
    
     private string ReadHandOverData = "simplesoilModel.xml";    
        
     private string errorFileName="error.xml";
     public void seterrorFileName(string path)
     {
         errorFileName = path;
     }
     public string GeterrorFileName() { return errorFileName; }
     public const int totalNumberLivestockCategories = 1;
    public const int totalNumberHousingCategories = 1;
    public const int totalNumberSpeciesGroups = 1;
    public const int totalNumberStorageTypes = 1;
    public const double avgNumberOfDays = 365;
    public const double NtoCrudeProtein = 6.25;
    public const double absoluteTemp = 273.15;
    public const int maxNumberFeedItems = 2000;
    
    public List<housing> listOfHousing = new List<housing>();

    public List<manureStore> listOfManurestores = new List<manureStore>();

    public class product
    {
        public double Modelled_yield;
        public double Expected_yield;
        public double Potential_yield;
        public double waterLimited_yield;
        public double Grazed_yield;
        public string Harvested;
        public feedItem composition;
        public int usableForBedding;
        public string Units;
        public bool burn;
        public double ResidueGrazingAmount;
        public product()
        {
            Modelled_yield = 0;
            Expected_yield = 0;
           waterLimited_yield = 0;
            Grazed_yield = 0;
            Potential_yield = 0;
            Harvested = "";
            usableForBedding = 0;
            Units = "";
            burn = false;
            ResidueGrazingAmount = 0;
            composition = new feedItem();
        }
        
        public double GetPotential_yield() { return Potential_yield; }
       
    }
   
    public feedItem thebeddingMaterial = new feedItem();   

    //need to calculate these values
    
    public struct manurestoreRecord
    {
        manureStore theStore;
        double propYearGrazing;
        public void SetpropYearGrazing(double aVal) { propYearGrazing = aVal; }
        public manure manureToStorage;
        public void SetmanureToStorage(manure amanureToStorage) { manureToStorage = amanureToStorage; }
        public double GetpropYearGrazing() { return propYearGrazing; }
        public manure GetmanureToStorage() { return manureToStorage; }
        public manureStore GettheStore() { return theStore; }
        public void SettheStore(manureStore aStore) { theStore = aStore; }
        
    }

    //the theManureExchangeClass is used to keep track of the manure generated on the farm and the manure that must be imported 
    public class theManureExchangeClass
    {
        private List<manure> manuresStored;
        private List<manure> manuresProduced;
        private List<manure> manuresImported;
        private List<manure> manuresUsed;
                
        public theManureExchangeClass()
        {
            manuresStored = new List<manure>();
            manuresProduced = new List<manure>();
            manuresImported = new List<manure>();
            manuresUsed = new List<manure>();
        }
        //! adds manure to the list of manures available
               
    }//end of manure exchange
    private XmlWriter writer;
    //public XElement writerCtool;
    //public XmlWriter writerCtool;
    private System.IO.StreamWriter tabFile;
    private System.IO.StreamWriter DebugFile;
    private System.IO.StreamWriter PlantFile;
    private System.IO.StreamWriter livestockFile;
    private System.IO.StreamWriter CtoolFile;
    private string plantfileName;
    private string CtoolfileName;
    private string DebugfileName;
    private string livestockfileName;
    string PlantHeaderName;
    string PlantHeaderUnits;
    string CtoolHeaderName;
    string CtoolHeaderUnits;
    string livestockHeaderName;
    string livestockHeaderUnits;  

    public void CloseOutputXML()
    {
        try
        {
            if (Writeoutputxlm)
            {
                writer.WriteEndDocument();
                writer.Close();
            }
        }
        catch
        {
        }
    }

    public void writeEndTab()
    {
        if (Writeoutputxlm)
        writer.WriteEndElement();
    }
    public void writeInformationToFiles(string name, string Description, string Units, bool value, string parens)
    {
        writeInformationToFiles(name, Description, Units, Convert.ToString(value), parens);
    }
    public void writeInformationToFiles(string name, string Description, string Units, double value, string parens)
    {
        writeInformationToFiles(name, Description, Units, Convert.ToString(Math.Round(value, 4)), parens);
    }
    public void writeInformationToFiles(string name, string Description, string Units, int value, string parens)
    {
        writeInformationToFiles(name, Description, Units, Convert.ToString(value), parens);
    }

    public void writeInformationToFiles(string name, string Description, string Units, string value,string parens)
    {
        if (Writeoutputxlm)
        {
            writer.WriteStartElement(name);
            writer.WriteStartElement("Description");
            writer.WriteValue(Description);
            writer.WriteEndElement();
            writer.WriteStartElement("Units");
            writer.WriteValue(Units);
            writer.WriteEndElement();
            writer.WriteStartElement("Name");
            writer.WriteValue(name);
            writer.WriteEndElement();
            writer.WriteStartElement("Value");
            writer.WriteValue(value);
            writer.WriteEndElement();
            writer.WriteStartElement("StringUI");
            writer.WriteValue(name + parens);
            writer.WriteEndElement();
            writer.WriteEndElement();
        }
        if (Writeoutputxls)
        {
            tabFile.Write("Description" + "\t");
            tabFile.Write(Description + "\t");
            tabFile.Write("Units" + "\t");
            tabFile.Write(Units + "\t");
            tabFile.Write("Name" + "\t");
            tabFile.Write(name + parens + "\t");
            tabFile.Write("Value" + "\t");
            tabFile.Write(value + "\n");
        }
       
    }
    
    static bool usedPlant = false;
  
    public void ClosePlantFile()
    {
        try
        {
            if (WritePlant)
            PlantFile.Close();  
        }
        catch
        {
        }
    }
   
    public void CloseDebugFile()
    {

        try
        {
            if (WriteDebug)
            {
                DebugFile.Write(headerDebug);
                DebugFile.Write(dataDebug);
                DebugFile.Close();
                if (headerDebug == null)
                    File.Delete(DebugfileName);
            }
        }
        catch
        {
        }
    }
    private bool DebugHeader=true;
    private string headerDebug;
    private string dataDebug;    
    public void OpenCtoolFile()
    {
        if(Writectoolxls)
        CtoolFile = new System.IO.StreamWriter(CtoolfileName);
     
    }
    public void CloseCtoolFile()
    {
        if (Writectoolxls)
        CtoolFile.Close();
      
    }
    public void CloseLivestockFile()
    {
        try
        {
            if (Writelivestock)
            livestockFile.Close();
        }
        catch
        {
        }
    }
    public void CloseOutputTabFile()
    {
        try
        {
            if (Writeoutputxls)
                tabFile.Close();
        }
        catch
        {
        }

    }    
    public void Error(string erroMsg, string stakTrace="",bool withException = true)
    {
        try
        {
            if (withException == true)
            {
                CloseLivestockFile();
                
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
        try
        {
            if (withException == true)
            {
                CloseDebugFile();

            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
        try
        {
            if (withException == true)
            {
                CloseLogfile();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
        try
        {
            if (withException == true)
            {
                CloseOutputTabFile();
                CloseLivestockFile();
                CloseDebugFile();
                CloseLogfile();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
        try
        {
            if (withException == true)
            {
                CloseOutputXML();
                CloseOutputTabFile();
                ClosePlantFile();
                CloseLivestockFile();
                CloseCtoolFile();
                CloseDebugFile();
                CloseLogfile();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
        if (!erroMsg.Contains("farm Fail"))
        {
            Console.WriteLine(GlobalVars.Instance.GeterrorFileName());
            System.IO.StreamWriter files = new System.IO.StreamWriter(GlobalVars.Instance.GeterrorFileName());
            files.WriteLine(erroMsg + " " + stakTrace);
            files.Close();
            Console.WriteLine(erroMsg + " " + stakTrace);
            sw.Stop();
            Console.WriteLine("RunTime (hrs:mins:secs) " + sw.Elapsed);
            if (GlobalVars.Instance.getPauseBeforeExit())
                Console.Read();
        }
        
        if (withException == true)
            throw new System.ArgumentException("farm Fail", "farm Fail");
    }
    public theManureExchangeClass theManureExchange;
    public void initialiseExcretaExchange()
    {
        for (int i = 0; i < maxNumberFeedItems; i++)
        {
            grazedArray[i].ruminantDMgrazed = 0;  //kg
            grazedArray[i].fieldDMgrazed = 0;  //kg
            grazedArray[i].urineC = 0;  //kg
            grazedArray[i].urineN = 0;  //kg
            grazedArray[i].faecesC = 0;  //kg
            grazedArray[i].faecesN = 0;  //kg
            grazedArray[i].fieldCH4C = 0; //kg
        }
    }
    
    public void initialiseFeedAndProductLists()
    {
        for (int i = 0; i < maxNumberFeedItems; i++)
        {
            feedItem aproduct = new feedItem();
            allFeedAndProductsUsed[i] = new product();
            allFeedAndProductsUsed[i].composition = aproduct ;
            allFeedAndProductsUsed[i].composition.setFeedCode(i);
            aproduct = new feedItem();
            allFeedAndProductsProduced[i] = new product();
            allFeedAndProductsProduced[i].composition=aproduct;
            allFeedAndProductsProduced[i].composition.setFeedCode(i);
            aproduct = new feedItem();
            allFeedAndProductsPotential[i] = new product();
            allFeedAndProductsPotential[i].composition = aproduct;
            allFeedAndProductsPotential[i].composition.setFeedCode(i);
            aproduct = new feedItem();
            allFeedAndProductFieldProduction[i] = new product();
            allFeedAndProductFieldProduction[i].composition = aproduct;
            allFeedAndProductFieldProduction[i].composition.setFeedCode(i);
            aproduct = new feedItem();
            allFeedAndProductTradeBalance[i] = new product();
            allFeedAndProductTradeBalance[i].composition=aproduct;
            allFeedAndProductTradeBalance[i].composition.setFeedCode(i);
            aproduct = new feedItem();            
            allUnutilisedGrazableFeed[i] = new product();
            allUnutilisedGrazableFeed[i].composition = aproduct;
            allUnutilisedGrazableFeed[i].composition.setFeedCode(i);
       }
    }

    public void log(string informatio, int level)
    {
        if (level <= verbocity)
        {
            if (logScreen)
                Console.WriteLine(informatio);
            if (logFile)
            {
                try
                {
                    logFileStream.WriteLine(informatio);

                }
                catch
                {

                }
                
            }
        }

    }
    
    public void CloseLogfile()
    {
        if (logFileStream != null)
            logFileStream.Close();
    }

}