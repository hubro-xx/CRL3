<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="SqlTransaction.aspx.cs" Inherits="WebTest.SqlTransaction" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h4>以下演示了如何使用事务</h4>
    <blockquote>注此事务是由ADO.NET控制,在回滚和提交时,如果遇上网络异常或数据库服务器异常,将导致事务出错<br />
    要保证绝对稳定,建议业务写成存储过程,在存储过程里进行控制<br />
当前事务为TransactionScope实现,支持多库
    </blockquote>
     
    <asp:Button ID="Button3" runat="server" Text="使用封装事务(TransactionScope)" OnClick="Button3_Click" />
    <asp:Button ID="Button4" runat="server" OnClick="Button4_Click" Text="使用封装事务(DbTransaction)" />
    <asp:Button ID="Button5" runat="server" OnClick="Button5_Click" Text="使用嵌套封装事务(DbTransaction)" />
    <pre>
    var helper = dbHelper;
    //简化了事务写法,自动提交回滚
    return PackageTrans((out string ex) =>
    {
        ex = "";
        var product = new ProductData();
        product.BarCode = "sdfsdf";
        product.Number = 10;
        ProductDataManage.Instance.Add(product);

        product = new ProductData();
        //product.BarCode = "sdfsdf2";
        //product.Number = 12;
        ProductDataManage.Instance.Add(product);//不符合数据校验规则,将会抛出异常
        //return false;
        return true;
    }, out error);
    </pre>
</asp:Content>
