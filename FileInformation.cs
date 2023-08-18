#define WIDE_AREA
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.IO;
using System.Xml.XPath;
#if (server)
using System.Web;
#endif
    public class FileInformation
    {
        //storing the currently used files
        private string fileNameOrg = "test.xml";
        private string fileNameAlt = "test.xml";

        //storing the path down the xml tree
        public List<int> Identity = new List<int>();
        public List<string> PathNames = new List<string>();
        //storing currently used primary file
        private Node treeOrg;
        //storing curretnly used alternative file 
        private Node treeAlt;
        private bool pauseBeforeExit = false;

        public void reset()
        {
            FileName = new List<string>();
            AllNodes = new List<Node>();
        }

        public FileInformation()
        {

        }
#if (server)

    List<string> FileName = new List<string>();
    List<Node> AllNodes = new List<Node>();
#else
        // contains a list of files that is stored in memery
        static List<string> FileName = new List<string>();
        //a list of xml tress that has been read into memery
        static List<Node> AllNodes = new List<Node>();
#endif
        //public string GetFileName() { return FileName; }

        private void ReadingFile(string nameOfFile)
        {
#if (server)
        FileName = (List<string>)HttpContext.Current.Session["FileName"];
        AllNodes = (List<Node>)HttpContext.Current.Session["AllNodes"];
        if(FileName==null)
        {
            FileName = new List<string>();
            AllNodes = new List<Node>();
        }
#endif
            string file = "";
            //getting the file name from path
            string[] partFileName = nameOfFile.Split('\\');

            bool inUse = false;
            //check if we already have the file in memery
            for (int i = 0; i < FileName.Count; i++)
            {
                //checks if we have the generic file
                if (FileName.ElementAt(i).CompareTo(partFileName[partFileName.Count() - 1]) == 0 && !nameOfFile.Contains("Alternative"))
                {
                    //using the generic file
                    inUse = true;
                    treeOrg = AllNodes.ElementAt(i);
                }
                if (FileName.ElementAt(i).CompareTo(partFileName[partFileName.Count() - 1]) == 0 && nameOfFile.Contains("Alternative"))
                {
                    //using the alternative file
                    inUse = true;
                    treeAlt = AllNodes.ElementAt(i);
                }


            }
            //the file is not found. so it is going to be read
            if (inUse == false)
            {

                treeOrg = new Node();
                string itemName = "ino";
                try
                {
                    XmlReader data = null;
                    try
                    {
                        //chekc if the file does exist
                        while (!File.Exists(nameOfFile))
                        {
                            //file does not exist. Checking if the files is in the above directory.
                            string[] path = nameOfFile.Split('\\');
                            //at the top directoy so file is not found
                            if (path.Count() == 1 || (path.Count() == 2 && path[0][1] == ':'))
                                break;
                            nameOfFile = "";
                            //creating new path for checking the file. The dir is the parent dir
                            for (int i = 0; i < path.Count() - 2; i++)
                            {
                                nameOfFile += path[i] + '\\';
                            }
                            nameOfFile += path[path.Count() - 1];

                        }

                        if (nameOfFile.Contains("Alternative"))
                        {
                            file = nameOfFile;
                        }
                        else
                        {
                            file = nameOfFile;
                        }
                        //reading file
                        data = XmlReader.Create(nameOfFile);
                        string[] partFileNames = nameOfFile.Split('\\');
                        //saving file name so we dont read it again.
                        FileName.Add(partFileNames[partFileNames.Count() - 1]);
#if (server)
                    HttpContext.Current.Session["FileName"] = FileName;
#endif

                    }
                    catch
                    {
                        //if the generic file (eg non-alternative) is not found then we trough an error
                        if (!nameOfFile.Contains("Alternative.xml"))
                        {
                            GlobalVars.Instance.Error(nameOfFile + " not found", "in FileInformation(string nameOfFile)", false);
                            throw new System.Exception("farm Fail");
                        }
                    }
                    //start alaysy the xml-tree
                    while (data.Read())
                    {
                        if (data.NodeType == XmlNodeType.Element)
                        {
                            XElement el = XNode.ReadFrom(data) as XElement;
                            IEnumerable<XElement> node = el.Elements();
                            //running trough each sub-trees from the top node
                            for (int i = 0; i < node.Count(); i++)
                            {
                                IEnumerable<XElement> ting = node.ElementAt(i).Elements();
                                //creating a new node with value and name
                                Node newNode = new Node();
                                newNode.setNodeValue(node.ElementAt(i).Value);
                                newNode.setNodeName(node.ElementAt(i).Name.ToString());
                                //reading the nodes children
                                recursionRead(ting, ref newNode);
                                //adding the new node to the tree
                                treeOrg.addChild(newNode);


                            }
                        }
                    }

                    data.Close();
                    //file name saved
                    treeOrg.FileName = file;
                    //saving the xml-tree
                    AllNodes.Add(treeOrg);
#if (server)
                HttpContext.Current.Session["AllNodes"] = AllNodes;
#endif


                }
                catch (Exception e)
                {
                    if (nameOfFile != null)
                        if (!nameOfFile.Contains("Alternative.xml"))
                        {
                            GlobalVars.Instance.log(e.ToString(), 5);
                            if (e.Message.CompareTo("farm Fail") != 0)
                            {
                                string messageString = ("problem with reading: " + nameOfFile + " because of " + e.ToString()) + "\r\n";
                                messageString += ("model terminated") + "\r\n";
                                messageString += ("the path is: ");
                                for (int i = 0; i < PathNames.Count; i++)
                                {
                                    messageString += (PathNames[i] + "(" + Identity[i].ToString() + ")");
                                }


                                GlobalVars.Instance.Error(messageString, e.StackTrace, true);
                            }
                            else
                            {
                                throw new System.Exception("farm Fail");
                            }
                        }
                }
            }
        }
        public FileInformation(string[] nameOfFile)
        {
            // Storing the filenames
            fileNameOrg = nameOfFile[0];
            fileNameAlt = nameOfFile[1];//name.Split('.')[0] + "Alternative.xml";
            
            //reading if nessesary
            if (nameOfFile[1]!=null)
                ReadingFile(fileNameAlt);
            ReadingFile(fileNameOrg);
            //choosing the the file from memory
            if (nameOfFile[1]!=null)
                ReadingFile(fileNameAlt);
            ReadingFile(fileNameOrg);


        }

        private void recursionRead(IEnumerable<XElement> node, ref Node subNode)
        {

            for (int i = 0; i < node.Count(); i++)
            {
                //creating the new node
                IEnumerable<XElement> ting = node.ElementAt(i).Elements();
                Node child = new Node();
                //setting values and string for the new node
                child.setNodeName(node.ElementAt(i).Name.ToString());
                child.setNodeValue(node.ElementAt(i).Value);
                //reading the nodes children
                recursionRead(ting, ref child);
                //adding the node to the tree
                subNode.SubNode.Add(child);

            }

        }

       
        //checks if an ID do exist
        public bool doesIDExist(int ID)
        {
            return recursionDoesIDExist(treeOrg.SubNode, ID, 0);
        }

        //seaching down the xml tree recursively to section found 
        private bool recursionDoesIDExist(List<Node> node, int id, int iteration)
        {
            if (Identity.Count() == iteration)
            {
                for (int i = 0; i < node.Count(); i++)
                {

                    //section is found
                    if (PathNames[iteration].CompareTo(node.ElementAt(i).getNodeName().ToString()) == 0)
                    {
                        List<Node> ting = node.ElementAt(i).SubNode;
                        //testing if current section has the ID
                        if (Convert.ToInt32(ting.ElementAt(0).getNodeValue()) == id)
                            return true;
                    }
                }
            }
            else
            {
                //section not found.Finding the right sub-tree to be analyzed 
                for (int j = 0; j < node.Count(); j++)
                {
                    if (node.ElementAt(j).getNodeName().ToString().CompareTo(PathNames[iteration]) == 0)
                    {
                        //sub tree has been selected
                        List<Node> ting = node.ElementAt(j).SubNode;
                        if (ting.ElementAt(0).getNodeValue() == Identity[iteration].ToString() || Identity[iteration] == -1)
                        {
                            //returning the result from that sub-treee
                            return recursionDoesIDExist(ting, id, iteration + 1);
                        }
                    }
                }
            }
            //id not fount. 
            return false;

        }

        //getting the path in xml tree
        private string pathToString()
        {
            string returnValue = "";
            for (int i = 0; i < Identity.Count(); i++)
            {
                returnValue += PathNames[i] + "(" + Identity[i] + ")";
            }
            if (PathNames.Count > Identity.Count)
            {
                returnValue += PathNames[Identity.Count()];
            }
            return returnValue;

        }
       
        //setting the path in the xml-tree. it must be on item-name(item-id).item-name(item-id)..... if there is no ID then -1 should be used.
        public void setPath(string name)
        {
            //clearing old data
            PathNames.Clear();
            Identity.Clear();
            string[] items = name.Split('.');
            for (int i = 0; i < items.Count(); i++)
            {
                //getting the ID-number
                int first = items[i].IndexOf('(');
                int last = items[i].IndexOf(')');
                string tmp = items[i];
                if (last != -1 && first != -1)
                {
                    //store the id number if found
                    try
                    {
                        tmp = items[i].Remove(last, 1);
                        tmp = tmp.Remove(0, first + 1);
                        Identity.Add(Convert.ToInt32(tmp));
                        tmp = items[i].Remove(first);
                    }
                    catch
                    {
                        //the string is not in correct format. throwing an error
                        string messageString = (name + " is wrong") + "\n";
                        messageString += ("model terminated") + "\n";
                        messageString += ("the file name is " + fileNameOrg) + "\n";
                        GlobalVars.Instance.Error(messageString);
                    }
                }
                //store the item name
                PathNames.Add(tmp);
            }

        }

        //getting what is stored at the path as an interger
        public int getItemInt(string itemName, bool stopOnError = true)
        {
            string output = getItemString(itemName, stopOnError);
            if (output.CompareTo("nothing") != 0)
                return Convert.ToInt32(output);
            else return -1;
        }
        //getting what is stored at the path as an double. If the items contains a comma then it will hrow an error
        public double getItemDouble(string itemName, bool stopOnError = true)
        {
            string stuff = getItemString(itemName, stopOnError);
            if (stuff.Contains(","))
            {

                GlobalVars.Instance.Error("the data value for " + itemName + " contains a comma for file " + fileNameOrg + " or " + fileNameAlt);
            }
            if (stuff.CompareTo("nothing") != 0)
                return Convert.ToDouble(stuff);
            else return 0.0;
        }
        //getting what is stored at the path as a bool.
        public bool getItemBool(string itemName, bool stopOnError = true)
        {
            string stuff = getItemString(itemName, stopOnError);
            if (stuff.CompareTo("nothing") != 0)
                return Convert.ToBoolean(stuff);
            else return true;
        }

        //Finds the itemName in xml in the xml file at path and returning it as an int.
        public int getItemInt(string itemName, string path, bool stopOnError = true)
        {
            setPath(path);
            return Convert.ToInt32(getItemString(itemName, stopOnError));
        }
        //Finds the itemName in xml in the xml file at path and returning it as a Double.
        public double getItemDouble(string itemName, string path, bool stopOnError = true)
        {
            setPath(path);
            return Convert.ToDouble(getItemString(itemName, stopOnError));
        }
        //Finds the itemName in xml in the xml file at path and returning it as a bool.
       
        //Finds the itemName in xml in the xml file at path and returning it as a string.
        public string getItemString(string itemName, string path, bool stopOnError = true)
        {
            setPath(path);
            return getItemString(itemName, stopOnError);
        }
        //geting number of sections. Is not currenly in use 
       
        //getting the minimum section nr and the section nr  nr.
        public void getSectionNumber(ref int min, ref int max)
        {

            recursionForSectionNumber(treeOrg.SubNode, ref min, ref max, 0);

        }
        //seaching for the path down the xml-tree and report back how many section there is
        private bool recursionForSectionNumber(List<Node> node, ref int min, ref int max, int iteration)
        {
            if (Identity.Count() == iteration)
            {
                for (int i = 0; i < node.Count(); i++)
                {
                    //section found
                    if (PathNames[iteration].CompareTo(node.ElementAt(i).getNodeName().ToString()) == 0)
                    {
                        //finding the minimum and maximum section
                        List<Node> ting = node.ElementAt(i).SubNode;
                        if (Convert.ToInt32(ting.ElementAt(0).getNodeValue()) > max)
                            max = Convert.ToInt32(ting.ElementAt(0).getNodeValue());
                        if (Convert.ToInt32(ting.ElementAt(0).getNodeValue()) < min)
                            min = Convert.ToInt32(ting.ElementAt(0).getNodeValue());

                    }
                }
                return true;
            }
            else
            {
                //section not found. Seaching further down the xml tree
                for (int j = 0; j < node.Count(); j++)
                {
                    //match according to the path
                    if (node.ElementAt(j).getNodeName().ToString().CompareTo(PathNames[iteration]) == 0)
                    {
                        List<Node> ting = node.ElementAt(j).SubNode;
                        //seaching in the right child.
                        if (ting.ElementAt(0).getNodeValue() == Identity[iteration].ToString() || Identity[iteration] == -1)
                            if (recursionForSectionNumber(ting, ref min, ref max, iteration + 1) == true)
                                break;
                    }
                }
            }
            return false;
        }

        //seaching for itemnName
        public string getItemString(string itemName, bool stopOnError = true)
        {
            string info = "nothing";
            //seaching in the alternative file if it is present
            if (treeAlt != null)
            {
                //running trough all top level nodes to see which children that fits the criteria from the path
                for (int i = 0; i < treeAlt.SubNode.Count(); i++)
                    if (PathNames[0].CompareTo(treeAlt.SubNode[i].getNodeName()) == 0)
                    {
                        //child found. Seaching through its kids 
                        if (treeAlt.SubNode[i].SubNode.ElementAt(0).getNodeValue() == Identity[0].ToString() || Identity[0] == -1)
                            info = recursionForItem(treeAlt.SubNode[i].SubNode, itemName, 0, false);
                    }
            }

            bool found = false;
            //item found. Writing it to the log 
            if (info.CompareTo("nothing") != 0)
            {
                GlobalVars.Instance.log(pathToString() + itemName + " as " + info + "found in " + treeAlt.FileName, 6);
                found = true;
            }
            if (info.CompareTo("nothing") == 0)
            {
                //item not found. Seaching in normal file
                for (int i = 0; i < treeOrg.SubNode.Count(); i++)
                    if (PathNames[0].CompareTo(treeOrg.SubNode[i].getNodeName()) == 0)
                    {

                        if (treeOrg.SubNode[i].SubNode.ElementAt(0).getNodeValue() == Identity[0].ToString() || Identity[0] == -1)
                            info = recursionForItem(treeOrg.SubNode[i].SubNode, itemName, 0, stopOnError);
                    }
            }
            if (found == false)
                GlobalVars.Instance.log(pathToString() + itemName + " as " + info + " found in " + treeOrg.FileName, 6);

            //if the item is not found then we throw an error
            if (stopOnError == true && info.CompareTo("nothing") == 0)
            {
                string messageString = ("could not find " + itemName) + "\n";
                messageString += ("model terminated") + "\n";
                messageString += ("the path is: ");
                for (int i = 0; i < PathNames.Count; i++)
                {
                    messageString += (PathNames[i] + "(" + Identity[i].ToString() + ")");
                }

                messageString += ("the file name is " + fileNameOrg);
                GlobalVars.Instance.Error(messageString);
            }
            return info;


        }
        //seaching for and itemName recursivly 
        public string recursionForItem(List<Node> node, string itemName, int iteration, bool stopOnError)
        {
            //running trough all notes 
            for (int i = 0; i < node.Count(); i++)
            {
                //checks if we found the right child
                string tmp = node.ElementAt(i).getNodeName();
                if (node.ElementAt(0).getNodeName().ToString().CompareTo("Identity") == 0 || Identity[iteration] == -1)
                {
                    if (node.ElementAt(0).getNodeValue() == Identity[iteration].ToString() || Identity[iteration] == -1)
                    {
                        if (Identity.Count() == (iteration + 1))
                        {
                            for (int j = 0; j < node.Count(); j++)
                            {
                                if (node.ElementAt(j).getNodeName().ToString().CompareTo(itemName) == 0)
                                {
                                    if (node.ElementAt(j).getNodeValue().CompareTo("") == 0)
                                        break;
                                    else
                                    {
                                        //right child has been found. returning its value
                                        return node.ElementAt(j).getNodeValue();
                                    }
                                }
                            }
                        }
                        else
                        {
                            //the right child has not been found. 
                            //seaching for the next sub tree to be analysed
                            for (int j = 0; j < node.Count(); j++)
                            {

                                if (node.ElementAt(j).getNodeName().ToString().CompareTo(PathNames[iteration + 1]) == 0)
                                {
                                    //found the right sub tree. 
                                    List<Node> ting = node.ElementAt(j).SubNode;
                                    if (ting.ElementAt(0).getNodeValue() == Identity[iteration + 1].ToString() || Identity[iteration + 1] == -1)
                                    {
                                        //returning the result from that subtree 
                                        return recursionForItem(ting, itemName, iteration + 1, stopOnError);
                                    }
                                }

                            }
                        }

                    }
                }
            }
            //if item is not found and if it is critical that we find it then we are throwing an error 
            if (stopOnError)
            {
                string messageString = ("could not find " + itemName) + "\n";
                messageString += ("model terminated") + "\n";
                messageString += ("the path is: ");
                for (int i = 0; i < PathNames.Count; i++)
                {
                    messageString += (PathNames[i] + "(" + Identity[i].ToString() + ")");
                }

                messageString += ("the file name is " + fileNameOrg);
                GlobalVars.Instance.Error(messageString);
            }
            return "nothing";
        }

    }
