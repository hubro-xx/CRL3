<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="ExportAndImport.aspx.cs" Inherits="WebTest.ExportAndImport" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h4>以下演示了如何导出/导入数据</h4>
    <asp:Button ID="Button1" runat="server" OnClick="Button1_Click" Text="导出数据测试" />
    <pre>
    //导出为JSON
    var json = Code.ProductDataManage.Instance.ExportToJson(b => b.Id > 0);
    </pre>
    <asp:Button ID="Button2" runat="server" OnClick="Button2_Click" Text="导入数据测试" />
    <pre>
    //Code.ProductDataManage.Instance.ImportFromJson("json串", b => b.Id > 0);
    </pre>
</asp:Content>
