using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CRL;
namespace WebTest
{
    public partial class Update : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            //要更新属性集合
            CRL.ParameCollection c = new CRL.ParameCollection();
            c["ProductName"] = "product1";
            Code.ProductDataManage.Instance.Update(b => b.Id == 4, c);
            //按对象差异更新
            var p = new Code.ProductData() { Id = 4 };
            //手动修改值时,指定修改属性以在Update时识别,分以下几种形式
            p.Change(b => b.BarCode);//表示值被更改了
            p.Change(b => b.BarCode,"123");//通过参数赋值
            p.Change(b => b.BarCode == "123");//通过表达式赋值
            Code.ProductDataManage.Instance.Update(b => b.Id == 4, p);//指定查询更新
            p = Code.ProductDataManage.Instance.QueryItem(b => b.Id > 0);
            p.UserId += 1;
            Code.ProductDataManage.Instance.Update(p);//按主键更新,主键值是必须的
        }
    }
}