using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Qmigh.Framework.DataAccess;
using Qmigh.Framework.DataAccess.TableDefine;

namespace Crawl.Models
{
    [Map("8t_posts")]
    public class _8t_Posts : DataFieldBase
    {
        [Map("ID")]
        public int ID { get; set; }
        //上传作者ID
        [Map("post_author")]
        public int Post_Author { get; set; }
        //上传时间
        [Map("post_date")]
        public DateTime Post_Date { get; set; }
        //时间
        [Map("post_date_gmt")]
        public DateTime Post_Date_Gmt { get; set; }
        //上传内容
        [Map("post_content")]
        public string Post_Content { get; set; }
        //上传标题
        [Map("post_title")]
        public string Post_Title { get; set; }
        //上传摘抄
        [Map("post_excerpt")]
        public string Post_Excerpt { get; set; }
        //上传状态
        [Map("post_status")]
        public string Post_Status { get; set; }
        //评论状态
        [Map("comment_status")]
        public string Comment_Status { get; set; }
        //ping状态
        [Map("ping_status")]
        public string Ping_Status { get; set; }
        //上传密码
        [Map("post_password")]
        public string Post_Password { get; set; }
        //上传名字
        [Map("post_name")]
        public string Post_Name { get; set; }
        //
        [Map("to_ping")]
        public string To_Ping { get; set; }
        //
        [Map("pinged")]
        public string Pinged { get; set; }
        //
        [Map("post_modified")]
        public DateTime Post_Modified { get; set; }
        //
        [Map("post_modified_gmt")]
        public DateTime Post_Modified_Gmt { get; set; }
        //上传内容过滤
        [Map("post_content_filtered")]
        public string Post_Content_Filtered { get; set; }
        //上传父级
        [Map("post_parent")]
        public int Post_Parent { get; set; }
        //
        [Map("guid")]
        public string Guid { get; set; }
        //菜单顺序
        [Map("menu_order")]
        public int Menu_Order { get; set; }
        //上传类型
        [Map("post_type")]
        public string Post_Type { get; set; }
        //上传类型
        [Map("post_mime_type")]
        public string Post_Mime_Type { get; set; }
        //评论数
        [Map("comment_count")]
        public int Comment_Count { get; set; }
        //
        public bool IsExist { get; set; }
        


        //根据主页地址获取配置对象
        public static _8t_Posts WritePosts(string display_name, string ArtTitle, string htmlStr)
        {
            //根据昵称查询用户ID
            _8t_Posts post = new _8t_Posts();
            post.IsExist = false;
            var u = _8t_Users.SelectUser(display_name);
            //插入更新数据，并返回bool值
            if (u!=null) 
            {
                post = new _8t_Posts
                {
                    Post_Author = u.ID,
                    Post_Date = DateTime.Now,
                    Post_Date_Gmt = DateTime.Parse("2014-2-10 17:21:00"),
                    Post_Content = htmlStr,
                    Post_Title = ArtTitle.Trim(),
                    Post_Excerpt ="",
                    Post_Status = "draft",
                    Comment_Status = "open",
                    Ping_Status = "open",
                    Post_Password ="",
                    Post_Name ="",
                    To_Ping ="",
                    Pinged ="",
                    Post_Modified = DateTime.Now,
                    Post_Modified_Gmt = DateTime.Parse("2014-2-10 17:21:00"),
                    Post_Content_Filtered ="",
                    Post_Parent = 0,
                    Guid ="",
                    Menu_Order = 0,
                    Post_Type = "post",
                    Post_Mime_Type ="",
                    Comment_Count = 0,
                };
                //插入数据
                DataAccess.Insert(post);
                //更新Guid
                post.Guid = "http://www.18touch.com/?p=" + post.ID + "&preview=true";
                DataAccess.Update(post);

                post.IsExist = true;
            }

            return post;
        }


    }
}