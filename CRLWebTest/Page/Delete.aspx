<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="Delete.aspx.cs" Inherits="WebTest.Delete" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h4>以下演示了如何删除</h4>
    <asp:Button ID="Button1" runat="server" OnClick="Button1_Click" class="btn" Text="删除测试" />
    <pre>
    Code.ProductDataManage.Instance.Delete(b => b.Id == 0);//按条件删除
    Code.ProductDataManage.Instance.Delete(1);//按主键删除
    Code.OrderManage.Instance.Delete&lt;Code.ProductData>((a, b) => a.UserId == b.Id);//关联ProductData删除
    
    //使用完整语法删除 goup语法不支持
    var query = Code.ProductDataManage.Instance.GetLambdaQuery();
    query.Where(b => b.Id == 10);
    query.Join&lt;Code.Member>((a, b) => a.SupplierId == "10" && b.Name == "123");
    Code.ProductDataManage.Instance.Delete(query);

    </pre>
</asp:Content>
