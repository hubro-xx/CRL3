using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections;
namespace CRL.Dynamic
{
    internal sealed partial class DynamicObject
         : System.Dynamic.IDynamicMetaObjectProvider
         , IDictionary<string, object>
    {
        Dictionary<string, object> values;
        public DynamicObject(DataRow dr)
        {
            values = new Dictionary<string, object>();
            foreach (DataColumn col in dr.Table.Columns)
            {
                values.Add(col.ColumnName, dr[col.ColumnName]);
            }
        }
        int ICollection<KeyValuePair<string, object>>.Count
        {
            get
            {
                return values.Count();
            }
        }

        public bool TryGetValue(string name, out object value)
        {
            return values.TryGetValue(name, out value);
        }

        public override string ToString()
        {
            var sb = new StringBuilder("{DapperRow");
            foreach (var kv in this)
            {
                var value = kv.Value;
                sb.Append(", ").Append(kv.Key);
                if (value != null)
                {
                    sb.Append(" = '").Append(kv.Value).Append('\'');
                }
                else
                {
                    sb.Append(" = NULL");
                }
            }

            return sb.Append('}').ToString();
        }

        System.Dynamic.DynamicMetaObject System.Dynamic.IDynamicMetaObjectProvider.GetMetaObject(
            System.Linq.Expressions.Expression parameter)
        {
            return new DapperRowMetaObject(parameter, System.Dynamic.BindingRestrictions.Empty, this);
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #region Implementation of ICollection<KeyValuePair<string,object>>

        void ICollection<KeyValuePair<string, object>>.Add(KeyValuePair<string, object> item)
        {
            IDictionary<string, object> dic = this;
            dic.Add(item.Key, item.Value);
        }

        void ICollection<KeyValuePair<string, object>>.Clear()
        {
            values.Clear();
        }

        bool ICollection<KeyValuePair<string, object>>.Contains(KeyValuePair<string, object> item)
        {
            object value;
            return TryGetValue(item.Key, out value) && Equals(value, item.Value);
        }

        void ICollection<KeyValuePair<string, object>>.CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        {
            foreach (var kv in this)
            {
                array[arrayIndex++] = kv; // if they didn't leave enough space; not our fault
            }
        }

        bool ICollection<KeyValuePair<string, object>>.Remove(KeyValuePair<string, object> item)
        {
            IDictionary<string, object> dic = this;
            return dic.Remove(item.Key);
        }

        bool ICollection<KeyValuePair<string, object>>.IsReadOnly
        {
            get { return false; }
        }

        #endregion

        #region Implementation of IDictionary<string,object>

        bool IDictionary<string, object>.ContainsKey(string key)
        {
            return values.ContainsKey(key);
        }

        void IDictionary<string, object>.Add(string key, object value)
        {
            IDictionary<string, object> dic = this;
            dic[key] = value;
        }

        bool IDictionary<string, object>.Remove(string key)
        {
            return values.Remove(key);
        }

        object IDictionary<string, object>.this[string key]
        {
            get { object val; TryGetValue(key, out val); return val; }
            set { SetValue(key, value); }
        }
        public object SetValue(string key, object value)
        {
            return values[key] = value;
        }

        ICollection<string> IDictionary<string, object>.Keys
        {
            get { return this.Select(kv => kv.Key).ToArray(); }
        }

        ICollection<object> IDictionary<string, object>.Values
        {
            get { return this.Select(kv => kv.Value).ToArray(); }
        }

        #endregion
    }
}
