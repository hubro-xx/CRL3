<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="Query3.aspx.cs" Inherits="WebTest.Page.Query3" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h4>以下演示了关联查询</h4>
        <div id="topNavWrapper">
        <asp:TextBox ID="txtOutput" runat="server" Height="181px" TextMode="MultiLine" Visible="False" Width="523px"></asp:TextBox>
    </div>
    <blockquote>
        关联查询有以下两种形式
        <ul>
            <li>返回Select结果,结果为动态对象</li>
            <li>将结果附加给当前对象索引值</li>
        </ul>
        关联查询有累加效果,可关联多个表<br />
        可通过匿名对象指定返回的别名,如 BarCode1 = a.BarCode 返回 BarCode1<br />
        可按参数指定关联方式,Left,Inner,Right,默认为Inner
    </blockquote>
    <asp:Button ID="Button1" runat="server" OnClick="Button1_Click" Text="返回筛选查询" />
    <pre>
        //返回筛选值
        var query = Code.ProductDataManage.Instance.GetLambdaQuery();
        query.Top(10);
        var member = new Code.Member();
        member.Id = 11;
        query.Join&lt;Code.Member>((a, b) => a.UserId == member.Id && b.Id > 0,
                CRL.LambdaQuery.JoinType.Left
            ).Select((a, b) => new { BarCode1 = a.BarCode, Name1 = b.Name });
        int count = 0;
        var list = query.ToDynamic();
        txtOutput.Visible = true;
        txtOutput.Text = query.PrintQuery();
        foreach (dynamic item in list)
        {
            var str = string.Format("{0}______{1}&lt;br>", item.BarCode1, item.Name1);//动态对象
            Response.Write(str);
        }
    </pre>
    <asp:Button ID="Button2" runat="server" OnClick="Button2_Click" Text="关联值附加查询" />
    <pre>
        //把关联值存入对象内部索引
        //关联对象值都以索引方式存取
        var query = Code.ProductDataManage.Instance.GetLambdaQuery();
        query.Top(10);
        query.Join&lt;Code.Member>((a, b) => a.UserId == b.Id && b.Id > 0,
            CRL.LambdaQuery.JoinType.Left
            ).SelectAppendValue(b => new { Name1 = b.Name });
        var list = query.ToList();
        txtOutput.Visible = true;
        txtOutput.Text = query.PrintQuery();
        foreach (var item in list)
        {
            var str = string.Format("{0}______{1}&lt;br>", item.BarCode, item["Name1"]);//取名称为Name的索引值
            Response.Write(str);
        }
    </pre>

    <asp:Button ID="Button4" runat="server" Text="in查询" OnClick="Button4_Click" />
    <pre>
            var query = Code.ProductDataManage.Instance.GetLamadaQuery();
            query.Where(b => b.ProductId == "0");
            var query2 = query.CreateQuery&lt;Code.Member>();
            var view = query2.GroupBy(b => b.Id).Where(b => b.Id > 0).SelectV(b => b.Id);
            //等效为 product.UserId in(select UserId from order where product.SupplierId=10 and order.status=2)
            query.In(b => b.Id, view);
    </pre>
    除了in,还有not in,exists等,关系为
    <pre>
In          in
NotIn       not in
Equal       =
NotEqual    !=
Exists      exists
NotExists   not exists
    </pre>
    <asp:Button ID="Button3" runat="server" Text="返回匿名对象" OnClick="Button3_Click" />
</asp:Content>
