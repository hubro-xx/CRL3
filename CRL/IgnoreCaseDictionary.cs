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
    public class IgnoreCaseDictionary<T> : Dictionary<string, T> where T : class
    {
        Dictionary<string, string> keyMaping = new Dictionary<string, string>();
        string getKey(string key)
        {
            var key2 = key.ToUpper();
            if (!keyMaping.ContainsKey(key2))
            {
                keyMaping[key2] = key;
            }
            return keyMaping[key2];
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
                T obj = null;
                TryGetValue(getKey(key), out obj);
                return obj as T;
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
}
