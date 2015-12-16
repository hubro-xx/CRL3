using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CRL.Package.Article
{
    public class ArticleBusiness<TType, TModel> : BaseProvider<TModel>
        where TType : class
        where TModel : Article, new()
    {

        //public static ArticleBusiness<TType, TModel> Instance
        //{
        //    get { return new ArticleBusiness<TType, TModel>(); }
        //}

        
    }
}
