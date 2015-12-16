<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="UserTransactionTest.aspx.cs" Inherits="WebTest.UserTransactionTest" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h4>以下演示了使用CRL内部业务,实现了账户加款扣款,生成交易流水</h4>
        会员编号<asp:TextBox ID="TextBox1" runat="server" Width="89px">100</asp:TextBox>
        <asp:Button ID="Button1" runat="server" Text="查询余额" OnClick="Button1_Click" />
        <asp:Button ID="Button3" runat="server" OnClick="Button3_Click" Text="锁定" />
        <asp:TextBox ID="txtLockId" runat="server" Width="49px"></asp:TextBox>
    <asp:Button ID="Button4" runat="server" OnClick="Button4_Click" Text="解锁" />
        <br />
        金额<asp:TextBox ID="TextBox2" runat="server" Width="89px">100</asp:TextBox>
        <asp:DropDownList ID="drpOperate" runat="server">
            <asp:ListItem Value="1">收入</asp:ListItem>
            <asp:ListItem Value="2">支出</asp:ListItem>
        </asp:DropDownList>
        <asp:Button ID="Button2" runat="server" Text="确定" OnClick="Button2_Click" />
    <h4>交易流水 </h4>
        <table border="1" class="lst">
    <tr>
        <th>流水号</th>
        <th>账户ID</th>
        <th>交易金额</th>
        <th>方向</th>
        <th>当前余额</th>
        <th>上次余额</th>
        <th>交易类型</th>
        <th>备注</th>
        <th>时间</th>
    </tr>
           <%foreach(var item in data)
      { %>
        <tr>
            <td><%=item.TransactionNo %></td>
            <td><%=item.AccountId %></td>
            <td><%=item.Amount%></td>
            <td><%=item.OperateType %></td>
            <td><%=item.CurrentBalance%></td>
            <td><%=item.LastBalance%></td>
            <td><%=item.TradeType %></td>
            <td><%=item.Remark %></td>
            <td><%=item.AddTime %></td>
        </tr>
    <%} %>
</table>
</asp:Content>
