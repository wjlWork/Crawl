using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Crawl.Models
{
    public class PagingUrl
    {


        /// <summary>
        /// 获取下一页地址
        /// </summary>
        /// <param name="a_PageUrl">第一页地址</param>
        /// <param name="config">配置对象</param>
        /// <returns></returns>
        public static string NextPage(string a_PageUrl, string config_Xpath)
        {
            //下一页
            string Variable = "";
            string nextPage = null;
            HtmlNodeCollection nodes = HtmlAgilityPackHelper.GetHtmlNodes(a_PageUrl, config_Xpath);

            try
            {
                if (nodes != null)
                {
                    foreach (var n in nodes)
                    {
                        if (n.Attributes.Count == 0 || n.Attributes["href"] == null)
                        {
                        }
                        else
                        {
                            string u = n.Attributes["href"].Value;
                            if (u != null || !string.IsNullOrEmpty(u))
                            {
                                if (n.InnerText.Contains("下一页") || n.InnerText == "下一页" || n.InnerText.Equals("下一页") || n.InnerText.Equals("&gt;") || n.InnerText.Contains("下页"))
                                {
                                    Variable = UpdateUrl(a_PageUrl, u);
                                    if (a_PageUrl != Variable)
                                    {
                                        nextPage = Variable;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                DateTime time = DateTime.Now;
                Helpers.WriteLog(time.ToString() + ":" + ex.ToString(), "Log\\error.log");
            }

            return nextPage;
        }


        //修正路径，将相对路径更新为绝对路径
        public static string UpdateUrl(string sour_url, string herf)
        {
            string url = "";

            if (herf.Contains("http://"))
            {
                url = herf;
            }
            else
            {
                if (herf.Substring(0, 1) == "/")
                {
                    //主页
                    string[] us = sour_url.Split('/');
                    string u = us[0] + "/" + us[1] + "/" + us[2];
                    //
                    url = u + herf;
                }
                else if (herf.Substring(0, 1) == ".")
                {
                    if (herf.Substring(0, 2) == "./")
                    {
                        //主页
                        string[] us = sour_url.Split('/');
                        string u = "";
                        for (int i = 0; i < us.Length - 1; i++)
                        {
                            u += us[i] + "/";
                        }
                        int count = herf.Length;
                        url = u + herf.Substring(2, count - 2);

                    }
                    else if (herf.Substring(0, 3) == "../")
                    {
                        //主页
                        string[] us = sour_url.Split('/');
                        string u = "";
                        for (int i = 0; i < us.Length - 2; i++)
                        {
                            u += us[i] + "/";
                        }
                        int count = herf.Length;
                        url = u + herf.Substring(3, count - 3);
                    }
                }
                else
                {
                    if (herf.Contains(".html") || herf.Contains(".php") || herf.Contains(".aspx") || herf.Contains("/"))
                    {
                        //主页
                        string[] us = sour_url.Split('/');
                        string u = "";
                        for (int i = 0; i < us.Length - 1; i++)
                        {
                            u += us[i] + "/";
                        }
                        url = u + herf;
                    }
                }
            }
            return url;
        }











    }
}