#define WIDE_AREA
using System;
using System.Collections.Generic;
using System.Xml;
public class housing
{
    //inputs
    int HousingType;
    string Name;
    //parameters
    
    double feedWasteFactor;
    double HousingRefTemp;
    double EFNH3ref;
    double EFNH3housingTier2;
    double beddingFactor;

    //other variables
    double beddingDM;
    double Cinput;
    double CO2C;
    double CtoStorage;
    double FibreCToStorage;
    double propTimeThisHouse;
    double meanTemp;
    double timeOnPasture;
    double beddingC;
    string parens;
    livestock theLivestock;
    double NTanInstore;
    double proportionDegradable; 
    double proportionNondegradable;
    double proportionTAN;
    double feedWasteC=0;
    double NWasteFeed = 0;
    double Nbedding = 0;
    double NtanInhouse = 0;
    double NorgInHouse = 0;
    double faecalN = 0;
    double NNH3housing = 0;
    double NfedInHousing = 0;
    double Ninput = 0;
    double Nout = 0;
    double NLost = 0;
    double Nbalance = 0;
    feedItem wasteFeed = null;

    double[] TANtoThisStore = new double[2];
    double[] organicNtoThisStore = new double[2];
  
    public double GetNNH3housing() { return NNH3housing; }
   
    List<feedItem> feedInHouse;
    List<GlobalVars.manurestoreRecord> manurestoreDetails = new List<GlobalVars.manurestoreRecord>();
    public List<GlobalVars.manurestoreRecord> GetmanurestoreDetails() { return manurestoreDetails; }
    
    public housing(int aHousingType, livestock aLivestock, int houseIndex, int zoneNr, string aParens)
    {
        parens = aParens;
        theLivestock = aLivestock;
        timeOnPasture = aLivestock.GettimeOnPasture();
        HousingType = aHousingType;
        feedInHouse = new List<feedItem>();
        FileInformation paramFile = new FileInformation(GlobalVars.Instance.getParamFilePath());

        if (HousingType != 0)
        {
            string basePath = "AgroecologicalZone(" + zoneNr.ToString() + ").Housing";
            paramFile.setPath(basePath);
            int minHouse = 99, maxHouse = 0;
            paramFile.getSectionNumber(ref minHouse, ref maxHouse);
            int tmpHousingType = -1;

            bool found = false;
            int num = 0;
            for (int i = minHouse; i <= maxHouse; i++)
            {
                if (paramFile.doesIDExist(i))
                {
                    paramFile.Identity.Add(i);
                    tmpHousingType = paramFile.getItemInt("HousingType");
                    if (tmpHousingType == HousingType)
                    {
                        found = true;
                        num = i;
                        basePath += "(" + num.ToString() + ")";
                        break;
                    }
                    paramFile.Identity.RemoveAt(paramFile.Identity.Count - 1);
                }
            }
            if (found == false)
            {

                string messageString = aLivestock.Getname() + " could not link housing and manure storage";
                GlobalVars.Instance.Error(messageString);
            }
            Name = paramFile.getItemString("Name");
            paramFile.PathNames.Add("feedWasteFactor");
            paramFile.Identity.Add(-1);
            feedWasteFactor = paramFile.getItemDouble("Value");
            paramFile.PathNames[paramFile.PathNames.Count - 1] = "beddingFactor";
            beddingFactor = paramFile.getItemDouble("Value");
            paramFile.PathNames[paramFile.PathNames.Count - 1] = "HousingRefTemp";
            HousingRefTemp = paramFile.getItemDouble("Value");
            paramFile.PathNames[paramFile.PathNames.Count - 1] = "EFNH3housingRef";
            EFNH3ref = paramFile.getItemDouble("Value");
            if (GlobalVars.Instance.getcurrentInventorySystem() == 2)
            {
                paramFile.PathNames[paramFile.PathNames.Count - 1] = "meanTemp";
                meanTemp = paramFile.getItemDouble("Value");
            }
            EFNH3housingTier2 = paramFile.getItemDouble("Value", basePath + ".EFNH3housingTier2(-1)");
            paramFile.PathNames[paramFile.PathNames.Count - 1] = "ProportionDegradable";
            proportionDegradable = paramFile.getItemDouble("Value");
            paramFile.PathNames[paramFile.PathNames.Count - 1] = "ProportionNondegradable";
            proportionNondegradable = paramFile.getItemDouble("Value");
            paramFile.PathNames[paramFile.PathNames.Count - 1] = "ProportionTAN";
            proportionTAN = paramFile.getItemDouble("Value");
            GlobalVars.manurestoreRecord amanurestoreRecord = new GlobalVars.manurestoreRecord();
            int numManureStores = theLivestock.GethousingDetails()[houseIndex].GetManureRecipient().Count;
            for (int j = 0; j < numManureStores; j++)
            {

                manureStore aStore = new manureStore(theLivestock.GethousingDetails()[houseIndex].GetManureRecipient()[j].GetStorageType(),
                    theLivestock.GetspeciesGroup(), zoneNr, parens + "_manureStore" + (j + 1).ToString());
                amanurestoreRecord.SettheStore(aStore);
                manure amanureToStore = new manure();
                amanureToStore.Setname(aStore.Getname());
                amanurestoreRecord.SetmanureToStorage(amanureToStore);
                manurestoreDetails.Add(amanurestoreRecord);

            }
        }
        for (int i = 0; i < aLivestock.GetfeedRation().Count; i++)
        {
            feedItem aFeedItem = aLivestock.GetfeedRation()[i];
            if (!aFeedItem.GetisGrazed())
                feedInHouse.Add(aFeedItem);
            else feedInHouse.Add(null);
            //        feedInHouse = aLivestock.GetfeedRation();
        }
    }
    

    public void DoHousing()
    {
#if WIDE_AREA
       
#endif
        DoCarbon();
        DoNitrogen();
        for (int i = 0; i < manurestoreDetails.Count; i++)
        {
            GlobalVars.manurestoreRecord amanurestoreRecord = manurestoreDetails[i];
            amanurestoreRecord.GettheStore().Addmanure(amanurestoreRecord.GetmanureToStorage(), amanurestoreRecord.GetpropYearGrazing());
        }
    }

    void DoCarbon()
    {
        double nonDegC;
        double DegC;
        double manureBo = theLivestock.GetBo();
        FibreCToStorage = 0;
        CtoStorage = GlobalVars.Instance.getalpha() * theLivestock.GetVS_excretion() * theLivestock.GetAvgNumberOfAnimal() *365.25;
        Cinput = CtoStorage;
        GlobalVars.manurestoreRecord amanurestoreRecord;
            switch (manurestoreDetails.Count)
            {
                case 0:
                    string messageString=("Error - No manure storage destinations");
                    GlobalVars.Instance.Error(messageString);
                    break;
                case 1:
                    //proportionNondegradable is ignored - only one store
                    amanurestoreRecord = manurestoreDetails[0];
                    nonDegC = FibreCToStorage;
                    DegC = CtoStorage - FibreCToStorage;
                    amanurestoreRecord.GetmanureToStorage().SetdegC(DegC);
                    amanurestoreRecord.GetmanureToStorage().SetnonDegC(nonDegC);
                    amanurestoreRecord.GetmanureToStorage().SetBo(manureBo);
                    amanurestoreRecord.SetpropYearGrazing(theLivestock.GettimeOnPasture());
                    break;
                case 2:
                    bool gotSolid = false;
                    bool gotLiquid = false;
                    for (int i = 0; i < manurestoreDetails.Count; i++)
                    {
                        amanurestoreRecord = manurestoreDetails[i];
                        if ((proportionNondegradable == 0) && (proportionDegradable == 0))
                        {
                            messageString = ("Error - proportionNondegradable & proportionDegradable are both zero, in housing " + Name);
                            GlobalVars.Instance.Error(messageString);
                            break;
                        }
                        if (amanurestoreRecord.GettheStore().GetStoresSolid())
                        {
                            nonDegC = proportionNondegradable * FibreCToStorage;
                            DegC = proportionDegradable * (CtoStorage - FibreCToStorage);
                            if (gotSolid)
                            {
                                messageString = ("Error - two manure storage destinations receive solid manure");
                                GlobalVars.Instance.Error(messageString);
                                break;
                            }
                            else
                                gotSolid = true;
                        }
                        else
                        {
                            nonDegC = (1 - proportionNondegradable) * FibreCToStorage;
                            DegC = (1 - proportionDegradable) * (CtoStorage - FibreCToStorage);
                            if (gotLiquid)
                            {
                                messageString = ("Error - two manure storage destinations receive liquid manure");
                                GlobalVars.Instance.Error(messageString);
                                break;
                            }
                            else
                                gotLiquid = true;
                        }
                        if (DegC < 0)
                        {
                            messageString = "Error - degradable carbon is less than zero for " + Name;
                            GlobalVars.Instance.Error(messageString);
                        }
                        amanurestoreRecord.GetmanureToStorage().SetdegC(DegC);
                        amanurestoreRecord.GetmanureToStorage().SetnonDegC(nonDegC);
                        amanurestoreRecord.GetmanureToStorage().SetBo(manureBo);
                        amanurestoreRecord.SetpropYearGrazing(theLivestock.GettimeOnPasture());
                        //send C to manure store
                    }
                    break;
                default:
                    messageString=("Error - too many manure storage destinations");
                    GlobalVars.Instance.Error(messageString);
                    break;
            }
#if WIDE_AREA

#endif
    }
    public void DoNitrogen()
    {
#if WIDE_AREA
        NorgInHouse = theLivestock.GetfaecalN() * theLivestock.GetAvgNumberOfAnimal();
        NtanInhouse = theLivestock.GeturineN() * theLivestock.GetAvgNumberOfAnimal();
        NNH3housing = EFNH3housingTier2 * NtanInhouse;
        NTanInstore = NtanInhouse - NNH3housing;
        Ninput = NorgInHouse + NtanInhouse;

#endif
        GlobalVars.manurestoreRecord amanurestoreRecord;
        switch (manurestoreDetails.Count)
        {
            case 1:
                amanurestoreRecord = manurestoreDetails[0];
                TANtoThisStore[0] = NTanInstore;
                organicNtoThisStore[0] = NorgInHouse;
                amanurestoreRecord.GetmanureToStorage().SetTAN(TANtoThisStore[0]);
                amanurestoreRecord.GetmanureToStorage().SetorganicN(organicNtoThisStore[0]);
                break;
            case 2:
                for (int i = 0; i < manurestoreDetails.Count; i++)
                {
                    amanurestoreRecord = manurestoreDetails[i];
                    if (amanurestoreRecord.GettheStore().GetStoresSolid())
                    {
                        TANtoThisStore[i] = proportionTAN * NTanInstore;
                        organicNtoThisStore[i] = proportionDegradable * NorgInHouse;
                    }
                    else
                    {
                        TANtoThisStore[i] = (1 - proportionTAN) * NTanInstore;
                        organicNtoThisStore[i] = (1 - proportionDegradable) * NorgInHouse;
                    }
                    amanurestoreRecord.GetmanureToStorage().SetTAN(TANtoThisStore[i]);
                    amanurestoreRecord.GetmanureToStorage().SetorganicN(organicNtoThisStore[i]);
                }
                break;
            default:
                string messageString = ("Error - too many manure storage destinations");

                GlobalVars.Instance.Error(messageString);
                break;
        }
#if WIDE_AREA

#endif
    }
    public void Write()
    {
        theLivestock.Write();
        for (int i = 0; i < feedInHouse.Count; i++)
        {

            if (feedInHouse[i] != null)
            feedInHouse[i].Write(parens+"_");
        }

        GlobalVars.Instance.writeInformationToFiles("Name", "Name", "-", Name, parens);
        GlobalVars.Instance.writeInformationToFiles("HousingType", "Type of housing", "-", HousingType, parens);

        GlobalVars.Instance.writeInformationToFiles("Cinput", "Total C input to housing", "kg", Cinput, parens);
        GlobalVars.Instance.writeInformationToFiles("CO2C", "CO2-C emitted from housing", "kg", CO2C, parens);
        GlobalVars.Instance.writeInformationToFiles("CtoStorage", "C sent to manure storage", "kg", CtoStorage, parens);

        GlobalVars.Instance.writeInformationToFiles("Ninput", "Total N entering the housing", "kg", Ninput, parens);
        GlobalVars.Instance.writeInformationToFiles("NtanInhouse", "TAN entering the housing", "kg", NtanInhouse, parens);
        GlobalVars.Instance.writeInformationToFiles("NorgInHouse", "Organic N entering the housing", "kg", NorgInHouse, parens);
        GlobalVars.Instance.writeInformationToFiles("faecalNInHouseing", "Faecal N deposited", "kg", faecalN, parens);
        GlobalVars.Instance.writeInformationToFiles("Nbedding", "N added in bedding", "kg", Nbedding, parens);
        GlobalVars.Instance.writeInformationToFiles("NWasteFeed", "N added in waste feed", "kg", NWasteFeed, parens);
        GlobalVars.Instance.writeInformationToFiles("NNH3housing", "NH3-N emitted from housing", "kg", NNH3housing, parens);
        GlobalVars.Instance.writeInformationToFiles("NLost", "Total N lost", "kg", NLost, parens);
        GlobalVars.Instance.writeInformationToFiles("Nout", "N leaving housing", "kg", Nout, parens);
        GlobalVars.Instance.writeInformationToFiles("Nbalance", "N budget check", "kg", Nbalance, parens);
    } 
}
