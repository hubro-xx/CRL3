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
    public class ArticleBusiness<TType, TModel> : ArticleBusiness<TModel>
        where TType : class
        where TModel : Article, new()
    {

        //public static ArticleBusiness<TType, TModel> Instance
        //{
        //    get { return new ArticleBusiness<TType, TModel>(); }
        //}

        
    }
    public class ArticleBusiness<TModel> : BaseProvider<TModel>
        where TModel : Article, new()
    {
    }
}
