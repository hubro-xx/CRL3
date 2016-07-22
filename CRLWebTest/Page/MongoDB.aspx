<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="MongoDB.aspx.cs" Inherits="WebTest.Page.MongoDB" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <p style="margin: 10px auto; text-indent: 0px; color: rgb(0, 0, 0); font-family: verdana, Arial, Helvetica, sans-serif; font-size: 14px; font-style: normal; font-variant: normal; font-weight: normal; letter-spacing: normal; line-height: 21px; orphans: auto; text-align: start; text-transform: none; white-space: normal; widows: 1; word-spacing: 0px; -webkit-text-stroke-width: 0px;">
        由于MongoDB的特性,以下不能实现,调用可能会抛出异常<asp:Button ID="Button1" runat="server" OnClick="Button1_Click" Text="测试" />
    </p>
    <ul style="margin-left: 0px; padding-left: 40px; color: rgb(0, 0, 0); font-family: verdana, Arial, Helvetica, sans-serif; font-size: 14px; font-style: normal; font-variant: normal; font-weight: normal; letter-spacing: normal; line-height: 21px; orphans: auto; text-align: start; text-indent: 0px; text-transform: none; white-space: normal; widows: 1; word-spacing: 0px; -webkit-text-stroke-width: 0px;">
        <li style="margin-left: 0px; padding-left: 0px;">关联查询</li>
        <li style="margin-left: 0px; padding-left: 0px;">关联删除</li>
        <li style="margin-left: 0px; padding-left: 0px;">关联更新</li>
        <li style="margin-left: 0px; padding-left: 0px;">SQL语句查询</li>
        <li style="margin-left: 0px; padding-left: 0px;">事务</li>
        <li style="margin-left: 0px; padding-left: 0px;">存储过程</li>
        <li style="margin-left: 0px; padding-left: 0px;">自动编译</li>
        <li style="margin-left: 0px; padding-left: 0px;">部份SQL函数</li>
    </ul>
&nbsp;
</asp:Content>
