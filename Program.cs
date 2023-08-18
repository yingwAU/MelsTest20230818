using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using System.Xml.Linq;
using System.Diagnostics;
namespace FarmGHGcalc
{
    class Program
    {
        //args[0] and args[1] are farm number and scenario number respectively
        static void Main(string[] args)
        {
            model mod = new model();
            mod.run(args);
        }
    }
}
// this is first test for github yingAU "MelsTest"
// test github desktop to connect local files 2023/08/18