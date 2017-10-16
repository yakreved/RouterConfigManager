using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Globalization;
using System.Net;
using System.Web.Hosting;

namespace RouterConfigurationDownloader.GetConfigSSH
{
    public static class JuniperSSH
    {
        public enum ConfigFormat
        {
            stanza,
            set,
            xml
        }

        public static string GetConfig(string user, string password, string address, int port=22, ConfigFormat format = ConfigFormat.stanza)
        {
            using (var client = new SshClient(address, port, user, password))
            {
                client.Connect();
                //var stream = client.CreateShellStream("customCommand", 200, 24, 5000, 600, 1024);
                //SendCommand(stream, "configure");
                //var res = SendCommand(stream,"show | no-more");
                SshCommand cmd = null;
                if(format!= ConfigFormat.stanza)
                    cmd = client.RunCommand("configure; show | no-more | display "+ format.ToString());
                else cmd = client.RunCommand("configure; show | no-more");
                string res = cmd.Result;
                var lines = Regex.Split(res, "\r\n|\r|\n").Skip(6);
                return string.Join(Environment.NewLine, lines.ToArray());
            }
        }

        static IEnumerable<string> ChunksUpto(string str, int maxChunkSize)
        {
            for (int i = 0; i < str.Length; i += maxChunkSize)
                yield return str.Substring(i, Math.Min(maxChunkSize, str.Length - i));
        }

        private const string EOC = "*-* COMMAND DELIMITER *-*";
        public static string SetConfig(string config, string user, string password, string address, int port = 22)
        {
            var cfgTempFilePath = HostingEnvironment.MapPath(@"~/cfg.txt");
            File.WriteAllText(cfgTempFilePath,config);

            try
            {

                using (var client = new SshClient(address, port, user, password))
                {
                    client.ConnectionInfo.Timeout = TimeSpan.FromSeconds(10);
                    client.Connect();
                    var stream = client.CreateShellStream("customCommand", 200, 24, 5000, 600, 4096);
                    stream.WriteLine("configure");
                    stream.Flush();
                    //stream.WriteLine(EOC);
                    string res = stream.Expect(user);
                    //Console.WriteLine(res);

                    stream.WriteLine("load override ftp://win2012-iis.bell-main.bellintegrator.ru/cfg.txt");
                    stream.Flush();

                    Thread.Sleep(2000);
                    res = "\n===================================================\n" + stream.Expect("load complete");
                    Console.WriteLine(res);
                    var lines = Regex.Split(res, "\r\n|\r|\n");
                    return string.Join(Environment.NewLine, lines.ToArray());
                }
            }
            catch (Exception e)
            {
                return "Router unreachable";
            }

        }

    }
}