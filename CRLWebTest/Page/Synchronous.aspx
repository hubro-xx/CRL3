<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="Synchronous.aspx.cs" Inherits="WebTest.Synchronous" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h4>以下演示了如何同步对象表结构</h4>
    <div>
    <blockquote>
        新版增加属性后会自动创建数据库字段<br />
        当然也可以采用编程的方式进行检查
        <br />
        3.0新版增加了后台线程对表结构强制检查,就算人为删除字段,在下次运行时,也会自动创建上
    </blockquote>
&nbsp;<asp:Button ID="Button1" runat="server" OnClick="Button1_Click" Text="检查表" />
<pre>
    //创建表并检查字段
    Code.ProductDataManage.Instance.CreateTable();
    //检查表索引
    Code.ProductDataManage.Instance.CreateTableIndex();
</pre>
    </div>
</asp:Content>
