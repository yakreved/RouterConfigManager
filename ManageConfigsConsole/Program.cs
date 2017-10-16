using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ManageConfigsConsole
{
    static class Program
    {
        static void Main(string[] args)
        {
            string path = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "ConfigurationExamples\\stanza.txt");
            string config = File.ReadAllText(path);
            string path2 = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "ConfigurationExamples\\srx1_conf_01_stanza.txt");
            string configBroken = File.ReadAllText(path2);
            string res = RouterConfigurationDownloader.GetConfigSSH.JuniperSSH.SetConfig(configBroken, "lab", "lab123", "192.168.128.137", 22);
            Console.WriteLine(res);
            Assert.IsFalse(res.Contains("errors"));
            Console.ReadKey();
        }
    }
}
