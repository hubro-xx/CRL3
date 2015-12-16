using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Runtime.Remoting.Messaging;

namespace CRL.Attribute
{

    [AttributeUsage(AttributeTargets.Class)]
    public class TableAttribute : System.Attribute
    {
        public override string ToString()
        {
            return TableName;
        }
        /// <summary>
        /// 表名
        /// </summary>
        public string TableName
        {
            get;
            set;
        }
        /// <summary>
        /// 默认排序 如Id Desc
        /// </summary>
        public string DefaultSort
        {
            get;
            set;
        }
        /// <summary>
        /// 自增主键
        /// </summary>
        internal FieldAttribute PrimaryKey
        {
            get;
            set;
        }
        /// <summary>
        /// 对象类型
        /// </summary>
        public Type Type;
        //DBAdapter.DBAdapterBase _DBAdapter;
        ///// <summary>
        ///// 当前数据库适配器
        ///// </summary>
        //internal DBAdapter.DBAdapterBase DBAdapter
        //{
        //    get
        //    {
        //        if (_DBAdapter == null)
        //        {
        //            //throw new Exception("dBAdapter尚未初始化");
        //        }
        //        return _DBAdapter;
        //    }
        //    set
        //    {
        //        _DBAdapter = value;
        //    }
        //}
        /// <summary>
        /// 所有字段
        /// </summary>
        internal List<FieldAttribute> Fields = new List<FieldAttribute>();
    }
}
