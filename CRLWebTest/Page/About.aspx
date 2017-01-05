<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="About.aspx.cs" Inherits="WebTest.About" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <p class="lead">
    当前版本 CRL<%=WebTest.Code.Setting.GetVersion() %> <br />
    GitHub开源:https://github.com/hubro-xx/CRL3 <br />
    版本更新请关注:<a href="http://www.cnblogs.com/hubro">http://www.cnblogs.com/hubro</a>

    <br />
    QQ群:1582632 密语:CRL
        <br />
        有好的意见或建议请邮箱:hubro@163.com
    </p>
</asp:Content>
