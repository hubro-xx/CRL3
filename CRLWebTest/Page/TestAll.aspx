<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="TestAll.aspx.cs" Inherits="WebTest.Page.TestAll" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:Button ID="Button1" runat="server" OnClick="Button1_Click" Text="查询" />
    <asp:Button ID="Button2" runat="server" OnClick="Button2_Click" Text="更新" />
</asp:Content>
