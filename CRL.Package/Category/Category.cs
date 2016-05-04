/**
* CRL 快速开发框架 V3.1
* Copyright (c) 2016 Hubro All rights reserved.
* GitHub https://github.com/hubro-xx/CRL3
* 主页 http://www.cnblogs.com/hubro
* 在线文档 http://crl.changqidongli.com/
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CRL.Package.Category
{
    /// <summary>
    /// 分类,由于缓存,只能实现一种类型,不要继承此类实现多个实例
    /// </summary>
    public class Category : IModelBase
    {
        public override string CheckData()
        {
            if(string.IsNullOrEmpty(SequenceCode))
            {
                return "SequenceCode不能为空";
            }
            return "";
        }
        /// <summary>
        /// 类型,以作不同用途
        /// </summary>
        public int DataType
        {
            get;
            set;
        }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name
        {
            get;
            set;
        }
        /// <summary>
        /// 编码
        /// </summary>
        public string SequenceCode
        {
            get;
            set;
        }
        /// <summary>
        /// 父级编码
        /// </summary>
        public string ParentCode
        {
            get;
            set;
        }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark
        {
            get;
            set;
        }
        /// <summary>
        /// 是否禁用
        /// </summary>
        public bool Disable
        {
            get;
            set;
        }
        /// <summary>
        /// 排序
        /// </summary>
        public int Sort
        {
            get;
            set;
        }
        public override string ToString()
        {
            return string.Format("{0} {1}", Name, SequenceCode);
        }
    }
}
