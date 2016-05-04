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

namespace CRL.Package.Article
{
    /// <summary>
    /// 文章内容
    /// </summary>
    public class Article : IModelBase
    {
        public override string CheckData()
        {
            return "";
        }
        /// <summary>
        /// 标题
        /// </summary>
        public string Title
        {
            get;
            set;
        }
        /// <summary>
        /// 内容
        /// </summary>
        [Attribute.Field(Length = 4000)]
        public string Content
        {
            get;
            set;
        }
        /// <summary>
        /// 作者
        /// </summary>
        public string Author
        {
            get;
            set;
        }
        /// <summary>
        /// 类型
        /// </summary>
        [Attribute.Field(FieldIndexType = Attribute.FieldIndexType.非聚集)]
        public string ArticleType
        {
            get;
            set;
        }
        /// <summary>
        /// 分类
        /// </summary>
        [Attribute.Field(FieldIndexType = Attribute.FieldIndexType.非聚集)]
        public string CategoryCode
        {
            get;
            set;
        }
        /// <summary>
        /// 点击
        /// </summary>
        public int Hit
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
    }
}
