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
            //将lambda表达式转换可传输的json,并还原

            int m = 10;
            var query = new CRL.LambdaQuery.CRLExpression.CRLExpressionVisitor<Code.ProductData>();
            var re = query.Where(b => b.ProductName.Substring(2) == "22" && b.Id > 10);
            //var re = query.Where(b => b.ProductName.Substring(2)=="22");
            //var re = query.Where(b => b.ProductName.IndexOf("22")==1);

            //还原
            var obj = CRL.LambdaQuery.CRLExpression.CRLQueryExpression.FromJson(re);
            var expression = query.CreateLambda(obj.Expression);
            //re = CRL.CacheServer.CacheServer.Query(obj);
            TextBox1.Text = re.ToString();
        }
    }
}