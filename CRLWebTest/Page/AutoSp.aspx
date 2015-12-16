<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="AutoSp.aspx.cs" Inherits="WebTest.AutoSp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h4>以下演示了将标准查询或SQL语句动态编译成存储过程</h4>
    <blockquote>
    对应的数据库帐号需要有创建存储过程的权限<br />
    以下查询会成成类似 ZautoSp_1E719B3EE5AFF6F4 的储储过程,删除后再查询时会自动生成
    </blockquote>
    <asp:Button ID="Button1" runat="server" OnClick="Button1_Click" Text="查询转换为存储过程" />
<pre>
    var query = Code.ProductDataManage.Instance.GetLamadaQuery();
    //会按条件不同创建不同的存储过程
    query.Where(b => b.Id < 700);
    string name = Request["name"];
    if (!string.IsNullOrEmpty(name))
    {
        query.Where(b => b.InterFaceUser == name);
    }
    query.CompileToSp(true);
    var list = Code.ProductDataManage.Instance.QueryList(query);
</pre>
<asp:Button ID="Button2" runat="server" OnClick="Button2_Click" Text="将SQL查询转换为存储过程查询" />
<pre>
    var helper = dbHelper;
    string sql = "select * from ProductData where datediff(d,addtime,@date)=0";
    helper.AddParam("date", date);
    return helper.AutoSpQuery<ProductData>(sql);
</pre>
<br />
</asp:Content>
