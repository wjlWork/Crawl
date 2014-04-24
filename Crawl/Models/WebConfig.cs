using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Qmigh.Framework.DataAccess;
using System.Xml;
using Qmigh.Framework.DataAccess.TableDefine;

namespace Crawl.Models
{
    [Map("webconfig")]
    public class WebConfig:DataFieldBase
    {
        ////ID
        //[Map("config_id")]
        //public int ConfigId { get; set; }
        //主页Url
        [Map("home_url")]
        public string HomeUrl { get; set; }

        //文章列表Url
        [Map("title_url")]
        public string ListUrl { get; set; }

        //网站名称
        [Map("web_name")]
        public string WebName { get; set; }

        //获取文章列表的Xpath
        [Map("list_xpath")]
        public string ListXpath { get; set; }

        //获取文章列表的分页Xpath
        [Map("list_page_xpath")]
        public string ListPageXpath { get; set; }

        //获取文章的Xpath
        [Map("art_xpath")]
        public string ArtXpath { get; set; }

        //分页Xpath
        [Map("page_xpath")]
        public string PageXpath { get; set; }

        //更新图片路径
        [Map("updatePath")]
        public string UpdatePath { get; set; }

        //图片水印位置
        [Map("location")]
        public string Location { get; set; }


        //查询
        public static List<WebConfig> Select()
        {
            var o = Table.Object<WebConfig>()
                         .SelectList();
            
            return o;
        }

        //根据主页地址获取配置对象
        public static WebConfig SelectHomeCon(string url)
        {
            var o = Table.Object<WebConfig>()
                         .Where(m => m.HomeUrl, url)                     
                         .SelectList().FirstOrDefault();

            return o;
        }

        //根据文章列表地址获取配置对象
        public static WebConfig SelectListCon(string url)
        {
            var o = Table.Object<WebConfig>()
                         .Where(m => m.ListUrl, url)
                         .SelectList().FirstOrDefault();

            return o;
        }

        //增加、修改
        public static void Update()
        {
            string SouUrl = HttpContext.Current.Request.QueryString["SouUrl"];
            string ListUrl = HttpContext.Current.Request.QueryString["ListUrl"];
            string WebName = HttpContext.Current.Request.QueryString["WebName"];
            string ListReg = HttpContext.Current.Request.QueryString["ListReg"];
            string ListPageReg = HttpContext.Current.Request.QueryString["ListPageReg"];
            string ArtReg = HttpContext.Current.Request.QueryString["ArtReg"];
            string PageReg = HttpContext.Current.Request.QueryString["PageReg"];
            string UpdatePath = HttpContext.Current.Request.QueryString["UpdatePath"];
            string Location = HttpContext.Current.Request.QueryString["Location"];
            if (!string.IsNullOrEmpty(SouUrl))
            {
                var o = Table.Object<WebConfig>()
                             .Where(m => m.HomeUrl, SouUrl)
                             .Where(m => m.WebName, WebName)
                             .SelectList().FirstOrDefault();

                if (o != null)
                {
                    o.ListUrl = ListUrl;
                    o.WebName = WebName;
                    o.ListXpath = ListReg;
                    o.ListPageXpath = ListPageReg;
                    o.ArtXpath = ArtReg;
                    o.PageXpath = PageReg;
                    o.UpdatePath = UpdatePath;
                    o.Location = Location;
                    DataAccess.Update(o);
                }
                else
                {
                    WebConfig configs = new WebConfig
                    {
                        HomeUrl = SouUrl,
                        ListUrl = ListUrl,
                        WebName = WebName,
                        ListXpath = ListReg,
                        ListPageXpath = ListPageReg,
                        ArtXpath = ArtReg,
                        PageXpath =PageReg,
                        UpdatePath =UpdatePath,
                        Location = Location
                    };

                    DataAccess.Insert(configs);
                }
            }

        }

        /// <summary>
        /// 获得下来列表的项集合
        /// </summary>
        /// <returns></returns>
        public static List<WebConfig> SelectItem()
        {
            List<WebConfig> item = WebConfig.Select();
            return item;
        }



    }
}