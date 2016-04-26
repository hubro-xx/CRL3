<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="Sharding.aspx.cs" Inherits="WebTest.Page.Sharding" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h4>以下演示了分库分表的过程</h4>
    <blockquote>
        以下会创建两个库 db1,db2<br />
        db1会员编号为1~10 ,db2会员编号为 11~20 ,当插入会员编号小于11的数据,则会定位到db1,11到20则会定位到db2<br />
        订单表OrderSharding设定为最大主数据容量5,1~5编号的会员订单会放在OrderSharding,6~10则会放到OrderSharding_1<br />
        本分表方案参考<a href="http://blog.csdn.net/bluishglc/article/details/7696085" target="_blank">http://blog.csdn.net/bluishglc/article/details/7696085</a> 垂直再水分拆分
        实现原理:http://www.cnblogs.com/hubro/p/4821399.html
    </blockquote>
    <h4>结构图表示为</h4>
    <img src="/img/sharding.jpg" />
    <br />
    <asp:Button ID="Button1" runat="server" OnClick="Button1_Click" Text="初始库数据映射" />
        <br />
        会员编号<asp:TextBox ID="TextBox1" runat="server">1</asp:TextBox>
        <asp:Button ID="Button4" runat="server" OnClick="Button4_Click" Text="插入测试" />
        <asp:Label ID="Label1" runat="server"></asp:Label>
        <br />

        <asp:Button ID="Button3" runat="server" Text="查找会员" OnClick="Button3_Click" />
    <pre>
    var id = Convert.ToInt32(TextBox1.Text);
    var list = Code.Sharding.MemberManage.Instance.SetLocation(id).QueryList(b => b.Id == id);
    </pre>
    
        <asp:Button ID="Button2" runat="server" Text="查找订单" OnClick="Button2_Click" />
    <pre>
    var id  = Convert.ToInt32(TextBox1.Text);
    var list = Code.Sharding.OrderManage.Instance.SetLocation(id).QueryList(b => b.MemberId == id);
    </pre>
        <asp:Button ID="Button5" runat="server" OnClick="Button5_Click" Text="联合查询当前库" />
     <pre>
    var id = Convert.ToInt32(TextBox1.Text);
    var orderManage = Code.Sharding.OrderManage.Instance.SetLocation(id);
    var query = orderManage.GetLambdaQuery();
    query.ShardingUnion(UnionType.UnionAll);
    var list = query.ToList();
     </pre>
    <asp:GridView ID="GridView1" runat="server">
    </asp:GridView>

    
</asp:Content>
