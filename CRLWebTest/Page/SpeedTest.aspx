<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="SpeedTest.aspx.cs" Inherits="WebTest.Page.SpeedTest" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    查询行数/次数<asp:TextBox ID="TextBox1" runat="server">50000</asp:TextBox>
    <asp:Button ID="Button1" runat="server" OnClick="Button1_Click" Text="多行" />
    <asp:Button ID="Button2" runat="server" OnClick="Button2_Click" Text="轮循" />
    <br />
    <asp:TextBox ID="TextBox2" runat="server" Height="308px" TextMode="MultiLine" Width="530px"></asp:TextBox>
&nbsp;
</asp:Content>
