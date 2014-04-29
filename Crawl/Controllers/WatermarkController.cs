using Crawl.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Crawl.Controllers
{
    public class WatermarkController : Controller
    {
        //
        // GET: /Watermark/

        public ActionResult Index()
        {
            List<WebConfig> wConfs = WebConfig.Select();
            return View("WatermarkConfig", wConfs);
        }

        public ActionResult WatermarkConfig()
        {
            string ListUrl = Request.QueryString["SiteName"];
            int watermark_X = int.Parse(Request.QueryString["Watermark_X"]);
            int watermark_Y = int.Parse(Request.QueryString["Watermark_Y"]);
            int watermark_H = int.Parse(Request.QueryString["Watermark_H"]);
            int watermark_W = int.Parse(Request.QueryString["Watermark_W"]);

            if (ListUrl != null)
            {
                WebConfig wConf = WebConfig.ConfigFormUrl(ListUrl);
                if (wConf != null)
                {
                    wConf.Watermark_X = watermark_X;
                    wConf.Watermark_Y = watermark_Y;
                    wConf.Watermark_H = watermark_H;
                    wConf.Watermark_W = watermark_W;
                }
                WebConfig.Update(wConf);
            }
            else
            {
                List<WebConfig> wConfs = WebConfig.Select();
                return View(wConfs);
            }
            return View("WatermarkConfig");
        }



    }
}
