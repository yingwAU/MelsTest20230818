using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

    //contains a node in the xml tree
    public class Node
    {
        //name of that node
        private string nodeName;
        //value of that note
        private string nodeValue;
        //file where it was found
        public string FileName;
        //all children of that node
        public List<Node> SubNode;

        public Node()
        {
            SubNode = new List<Node>();
        }
       
        //addding a child to that node
        public void addChild(Node newNode)
        {
            SubNode.Add(newNode);
        }
        public void setNodeName(string name)
        {
            nodeName = name;
        }
        public string getNodeName()
        {
           return  nodeName;
        }
        public void setNodeValue(string value)
        {
            nodeValue = value;
        }
        public string getNodeValue()
        {
            return nodeValue;
        }
    }
