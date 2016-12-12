/**
* CRL 快速开发框架 V4.0
* Copyright (c) 2016 Hubro All rights reserved.
* GitHub https://github.com/hubro-xx/CRL3
* 主页 http://www.cnblogs.com/hubro
* 在线文档 http://crl.changqidongli.com/
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CRL
{
    /// <summary>
    /// 不区分大小的字典
    /// </summary>
    public class IgnoreCaseDictionary<T> : Dictionary<string, T>
    {
        public IgnoreCaseDictionary()
        {
        }
        public IgnoreCaseDictionary(IDictionary<string, T> dic)
            : base(dic)
        {
        }
        Dictionary<string, string> keyMaping = new Dictionary<string, string>();
        string getKey(string key)
        {
            var key2 = key.ToUpper();
            string value;
            var a = keyMaping.TryGetValue(key2, out value);
            if (!a)
            {
                keyMaping[key2] = key;
                return key;
            }
            return value;
        }
        /// <summary>
        /// 获取键值,按小写
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public new T this[string key]
        {
            get
            {
                T obj = default(T);
                TryGetValue(getKey(key), out obj);
                return (T)obj;
            }
            set
            {
                base[getKey(key)] = value;
            }
        }
        /// <summary>
        /// 添加到字典
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public new void Add(string key, T value)
        {
            base.Add(getKey(key), value);
        }
        public new bool TryGetValue(string key, out T value)
        {
            return base.TryGetValue(getKey(key), out value);
        }
        public new bool ContainsKey(string key)
        {
            return base.ContainsKey(getKey(key));
        }
        public new bool Remove(string key)
        {
            return base.Remove(getKey(key));
        }
    }

    /// <summary>
    /// 键值的集合,不区分大小写
    /// 如果不需要以参数形式处理,名称前加上$ 如 c2["$SoldCount"]="SoldCount+" + num;
    /// </summary>
    public class ParameCollection : IgnoreCaseDictionary<object>
    {
        public ParameCollection()
        {
        }
        public ParameCollection(IDictionary<string, object> dic)
            : base(dic)
        {
        }
    }
}
