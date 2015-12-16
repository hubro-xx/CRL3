<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="Delete.aspx.cs" Inherits="WebTest.Delete" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h4>以下演示了如何删除</h4>
    <asp:Button ID="Button1" runat="server" OnClick="Button1_Click" class="btn" Text="删除测试" />
    <pre>
    Code.ProductDataManage.Instance.Delete(b => b.Id == 0);//按条件删除
    Code.ProductDataManage.Instance.Delete(1);//按主键删除
    </pre>
</asp:Content>
