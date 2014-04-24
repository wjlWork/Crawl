using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;
using System.Collections;
using HtmlAgilityPack;

namespace Crawl.Models
{
    public class Title
    {
        //文章URL
        public string Url { get; set; }
        //文章标题
        public string TitleName { get; set; }
        //
        public string HtmlStr { get; set; }


        /// <summary>
        /// 获得全部列表页面的文章地址
        /// </summary>
        /// <param name="a_PageUrl">列表第一页</param>
        /// <param name="config">配置对象</param>
        /// <param name="page">获取多少页</param>
        /// <returns></returns>
        public static List<Title> Multi_Page(string a_PageUrl, WebConfig config,int page)
        {
            //计数器
            int count = 1;
            //第一页
            List<Title> tls = new List<Title>();
            List<Title> ls = GetTitles(a_PageUrl, config);
            tls = ls;
            //下一页
            string nextUrl = PagingUrl.NextPage(a_PageUrl, config.ListPageXpath);
            while ((nextUrl != null || !string.IsNullOrEmpty(nextUrl)) && (count < page|| page == 0))
            {
                ls = GetTitles(nextUrl, config);
                tls.AddRange(ls);
                nextUrl = PagingUrl.NextPage(a_PageUrl, config.ListPageXpath);

                count++;
            }
            return tls;
        }




        //获得Title对象集合
        public static List<Title> GetTitles(string ListUrl, WebConfig config)
        {
            List<Title> titleList = new List<Title>();
            HtmlNodeCollection nodes = HtmlAgilityPackHelper.GetHtmlNodes(ListUrl, config.ListXpath);

            try
            {
                foreach (var n in nodes)
                {
                    Title t = new Title();
                    //获得文章地址
                    string href = n.Attributes["href"].Value;
                    t.Url = PagingUrl.UpdateUrl(ListUrl, href);//对相对路径的情况进行处理
                    t.TitleName = n.InnerText;
                    t.HtmlStr = n.InnerHtml;
                    titleList.Add(t);
                }
            }
            catch (Exception ex)
            {
                DateTime time = DateTime.Now;
                Helpers.WriteLog(time.ToString(), ex.ToString());
            }
            return titleList;
        }





    }
}