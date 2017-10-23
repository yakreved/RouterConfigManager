using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Script.Serialization;

namespace RouterConfigurationDownloader.Models
{
    public sealed class Config
    {
        public List<RouterSsh> Routers { get; set; } = new List<RouterSsh>();
        
        public static Config Load()
        {
            try
            {
                var fileContents = System.IO.File.ReadAllText(HostingEnvironment.MapPath(@"~/routers.txt"));
                return new JavaScriptSerializer().Deserialize<Config>(fileContents);
            }
            catch { }
            return new Config();
        }
        public void Save()
        {
            var serialized = new JavaScriptSerializer().Serialize(this);
            System.IO.File.WriteAllText(HostingEnvironment.MapPath(@"~/routers.txt"), serialized);
        }
    }

    public class RouterSsh
    {
        public enum RouterType { JunOS, CISCO, NetScreen }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Address { get; set; }
        public int Port { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        public string UserPassword { get; set; }
        public RouterType RType { get;set;}

        public List<string> GetFormats()
        {
            if (this.RType == RouterType.JunOS)
                return new List<string>() { "stanza", "set", "xml" };
            else return new List<string>() { "cli" };
        }
    }
}