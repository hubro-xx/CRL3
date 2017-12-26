<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="DbSet.aspx.cs" Inherits="WebTest.Page.DbSet" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h4>DbSet结构</h4>
        <blockquote>
    为让对象关联更方便,4.52版本增加了DbSet的方式
     
            
    </blockquote>
    在对象中实现关联
    <pre>
         public class Order : CRL.IModelBase
    {
        public CRL.Set.DbSet&lt;ProductData&gt; Products
        {
            get
            {
                return GetDbSet&lt;ProductData&gt;(b => b.Id, ProductId);
            }
        }
      }
    </pre>
    表示含义为ProductData.Id==Order.ProductId<br />
    这样DbSet&lt;ProductData&gt; 中所有的操作都会加上此关联<br />
    DbSet提供了常用的一些方法,如下:
    <pre>
        var order = new Code.Order();
            //所有
            var product = order.Products.ToList();

            //返回关联过的查询
            var product2 = order.Products.GetQuery();

            var p = new Code.ProductData();
            //添加一项
            order.Products.Add(p);

            order.Products.Delete(p);//删除一项

            //返回完整的BaseProvider
            var provider = order.Products.GetProvider();
    </pre>
<asp:Button ID="Button1" runat="server" Text="测试" OnClick="Button1_Click" />
</asp:Content>
