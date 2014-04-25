using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Crawl.Models;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;
using System.Text;
using System.Web.UI;
using HtmlAgilityPack;

namespace Crawl.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Test/

        public ActionResult a_Art()
        {
            return View();
        }

        public ActionResult Index()
        {
            ViewBag.Items =WebConfig.Select();

            return View();
        }

        /// <summary>
        /// 获取文章标题
        /// </summary>
        /// <returns></returns>
        public ActionResult GetTitle()
        {
            //加载下拉列表的项
            ViewBag.Items = WebConfig.Select();
            //根据网站文章列表地址获取网站配置对象
            string SourUrl = Request.QueryString["UrlSelect"];
            ViewBag.ListUrl = SourUrl;
            WebConfig config = WebConfig.SelectListCon(SourUrl);
            //获取列表的Xpath表达式
            List<Title> title = null;

            if (!string.IsNullOrEmpty(SourUrl) && config != null)
            {
                title = Title.Multi_Page(SourUrl, config,2);
                if (title.Count == 0)
                {
                    title = null;
                }
            }

            return View("Index", title);
        }

        /// <summary>
        /// 获取文章
        /// </summary>
        /// <returns></returns>
        public ActionResult GetContent()
        {
            try
            {
                //获得编辑昵称
                string DisPlay_Name = Request.Form["DisPlay_Name"];
                //获取标题列表的页面地址
                string listUrl = Request.Form["ListUrl"];
                //获取文章内容页面的地址
                string ArtUrl = Request.Form["UrlValue"];

                //根据标题列表页面URL获取页面配置对象config
                WebConfig config = WebConfig.SelectListCon(listUrl);
                string[] str;
                //获取文章的html
                if (!string.IsNullOrEmpty(ArtUrl))
                {
                    //str = Request.QueryString["UrlValue"];
                    str = Request.Form.GetValues("UrlValue");
                    for (int i = 0; i < str.LongLength; i++)
                    {
                        string[] UrlTitle = str[i].Split('|');
                        string urlValue = UrlTitle[0];
                        string titleValue = UrlTitle[1];

                        //获得水印位置
                        string ImgWaterMarkLocation = Request.Form[str[i]];

                        //判断的文章链接是否完整
                        string UrlStr = PagingUrl.UpdateUrl(ArtUrl, urlValue);
                        //获取文章对象
                        Article article = Article.GetArticle(UrlStr, config, ImgWaterMarkLocation);
                        //返回替换链接后的ArticleHtml属性
                        string html = Article.UpdateArticleHtml(article,config.UpdatePath);
                        //Byte[] ch = UTF8Encoding.UTF8.GetBytes(html);
                        //保存文章
                        _8t_Posts post = _8t_Posts.WritePosts(DisPlay_Name, titleValue, html);
                        //打开转帖地址
                        string ht = "<script type='text/JavaScript'>window.open('" + post.Guid + "','_blank')</script>";

                        //Response.Redirect(post.Guid);
                        Response.Write(ht);
                        //Page.ClientScript.RegisterStartupScript(this.GetType(), "", " <script type='text/JavaScript'>window.open('" + post.Guid + "'); </script>");
                        //Helpers.DefaultBrowser(post.Guid);
                        if (!post.IsExist)
                        {
                            ViewBag.isExist = "昵称不存在";
                        }
                        else
                        {
                            ViewBag.isExist = "保存成功";
                        }
                        //Helpers.WriteLog("文章内容", html);
                    }
                }
            }
            catch (Exception ex)
            {
                DateTime time = DateTime.Now;
                Helpers.WriteLog(time.ToString(), ex.ToString());
            }      
            return View("GetTitle");
        }

        /// <summary>
        /// 单独获取一篇文章
        /// </summary>
        /// <returns></returns>
        public ActionResult Art()
        {
            string ArtUrl = Request.QueryString["ArtUrl"];
            string DisPlay_Name = Request.QueryString["DisPlay_Name"];
            string ImgWaterMark = Request.QueryString["ImgWaterMark"];

            if (!string.IsNullOrEmpty(ArtUrl) && !string.IsNullOrEmpty(DisPlay_Name))
            {
                //声明文章文档对象
                HtmlDocument doc = HtmlAgilityPackHelper.GetDocument(ArtUrl);
                //根据文章地址得到主页地址，以便于查询配置表
                string Url = Helpers.HomeUrl(ArtUrl);
                //根据主页URL获取页面配置对象config
                WebConfig config = WebConfig.SelectHomeCon(Url);
                if (config != null)
                {
                    //网页标题，做文章标题用
                    string title = Article.GetArtTitle(doc);
                    //获取文章对象
                    //Article article = Article.GetArticle(ArtUrl, config.ArtXpath, config.Location);
                    Article art = Article.GetArticle(ArtUrl, config, ImgWaterMark);

                    //返回替换链接后的ArticleHtml属性
                    string html = Article.UpdateArticleHtml(art, config.UpdatePath);
                    //保存文章
                    _8t_Posts post = _8t_Posts.WritePosts(DisPlay_Name, title, html);
                    //打开转帖地址
                    string ht = "<script type='text/JavaScript'>window.open('" + post.Guid + "','_blank')</script>";
                    Response.Write(ht);
                    //判断是否保存成功
                    if (!post.IsExist)
                    {
                        ViewBag.isExist = "昵称不存在";
                    }
                    else
                    {
                        ViewBag.isExist = "保存成功";
                    }
                }
                else
                {
                    ViewBag.isExist = "保存不成功，请检查是否支持此网站！";
                }
            }

            return View("GetTitle");
        }






        public ActionResult CheckXpath()
        {
            Article article = null;
            WebConfig config = null;
            List<string> ImgUrlList = new List<string>();
            string ArtHtmlStr = "";
            string ArtUrl = Request.QueryString["ArtUrl"];
            string DisPlay_Name = Request.QueryString["DisPlay_Name"];
            string titleValue = Request.QueryString["titleValue"];
            if (!string.IsNullOrEmpty(ArtUrl))
            {
                string[] ArtUrls = ArtUrl.Split('|');
                //根据标题列表页面URL获取页面配置对象config
                config = WebConfig.SelectListCon(ArtUrls[0]);
                for (int i = 1; i < ArtUrls.Length; i++)
                {                        
                    //获取文章对象
                    article = Article.SinglePage(ArtUrls[i], config.ArtXpath, config.Location);
                    foreach (var ls in article.ImgUrls)
                    {
                        ImgUrlList.Add(ls);
                    }
                    ArtHtmlStr += article.ArticleHtml;
                }

                //返回替换链接后的ArticleHtml属性
                article.ArticleHtml = ArtHtmlStr;
                article.ImgUrls = ImgUrlList;
                string html = Article.UpdateArticleHtml(article, config.UpdatePath);
                //保存文章
                _8t_Posts post = _8t_Posts.WritePosts(DisPlay_Name, titleValue, html);
            }


            string webUrl = Request.QueryString["webUrl"];
            string xpath = Request.QueryString["xpath"];
            if (!string.IsNullOrEmpty(webUrl) && !string.IsNullOrEmpty(xpath))
            {
                HtmlAgilityPackHelper.Node = HtmlAgilityPackHelper.GetNode(webUrl, xpath);
                ViewBag.html = HtmlAgilityPackHelper.Node.InnerHtml;
            }



            return View();
        }


        public ActionResult LoginList()
        {
            string password = Request.QueryString["password"];
            if (password == "chaohaowan")
            {
                List<WebConfig> configs = WebConfig.Select();
                return View("ConfigList", configs);
            }
            return View();
        }


        //public ActionResult ConfigList()
        //{
        //    List<WebConfig> configs = WebConfig.Select();
        //    return View("ConfigList", configs);
        //}






        public ActionResult CreateConfig(string id,string url)
        {
            ViewBag.id = id;
            ViewBag.url = url;
            WebConfig config = WebConfig.SelectHomeCon(url);
            WebConfig.Update();

            return View("CreateConfig", config);
        }



        public ActionResult Test()
        {

            return View("GetTitle");
        }

        /// <summary>
        /// 水印配置
        /// </summary>
        /// <returns></returns>
        public ActionResult WatermarkConfig()
        {
            return View();
        }


    }
}
