#define WIDE_AREA
using System;
using System.Collections.Generic;
using System.Xml;
public class livestock
{
    public struct ManureRecipient
    {
        int ManureStorageID;
        string parens;
        public void setParens(string aParens){parens=aParens;}
        public void setManureStorageID(int aType)
        {
            ManureStorageID = aType;
        }
        public int GetStorageType() { return ManureStorageID; }
        public void Write()
        {
            GlobalVars.Instance.writeInformationToFiles("StorageType", "Type of manure store", "-", ManureStorageID, parens);
        }
    }
    public struct housingRecord
    {
        int HousingType;
        double propTime;
        string NameOfHousing;
        string parens;
        public List<ManureRecipient> Recipient;
        public void SetNameOfHousing(string aName) { NameOfHousing = aName; }
        public void SetHousingType(int aVal) { HousingType = aVal; }
        public void SetpropTime(double aVal) { propTime = aVal; }
        public int GetHousingType() { return HousingType; }
        public string GetHousingName() { return NameOfHousing; }
        public double GetpropTime() { return propTime; }
        public List<ManureRecipient> GetManureRecipient() { return Recipient; }
        public void setParens(string aParens) { 
            parens = aParens; 
        }
        public void Write()
        {
            GlobalVars.Instance.writeInformationToFiles("HousingType", "Type of housing", "-", HousingType,parens);
            GlobalVars.Instance.writeInformationToFiles("propTime", "Proportion of time spent in house", "-", propTime, parens);
            for (int i=0; i < Recipient.Count; i++)
                Recipient[i].Write();
        }
    }
   
    string path;

    //characteristics of livestock
    bool isRuminant;
    bool isDairy; // true if this is a milk-producing animal
#if WIDE_AREA
    double TAN_excreted;
    double OrganicN_excreted;
    double CH4emissionFactor;
    double VS_excretion;
#endif
    double mu_base;   //energy intake level below which there is no reduction in energy utilisation
    double mu_b; //
    //input data
    double avgNumberOfAnimal;

    double avgProductionMilk;
    double avgProductionMeat;
    double avgProductionECM;
    double efficiencyProteinMilk;
    List<housingRecord> housingDetails;
    //parameters
    int identity;
    private string parens;
    int speciesGroup;
    int LivestockType; //finding parameter for this type
    double liveweight;
    double startWeight;
    double endWeight;
    double duration;
    double urineProp;
    string name;
    double growthNconc;
    double growthCconc;
    double milkNconc;
    double milkCconc;
    double milkFat;
    double age;
    double maintenanceEnergyCoeff;
    double growthEnergyDemandCoeff;
    double milkAdjustmentCoeff;
    bool housedDuringGrazing;
    double mortalityCoefficient;
    double entericTier2MCF;
    double Bo;
    double nitrateEfficiency;
    //other variables
    double energyIntake;
    double energyDemand;
    double energyUseForMaintenance;
    double energyUseForGrowth;
    double energyUseForMilk;
    double energyUseForGrazing;
    double energyFromRemobilisation;
    double maintenanceEnergyDeficit;
    double growthEnergyDeficit;
    double concentrateEnergy;

    double DMintake;
    double DMgrazed;
    double FE;
    double concentrateDM;
    double Nintake;
    double diet_ash;
    double diet_fibre;
    double diet_fat;
    double diet_NFE;
    double digestibilityDiet;
    double diet_nitrate;//kg/kg
    double Cintake;
    double energyLevel;
    double milkN;
    double milkC;
    double growthN;
    double growthC;
    double mortalitiesN;
    double mortalitiesC;
    double urineC;
    double urineN;
    double faecalC;
    double CCH4GR;
    double faecalN;
    double NexcretionToPasture;
    double CexcretionToPasture;
    double CH4C;
    double CO2C;
    double grazedN = 0;
    double grazedDM = 0;
    double pastureFedN = 0;
    double mexp=0; //Mass of manure from species group sg 
    double cman=0;// Mass of manure from species group sg
    double nman = 0;
    double vsg=0;//Annual production of manure from species group sg and store type s
    bool proteinLimited;
   // feedItem milk;  //used to hold milk produced
    public double getCH4C() { return CH4C; }
   
    public double timeOnPasture;
   
    List<feedItem> feedRation;
      
    public double GeturineN() { return urineN; }
    public double GetfaecalN() { return faecalN; }  
    public double GetAvgNumberOfAnimal(){return avgNumberOfAnimal;}
    public double GettimeOnPasture() { return timeOnPasture; }
    public List<feedItem> GetfeedRation() { return feedRation; }
    public double GetBo() { return Bo; }
    public string Getname() { return name; }   
    public int GetspeciesGroup() { return speciesGroup; }
#if WIDE_AREA
    public double GetOrganicN_excreted() { return OrganicN_excreted; }
    public double GetTAN_excreted() { return TAN_excreted; }
    public double GetVS_excretion() { return VS_excretion; }
   #endif
    public List<housingRecord> GethousingDetails() { return housingDetails; }
   
    public livestock(string aPath, int id, int zoneNr, int AnimalNr, string aParens)
    {
        parens = aParens;
        FileInformation livestockFile =new FileInformation(GlobalVars.Instance.getFarmFilePath());
        identity = id;
        path = aPath+"("+id.ToString()+")";
        livestockFile.setPath(path);
        feedRation = new List<feedItem>();
        urineProp = 0;
        DMintake =0;
        DMgrazed = 0;
        energyDemand = 0;
        energyIntake = 0;
        diet_ash = 0;
        diet_nitrate = 0;
        digestibilityDiet = 0;
        timeOnPasture = 0;
        proteinLimited = false;
        name = livestockFile.getItemString("NameOfAnimals");
        avgNumberOfAnimal = livestockFile.getItemDouble("NumberOfAnimals");
        housingDetails = new List<housingRecord>();
        if (avgNumberOfAnimal > 0)
        {
            LivestockType = livestockFile.getItemInt("LivestockType");
            speciesGroup = livestockFile.getItemInt("Species_group");

            FileInformation paramFile = new FileInformation(GlobalVars.Instance.getParamFilePath());

            //read livestock parameters from constants.xml
            string basePath = "AgroecologicalZone(" + zoneNr.ToString() + ").Livestock";
            int min = 99, max = 0;
            paramFile.setPath(basePath);
            paramFile.getSectionNumber(ref min, ref max);
            bool gotit = false;
            int livestockID = 0;
            for (int i = min; i <= max; i++)
            {
                if (paramFile.doesIDExist(i))
                {
                    string testPath = basePath + "(" + i.ToString() + ").LivestockType(0)";
                    int testLivestockType = paramFile.getItemInt("Value", testPath);
                    testPath = basePath + "(" + i.ToString() + ").SpeciesGroup(0)";
                    int testspeciesGroup = paramFile.getItemInt("Value", testPath);
                   // Console.WriteLine(speciesGroup + " " + LivestockType + " " + testspeciesGroup + " " + testLivestockType);
                    if ((testLivestockType == LivestockType) && (testspeciesGroup == speciesGroup))
                    {
                        livestockID = i;
                        gotit = true;
                        break;
                    }
                    paramFile.setPath(basePath);
                }
            }
            if (gotit == false)
            {
                string messageString = ("Livestock " + name + " Species " + speciesGroup.ToString() + ", Livestocktype  " + LivestockType.ToString() + " not found in parameters.xml");
                GlobalVars.Instance.Error(messageString);
            }
            basePath = "AgroecologicalZone(" + zoneNr.ToString() + ").Livestock(" + Convert.ToInt32(livestockID) + ")";
            paramFile.setPath(basePath + ".WideArea(-1).TAN(-1)");
            TAN_excreted = paramFile.getItemDouble("Value");
            paramFile.setPath(basePath + ".WideArea(-1).OrganicN(-1)");
            OrganicN_excreted = paramFile.getItemDouble("Value");
            paramFile.setPath(basePath + ".WideArea(-1).Enteric_CH4(-1)");
            CH4emissionFactor = paramFile.getItemDouble("Value");
            paramFile.setPath(basePath + ".WideArea(-1).VS_excretion(-1)");
            VS_excretion = paramFile.getItemDouble("Value");
            
            paramFile.setPath(basePath + ".efficiencyProteinMilk(0)");
            efficiencyProteinMilk = paramFile.getItemDouble("Value");
            
            paramFile.setPath(basePath + ".growthNconc(0)");
            growthNconc = paramFile.getItemDouble("Value"); 
            paramFile.setPath(basePath + ".growthCconc(0)");
            growthCconc = paramFile.getItemDouble("Value");
            paramFile.setPath(basePath + ".urineProp(0)");
            urineProp = paramFile.getItemDouble("Value");
            paramFile.setPath(basePath + ".maintenanceEnergyCoeff(0)");
            maintenanceEnergyCoeff = paramFile.getItemDouble("Value");
            paramFile.setPath(basePath + ".growthEnergyDemandCoeff(0)");
            growthEnergyDemandCoeff = paramFile.getItemDouble("Value");
            if (isDairy)
            {
                paramFile.setPath(basePath + ".milkAdjustmentCoeff(0)");
                milkAdjustmentCoeff = paramFile.getItemDouble("Value");
                paramFile.setPath(basePath + ".milkFat(0)");
                milkFat = paramFile.getItemDouble("Value");
            }
            paramFile.setPath(basePath + ".isRuminant(0)");
            isRuminant = paramFile.getItemBool("Value");
            paramFile.setPath(basePath + ".isDairy(0)");
            isDairy = paramFile.getItemBool("Value");
            paramFile.setPath(basePath + ".Liveweight(0)");
            liveweight = paramFile.getItemDouble("Value");
            paramFile.setPath(basePath + ".Age(0)");
            age = paramFile.getItemDouble("Value");
            paramFile.setPath(basePath + ".HousedDuringGrazing(-1)");
            housedDuringGrazing = paramFile.getItemBool("Value");
            paramFile.setPath(basePath + ".Mortality(0)");
            mortalityCoefficient = paramFile.getItemDouble("Value");
            entericTier2MCF = paramFile.getItemDouble("Value", basePath + ".entericTier2MCF(-1)");
            Bo = paramFile.getItemDouble("Value", basePath + ".Bo(-1)");
            if (isRuminant)
            {
                paramFile.setPath(basePath + ".mu_b(0)");
                mu_b = paramFile.getItemDouble("Value");
                paramFile.setPath(basePath + ".mu_base(0)");
                mu_base = paramFile.getItemDouble("Value");
                paramFile.setPath(basePath + ".milkNconc(0)");
                milkNconc = paramFile.getItemDouble("Value");
                paramFile.setPath(basePath + ".milkCconc(0)");
                milkCconc = paramFile.getItemDouble("Value");
            }
            //back to reading user input
            if (isDairy)
            {
                paramFile.setPath(basePath + ".weightGainDairy(0)");
                avgProductionMeat = paramFile.getItemDouble("Value");
                avgProductionMeat /= GlobalVars.avgNumberOfDays;
            }
            else
            {
                if (speciesGroup == 1)
                    avgProductionMeat = livestockFile.getItemDouble("avgProductionMeat");
                else
                if (speciesGroup == 2)
                {
                }
            }


            string housingPath = path + ".Housing";
            min = 99;
            max = 0;
            livestockFile.setPath(housingPath);
            livestockFile.getSectionNumber(ref min, ref max);
            if (max > 0)
            {
                double testPropTime = 0;
                for (int i = min; i <= max; i++)
                {
                    if (livestockFile.doesIDExist(i))
                    {
                        housingRecord newHouse = new housingRecord();
                        newHouse.setParens(parens + "_housingRecord" + i.ToString());
                        livestockFile.Identity.Add(i);
                        newHouse.SetHousingType(livestockFile.getItemInt("HousingType"));
                        newHouse.SetNameOfHousing(livestockFile.getItemString("NameOfHousing"));
                        if (newHouse.GetHousingName() != "None")
                        {
                            newHouse.SetpropTime(livestockFile.getItemDouble("PropTime"));
                            testPropTime += newHouse.GetpropTime();
                            int maxManureRecipient = 0, minManureRecipient = 99;
                            newHouse.Recipient = new List<ManureRecipient>();
                            string RecipientPath = housingPath + '(' + i.ToString() + ").ManureRecipient";
                            livestockFile.setPath(RecipientPath);
                            livestockFile.getSectionNumber(ref minManureRecipient, ref maxManureRecipient);
                            for (int j = minManureRecipient; j <= maxManureRecipient; j++)
                            {
                                if (livestockFile.doesIDExist(j))
                                {
                                    ManureRecipient newRecipient = new ManureRecipient();
                                    newRecipient.setParens(parens + "_ManureRecipientI" + i.ToString() + "_ManureRecipientJ" + j.ToString());
                                    livestockFile.Identity.Add(j);
                                    int type = livestockFile.getItemInt("StorageType");
                                   
                                    newRecipient.setManureStorageID(type);
                                    newHouse.Recipient.Add(newRecipient);
                                    livestockFile.Identity.RemoveAt(livestockFile.Identity.Count - 1);
                                }
                            }
                            //Hack to remove surplus housing generated for pigs by user interface
                            if ((newHouse.GetHousingType() != 0) && (speciesGroup == 2))
                                livestockFile.Identity.RemoveAt(livestockFile.Identity.Count - 1);
                            housingDetails.Add(newHouse);
                        }
                        else
                        {
                            testPropTime = 1.0;
                            livestockFile.Identity.RemoveAt(livestockFile.Identity.Count - 1);
                        }
                    }
                }
                if (testPropTime != 1.0)
                {
                    string messageString = ("Sum of proportions of time in different housing does not equal 1.0 ");
                    GlobalVars.Instance.Error(messageString);
                }
            }
#if WIDE_AREA

#endif
        } //end if average number of animals >0
    }

    //these functions calculate energy demands using a specific energy system. the value calculated here should actually be read from file

    public void Write()
    {
        double numofDaysInYear = GlobalVars.avgNumberOfDays;
        GlobalVars.Instance.writeInformationToFiles("nameLiveStock", "Name", "-", name, parens);
        GlobalVars.Instance.writeInformationToFiles("speciesGroup", "Species identifier", "-", speciesGroup, parens);
        GlobalVars.Instance.writeInformationToFiles("LivestockType", "Livestock type", "", LivestockType, parens);
        GlobalVars.Instance.writeInformationToFiles("liveweight", "Liveweight", "kg", liveweight, parens);
        GlobalVars.Instance.writeInformationToFiles("isRuminant", "Is a ruminant", "-", isRuminant, parens);
        GlobalVars.Instance.writeInformationToFiles("avgNumberOfAnimal", "Annual average number of animals", "-", avgNumberOfAnimal, parens);

        GlobalVars.Instance.writeInformationToFiles("DMintake", "Intake of DM", "kg/day", DMintake / numofDaysInYear, parens);
        GlobalVars.Instance.writeInformationToFiles("energyIntake", "Intake of energy", "MJ/day", energyIntake / numofDaysInYear, parens);
        GlobalVars.Instance.writeInformationToFiles("energyUseForGrowth", "Energy used for growth", "MJ/day", energyUseForGrowth / numofDaysInYear, parens);
        GlobalVars.Instance.writeInformationToFiles("energyUseForMilk", "Energy used for milk production", "MJ/day", energyUseForMilk / numofDaysInYear, parens);
        GlobalVars.Instance.writeInformationToFiles("energyFromRemobilisation", "Energy supplied by remobilisation", "MJ/day", energyFromRemobilisation / numofDaysInYear, parens);
        GlobalVars.Instance.writeInformationToFiles("energyUseForMaintenance", "Energy used for maintenance", "MJ/day", energyUseForMaintenance / numofDaysInYear, parens);
        GlobalVars.Instance.writeInformationToFiles("maintenanceEnergyDeficit", "Maintenance energy deficit", "MJ/day", maintenanceEnergyDeficit / numofDaysInYear, parens);
        GlobalVars.Instance.writeInformationToFiles("growthEnergyDeficit", "Growth energy deficit", "MJ/day", growthEnergyDeficit / numofDaysInYear, parens);
        //GlobalVars.Instance.writeInformationToFiles("milkEnergyDeficit", "Deficit in energy required for milk production", "MJ", milkEnergyDeficit);

        GlobalVars.Instance.writeInformationToFiles("diet_ash", "Ash in diet", "kg", diet_ash, parens);
        GlobalVars.Instance.writeInformationToFiles("diet_fibre", "Fibre in diet", "kg", diet_fibre, parens);
        GlobalVars.Instance.writeInformationToFiles("diet_fat", "Fat in diet", "kg", diet_fat, parens);
        GlobalVars.Instance.writeInformationToFiles("diet_NFE", "NFE  in diet", "kg", diet_NFE, parens);
        GlobalVars.Instance.writeInformationToFiles("digestibilityDiet", "Diet DM digestibility", "kg/kg", digestibilityDiet, parens);

        GlobalVars.Instance.writeInformationToFiles("Cintake", "Intake of C", "kg", Cintake, parens);
        GlobalVars.Instance.writeInformationToFiles("milkC", "C in milk", "kg", milkC, parens);
        GlobalVars.Instance.writeInformationToFiles("growthC", "C in growth", "kg", growthC, parens);
        GlobalVars.Instance.writeInformationToFiles("urineCLiveStock", "C in urine", "kg", urineC, parens);
        GlobalVars.Instance.writeInformationToFiles("faecalCLiveStock", "C in faeces", "kg", faecalC, parens);
        GlobalVars.Instance.writeInformationToFiles("CH4C", "CH4-C emitted", "kg", CH4C, parens);
        GlobalVars.Instance.writeInformationToFiles("CO2C", "CO2-C emitted", "kg", CO2C, parens);
        //GlobalVars.Instance.writeInformationToFiles("energyLevel", "??", "??", energyLevel);
        GlobalVars.Instance.writeInformationToFiles("Nintake", "Intake of N", "kg", Nintake, parens);
        GlobalVars.Instance.writeInformationToFiles("milkN", "N in milk", "kg", milkN, parens);
        GlobalVars.Instance.writeInformationToFiles("growthN", "N in growth", "kg", growthN, parens);
        GlobalVars.Instance.writeInformationToFiles("mortalitiesN", "N in mortalities", "kg", mortalitiesN, parens);
        GlobalVars.Instance.writeInformationToFiles("urineN", "N in urine", "kg", urineN, parens);
        GlobalVars.Instance.writeInformationToFiles("faecalN", "N in faeces", "kg", faecalN, parens);

        GlobalVars.Instance.writeInformationToFiles("avgDailyProductionMilk", "Average daily milk production", "kg/day", avgProductionMilk, parens);
        double temp = avgProductionMilk * GlobalVars.avgNumberOfDays;
        GlobalVars.Instance.writeInformationToFiles("avgProductionMilk", "Average yearly milk production", "kg", temp, parens);
        if (avgProductionMilk > 0.0)
        {
            double percentMilkProtein = (milkN * 6.23 * 100.0) / (avgProductionMilk * GlobalVars.avgNumberOfDays);
            avgProductionECM = GlobalVars.Instance.GetECM(avgProductionMilk, (milkFat / 10.0), percentMilkProtein);
        }
        else
            avgProductionECM = 0;
        GlobalVars.Instance.writeInformationToFiles("avgProductionECM", "Average energy-corrected milk production", "kg/day", avgProductionECM * 365.0, parens);
        GlobalVars.Instance.writeInformationToFiles("avgDailyProductionECM", "Average daily energy-corrected milk production", "kg/day", avgProductionECM, parens);
        GlobalVars.Instance.writeInformationToFiles("avgProductionMeat", "Average weight change", "g/day", avgProductionMeat * 1000.0, parens);
        //GlobalVars.Instance.writeInformationToFiles("housedDuringGrazing", "??", "??", housedDuringGrazing);
        for (int i = 0; i < housingDetails.Count; i++)
            housingDetails[i].Write();
    } 

#if WIDE_AREA
    public void DoLivestockIPCC()
    {
        //values for annual average animal
        CH4C = CH4emissionFactor * 12 / 16;
        urineN = TAN_excreted;
        faecalN = OrganicN_excreted;
    }
#endif
    
}
