#define WIDE_AREA
using System;
using System.Collections.Generic;
using System.Xml;
class farmBalanceClass
{
    double liveFeedImportN = 0;
    //Jonas put these under Herd
    //import of N in animal feed
    double livestockNintake = 0;

    //N in grazed feed
    double liveGrazedN = 0;
    //input of excretal N to housing
    double liveToHousingN = 0;
    //deposition of N by grazing livestock
    double liveToFieldN = 0;

    //Jonas put these under Housing
    //excretal N deposited in housing
    double houseInFromAnimalsN = 0;
    //Gaseous loss of N from housing
    double houseLossN = 0;
    //N in excreta from housing to storage (minus gaseous N losses)
    double houseExcretaToStorageN = 0;

    //N input to biogas plant as supplementary feedstock
    double biogasSupplN = 0;
    //C input to biogas plant as supplementary feedstock
    double biogasSupplC = 0;

    //Jonas put these under ManureStorage
    //N input to storage from excreta deposited in housing (minus NH3 emission)
    double storageFromHouseN = 0;
    //N input to storage in bedding
    double storageFromBeddingN = 0;
    //N input to storage in wasted feed
    double storageFromFeedWasteN = 0;
    //N lost in gaseous emissions from storage
    double storageGaseousLossN = 0;
    //N lost in runoff from storage
    double storageRunoffN = 0;

    //N in imported manure
    double manureImportN = 0;
    //N in exported manure
    double manureExportN = 0;
    double manureNexStorage = 0;


    //Jonas put these in Fields
    //N in manure applied to fields
    double manureToFieldN = 0;
    //N in gaseous emissions in the field
    double fieldGaseousLossN = 0;
    //N in nitrate leaching
    double fieldNitrateLeachedN = 0;
    //N removed by grazing animals
    double grazedN = 0;
    double Nharvested = 0;
    //Change in soil N storage
    double changeSoilN = 0;
    //N in plant material harvested for consumption by livestocl
    double fieldharvestedConsumedN = 0;
    double entericCH4CO2Eq = 0.0;
    double manureCH4CO2Eq = 0;
    double manureN2OCO2Eq = 0;
    double fieldN2OCO2Eq = 0;
    double fieldCH4CO2Eq = 0;
    double directGHGEmissionCO2Eq = 0;
    double soilCO2Eq = 0;
    double housingNH3CO2Eq = 0;
    double manurestoreNH3CO2Eq = 0;
    double fieldmanureNH3CO2Eq = 0;
    double fieldfertNH3CO2Eq =0;
    double leachedNCO2Eq = 0;
    double indirectGHGCO2Eq = 0;
    //!carbon fixation by crops (kg)
    double carbonFromPlants = 0;
    //! carbon imported in livestock manure (kg)
    double Cmanimp = 0;
    //!carbon imported in animal feed (kg)
    double CPlantProductImported = 0;
    //!carbon in bedding (kg)
    double CbeddingReq = 0;
    //!Carbon exported in milk (kg)
    double Cmilk = 0;
    //!Carbon exported in meat (kg)
    double Cmeat = 0;
    //!Carbon exported in dead animals (kg)
    double Cmortalities = 0;
    //Carbon exported in manure  (kg)
    double Cmanexp = 0;
    //!Carbon ín sold crop products (kg)
    double Csold = 0;
    //!total carbon loss to environment
    double CLost = 0;
    //!Carbon lost as methane from livestock
    double livestockCH4C = 0;
    //!Carbon lost as carbon dioxide from livestock
    double livestockCO2C = 0;
    double livstockCLoss = 0;
    //!Carbon lost as carbon dioxide from urea hydrolysis
    double housingCLoss = 0;
    //!Carbon lost as methane from manure storage
    double manurestoreCH4C = 0;
    //!Carbon lost as carbon dioxide from manure storage
    double manurestoreCO2C = 0;
    //!change in soil C
    double CDeltaSoil = 0;
    //!emissions of CO2 from soil
    double soilCO2_CEmission = 0;
    //!emissions of CH4 from excreta deposited during grazing
    double soilCH4_CEmission = 0;
    //!C lost from stored plant products
    double processStorageCloss = 0;
    //!C in organic matter leached from soil
    double soilCleached = 0;
    //!CO-C from burnt crop residues
    double burntResidueCOC = 0;
    //!Black C from burnt crop residues
    double burntResidueBlackC = 0;
    //!CO2-C from burnt crop residues
    double burntResidueCO2C = 0;
    //!C in CH4 from biogas reactor
    double biogasCH4C = 0;
    //!C in CO2 from biogas reactor
    double biogasCO2C = 0;
    //!C in manure organic matter lost in runoff from manure storage
    double manurestoreRunoffC = 0;
    //!C in crop residues remaining on the fields
    double residueCremaining = 0;
    //!continuity check for C
    double Cbalance=0;

    //! N input via N fixation in crops (kg)
    double nFix = 0;
    //!N lost from stored plant products
    double processStorageNloss = 0;
    //! N input via atmospheric deposition
    double Natm = 0;
    //!N input in N fertilisers (kg)
    double nFert = 0;//1.134 where are the import of fertiliser??//1.134
    //!N input in bedding (kg)
    double Nbedding = 0;
    //! carbon imported in livestock manure (kg)
    double Nmanimp = 0;
    //! N imported in animal feed (kg)
    double NPlantProductImported = 0;
    //! N sold in crop products
    double Nsold = 0;
    //!N exported in milk
    double Nmilk = 0;
    //! N exported in animal growth
    double NGrowth = 0;
    //! N exported in animal mortalities
    double Nmortalities = 0;
    //!N exported in animal manure
    double Nmanexp = 0;
    //total N export
    double NExport = 0;
    // N losses and change in N stored in soil
    double NDeltaSoil = 0;
    //!total N lost
    double NLost = 0;
    //!N lost as NH3 from housing
    double housingNH3Loss = 0;
    //!N2O-N emission from stored manure
    double manureN2Emission = 0;
    //!N2-N emission from stored manure
    double manureN2OEmission = 0;
    //!NH3-N emission from stored manure
    double manureNH3Emission = 0;
    double manurestoreNLoss = 0;
    double fieldNLoss = 0;
    //N2-N emission from soil
    double fieldN2Emission = 0;
    //N2O-N emission from soil
    double fieldN2OEmission = 0;
    //NH3-N-N emission from fertiliser
    double fertNH3NEmission = 0;
    //NH3-N emission from field-applied manure
    double fieldmanureNH3Emission = 0;
    //NH3-N emission from urine deposited in the field
    double fieldUrineNH3Emission = 0;
    //NO3-N leaching from soil
    double Nleaching = 0;
    //N excreted in housing
    double NexcretedHousing = 0;
    //N excreted during grazing
    double NexcretedField = 0;
    //N fed in housing
    double NfedInHousing = 0;
    //N fed in at pasture
    double NfedAtPasture = 0;
    //N from grazed feed
    double NinGrazedFeed = 0;
    //DM from grazed
    double DMinGrazedFeed = 0;
    //!Change in mineral N in soil
    double changeInMinN = 0;
    //!nitrous oxide emission from fertiliser
    double fertiliserN2OEmission = 0;
   //!nitrous oxide emissions from crop residues
    double cropResidueN2O = 0;
    //!leaching of organic nitrogen
    double organicNLeached = 0;
    //!N2O-N in gases from burnt crop residues
    double burntResidueN2ON = 0;
    //!NH3N in gases from burnt crop residues
    double burntResidueNH3N = 0;
    //!NOX in gases from burnt crop residues
    double burntResidueNOxN = 0;
    //!N in other gases from burnt crop residues
    double burntResidueOtherN = 0;
    //!runoff from manure storage
    double runoffN = 0;
    //!residual soil mineral N at end of crop sequence
    double residualSoilMineralN = 0;
    //!total losses from pocess/storage of crop products, housing and manure storage
    double totalHouseStoreNloss = 0;
    //! total losses from fields
    double totalFieldNlosses = 0;
    //!change in total N storage (organic and inorganic)
    double changeAllSoilNstored = 0;
    //!N in crop residues remaining on the fields
    double residueNremaining = 0;
    //!farm N surplus (kg/ha/yr)
    double totalFarmNSurplus = 0;
    //!continuity check
    double Nbalance = 0;

    //farm milk production
    double farmMilkProduction = 0;
    //farm meat production
    double farmMeatProduction = 0;
    //average milk production per head
    double avgProductionMilkPerHead = 0;
    //total DM used by livestock, kg
    double farmLivestockDM = 0;
    //concentrate DM used
    double farmConcentrateDM = 0;
    //concentrate energy used
    double farmConcentrateEnergy = 0;
    //grazed DM used
    double farmGrazedDM = 0;
    //farm utilised grazable DM
    double farmUnutilisedGrazableDM = 0;
    //!farm area (ha)
    double agriculturalArea = 0;
    //!DM production on farm (tonnes/yr)
    double totalDMproduction = 0;
    //!Utilised DM production on farm (tonnes/yr)
    double utilisedDMproduction = 0;
    double FarmHarvestDM=0;
    string parens;
    double precip = 0;
    double evap = 0;
    double transpire = 0;
    double irrig = 0;
    double drainage = 0;
    double MaxPlantAvailWater = 0;
    double numDairy = 0;

    public farmBalanceClass(string aParens)
    {
        parens = aParens;
    }

    public double GetAgriculturalArea(List<CropSequenceClass> therotationList)
    {
        double area = 0;
        for (int i = 0; i < therotationList.Count; i++)
        {
            area += therotationList[i].getArea();
        }
        return area;
    }

    public void DoFarmBalances(int farmType,double farmArea,List<CropSequenceClass> rotationList, List<livestock> listOfLivestock, List<housing> listOfHousing,
        List<manureStore> listOfManurestores)
    {
        //Farm balances
        //C balance
        //C inputs
#if WIDE_AREA

#endif
#if WIDE_AREA
        for (int i = 0; i < listOfLivestock.Count; i++)
        {
            livestock anAnimalCategory = listOfLivestock[i];
            livestockCH4C += anAnimalCategory.getCH4C() * anAnimalCategory.GetAvgNumberOfAnimal();
        }
        for (int i = 0; i < listOfHousing.Count; i++)
        {
            housing ahouse = listOfHousing[i];
            housingNH3Loss += ahouse.GetNNH3housing();
        }
        manureToFieldN = 0;
        for (int i = 0; i < listOfManurestores.Count; i++)
        {
            manureStore amanurestore2 = listOfManurestores[i];
            manurestoreCH4C += amanurestore2.GetCCH4ST();
            manureN2OEmission += amanurestore2.GettotalNstoreN20();
            manureNH3Emission += amanurestore2.GettotalNstoreNH3();
            manureToFieldN += amanurestore2.GetManureOrganicN() + amanurestore2.GetManureTAN();
        }

        double Ninput = 0;
        for (int i = 0; i < listOfLivestock.Count; i++)
        {
            Ninput += (listOfLivestock[i].GetTAN_excreted() + listOfLivestock[i].GetOrganicN_excreted()) * listOfLivestock[i].GetAvgNumberOfAnimal();
        }
        for (int i = 0; i < rotationList.Count; i++)
        {
            nFix += rotationList[i].getNFix() ;//1.132
        }
        // N deposition from atmosphere
        for (int i = 0; i < rotationList.Count; i++)
        {
            Natm += rotationList[i].getNAtm() ;//1.133
        }
        //Fertiliser N
        for (int i = 0; i < rotationList.Count; i++)
        {
            nFert += rotationList[i].getFertiliserNapplied();
        }
        for (int i = 0; i < rotationList.Count; i++)
        {
            fieldN2OEmission += rotationList[i].GetN2ONemission();
            soilCH4_CEmission += rotationList[i].getGrazingMethaneC();
            fertNH3NEmission += rotationList[i].GetFertNH3NEmission();
            fieldmanureNH3Emission += rotationList[i].GetManureNH3NEmission();
            Nleaching += rotationList[i].GettheNitrateLeaching();
            agriculturalArea += rotationList[i].getArea();
        }
              
#endif
        agriculturalArea = GetAgriculturalArea(rotationList);
        fieldN2OEmission /= agriculturalArea;
        soilCH4_CEmission /= agriculturalArea;
        fertNH3NEmission /= agriculturalArea;
        fieldmanureNH3Emission /= agriculturalArea;
        Nleaching /= agriculturalArea;
        manureToFieldN /= agriculturalArea;

        livestockCH4C /= agriculturalArea;
        manurestoreCH4C /= agriculturalArea;
        manureN2OEmission /= agriculturalArea;
        housingNH3Loss /= agriculturalArea;
        manureNH3Emission /= agriculturalArea;
 
        //do GHG budget
        entericCH4CO2Eq = livestockCH4C * GlobalVars.Instance.GetCO2EqCH4();
        manureCH4CO2Eq = manurestoreCH4C * GlobalVars.Instance.GetCO2EqCH4();
        manureN2OCO2Eq = manureN2OEmission * GlobalVars.Instance.GetCO2EqN2O();
        fieldN2OCO2Eq = fieldN2OEmission * GlobalVars.Instance.GetCO2EqN2O();
        fieldCH4CO2Eq = soilCH4_CEmission * GlobalVars.Instance.GetCO2EqCH4();
        soilCO2Eq = -1 * CDeltaSoil * GlobalVars.Instance.GetCO2EqsoilC();
        directGHGEmissionCO2Eq = entericCH4CO2Eq + manureCH4CO2Eq + manureN2OCO2Eq + fieldN2OCO2Eq + soilCO2Eq + fieldCH4CO2Eq;

        housingNH3CO2Eq = housingNH3Loss * GlobalVars.Instance.GetIndirectNH3N2OFactor() * GlobalVars.Instance.GetCO2EqN2O();
        manurestoreNH3CO2Eq = manureNH3Emission * GlobalVars.Instance.GetIndirectNH3N2OFactor() * GlobalVars.Instance.GetCO2EqN2O();
        fieldmanureNH3CO2Eq = fieldmanureNH3Emission * GlobalVars.Instance.GetIndirectNH3N2OFactor() * GlobalVars.Instance.GetCO2EqN2O();
        fieldfertNH3CO2Eq = fertNH3NEmission * GlobalVars.Instance.GetIndirectNH3N2OFactor() * GlobalVars.Instance.GetCO2EqN2O();
        leachedNCO2Eq = Nleaching * GlobalVars.Instance.GetIndirectNO3N2OFactor() * GlobalVars.Instance.GetCO2EqN2O();
        indirectGHGCO2Eq = housingNH3CO2Eq + manurestoreNH3CO2Eq + fieldmanureNH3CO2Eq + fieldfertNH3CO2Eq + leachedNCO2Eq;

        double croppedArea = 0;
        for (int i = 0; i < rotationList.Count; i++)
        {
            double Napplied = rotationList[i].GetManureNapplied() + rotationList[i].getFertiliserNapplied();
            if (Napplied>0)
                croppedArea += rotationList[i].getArea();
        }

        //GHG emissions from sources other than fields
        double OtherGHGemissions = entericCH4CO2Eq+manureCH4CO2Eq+manureN2OCO2Eq+housingNH3CO2Eq+manurestoreNH3CO2Eq;

        for (int i = 0; i < rotationList.Count; i++)
        {
            rotationList[i].WriteGHGdata(croppedArea,OtherGHGemissions);
        }

        double totalGHGCO2Eq = directGHGEmissionCO2Eq + indirectGHGCO2Eq;
     
        double fertNapplied = 0;
        for (int i = 0; i < rotationList.Count; i++)
        {
            fertNapplied += rotationList[i].getFertiliserNapplied()/agriculturalArea;
        }
        //Manure N
        double manNapplied = 0;
        for (int i = 0; i < rotationList.Count; i++)
        {
            manNapplied += rotationList[i].GetManureNapplied()/agriculturalArea;
        }

#if WIDE_AREA
        VMP3.Instance.WriteFarm(farmType.ToString() + "\t" + farmArea.ToString() + "\t" + entericCH4CO2Eq.ToString("0.") + "\t" + manureCH4CO2Eq.ToString("0.") + "\t" + manureN2OCO2Eq.ToString("0.") + "\t" + 
                housingNH3CO2Eq.ToString("0.") + "\t" + manurestoreNH3CO2Eq.ToString("0.") + "\t" + fieldCH4CO2Eq.ToString("0.") + "\t"+ fieldN2OCO2Eq.ToString() + "\t" +
                fieldCH4CO2Eq.ToString("0.") + "\t" + fieldfertNH3CO2Eq.ToString("0.") + "\t" + fieldmanureNH3CO2Eq.ToString("0.") + "\t" + leachedNCO2Eq.ToString("0.") + "\t" +
                totalGHGCO2Eq.ToString("0.") + "\t" +
                housingNH3Loss.ToString("0.") + "\t" + manureNH3Emission.ToString("0.") + "\t" + Nleaching.ToString() + 
                "\t" +fertNapplied.ToString() + "\t" + manNapplied.ToString()+"\t" + manureToFieldN.ToString());

        VMP3.Instance.WriteLineFarm("");

#endif
    }
   
}