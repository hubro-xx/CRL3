using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebTest.Page
{
    public partial class ViewQuery : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var query = Code.ProductDataManage.Instance.GetLambdaQuery();
            var query2 = query.CreateQuery<Code.Order>();
            //返回匿名结果
            var result1 = query.SelectV(b => new { id = b.Id, name = b.CategoryName }).ToList();

            //关联一个子查询
            var viewJoin = query2.Where(b => b.Id > 10).SelectV(b => b);
            var result2 = query.Join(viewJoin, (a, b) => a.UserId == b.UserId).Select((a, b) => new { a.CategoryName, b.OrderId }).ToList();

            //联合查询
            var view1 = query.SelectV(b => new { a1 = b.Id, a2 = b.ProductName });
            var view2 = query2.SelectV(b => new { a1 = b.Id, a2 = b.Remark });
            var result3 = view1.Union(view2).OrderBy(b => b.a1).OrderBy(b => b.a2, false).ToList();

            //按IN查询
            var view3 = query2.Where(b => b.Remark == "123").SelectV(b => b.Id);
            query.In(view3, b => b.UserId);
        }
    }
}