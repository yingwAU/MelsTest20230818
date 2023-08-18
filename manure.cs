using System;
using System.Xml;
public class manure
{
    string path;
    int id;
    double DM;
    double nonDegDM;
    double degDM;
    double nonDegC;
    double orgDegC;
    double degC;
    double humicC;
    double organicN;
    double TAN;
    double humicN;
    double Bo;
    int manureType;
    int speciesGroup;
    string name;
    bool isSolid;
    string parens;
    public void SetdegC(double aValue) { degC = aValue; }
    public void SetnonDegC(double aValue) { nonDegC = aValue; }
    public void SethumicC(double aValue) { humicC = aValue; }
    public void SetTAN(double aValue) { TAN = aValue; }
    public void SetorganicN(double aValue) { organicN = aValue; }   
    public void SetspeciesGroup(int aValue) { speciesGroup = aValue; }
    public void SetmanureType(int aValue) { manureType = aValue; }
    public void SetisSolid(bool aValue) { isSolid = aValue; }
    public void SethumicN(double aVal) { humicN = aVal; }
    public void SetBo(double aVal) { Bo = aVal; }
    public void Setname(string aname) { name = aname; }
    public double GetnonDegC() { return nonDegC; }
    public double GethumicC() { return humicC; }
    public double GetdegC() { return degC; }
    public double GetOrgDegC() { return orgDegC; }
    public double GetTAN() { return TAN; }
    public double GetorganicN() { return organicN; }
    public int GetmanureType() { return manureType; }
    public bool GetisSolid() { return isSolid; }
    public double GethumicN() { return humicN; }
    public double GetBo() { return Bo; }   
    public manure()
    {
        DM =0;
        nonDegDM =0;
        degDM = 0;
        nonDegC = 0;
        degC = 0;
        humicC = 0;
        organicN = 0;
        TAN = 0;
        humicN = 0;
        manureType = 0;
        speciesGroup = 0;
        Bo = 0;
        name = "";
    }

    //create new instance of manure, with amount determined by N required
    
    public void AddManure(manure aManure)
    {
        double totalC = nonDegC + degC;
        double oldBo = Bo * totalC;
        double donorC = aManure.degC + aManure.nonDegC;
        double addedBo = aManure.Bo * donorC;
        Bo = (oldBo + addedBo) / (totalC + donorC);
        DM += aManure.DM;
        nonDegDM += aManure.nonDegDM;
        degDM += aManure.degDM;
        nonDegC += aManure.nonDegC;
        degC += aManure.degC;
        humicC += aManure.humicC;
        organicN += aManure.organicN;
        TAN += aManure.TAN;
        humicN += aManure.humicN;
    }
    public void Write(string addedInfo)
    {
        parens = "_" + addedInfo + "_" + name;
        GlobalVars.Instance.writeInformationToFiles("name", "Name", "-", name, parens);
        GlobalVars.Instance.writeInformationToFiles("speciesGroup", "Species number", "-", speciesGroup, parens);
        GlobalVars.Instance.writeInformationToFiles("typeStored", "Storage type", "-", manureType, parens);
        GlobalVars.Instance.writeInformationToFiles("DM", "Dry matter", "kg", DM, parens);
        GlobalVars.Instance.writeInformationToFiles("nonDegDM", "Non-degradable DM", "kg", nonDegDM, parens);
        GlobalVars.Instance.writeInformationToFiles("degDM", "Degradable DM", "kg", degDM, parens);
        GlobalVars.Instance.writeInformationToFiles("nonDegC", "Non-degradable C", "kg", nonDegC, parens);
        GlobalVars.Instance.writeInformationToFiles("degC", "Degradable C", "kg", degC, parens);
        GlobalVars.Instance.writeInformationToFiles("humicC", "Humic C", "kg", humicC, parens);
        GlobalVars.Instance.writeInformationToFiles("TAN", "TAN", "kg", TAN, parens);
        GlobalVars.Instance.writeInformationToFiles("organicN", "Organic N", "kg", organicN, parens);
    }
}
