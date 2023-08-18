using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.IO;
using System.Xml.XPath;
public class feedItem
{
    double amount;
    string name;
    int feedCode;
    int ID;
    string path;
    bool isMain;
    double energy_conc;
    double ash_conc;
    double C_conc;
    double N_conc;
    double nitrogenfreeExtract_conc;
    double fibre_conc;
    double fat_conc;
    double nitrate_conc;
    double DMdigestibility;
    bool isGrazed;
    bool fedAtPasture;
    bool beddingMaterial;
    double StoreProcessFactor;
    public void setFeedCode(int aFeedCode) { feedCode = aFeedCode; }
    public int GetFeedCode(){return feedCode;}
    public void Setamount(double anamount) { amount = anamount; }
    public void SetC_conc(double aVal) { C_conc = aVal; }
    public void SetN_conc(double aVal) { N_conc = aVal; }   
    public double Getamount() { return amount; }
    public string GetName() { return name; }
    public double Getenergy_conc() { return energy_conc; }
    public double Getash_conc() { return ash_conc; }
    public double GetC_conc() { return C_conc; }
    public double GetN_conc() { return N_conc; }
    public double GetnitrogenfreeExtract_conc() { return nitrogenfreeExtract_conc; }
    public double Getfibre_conc() { return fibre_conc; }
    public double Getfat_conc() { return fat_conc; }
    public double GetDMdigestibility() { return DMdigestibility; }  
    public bool GetisGrazed() { return isGrazed; }

    private string Unit;
    string parens;
    public feedItem()
    {
        name = "None";
        path = "None";
        amount=0;
        feedCode=0;
        isMain=false;
        energy_conc=0;
        ash_conc=0;
        C_conc=0;
        N_conc=0;
        nitrogenfreeExtract_conc=0;
        fibre_conc=0;
        fat_conc=0;
        nitrate_conc = 0;
        DMdigestibility=0;
        isGrazed=false;
        beddingMaterial=false;
        StoreProcessFactor=0;
        Unit = "kg/day";
    }
    public feedItem(string feeditemPath, int id, bool getamount, string aParens)
    {
        parens = aParens;
        ID = id;
        path = feeditemPath + '(' + id + ')';
        FileInformation feedFile = new FileInformation(GlobalVars.Instance.getFarmFilePath());
        feedFile.setPath(path);
        if (getamount == true)
        {
            amount = feedFile.getItemDouble("Amount");
            Unit = feedFile.getItemString("Unit");
        }
        //name = feedFile.getItemString("Name");
        feedCode = feedFile.getItemInt("FeedCode");
        if ((feedCode > 1000) && (feedCode < 2000))
        {
            feedCode -= 1000;
        }
        if (feedCode > 2000)
        {
            feedCode -= 2000;
        }
        
        isGrazed = false;

        //Hack to allow grazed crop residues to refer to alternative feed, until interface fixed
        if (feedCode == 310)  //grazed millet straw residue is coded as acidified raw milk
        {
            feedCode = 925;
            isGrazed = true;
        }
        if (feedCode == 312) //grazed groundnut straw residue is coded as Whey
        {
            feedCode = 926;
            isGrazed = true;
        }
        if (feedCode == 921)  //grazed fallow grass dry is coded as willow foliage
        {
            feedCode = 934;
            isGrazed = true;
        }
        if (feedCode == 298)  //grazed savanna grass dry residue is coded as beech granules
        {
            feedCode = 820;
            isGrazed = true;
        }
        //end of hack
        
        GetStandardFeedItem(feedCode);
        
    }

    public void GetStandardFeedItem(int targetFeedCode)
    {
        DateTime start = DateTime.Now;
       
        FileInformation file=new FileInformation(GlobalVars.Instance.getfeeditemFilePath());
        file.setPath("AgroecologicalZone(" + GlobalVars.Instance.GetZone() + ").feedItem");
        int min = 99; int max = 0;
        file.getSectionNumber(ref min, ref max);

        bool found = false;
        for(int i=min;i<=max;i++)
        {
            file.setPath("AgroecologicalZone(" + GlobalVars.Instance.GetZone() + ").feedItem");
            if(file.doesIDExist(i))
            {
                file.setPath("AgroecologicalZone(" + GlobalVars.Instance.GetZone() + ").feedItem("+i.ToString()+")");
                int StandardFeedCode = file.getItemInt("FeedCode");
                if (StandardFeedCode == targetFeedCode)
                {
                    found = true;
                    feedCode = targetFeedCode;
                    name = file.getItemString("Name");
                    file.setPath("AgroecologicalZone(" + GlobalVars.Instance.GetZone() + ").feedItem(" + i.ToString() + ").Main(-1)");
                    isMain = file.getItemBool("Value");
                    file.setPath("AgroecologicalZone(" + GlobalVars.Instance.GetZone() + ").feedItem(" + i.ToString() + ").Fibre_concentration(-1)");
                    fibre_conc = file.getItemDouble("Value");
                    file.setPath("AgroecologicalZone(" + GlobalVars.Instance.GetZone() + ").feedItem(" + i.ToString() + ").NFE_concentration(-1)");
                    nitrogenfreeExtract_conc = file.getItemDouble("Value");
                    file.setPath("AgroecologicalZone(" + GlobalVars.Instance.GetZone() + ").feedItem(" + i.ToString() + ").CrudeProtein_concentration(-1)");
                    double CrudeProtein_concentration = file.getItemDouble("Value");
                    SetN_conc(CrudeProtein_concentration / GlobalVars.NtoCrudeProtein);
                    file.setPath("AgroecologicalZone(" + GlobalVars.Instance.GetZone() + ").feedItem(" + i.ToString() + ").Fat_concentration(-1)");
                    fat_conc = file.getItemDouble("Value");
       
                    file.setPath("AgroecologicalZone(" + GlobalVars.Instance.GetZone() + ").feedItem(" + i.ToString() + ").Energy_concentration(-1)");
                    energy_conc = file.getItemDouble("Value"); 
                    energy_conc *= GlobalVars.Instance.GetdigestEnergyToME();

                    file.setPath("AgroecologicalZone(" + GlobalVars.Instance.GetZone() + ").feedItem(" + i.ToString() + ").Ash_concentration(-1)");
                    ash_conc = file.getItemDouble("Value");
                    SetC_conc((1.0 - Getash_conc()) * 0.46);
                    if (energy_conc == 0)
                        SetC_conc(0);
                    file.setPath("AgroecologicalZone(" + GlobalVars.Instance.GetZone() + ").feedItem(" + i.ToString() + ").Nitrate_concentration(-1)");
                    nitrate_conc = file.getItemDouble("Value");
     
                    file.setPath("AgroecologicalZone(" + GlobalVars.Instance.GetZone() + ").feedItem(" + i.ToString() + ").DMDigestibility(-1)");
                    DMdigestibility = file.getItemDouble("Value");
     
                    file.setPath("AgroecologicalZone(" + GlobalVars.Instance.GetZone() + ").feedItem(" + i.ToString() + ").Bedding_material(-1)");
                    beddingMaterial =file.getItemBool("Value");

                    file.setPath("AgroecologicalZone(" + GlobalVars.Instance.GetZone() + ").feedItem(" + i.ToString() + ").processStorageLoss(-1)");
                    StoreProcessFactor = file.getItemDouble("Value");
    
                    break;
                }
            }
        }
    
        if (found == false)
        {
            string messageString=("could not find feeditem ");
            messageString+=feedCode.ToString() + " name = " + name +  "\n";
            GlobalVars.Instance.Error(messageString);
        }
 
        TimeSpan timeItTook = DateTime.Now - start;
}
    public int AddFeedItem(feedItem afeedItem, bool pooling, bool isBedding=false)
    {
        if ((afeedItem.GetFeedCode() != feedCode) && (pooling != true))
        {
              string messageString=("Error; attempt to combine two different feed items");
               GlobalVars.Instance.Error(messageString);
        }
        double donorAmount = afeedItem.Getamount();
        name = afeedItem.GetName();
        feedCode = afeedItem.GetFeedCode();
        if (donorAmount != 0)
        {
            energy_conc = (energy_conc * amount + donorAmount * afeedItem.Getenergy_conc()) / (amount + donorAmount);
            ash_conc = (ash_conc * amount + donorAmount * afeedItem.Getash_conc()) / (amount + donorAmount);
            C_conc = (C_conc * amount + donorAmount * afeedItem.GetC_conc()) / (amount + donorAmount);
            N_conc = (N_conc * amount + donorAmount * afeedItem.GetN_conc()) / (amount + donorAmount);
            nitrogenfreeExtract_conc = (nitrogenfreeExtract_conc * amount + donorAmount * afeedItem.GetnitrogenfreeExtract_conc()) / (amount + donorAmount);
            fibre_conc = (fibre_conc * amount + donorAmount * afeedItem.Getfibre_conc()) / (amount + donorAmount);
            fat_conc = (fat_conc * amount + donorAmount * afeedItem.Getfat_conc()) / (amount + donorAmount);
            DMdigestibility = (DMdigestibility * amount + donorAmount * afeedItem.GetDMdigestibility()) / (amount + donorAmount);
            amount += donorAmount;
            if (isBedding)
                beddingMaterial = true;
        }
        return 0;
    }
    //Jonas - this is dangerous code. The feedItem concentrations could go negative. I have no immediate solution
      
    public void Write(string theParens)
    {
        parens = theParens + "feedCode" + feedCode.ToString();
        GlobalVars.Instance.writeInformationToFiles("name", "Name", "-", name,parens);
        GlobalVars.Instance.writeInformationToFiles("amount", "Amount", "kg DM", amount, parens);
        GlobalVars.Instance.writeInformationToFiles("feedCode", "Feed code", "-", feedCode, parens);
        GlobalVars.Instance.writeInformationToFiles("ash_conc", "Ash concentration", "kg/kg DM", ash_conc, parens);
        GlobalVars.Instance.writeInformationToFiles("C_conc", "C concentration", "kg/kg DM", C_conc, parens);
        GlobalVars.Instance.writeInformationToFiles("N_conc", "N concentration", "kg/kg DM", N_conc, parens);
        GlobalVars.Instance.writeInformationToFiles("nitrogenfreeExtract_conc", "NFE concentration", "kg/kg DM", nitrogenfreeExtract_conc, parens);
        GlobalVars.Instance.writeInformationToFiles("fibre_conc", "Fibre concentration", "kg/kg DM", fibre_conc, parens);
        GlobalVars.Instance.writeInformationToFiles("fat_conc", "Fat concentration", "kg/kg DM", fat_conc, parens);
        GlobalVars.Instance.writeInformationToFiles("energy_conc", "Energy concentration", "ME/kg DM", energy_conc, parens);
        GlobalVars.Instance.writeInformationToFiles("DMdigestibility", "DM digestibility", "kg/kg DM", DMdigestibility, parens);
        GlobalVars.Instance.writeInformationToFiles("isGrazed", "Fed at pasture", "-", isGrazed, parens);
        GlobalVars.Instance.writeInformationToFiles("beddingMaterial", "Can be used for bedding", "-", beddingMaterial, parens);     
    }  
    public double GetBo()
    {
        double NDF = (144.5 - 1.54 * DMdigestibility * 100)/10; //NDF as percentage of DM
        double CelAndHemi = NDF - fibre_conc * 100;
        if (CelAndHemi < 0)
            CelAndHemi = 0;
        double Bo = 19.05 * N_conc * 6.25 * 100 + 27.73 * fat_conc * 100 + 1.75 * CelAndHemi;
        Bo /= 1000;
        return Bo;
    }
}

