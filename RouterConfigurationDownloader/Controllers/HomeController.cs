using RouterConfigurationDownloader.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using static RouterConfigurationDownloader.GetConfigSSH.JuniperSSH;

namespace RouterConfigurationDownloader.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var config = Config.Load();
            return View(config);
        }

        public ActionResult Create(RouterSsh nRouter)
        {
            if (Request.HttpMethod == "POST")
            {
                var config = Config.Load();
                config.Routers.Add(nRouter);
                config.Save();
                return RedirectToAction("Index");
            }
            return View();
        }
        public ActionResult Edit(string name)
        {
            var config = Config.Load();
            var tEdit = config.Routers.Where(r => r.Name == name).FirstOrDefault();
            if (tEdit != null)
                return View(tEdit);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Edit(RouterSsh nRouter)
        {
            if (Request.HttpMethod == "POST")
            {
                var config = Config.Load();
                config.Routers.Add(nRouter);
                var tEdit = config.Routers.Where(r => r.Name == nRouter.Name).FirstOrDefault();
                config.Routers[config.Routers.IndexOf(tEdit)] = nRouter;
                config.Save();
                return RedirectToAction("Index");
            }
            return View();
        }

        public ActionResult Delete(string name)
        {
            var config = Config.Load();
            var tDelete = config.Routers.Where(r => r.Name == name).FirstOrDefault();
            if (tDelete != null)
            {
                config.Routers.Remove(tDelete);
            }
            config.Save();
            return RedirectToAction("Index");
        }

        public ActionResult GetConfig(string name, string format)
        {
            //var res = RouterConfigurationDownloader.GetConfigSSH.JuniperSSH.GetConfig("lab", "lab123", "10.201.223.72",2222);
            try
            {
                var config = Config.Load();
                var tDownload = config.Routers.Where(r => r.Name == name).FirstOrDefault();
                if (tDownload != null)
                {
                    string res = null;
                    if (tDownload.RType == RouterSsh.RouterType.JunOS)
                    {
                        ConfigFormat cFormat = ConfigFormat.set;
                        Enum.TryParse<ConfigFormat>(format, out cFormat);
                        res = GetConfigSSH.JuniperSSH.GetConfig(tDownload.UserName, tDownload.UserPassword, tDownload.Address, tDownload.Port, cFormat);
                    }
                    return File(Encoding.ASCII.GetBytes(res), "plain/text", tDownload.Name+"_"+DateTime.Now.ToShortDateString()+"_config.txt");
                }
            }
            catch { }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Upload(string name, HttpPostedFileBase file)
        {
            if (file != null)
            {
                string cfg = (new System.IO.StreamReader(file.InputStream)).ReadToEnd();
                var config = Config.Load();
                var router = config.Routers.Where(r => r.Name == name).FirstOrDefault();
                if (router != null)
                {
                    var res = SetConfig(cfg, router.UserName, router.UserPassword, router.Address, router.Port);
                    Session["Alert"] = res;
                }
            }
            return RedirectToAction("Index");
        }
    }
}