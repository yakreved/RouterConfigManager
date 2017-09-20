using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageConfigsConsole
{
    static class Program
    {
        static void Main(string[] args)
        {
            string path = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "ConfigurationExamples\\stanza.txt");
            string config = File.ReadAllText(path);
            RouterConfigurationDownloader.GetConfigSSH.JuniperSSH.SetConfig(config, "lab", "lab123", "192.168.128.154", 22);
            Console.ReadKey();
        }
    }
}
