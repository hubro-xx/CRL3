<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="OrderTest.aspx.cs" Inherits="WebTest.OrderTest" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h4>以下演示了如何继承实现业务</h4>
    <p>
    <asp:Button ID="Button1" runat="server" OnClick="Button1_Click" Text="添加一项" />
    <pre>
    var order = new Code.Order() { UserId = "1", Remark = "test" };
    order.Channel = "channel1";//Channel是新增加的属性
    //通过继承实现新增业务,不需要再写提交订单方法
    Code.OrderManage.Instance.SubmitOrder(order);
    </pre>
</p>
</asp:Content>
