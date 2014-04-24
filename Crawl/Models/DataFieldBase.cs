using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Qmigh.Framework.DataAccess;

namespace Crawl.Models
{
    public abstract class DataFieldBase : TracableObject
    {
        private string _data;

        [Map("data")]
        public string Data
        {
            get { return _data; }
            set
            {
                _data = value;
                _datax = null;
            }
        }

        private Dictionary<string, string> _datax;
        public Dictionary<string, string> DataX
        {
            get
            {
                if (_datax != null) return _datax;
                if (!string.IsNullOrWhiteSpace(Data))
                {
                    try
                    {
                        _datax = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(Data);
                    }
                    catch
                    {
                    }
                }
                return _datax ?? (_datax = new Dictionary<string, string>());
            }
            set { _datax = value ?? new Dictionary<string, string>(); }
        }

        protected void SerializeData()
        {
            if (_datax != null)
            {
                Data = Newtonsoft.Json.JsonConvert.SerializeObject(_datax);
            }
        }

        public string GetData(string key)
        {
            return DataX.ContainsKey(key) ? DataX[key] : null;
        }

        public void SetData(string key, string value)
        {
            DataX[key] = value;
        }

        public T GetData<T>(string key)
        {
            return GetData(key, default(T));
        }

        public T GetData<T>(string key, T defaultValue)
        {
            var s = GetData(key);
            if (string.IsNullOrWhiteSpace(s)) return defaultValue;
            try
            {
                return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(s);
            }
            catch
            {
                return defaultValue;
            }
        }

        public void SetData<T>(string key, T value)
        {
            SetData(key, Newtonsoft.Json.JsonConvert.SerializeObject(value));
        }
    }
}