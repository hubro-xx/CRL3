<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="ReadSeparation.aspx.cs" Inherits="WebTest.Page.ReadSeparation" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <h4>读写分离</h4>
    <blockquote>
    
        主库
        <asp:GridView ID="GridView1" runat="server">
        </asp:GridView>
        <asp:Button ID="Button2" runat="server" OnClick="Button2_Click" Text="查询数据" />
        <asp:Button ID="Button1" runat="server" OnClick="Button1_Click" Text="更新数据" />
        <br />
        从库
        <asp:GridView ID="GridView2" runat="server">
        </asp:GridView>
    
    </blockquote>
</asp:Content>
