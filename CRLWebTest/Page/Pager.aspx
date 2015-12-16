<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="Pager.aspx.cs" Inherits="WebTest.Page.Pager" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h4>以下演示了如何进行分页查询</h4>
    <div id="topNavWrapper">
        <asp:TextBox ID="txtOutput" runat="server" Height="181px" TextMode="MultiLine" Visible="False" Width="523px"></asp:TextBox>
    </div>
    <blockquote>
        <ul>
            <li>使用LambdaQuery完整查询调用Page方法就可以分页了,通过ToList和ToDynamic方法返回指定类型结果
            </li>
            <li>当设置了关联,Group语法,也会按关联,Group语法解析</li>
            <li>当调用了Select方法筛选字段,则需要根据实际情况返回结果类型</li>
            <li>
                返回结果可以有以下几种类型
                <ul>
                    <li> List&lt;dynamic> ToDynamic() 按筛选值返回动态类型</li>
                    <li> List&lt;TResult> ToList&lt;TResult>() 按筛选值返回指定类型</li>
                    <li> List&lt;T> ToList() 直接返回当前类型</li>
                    <li> Dictionary&lt;TKey, TValue> ToDictionary&lt;TKey, TValue>() 按筛选值返回字典(不支持分页)</li>
                </ul>
            </li>
            <li>同样,当未调用Page方法设定分页,也会按以上结果类型返回</li>
        </ul>
    </blockquote>
    <p>
        <asp:Button ID="Button3" runat="server" OnClick="Button3_Click" Text="直接返回当前对象" />
    <pre>
    int pageSize = 15;
    int page = 1;
    int count;
    var query = Code.ProductDataManage.Instance.GetLambdaQuery();
    query.Where(b => b.Id > 0);
    query.Page(pageSize, page);
    query.OrderBy(b => b.Id, true);
    var list = query.ToList();//返回当前对象类型
    count = query.RowCount;
    txtOutput.Visible = true;
    txtOutput.Text = query.PrintQuery();
    </pre>

    <asp:Button ID="Button4" runat="server" OnClick="Button4_Click" Text="按筛选值返回动态对象" />
        以下为一个Group查询,结果为按选择的字段生成的动态对象
        <pre>
    //using CRL以获取扩展方法
    var query = Code.ProductDataManage.Instance.GetLambdaQuery();
    query.Page(15, 1);
    query.Where(b => b.Id>0);
    int count;
    //选择GROUP字段
    query.Select(b => new
    {
        b.BarCode,
        b.ProductName,
        total = b.BarCode.COUNT(),//等效为count(BarCode) as total
        sum1 = b.Number.SUM()//等效为sum(Number) as sum1
    });
    //GROUP条件
    query.GroupBy(b => new { b.BarCode, b.ProductName });
    //having
    query.GroupHaving(b => b.Number.SUM() >= 0);
    //设置排序
    query.OrderBy(b => b.BarCode.Count(), true);//等效为 order by count(BarCode) desc
    var list = query.ToDynamic();//按选择的字段返回动态类型
    txtOutput.Visible = true;
    txtOutput.Text = query.PrintQuery();
    foreach (dynamic item in list)
    {
        var str = string.Format("{0}______{1} {2} {3}&lt;br>", item.BarCode, item.ProductName, item.total, item.sum1);//动态对象
        Response.Write(str);
    }
    </pre>
        <asp:Button ID="Button1" runat="server" Text="按筛选值返回指定对象" OnClick="Button1_Click" />
        定义一个对象
        <pre>
    class ClassTemp
    {
        public string ProductName { get; set; }
        public string BarCode { get; set; }
    }</pre>
        以定义对象返回结果
        <pre>
    int pageSize = 15;
    int page = 1;
    int count;
    var query = Code.ProductDataManage.Instance.GetLambdaQuery();
    query.Select(b => new { b.ProductName,b.BarCode});//选择指定字段
    query.Where(b => b.Id>0);
    query.Page(pageSize, page);
    query.OrderBy(b => b.Id, true);
    var list = query.ToList&lt;ClassTemp>();//按选择字段指定类型转换
        </pre>
</asp:Content>

