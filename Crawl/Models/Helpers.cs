using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using Microsoft.Win32;
using System.Diagnostics;
using HtmlAgilityPack;
using Mozilla.NUniversalCharDet;

namespace Crawl.Models
{
    public class Helpers
    {
        //因为网页编码方式有gbk和utf8，所以做一份utf8副本和一份gbk副本，然后将utf8转换为bytes，判断bytes内是否有乱码标识（连续三个byte表示为239 191 189）
        public static string getHtml(string url, string charSet)
        {
            Byte[] pageData = null;
            try
            {
                if (url == null || url.Trim() == "")
                    return null;
                XWebClient wc = new XWebClient();
                wc.Credentials = CredentialCache.DefaultCredentials;
                wc.Headers["User-Agent"] = "blah";
                pageData = wc.DownloadData(url);

            }
            catch (WebException ex)
            {
                Helpers.WriteLog("请求期间异常", "请检查网络" + ex.ToString());
            }
            if (pageData == null)
                return null;

            //如果编码为空，则采用自动探测编码
            if (charSet == null || string.IsNullOrEmpty(charSet))
            {
                charSet = DetectEncoding_Bytes(pageData);
            }

            return Encoding.GetEncoding(charSet).GetString(pageData);
        }


        //自动探测编码
        public static string DetectEncoding_Bytes(byte[] DetectBuff)
        {
            //int nDetLen = 0;
            UniversalDetector Det = new UniversalDetector(null);
            //while (!Det.IsDone())
            {
                Det.HandleData(DetectBuff, 0, DetectBuff.Length);
            }
            Det.DataEnd();
            if (Det.GetDetectedCharset() != null)
            {
                return Det.GetDetectedCharset();
            }

            return "utf-8";
        }

        /// <summary>
        /// 判断是否有乱码
        /// </summary>
        /// <param name="txt">需判断的文本</param>
        /// <returns></returns>
        private static bool isLuan(string txt)
        {
            var bytes = Encoding.UTF8.GetBytes(txt);
            //239 191 189
            for (var i = 0; i < bytes.Length; i++)
            {
                if (i < bytes.Length - 3)
                    if (bytes[i] == 239 && bytes[i + 1] == 191 && bytes[i + 2] == 189)
                    {
                        return true;
                    }
            }
            return false;
        }


        //记录log
        public static void WriteLog(string mesaage, string receive)
        {
            string path = System.Web.HttpContext.Current.Server.MapPath("~/Sources/wx.log");
            FileStream stream = new FileStream(path, FileMode.Append);
            string data = DateTime.Now + mesaage + ":" + receive;
            StreamWriter writer = new StreamWriter(stream, Encoding.Default);
            writer.WriteLine(data);
            writer.Close();
            stream.Close();
        }

        //路径
        public static string SetPath(string Path)
        {
            string[] strs = Path.Split('/');
            string headPath = "~/" + strs[1];
            string rootPath = System.Web.HttpContext.Current.Server.MapPath(headPath);
            if (!Directory.Exists(rootPath))
            {
                Directory.CreateDirectory(rootPath);
            }
            string PathStr = System.Web.HttpContext.Current.Server.MapPath(Path);

            return PathStr;
        }

        //文件路径
        public static string FillPath(string durl, string SouPath)
        {
            string Path = Helpers.SetPath(SouPath);
            string[] str = durl.Split('/');
            int i = str.Length;
            string filePath = Path + "/" + str.GetValue(i - 1);

            return filePath;
        }

        //图片下载存放路径
        public static string PicPath(string durl, string SouPath)
        {
            string Path = Helpers.SetPath(SouPath);
            string[] str = durl.Split('/');
            int i = str.Length;

            string part = DateTime.Now.ToString("MM") + DateTime.Now.ToString("dd") + DateTime.Now.ToString("hh") + DateTime.Now.ToString("mm") + DateTime.Now.ToString("ss");
            string filePath = Path + "/" + part + "_" + str.GetValue(i - 1);

            return filePath;
        }

        //图片最后存储的位置,并返回
        public static List<string> StoragePic(string FilePath)
        {
            List<string> files = new List<string>();
            //文件夹路径,没有就创建文件夹
            string ChildFile = "Cr_" + DateTime.Now.ToString("yyyy") + DateTime.Now.ToString("MM") + DateTime.Now.ToString("dd");
            string path = System.Web.HttpContext.Current.Server.MapPath("~/image");
            path = path + '/' + ChildFile;
            if (!Directory.Exists(path))//如果不存在就创建file文件夹 
            {
                Directory.CreateDirectory(path);//创建该文件夹 
            }

            //构造新路径
            string[] str = FilePath.Split('/');
            int i = str.Length;
            string NewPath = path + "/" + str.GetValue(i - 1);
            //构造上传七牛时的key
            string key = ChildFile + "/" + str.GetValue(i - 1);
            files.Add(key);
            files.Add(NewPath);
            //移动到新位置
            if (System.IO.File.Exists(NewPath))//如果文件存在则删除
            {
                System.IO.File.Delete(NewPath);
            }
            System.IO.File.Move(FilePath, NewPath);

            return files;
        }
        //调用IE打开URL
        public static void IE(string url)
        {
            Process.Start("iexplore.exe", url);
        }
        //调用默认浏览器打开URL
        public static void DefaultBrowser(string url)
        {
            try
            {
                RegistryKey key = Registry.ClassesRoot.OpenSubKey(@"http\shell\open\command");
                string s = key.GetValue("").ToString();

                Regex reg = new Regex("\"([^\"]+)\"");
                MatchCollection matchs = reg.Matches(s);

                string browser = "";
                if (matchs.Count > 0)
                {
                    browser = matchs[0].Groups[1].Value;
                    Process.Start(browser, url);
                }
            }
            catch (Exception ex)
            {
                Helpers.WriteLog("", ex.ToString());
            }
          
        }



        ////修正路径，将相对路径更新为绝对路径
        //public static string UpdateUrl111(string sour_url, string herf)
        //{
        //    string url = "";

        //    if (herf.Contains("http://"))
        //    {
        //        url = herf;
        //    }
        //    else
        //    {
        //        if (herf.Substring(0, 1) == "/")
        //        {
        //            //主页
        //            string[] us = sour_url.Split('/');
        //            string u = us[0] + "/" + us[1] + "/" + us[2];
        //            //
        //            url = u + herf;
        //        }
        //        else if (herf.Substring(0, 1) == ".")
        //        {
        //            if (herf.Substring(0, 2) == "./")
        //            {
        //                //主页
        //                string[] us = sour_url.Split('/');
        //                string u = "";
        //                for (int i = 0; i < us.Length - 1; i++)
        //                {
        //                    u += us[i] + "/";
        //                }
        //                int count = herf.Length;
        //                url = u + herf.Substring(2, count - 2);

        //            }
        //            else if (herf.Substring(0, 3) == "../")
        //            {
        //                //主页
        //                string[] us = sour_url.Split('/');
        //                string u = "";
        //                for (int i = 0; i < us.Length - 2; i++)
        //                {
        //                    u += us[i] + "/";
        //                }
        //                int count = herf.Length;
        //                url = u + herf.Substring(3, count - 3);
        //            }
        //        }
        //        else
        //        {
        //            if (herf.Contains(".html") || herf.Contains(".php") || herf.Contains(".aspx") || herf.Contains("/"))
        //            {
        //                //主页
        //                string[] us = sour_url.Split('/');
        //                string u = "";
        //                for (int i = 0; i < us.Length - 1; i++)
        //                {
        //                    u += us[i] + "/";
        //                }
        //                url = u + herf;
        //            }
        //        }
        //    }
        //    return url;
        //}









        /// <summary>
        /// 将列表URL或文章URL筛选为主页URL
        /// </summary>
        /// <url>
        /// 需筛选的URL
        /// </url>
        /// <returns></returns>
        public static string HomeUrl(string url)
        {
            string[] strs = url.Split('/');
            url = strs[0] + "/" + strs[1] + "/" + strs[2];
            return url;
        }


        public static string SourUrl(string SourUrl)
        {
            string[] u = SourUrl.Split('/');
            string url = u[0] + "/" + u[1] + "/" + u[2] + "/";

            return url;
        }

        //
        public static string StrucUrl(string str)
        {
            string[] strs = str.Split('/');
            string Ostr = strs[0];
            string value = "";
            if (Ostr == ".." || Ostr.Equals(".."))
            {
                for (int i = 1; i < strs.Length; i++)
                {
                    value += "/" + strs[i];
                }
            }
            else
            {
                value = "/" + str;
            }
            return value;
        }

    }
}