using Qiniu.IO;
using Qiniu.RS;
using Qiniu.Conf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Crawl.Models;

namespace Crawl.Models
{
    public class QiniuPut
    {

        /// <summary>
        /// 上传文件测试
        /// </summary>
        /// <param name="bucket">接入凭证</param>
        /// <param name="key">上传后文件key</param>
        /// <param name="fname">需上传的本地文件</param>
        public static void PutFile(string bucket, string key, string fname)
        {
            //签名认证
            Config.ACCESS_KEY = "GIr2Wg1e0nfljMbEs696Q1v80KyXqyiaeH5iakpk";
            Config.SECRET_KEY = "ncUQYj9zogA-X-5yfE-IytIJSov0xZcpGYywOkA3";
            //初始化对象
            IOClient target = new IOClient();
            PutPolicy put = new PutPolicy(bucket);
            //string tok = put.Token();
            PutExtra extra = new PutExtra(); // TODO: 初始化为适当的值
            extra.MimeType = "text/plain";
            extra.Crc32 = 123;
            extra.CheckCrc = CheckCrcType.CHECK;
            extra.Params = new System.Collections.Generic.Dictionary<string, string>();

            //无论成功或失败，上传结束时触发的事件
            target.PutFinished += new EventHandler<PutRet>((o, ret) =>
            {
                if (ret.OK)
                {
                    Helpers.WriteLog("Hash: ", ret.Hash);
                }
                else
                {
                    Helpers.WriteLog("Failed to PutFile","");
                }
            });
            //上传文件
            target.PutFile(put.Token(), key, fname, extra);

        }





    }
}