using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HtmlAgilityPack;

namespace Crawl.Models
{
    public class HtmlAgilityPackHelper
    {
        public static HtmlNode Node { get; set; }

        public static HtmlNodeCollection NodeCollection { get; set; }


        /// <summary>
        /// 获得html代码的节点
        /// </summary>
        /// <param name="url"></param>
        /// <param name="xpath"></param>
        /// <returns></returns>
        public static HtmlNode GetNode(string url,string xpath)
        {
            //获取html源码
            string htmlStr = Helpers.getHtml(url,"");
            //声明变量
            HtmlNode navNode = null;
            if (!string.IsNullOrEmpty(htmlStr))
            {
                //实例化HtmlAgilityPack.HtmlDocument对象
                HtmlDocument doc = new HtmlDocument();
                //载入HTML
                doc.LoadHtml(htmlStr);

                if (xpath != null)
                {
                    //根据节点
                    navNode = doc.DocumentNode.SelectSingleNode(xpath);
                }
            }
            return navNode;
        }

        /// <summary>
        /// 获得html代码块的节点集合
        /// </summary>
        /// <param name="url"></param>
        /// <param name="xpath"></param>
        /// <returns></returns>
        public static HtmlNodeCollection GetHtmlNodes(string url,string xpath)
        {
            HtmlNodeCollection navNodes = null;
            if (xpath != null)
            {
                try
                {
                    //获取html源码
                    string htmlStr = Helpers.getHtml(url, "");
                    //实例化HtmlAgilityPack.HtmlDocument对象
                    HtmlDocument doc = new HtmlDocument();
                    //载入HTML
                    doc.LoadHtml(htmlStr);

                    //根据Xpath节点NODE的ID获取节点集
                    navNodes = doc.DocumentNode.SelectNodes(xpath);
                }
                catch (Exception ex)
                {
                    Helpers.WriteLog("获取节点集代码异常", ex.ToString());
                }
            }
            return navNodes;
        }


        /// <summary>
        /// 获得html代码块的节点集合
        /// </summary>
        /// <param name="url"></param>
        /// <param name="xpath"></param>
        /// <returns></returns>
        public static HtmlNodeCollection _GetHtmlNodes(HtmlDocument doc, string xpath)
        {
            HtmlNodeCollection navNodes = null;
            try
            {
                if (xpath != null)
                {
                    //根据Xpath节点NODE的ID获取节点集
                    navNodes = doc.DocumentNode.SelectNodes(xpath);
                }
            }
            catch (Exception ex)
            {
                Helpers.WriteLog("获取节点集代码异常", ex.ToString());
            }
            return navNodes;
        }

        /// <summary>
        /// 获得一个节点
        /// </summary>
        /// <param name="doc">文档对象</param>
        /// <param name="xpath">查找路径</param>
        /// <returns></returns>
        public static HtmlNode _GetNode(HtmlDocument doc, string xpath)
        {
            //根据节点
            HtmlNode navNode = null;
            try
            {
                navNode = doc.DocumentNode.SelectSingleNode(xpath);
            }
            catch (Exception ex)
            {
                Helpers.WriteLog(ex.ToString(), "Log\\error.log");
            }
            return navNode;
        }



        public static HtmlDocument GetDocument(string url)
        {
            //获取html源码
            string htmlStr = Helpers.getHtml(url, "");
            //声明变量
            HtmlDocument doc = null;
            if (!string.IsNullOrEmpty(htmlStr))
            {
                //实例化HtmlAgilityPack.HtmlDocument对象
                doc = new HtmlDocument();
                //载入HTML
                doc.LoadHtml(htmlStr);
            }
            return doc;
        }













    }
}