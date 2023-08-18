#define WIDE_AREA
using System;
using System.Collections.Generic;
using System.Xml;
public class manureStore
{
    string path;
    //inputs
    //parameters
    string name;
    int ManureStorageID;
    int speciesGroup;
    double b1;
    double lnArr;
    double StorageRefTemp;
    double meanTemp;
    double MCF; //needs to be AEZ specific
    //double Bo;//needs to be AEZ specific
    double EFStoreNH3;
    double EFStoreN20;
    double EFStoreN2;
    double Lambda;
    double propGasCapture;

    //other variables
    string parens;
    int identity;
    manure theManure;
    housing theHousing;

    double tstore=0;
    double CdegradationRate;
    double Cinput=0;
    double CCH4ST=0;
    double CCO2ST = 0;
    double CCH4GR = 0;
    double Cdegradation = 0;
    double NTanInstore;
    double NorgInstore;
    double NDegOrgOut;
    double ohmOrg;
    double ohmTAN;
    double NRunOffOrg;
    double CRunOffOrg;
    double newNHUM;
    double NRunoffHum;
    double NorgOutStore;
    double NTanOutstore;
    double NTANLost;
    double NrunoffTan;
    double totalNstoreNH3;
    double totalNstoreN2;
    double totalNstoreN20;
    double Ninput = 0;
    double Nout = 0;
    double NLost = 0;
    double Nbalance = 0;
    double biogasCH4C = 0;
    double biogasCO2C = 0;
    double supplementaryN = 0;
    double supplementaryC = 0;

    public List<feedItem> supplementaryFeedstock;
    public double GetCCH4ST() { return CCH4ST; }  
    public double GettotalNstoreNH3() { return totalNstoreNH3; }   
    public double GettotalNstoreN20() { return totalNstoreN20; }
    public double GetManureOrganicN() { return theManure.GetorganicN(); }
    public double GetManureTAN() { return theManure.GetTAN(); }
 
    public double GetManureC()
    {
        double retVal = 0;
        retVal = theManure.GetdegC() + theManure.GethumicC() + theManure.GetnonDegC();
        return retVal;
    }
    public double GetManureN()
    {
        double retVal = 0;
        retVal = theManure.GetTAN()+ theManure.GetorganicN();
        return retVal;
    }    
    public manureStore(string aPath, int id, int zoneNr, string aParens)
    {
        supplementaryFeedstock = new List<feedItem>();
        string parens = aParens;
        FileInformation manureStoreFile = new FileInformation(GlobalVars.Instance.getFarmFilePath());
        identity = id;
        path=aPath+'('+id.ToString()+')';

        manureStoreFile.setPath(path);
        name=manureStoreFile.getItemString("NameOfStorage");
        ManureStorageID = manureStoreFile.getItemInt("StorageType");
        speciesGroup = manureStoreFile.getItemInt("SpeciesGroup");
        getParameters(zoneNr);
    }
    public manureStore(int manureStorageType, int livestockSpeciesGroup, int zoneNr, string aParens)
    {
        supplementaryFeedstock = new List<feedItem>();
        ManureStorageID = manureStorageType;
        speciesGroup = livestockSpeciesGroup;
        getParameters(zoneNr);
        parens = aParens;
    }
    public void getParameters(int zoneNr)
    {
        FileInformation manureParamFile = new FileInformation(GlobalVars.Instance.getParamFilePath());
        manureParamFile.setPath("AgroecologicalZone("+zoneNr.ToString()+").ManureStorage");
        int maxManure = 0, minManure = 99;
        manureParamFile.getSectionNumber(ref minManure, ref maxManure);

        bool found = false;
        int num=0;
        //GlobalVars.Instance.log("ind " + " Req " + " test" + " sg ");
        for (int i = minManure; i <= maxManure; i++)
        {
            if (manureParamFile.doesIDExist(i))
            {
                manureParamFile.Identity.Add(i);
                int tmpStorageType = manureParamFile.getItemInt("StorageType");
                int tmpSpeciesGroup = manureParamFile.getItemInt("SpeciesGroup");
                name = manureParamFile.getItemString("Name");
                //  GlobalVars.Instance.log(i.ToString()+ " " + storageType.ToString()+ " "+ tmpStorageType.ToString()+ " "+ speciesGroup.ToString()+ " "+ tmpSpeciesGroup.ToString());
                if (ManureStorageID == tmpStorageType & speciesGroup == tmpSpeciesGroup)
                {
                    found = true;
                    num = i;
                    break;
                }
                manureParamFile.Identity.RemoveAt(manureParamFile.Identity.Count - 1);
            }
        }
        if (found == false)
        {
            string messageString = ("could not match StorageType and SpeciesGroup at ManureStore. Was trying to find StorageType" + ManureStorageID.ToString() + " and speciesGroup " + speciesGroup.ToString());
          GlobalVars.Instance.Error(messageString);
        }
        string RecipientPath = "AgroecologicalZone("+zoneNr.ToString()+").ManureStorage" + '(' + num.ToString() + ").StoresSolid(-1)";
        bool StoresSolid;
        string tempString = manureParamFile.getItemString("Value",RecipientPath);
        if (tempString == "true")
            StoresSolid = true;
        else
            StoresSolid = false;
        manureParamFile.PathNames[manureParamFile.PathNames.Count - 1] = "b1";
        b1 = manureParamFile.getItemDouble("Value");
//        manureParamFile.getItemDouble("Value");
        manureParamFile.PathNames[manureParamFile.PathNames.Count - 1] = "lnArr";
        lnArr = manureParamFile.getItemDouble("Value");
        manureParamFile.PathNames[manureParamFile.PathNames.Count - 1] = "ohmOrg";
        ohmOrg = manureParamFile.getItemDouble("Value");
        manureParamFile.PathNames[manureParamFile.PathNames.Count - 1] = "ohmTAN";
        ohmTAN = manureParamFile.getItemDouble("Value");
        manureParamFile.PathNames[manureParamFile.PathNames.Count - 1] = "MCF";
        MCF = manureParamFile.getItemDouble("Value");
        if (GlobalVars.Instance.getcurrentInventorySystem() == 2)
        {
            manureParamFile.PathNames[manureParamFile.PathNames.Count - 1] = "meanTemp";
            meanTemp = manureParamFile.getItemDouble("Value");
        }
        //    double propGasCapture;
        manureParamFile.PathNames[manureParamFile.PathNames.Count - 1] = "PropGasCapture";
        propGasCapture = manureParamFile.getItemDouble("Value");

        if ((GlobalVars.Instance.getcurrentInventorySystem() == 1)||(GlobalVars.Instance.getcurrentInventorySystem() == 2))
        {
            manureParamFile.PathNames[manureParamFile.PathNames.Count - 1] = "EFNH3storageIPCC";
            EFStoreNH3 = manureParamFile.getItemDouble("Value");
            manureParamFile.PathNames[manureParamFile.PathNames.Count - 1] = "EFN2OstorageIPCC";
            EFStoreN20 = manureParamFile.getItemDouble("Value");
        }
        else
        {
            manureParamFile.PathNames[manureParamFile.PathNames.Count - 1] = "EFNH3storageRef";
            EFStoreNH3 = manureParamFile.getItemDouble("Value");
            manureParamFile.PathNames[manureParamFile.PathNames.Count - 1] = "EFN2OstorageRef";
            EFStoreN20 = manureParamFile.getItemDouble("Value");
            manureParamFile.PathNames[manureParamFile.PathNames.Count - 1] = "StorageRefTemp";
            StorageRefTemp = manureParamFile.getItemDouble("Value");
        }
        manureParamFile.PathNames[manureParamFile.PathNames.Count - 1] = "lambda_m";
        Lambda = manureParamFile.getItemDouble("Value");
        string aPath = "AgroecologicalZone(" + zoneNr.ToString() + ").ManureStorage(" + num.ToString() + ").SupplementaryFeedstocks(-1).Feedstock";
        manureParamFile.setPath(aPath);
        int minsuppFeed = 99, maxsuppFeed = 0;
        manureParamFile.getSectionNumber(ref minsuppFeed, ref maxsuppFeed);
        for (int k = minsuppFeed; k <= maxsuppFeed; k++)
        {
            manureParamFile.Identity.Add(k);
            feedItem aFeedstock = new feedItem();
            aFeedstock.Setamount(manureParamFile.getItemDouble("Amount"));
            aFeedstock.GetStandardFeedItem(manureParamFile.getItemInt("FeedCode"));
            supplementaryFeedstock.Add(aFeedstock);
            manureParamFile.Identity.RemoveAt(manureParamFile.Identity.Count - 1);
        }

        theManure = new manure();
        theManure.SetisSolid(StoresSolid);
        //indicate the type of manure
        theManure.SetspeciesGroup(speciesGroup);
        FileInformation file = new FileInformation(GlobalVars.Instance.getfertManFilePath());
        file.setPath("AgroecologicalZone("+GlobalVars.Instance.GetZone().ToString()+").manure");
        int min = 99; int max = 0;
        file.getSectionNumber(ref min, ref max);
  
        bool gotit = false;
        int j = min;
        while ((j <= max) && (gotit == false))
        {
            if(file.doesIDExist(j))
            {
        
                file.Identity.Add(j);
                int StoredTypeFile = file.getItemInt("ManureType");
                int SpeciesGroupFile = file.getItemInt("SpeciesGroup");
                string manureName = file.getItemString("Name");
                if (StoredTypeFile == ManureStorageID && SpeciesGroupFile == speciesGroup)
                {
                    //itemNr = j;
                    theManure.SetmanureType(ManureStorageID);
                    theManure.Setname(manureName);
                    gotit = true;
                }
                j++;
                file.Identity.RemoveAt(file.Identity.Count-1);
            }
        }
        if (gotit == false)
        {
            string messageString = "Error - manure type not found for manure storage " + name + " ManureStorageID = " 
                + ManureStorageID.ToString() + " Species group = " + speciesGroup.ToString();
            GlobalVars.Instance.Error(messageString);
        }
       // theManure.SetmanureType(itemNr);
    }
   
    public void SettheHousing(housing ahouse){theHousing=ahouse;}
    public string Getname() { return name; }  
    public bool GetStoresSolid() { return theManure.GetisSolid(); }
    public void Addmanure(manure amanure, double proportionOfYearGrazing)
    {
        theManure.AddManure(amanure);
        Cinput += amanure.GetdegC();
        Cinput += amanure.GetnonDegC();
        tstore += ((1 - proportionOfYearGrazing) * (theManure.GetdegC() + theManure.GetnonDegC()) / (2 * (theManure.GetdegC() + theManure.GetnonDegC())));
    }
    public void DoManurestore()
    {
        supplementaryC = 0;
        supplementaryN = 0;
        double temp = theManure.GetnonDegC() + theManure.GetdegC() +theManure.GetTAN() + theManure.GetorganicN();
        if (temp > 0.0)
        {
            double Bo = theManure.GetBo();
            if (supplementaryFeedstock.Count > 0)
            {
                double degSupplC = 0;
                double nondegSupplC = 0;
                double supplN = 0;
                double manureDM = Cinput / 0.42; //hack!
                double cumBo = 0;
                double cumSupplDM=0;
                for (int i = 0; i < supplementaryFeedstock.Count; i++)
                {
                    feedItem aFeedstock = supplementaryFeedstock[i];
                    double amountThisFeedstock = manureDM * aFeedstock.Getamount(); //kg DM
                    cumSupplDM+=amountThisFeedstock;
                    cumBo += amountThisFeedstock * aFeedstock.GetBo();
                    nondegSupplC += amountThisFeedstock * aFeedstock.GetC_conc() * aFeedstock.Getfibre_conc();
                    degSupplC += amountThisFeedstock * aFeedstock.GetC_conc() * (1 - aFeedstock.Getfibre_conc());
                    supplementaryC += nondegSupplC + degSupplC;
                    supplN = amountThisFeedstock * aFeedstock.GetN_conc();
                    supplementaryN += supplN;
                    aFeedstock.Setamount(amountThisFeedstock); //convert from amount per unit manure DM mass to total amount
                    GlobalVars.Instance.allFeedAndProductsUsed[aFeedstock.GetFeedCode()].composition.AddFeedItem(aFeedstock, false);
                }
                double aveSupplBo = cumBo / cumSupplDM;
                Bo = (manureDM * Bo + cumSupplDM * aveSupplBo) / (manureDM + cumSupplDM);
                theManure.SetBo(Bo);
                theManure.SetdegC(theManure.GetdegC() + degSupplC);
                theManure.SetnonDegC(theManure.GetnonDegC() + nondegSupplC);
                theManure.SetorganicN(theManure.GetorganicN() + supplementaryN);
            }
            DoCarbon();
            DoNitrogen();
            CheckManureStoreNBalance();
            temp = theManure.GetnonDegC() + theManure.GetdegC();
        }
    }
    public void DoCarbon()
    {
        Cinput = GetManureC();
        double tor = GlobalVars.Instance.gettor();
        double rgas = GlobalVars.Instance.getrgas();
        double aveTemperature = GlobalVars.Instance.theZoneData.GetaverageAirTemperature();
        
        if ((GlobalVars.Instance.getcurrentInventorySystem() == 1)||(GlobalVars.Instance.getcurrentInventorySystem() == 2))
        {
            CRunOffOrg = Cinput * ohmOrg; //assume runoff occurs immediately, before degradation
            theManure.SetdegC(theManure.GetdegC() * (1-ohmOrg));
            theManure.SetnonDegC(theManure.GetnonDegC() * (1-ohmOrg));
            if ((theManure.GetmanureType() != 5) && (theManure.GetmanureType() < 11))
            {
                bool isCovered=false;
                switch (theManure.GetmanureType())
                {
                    case 2:isCovered=true;
                        break;
                    case 4:isCovered=true;
                        break;
                    case 9:isCovered=true;
                        break;
                    case 10:isCovered=true;
                        break;
                }
                if (GlobalVars.Instance.getcurrentInventorySystem() == 1)
                {
                    if (theManure.GetisSolid())
                    {
                        if (aveTemperature < 14.5)
                            MCF = 0.02;
                        if ((aveTemperature >= 14.5) && (aveTemperature < 25.5))
                            MCF = 0.04;
                        if (aveTemperature >= 25.5)
                            MCF = 0.05;
                    }
                    else
                    {
                        if (isCovered)
                            MCF = Math.Exp(0.0896159864767708 * aveTemperature - 3.1458426322101);
                        else
                            MCF = Math.Exp(0.088371620269402 * aveTemperature - 2.64281541545576);
                    }
                }
            }
            double Bo = theManure.GetBo();
            double testit = (1 / GlobalVars.Instance.getalpha()) * (theManure.GetdegC() + theManure.GetnonDegC() + theManure.GethumicC());
            CCH4ST = (1/ GlobalVars.Instance.getalpha()) * (theManure.GetdegC() + theManure.GetnonDegC() + theManure.GethumicC()) * (Bo * 0.67 * MCF);//1.46
           
            double km = 0.39;
            double VS = (theManure.GetdegC() + theManure.GetnonDegC() + theManure.GethumicC()) / GlobalVars.Instance.getalpha();

            CCH4ST = MCF * VS * Bo * 0.67 * 12 / 16;

            CCO2ST = (CCH4ST * (1 - tor)) / tor;//1.47
            double biogasC = CCH4ST + CCO2ST;
            Cdegradation = biogasC / (1 - GlobalVars.Instance.getHumification_const());
            if (Cdegradation>theManure.GetdegC())
            {
                if (Cdegradation > (theManure.GetdegC() + theManure.GetnonDegC()))
                {
                    string message1 = "C degradation greater than sum of degradable and non-degradable C in store " + name;
                    GlobalVars.Instance.Error(message1);
                }
                else
                {
                    Cdegradation -= theManure.GetdegC();
                    theManure.SetdegC(0.0);
                    theManure.SetnonDegC(theManure.GetnonDegC() - Cdegradation);
                }
            }
            else
                theManure.SetdegC(theManure.GetdegC() - Cdegradation);
            theManure.SethumicC(theManure.GethumicC() + Cdegradation * GlobalVars.Instance.getHumification_const());
        }
        else
        {
            string message1 = "Un-upgraded code in manure storage " + name;
            GlobalVars.Instance.Error(message1);

        }
        biogasCH4C = propGasCapture * CCH4ST;
        CCH4ST -= biogasCH4C;
        biogasCO2C = propGasCapture * CCO2ST;
        CCO2ST -= biogasCO2C;
        CheckManureStoreCBalance();
    }
   
    public void DoNitrogen()
    {
        EFStoreN2 = Lambda * EFStoreN20;//1.66
        NTanInstore = theManure.GetTAN();
        NorgInstore = theManure.GetorganicN();
#if WIDE_AREA
        double totalOrgNdegradation = 0;
        NorgOutStore = NorgInstore;

#endif

        if ((GlobalVars.Instance.getcurrentInventorySystem() == 1)||(GlobalVars.Instance.getcurrentInventorySystem() == 2))
        {
            totalNstoreN20 = EFStoreN20 * (NTanInstore + NorgInstore); //1.64 - not quite..

            totalNstoreNH3 = EFStoreNH3 * (NTanInstore + totalOrgNdegradation - newNHUM);//1.65 - not quite..
            totalNstoreN2 = Lambda * totalNstoreN20;//1.66
            NrunoffTan = NTanInstore  * ohmTAN;
            NTanOutstore = NTanInstore + totalOrgNdegradation - (totalNstoreN20 + totalNstoreN2 + totalNstoreNH3 + NrunoffTan + newNHUM);
        }
        else
        {
            double CN = theManure.GetdegC() / NorgInstore;
            double StorageRefTemp = 0;
            double EFNH3ref = 0;
            double KHø = 1 - 1.69 + 1447.7 / (meanTemp + GlobalVars.absoluteTemp);
            double KHref = 1 - 1.69 + 1447.7 / (StorageRefTemp + GlobalVars.absoluteTemp);
            EFStoreNH3 = KHref / KHø * EFNH3ref; //1.67
            double EFsum = EFStoreNH3 + EFStoreN20 + EFStoreN2;

            NTanOutstore = ((CdegradationRate * (1 / CN - GlobalVars.Instance.getHumification_const() / GlobalVars.Instance.getCNhum())
                * theManure.GetOrgDegC()) / ((EFsum + ohmTAN) - (ohmOrg + CdegradationRate / CN)))
                * Math.Pow(Math.E, -(ohmOrg + CdegradationRate / CN) * tstore);//1.63
            //NTanOutstore += NTanInstore - (theManure.GetdegC() * (1 / CN - tau / GlobalVars.Instance.getCNhum()) * theManure.GetOrgDegC()) / ((EFsum + ohmTAN) - (ohmOrg + theManure.GetdegC() / CN)) * Math.Pow(Math.E, -(EFsum + ohmOrg) * tstore);//1.63
            NTANLost = NorgInstore + NTanInstore - (NTanInstore + NrunoffTan + NTanOutstore);//1.68
            NrunoffTan = ohmTAN / (ohmTAN + EFStoreNH3 + EFStoreN20 + EFStoreN2) * NTANLost;
            totalNstoreNH3 = NTANLost * EFStoreNH3 / (ohmTAN + EFStoreN2 + EFStoreN20 + EFStoreNH3);
            totalNstoreN2 = NTANLost * EFStoreN2 / (ohmTAN + EFStoreN2 + EFStoreN20 + EFStoreNH3);
            totalNstoreN20 = NTANLost * EFStoreN20 / (ohmTAN + EFStoreN2 + EFStoreN20 + EFStoreNH3);
        }
        theManure.SethumicN(theManure.GethumicN() + newNHUM);
        theManure.SetorganicN(NorgOutStore);
        theManure.SetTAN(NTanOutstore);
    }
    public void Write()
    {
        theHousing.Write();
        GlobalVars.Instance.writeInformationToFiles("name", "Name", "-", name, parens);
        GlobalVars.Instance.writeInformationToFiles("identity", "ID", "-", identity, parens);
        GlobalVars.Instance.writeInformationToFiles("Cinput", "C input", "kg", Cinput, parens);
        GlobalVars.Instance.writeInformationToFiles("CCH4ST", "CH4-C emitted", "kg", CCH4ST, parens);
        GlobalVars.Instance.writeInformationToFiles("CCO2ST", "CO2-C emitted", "kg", CCO2ST, parens);

        GlobalVars.Instance.writeInformationToFiles("Ninput", "N input", "kg", Ninput, parens);
        GlobalVars.Instance.writeInformationToFiles("NTanInstore", "TAN input to storage", "kg", NTanInstore, parens);
        GlobalVars.Instance.writeInformationToFiles("totalNstoreNH3", "NH3-N emitted", "kg", totalNstoreNH3, parens);
        GlobalVars.Instance.writeInformationToFiles("totalNstoreN2", "N2-N emitted", "kg", totalNstoreN2, parens);
        GlobalVars.Instance.writeInformationToFiles("totalNstoreN20", "N2O-N emitted", "kg", totalNstoreN20, parens);
        GlobalVars.Instance.writeInformationToFiles("NTANLost", "Total TAN lost", "kg", NTANLost, parens);
        GlobalVars.Instance.writeInformationToFiles("NDegOrgOut", "Degradable N ex storage", "kg", NDegOrgOut, parens);
        GlobalVars.Instance.writeInformationToFiles("newNHUM", "new Humic N created in manure storage", "kg", newNHUM, parens);
        GlobalVars.Instance.writeInformationToFiles("NorgOutStore", "Organic N ex storage", "kg", NorgOutStore, parens);
        GlobalVars.Instance.writeInformationToFiles("NRunoffHum", "Humic N in runoff", "kg", NRunoffHum, parens);
        GlobalVars.Instance.writeInformationToFiles("NrunoffTan", "TAN in runoff", "kg", NrunoffTan, parens);
        GlobalVars.Instance.writeInformationToFiles("NRunOffOrg", "Organic N in runoff", "kg", NRunOffOrg, parens);
        GlobalVars.Instance.writeInformationToFiles("NLost", "Total N lost", "kg", NLost, parens);
        
        theManure.Write("");
      //  if (writeEndTab)
            GlobalVars.Instance.writeEndTab();
    }
    public bool CheckManureStoreCBalance()
    {
        bool retVal = false;
        double Cout = GetManureC() + biogasCO2C + biogasCH4C;
        double CLost = CCH4ST + CCO2ST + CRunOffOrg;
        double Cbalance = Cinput - (Cout + CLost);
        double diff = Cbalance / Cinput;
        double tolerance = GlobalVars.Instance.getmaxToleratedError();
        if (Math.Abs(diff) > tolerance)
        {
                double errorPercent = 100 * diff;
                string message1 = "Error; Manure storage C balance error for " + name + " is more than the permitted margin\n";
                string message2 =message1+ "Percentage error = " + errorPercent.ToString("0.00") + "%";
                GlobalVars.Instance.Error(message2);
             
        }
        return retVal;
    }
    public bool CheckManureStoreNBalance()
    {
        bool retVal = false;
        Ninput = NTanInstore + NorgInstore;
        Nout = GetManureN();
        NLost = NRunOffOrg + NrunoffTan+ totalNstoreN2+totalNstoreN20+totalNstoreNH3;
        Nbalance = Ninput - (Nout + NLost);
        double diff = Nbalance / Ninput;
        double tolerance = GlobalVars.Instance.getmaxToleratedError();
        if (Math.Abs(diff) > tolerance)
        {
            Write();

                double errorPercent = 100 * diff;
       
                string messageString= ("Error; Manure storage N balance error for " + name + " is more than the permitted margin\n");
                messageString = messageString+("Percentage error = " + errorPercent.ToString("0.00") + "%");
        
                GlobalVars.Instance.Error(messageString);
          
        }
        return retVal;
    }
}
