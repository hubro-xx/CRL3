<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="Query1.aspx.cs" Inherits="WebTest.Query1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h4>以下演示了标准查询</h4>
    <p>
    <asp:Button ID="Button1" runat="server" OnClick="Button1_Click" class="btn" Text="查询一项" />
    <pre>
    //查询一项
    var item = Code.ProductDataManage.Instance.QueryItem(b => b.Id >0);
    </pre>
    <asp:Button ID="Button2" runat="server" OnClick="Button2_Click" class="btn" Text="查询多项" />
    <pre>
    //查询集合
    var list = Code.ProductDataManage.Instance.QueryList(b => b.Id < 650);
    </pre>
    <asp:GridView ID="GridView1" runat="server">
    </asp:GridView>
</p>
</asp:Content>
