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
                TryGetValue(key.ToLower(), out obj);
                return obj as T;
            }
            set
            {
                base[key.ToLower()] = value;
            }
        }
        /// <summary>
        /// 按小写名添加到字典
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public new void Add(string key, T value)
        {
            base.Add(key.ToLower(), value);
        }
        public new bool ContainsKey(string key)
        {
            return base.ContainsKey(key.ToLower());
        }
        public new bool Remove(string key)
        {
            return base.Remove(key.ToLower());
        }
    }
}
