<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="Insert.aspx.cs" Inherits="WebTest.Insert" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h4>以下演示了如何插入记录</h4>
    <blockquote>
    当数据库为MSSQL时,单个插入和批量插入实现不一样,单个插入为SQL语法,批量为SqlBulkCopy<br />
    此时批量没有事务控制,效率也最高,其它数据库暂没类似功能
    </blockquote>
    <asp:Button ID="Button1" runat="server" OnClick="Button1_Click" class="btn" Text="单个插入" />
    <pre>
    var item = new Code.ProductData() { InterFaceUser = "2222", ProductName = "product2", BarCode = "1212122" };
    Code.ProductDataManage.Instance.Add(item);
    </pre>
    <asp:Button ID="Button2" runat="server" OnClick="Button2_Click" class="btn" Text="批量插入" />
    <pre>
    var list = new List&lt;Code.ProductData&gt;();
    list.Add(new Code.ProductData() { InterFaceUser = "2222", ProductName = "product2", BarCode = "1212122" }); 
    list.Add(new Code.ProductData() { InterFaceUser = "2222", ProductName = "product3", BarCode = "21312313" });
    Code.ProductDataManage.Instance.BatchInsert(list);
    </pre>
</asp:Content>
