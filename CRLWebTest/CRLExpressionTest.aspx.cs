using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebTest
{
    public partial class CRLExpressionTest : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            int m = 10;
            var query = new CRL.LambdaQuery.CRLExpressionVisitor<Code.ProductData>();
            //var re = query.Where(b => b.ProductName.Contains("22"));
            var re = query.Where(b => b.ProductName.Substring(2)=="22");
            //var re = query.Where(b => b.ProductName.IndexOf("22")==1);
            var obj = CRL.LambdaQuery.CRLQueryExpression.FromJson(re);
            var expression = query.CreateLambda(obj.Expression);
            //re = CRL.CacheServer.CacheServer.Query(obj);
            Response.Write(re);
        }
    }
}