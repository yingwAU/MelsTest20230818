#define WIDE_AREA
using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;

public class CropSequenceClass
{
    //inputs
    string name;
    string soilType;
    int FarmType;
    double area;
    //parameters 
    //other variables to be output
    //other

    string path;
    int identity;
    List<CropClass> theCrops = new List<CropClass>();
    public XElement node = new XElement("data");
    
    double NdeltaSoil = 0;
#if WIDE_AREA
    double leachingFraction=0;
   
#endif
    private int soiltypeNo = 0;
    private int soilTypeCount = 0;   
    private string Parens;
    
    public double GettheNitrateLeaching()
    {
        return GettheNitrateLeaching(theCrops.Count);
    }

    public double GettheNitrateLeaching(int maxCrops) 
    {
        double Nleached = 0;
        for (int i = 0; i < maxCrops; i++)
            Nleached += theCrops[i].GetnitrateLeaching() * area;
        return Nleached;
    }
    
    public CropSequenceClass(string aPath, int aID, int zoneNr, int currentFarmType, string aParens, int asoilTypeCount)
    {
        Parens = aParens;
        path = aPath;
        identity = aID;
        FarmType = currentFarmType;
        FileInformation rotation = new FileInformation(GlobalVars.Instance.getFarmFilePath());
        path += "(" + identity.ToString() + ")";
        rotation.setPath(path);
        name = rotation.getItemString("NameOfRotation");
        area = rotation.getItemDouble("Area");
        soilType = rotation.getItemString("SoilType");
        soilTypeCount = asoilTypeCount;
        string crop = path + ".Crop";
        rotation.setPath(crop);
        int min = 99; int max = 0;
        rotation.getSectionNumber(ref min, ref max);
        //List<GlobalVars.product> residue=new List<GlobalVars.product>();
        for (int i = min; i <= max; i++)
        {
            if (rotation.doesIDExist(i))
            {
                CropClass aCrop = new CropClass(crop, i, zoneNr, name);
                aCrop.SetcropSequenceNo(identity);
                theCrops.Add(aCrop);
            }
        }
        
        getparameters(zoneNr);

        soiltypeNo = -1;
        for (int i = 0; i < GlobalVars.Instance.theZoneData.thesoilData.Count; i++)
        {
            if(GlobalVars.Instance.theZoneData.thesoilData[i].name.CompareTo(soilType)==0)
                soiltypeNo = i;
        }
        if (soiltypeNo == -1)
        {
            string messageString=("Error - could not find soil type " + soilType + " in parameter file\n");
            messageString+=("Crop sequence name = " + name);
            GlobalVars.Instance.Error(messageString);
        }
    }

    public void getparameters(int zoneNR)
    {
        double soilN2Factor = 0;
        bool gotit = false;
        int max = GlobalVars.Instance.theZoneData.thesoilData.Count;
        for (int i = 0; i < max; i++)
        {
            string soilname = GlobalVars.Instance.theZoneData.thesoilData[i].name;
            if (soilname == soilType)
            {
                soilN2Factor = GlobalVars.Instance.theZoneData.thesoilData[i].N2Factor;
                for (int j = 0; j < theCrops.Count; j++)
                {
                    CropClass aCrop = theCrops[j];
                    aCrop.setsoilN2Factor(soilN2Factor);
                }
                gotit = true;
                break;
            }
        }
        if (gotit == false)
        {
    
            string messageString=("Error - could not find soil type " + soilType + " in parameter file\n");
            messageString+=("Crop sequence name = " + name);
            GlobalVars.Instance.Error(messageString);
        }
    }
    //!Adjust the crop dates so that the first year is year 1 rather than calendar year
   
   public double getArea()    { return area;}

   public double getGrazingMethaneC()
    {
        double result = 0;
        for (int i = 0; i < theCrops.Count; i++)
        {
            result += theCrops[i].GetgrazingCH4C() * area;
        }
        return result;
    }

    public double GetManureNapplied()
    {
        return GetManureNapplied(theCrops.Count);
    }
    
    public double GetManureNapplied(int maxCrops)
    {
        double retVal = 0;
        for (int i = 0; i < maxCrops; i++)
            retVal += theCrops[i].GetManureNapplied() * area;
        return retVal;
    }
    
    public double getNFix()
    {
        return getNFix(theCrops.Count);
    }

    public double getNFix(int maxCrops)
    {
        double nFix = 0;
        for (int i = 0; i < theCrops.Count; i++)
        {
            nFix += theCrops[i].getNFix() * area;
        }
        return nFix;
    }

    public double getNAtm()
    {
        return getNAtm(theCrops.Count);
    }

    public double getNAtm(int maxCrops)
    {
        double nAtm = 0;
        for (int i = 0; i < theCrops.Count; i++)
        {
            nAtm += theCrops[i].getnAtm() * area;
        }
        return nAtm;
    }

    public double getManureNapplied()
    {
        return getManureNapplied(theCrops.Count);
    }

    public double getManureNapplied(int maxCrops)
    {
        double manureN = 0;
        for (int i = 0; i < theCrops.Count; i++)
        {
            for (int j = 0; j < theCrops[i].GetmanureApplied().Count; j++)
                manureN += theCrops[i].GetmanureApplied()[j].getNamount() * area;
        }
        return manureN;
    }

    public double getFertiliserNapplied()
    {
        return getFertiliserNapplied(theCrops.Count);
    }

    public double getFertiliserNapplied(int maxCrops)
    {
        double fertiliserN = 0;
        for (int i = 0; i < theCrops.Count; i++)
        {
            for (int j = 0; j < theCrops[i].GetfertiliserApplied().Count; j++)
            {
                if (theCrops[i].GetfertiliserApplied()[j].getName()!= "Nitrification inhibitor")
                    fertiliserN += theCrops[i].GetfertiliserApplied()[j].getNamount() * area;
            }
        }
        return fertiliserN;
    }

    public double GetManureNH3NEmission()
    {
            return GetManureNH3NEmission( theCrops.Count);
    }

    public double GetManureNH3NEmission(int maxCrops)
    {
        double retVal = 0;
        for (int i = 0; i < maxCrops; i++)
        {
            retVal += theCrops[i].GetmanureNH3Nemission() * area;
        }
        return retVal;
    }

    public double GetFertNH3NEmission()
    {
        return GetFertNH3NEmission(theCrops.Count);
    }
   
     public double GetFertNH3NEmission(int maxCrops)
    {
        double retVal = 0;
        for (int i = 0; i < maxCrops; i++)
        {
            retVal += theCrops[i].GetfertiliserNH3Nemission() * area;
        }
        return retVal;
    }
        
    public double GetN2ONemission()
    {
        return GetN2ONemission(theCrops.Count);
    }

    public double GetN2ONemission(int maxCrops)
    {
        double retVal = 0;
        for (int i = 0; i < theCrops.Count; i++)
        {
            retVal += theCrops[i].GetN2ONemission() * area;
        }
        return retVal;
    }
   
  public void CalculateNbudget()
    {
        for (int i = 0; i < theCrops.Count; i++)
        {
            theCrops[i].CalculateNinputs(leachingFraction, ref NdeltaSoil);
        }
    }
    public void WriteGHGdata(double croppedArea, double OtherGHGemissions)
    {
        double Napplied = getFertiliserNapplied() + getManureNapplied();
        double areaProportion = area / croppedArea;
        for (int i = 0; i < theCrops.Count; i++)
        {
            theCrops[i].WriteCropGHGbudget();
        }
        VMP3.Instance.WriteLineField("");
    }

}
