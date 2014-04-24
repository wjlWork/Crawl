using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HtmlAgilityPack;
using System.Net;
using System.IO;

namespace Crawl.Models
{
    public class Article
    {
        //文章url
        public string Url { get; set; }
        //图片url集合
        public List<string> ImgUrls { get; set; }
        //修改图片连接后的url集合
        public List<string> PicUrls { get; set; }
        //文章代码块
        public string ArticleHtml { get; set; }


        
        public static Article GetArticle(string a_PageUrl, WebConfig config,string Location)
        {
            Article article = new Article();
            HtmlNodeCollection nodes = HtmlAgilityPackHelper.GetHtmlNodes(a_PageUrl, config.PageXpath);
            if (nodes != null)
            {
                article = MultiPage(a_PageUrl, config, Location);
            }
            else
            {
                article = SinglePage(a_PageUrl, config.ArtXpath,Location);
            }

            return article;
        }

        /// <summary>
        /// 获取单页文章
        /// </summary>
        /// <param name="SourUrl">文章所在Url</param>
        /// <param name="xpath">获取文章代码块的xpath语句</param>
        /// <param name="Location">获取文章图片时添加的水印位置</param>
        public static Article SinglePage(string SourUrl, string xpath, string Location)
        {
            Article article = new Article();
            article.Url = SourUrl;
            HtmlNode node = HtmlAgilityPackHelper.GetNode(SourUrl, xpath);
            //node.SetAttributeValue("scr", "wwwwwwww");
            article.ArticleHtml = node.InnerHtml;
            HtmlDocument doc = new HtmlDocument();
            //再次载入HTML
            doc.LoadHtml(node.InnerHtml);
            HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//img");
            article.ImgUrls = new List<string>();
            article.PicUrls = new List<string>();

            try
            {
                foreach (var n in nodes)
                {
                    
                    string src = n.Attributes["src"].Value;
                    //得到完整的链接
                    string UrlStr = PagingUrl.UpdateUrl(SourUrl,src);
                    //下载图片
                    string picUrl = DownImg(UrlStr, Location);
                    //图片链接
                    article.ImgUrls.Add(UrlStr);
                    article.PicUrls.Add(picUrl);
                    //html内容
                    //string url = RecordLog.FillPath(UrlStr);
                    //HtmlNode nn = doc.DocumentNode.SelectSingleNode(node.InnerHtml);
                    //nn.SetAttributeValue("src", url);
                    //article.ArticleHtml = node.InnerHtml;
                }
            }
            catch (Exception ex)
            {
                DateTime time = DateTime.Now;
                Helpers.WriteLog(time.ToString()+":"+"获取文章失败，请检查XPath", ex.ToString());
            }
            //article.ArticleHtml = nodes.InnerHtml;
            return article;
        }



        /// <summary>
        /// 获得分页URL,支持有下页
        /// </summary>
        /// <param name="a_Page">第一页地址</param>
        /// <returns></returns>
        public static Article MultiPage(string a_PageUrl, WebConfig config, string Location)
        {
            //第一页
            Article article = SinglePage(a_PageUrl, config.ArtXpath, Location);
            Article art = new Article();
            string text = article.ArticleHtml;
            List<string> iu = article.ImgUrls;
            //下一页
            string nextPage = PagingUrl.NextPage(a_PageUrl, config.PageXpath);
            while (nextPage != null && !string.IsNullOrEmpty(nextPage))
            {
                art = SinglePage(nextPage, config.ArtXpath,Location);
                text += art.ArticleHtml;
                iu.AddRange(art.ImgUrls);
                nextPage = PagingUrl.NextPage(a_PageUrl, config.PageXpath);
            }

            article.ArticleHtml = text;
            article.ImgUrls = iu;
            return article;
        }


        /// <summary>
        /// 修改后的文章代码块
        /// </summary>
        /// <returns></returns>
        public static string UpdateArticleHtml(Article article,string updatePath)
        {
            List<string> ls = new List<string> ();
            string xurl = article.ArticleHtml;
            for (int i = 0; i < article.ImgUrls.Count; i++)
            {
                ls = article.PicUrls;
                string[] str = ls[i].Split('/');
                int ii = str.Length;
                //上传图片时的目录
                string ChildFile = "Cr_" + DateTime.Now.ToString("yyyy") + DateTime.Now.ToString("MM") + DateTime.Now.ToString("dd");
                string filePath = updatePath + "/" + ChildFile + "/" + str.GetValue(ii - 1);
                //替换图片链接
                xurl = xurl.Replace(article.ImgUrls[i], filePath);

            }

            return xurl;          
        }

        /// <summary>
        /// 下载文章图片并加水印
        /// </summary>
        /// <param name="PicUrl">图片地址</param>
        /// <param name="Location">添加水印的位置</param>
        public static string DownImg(string PicUrl, string Location)
        {
            WebClient client = new WebClient(); 
            string FilePath = Helpers.PicPath(PicUrl, "~/image");
            

            client.DownloadFile(new Uri(PicUrl), FilePath);

            //给图片加水印
            ImageWaterMark Iwm = new ImageWaterMark ();
            string IwmPath = System.Web.HttpContext.Current.Server.MapPath("~/image/uploadfile/ImageWater.jpg");
            string NewPath = Helpers.FillPath(PicUrl, "~/image/uploadfile");
            //高斯模糊处理掉原有的水印
            GdipEffect.AddGdipPic(FilePath, IwmPath, Location);
            //添加水印
            Iwm.addWaterMark(FilePath, NewPath, WaterMarkType.ImageMark, IwmPath, Location);
            //移动文件，先删除原件再移动加水印的文件
            if (System.IO.File.Exists(FilePath))
            {
                try
                {
                    System.IO.File.Delete(FilePath);
                }
                catch (System.IO.IOException e)
                {
                    Console.WriteLine(e.Message);
                }
            }

            System.IO.File.Move(NewPath, FilePath);
            //图片最终位置
            List<string> PutPaths = Helpers.StoragePic(FilePath);
            //图片上传到七牛云
            QiniuPut.PutFile("chaohaowan", PutPaths[0], PutPaths[1]);

            return PutPaths[0];
        }



        /// <summary>
        /// 修改后的文章代码块
        /// </summary>
        /// <returns></returns>
        public static string GetArtTitle(HtmlDocument doc)
        {
            //
            string title = "";
            //获得网页标题节点
            HtmlNode node = HtmlAgilityPackHelper._GetNode(doc, "//title");

            //如果节点不为空
            if (node != null)
            {
                title = node.InnerText;
            }

            return title;
        }













    }
}