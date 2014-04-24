using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Qmigh.Framework.DataAccess;
using Qmigh.Framework.DataAccess.TableDefine;

namespace Crawl.Models
{
    [Map("8t_users")]
    public class _8t_Users : DataFieldBase
    {
        [Map("ID")]
        public int ID { get; set; }

        [Map("display_name")]
        public string DisPlayName { get; set; }


        //根据昵称查找用户ID
        public static _8t_Users SelectUser(string display_name)
        {
            var o = Table.Object<_8t_Users>()
                         .Where(m => m.DisPlayName, display_name)
                         .SelectList().FirstOrDefault();
            return o;
        }





    }
}