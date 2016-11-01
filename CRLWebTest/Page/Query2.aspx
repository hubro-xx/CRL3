<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="Query2.aspx.cs" Inherits="WebTest.Query2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h4>以下演示了复杂查询</h4>
    <blockquote>
        查询语法除了支持基本的变量常量,以下也被支持
        <ul>
            <li>
                解析过的扩展方法 <br />query.Where(b => b.ProductName.Contains("122"));//包含字符串
                <br />
                详细的扩展方法,见<a href="/page/ExtensionMethod.aspx">扩展方法查询</a>
            </li>
            <li>字段间二元运算
                <br />
                query.Select(b => new {aa = b.Id * b.Number })//选择字段时二元运算
                <br />
                query.Where(b => b.Id < b.Id * b.Number)//条件选择时二元运算
            </li>
            <li>
                表达式不区分左右<br />
                query.Where(b => 10 > b.Id);和query.Where(b => b.Id < 10 ); 等效
            </li>
            <li>
                支持字段类型转换运算<br />
                query.Where(b => b.Id.ToString() == "123"); 等效为 cast(id as nvarchar)
            </li>
            <li>
                一元运算支持
                <br />
                query.Where(b => b.IsTop);//没有运算符的bool一元运算<br />
                query.Where(b => !b.IsTop)//等效为 isTop!=1 目前只处理了BOOL类型
                <br />
                query.Where(b => !b.ProductName.Contains("122"))//BOOL类型的扩展方法同样支持
            </li>
            <li>
                关联查询支持以上所有特性<br />
                query.Join&lt;Code.Member>((a, b) =&gt; a.UserId == b.Id &amp;&amp; a.Id == c).Select((a, b) =&gt; new { a.BarCode, Year2 = b.Year * b.Id });<br />
                关联查询支持关联表排序
                query.OrderBy&lt;Code.Member>(b => b.Name, true)
            </li>
            <li>
                多列排序,只需调用OrderBy方法多次<br />
                query.OrderBy(b => b.Id, true);//按ID倒序<br />
                query.OrderBy(b => b.Name, false)//按Name正序<br />
                结果等效为 order by Id desc,Name asc
            </li>
        </ul>
    </blockquote>
        <div id="topNavWrapper">
        <asp:TextBox ID="txtOutput" runat="server" Height="181px" TextMode="MultiLine" Visible="False" Width="523px"></asp:TextBox>
    </div>
    <asp:Button ID="Button1" runat="server" OnClick="Button1_Click" class="btn" Text="使表完整表达式查询" />
    <pre>
    var query = Code.ProductDataManage.Instance.GetLambdaQuery();
    query.Select(b => new { b.InterFaceUser, b.ProductName, b.PurchasePrice });//选择查询的字段
    int? n2 = 10;
    query.Top(100);//取多少条
    query.Where(b => b.Id < 700);//查询条件
    query.Where(b => b.ProductName.Contains("w2") || b.ProductName.Contains("sss"));
    string s = "ssss";
    int n = 10;
    classA a = new classA() { Name = "ffffff" };
    query.Where(b => b.ProductName == s && b.Id > n || b.ProductName.Contains("sss"));
    query.Where(b => b.Id == n2.Value);
    query.Where(b => b.ProductName == a.Name);
    query.Where(b => b.ProductName == a.Method());
    query.Where(b => b.ProductName.Contains("sss"));
    query.OrderBy(b => b.Id, true);//排序条件
    query.OrderBy(b => b.Number, false);//多列排序调用多次
    txtOutput.Visible = true;
    txtOutput.Text = query.PrintQuery();
    </pre>
    <asp:Button ID="Button5" runat="server" OnClick="Button5_Click" class="btn" Text="返回动态对象" />
    <pre>
    //此方法演示根据结果集返回动态对象
    var query = Code.ProductDataManage.Instance.GetLambdaQuery();
    query.Select(b => new { b.ProductChannel, b.BarCode });
    query.Where(b=>b.Id<10);
    var list = query.ToDynamic();
    </pre>
    <asp:Button ID="Button6" runat="server" OnClick="Button6_Click" class="btn" Text="SUM/COUNT" />
<pre>
    //按条件id>0,合计Number列
    var sum = Code.ProductDataManage.Instance.Sum(b => b.Id > 0, b => b.Number * b.UserId);
    //按条件id>0,进行总计
    var count = Code.ProductDataManage.Instance.Count(b => b.Id > 0);
    var max = Code.ProductDataManage.Instance.Max(b => b.Id > 0, b => b.Id);
    var min = Code.ProductDataManage.Instance.Min(b => b.Id > 0, b => b.Id);
</pre><asp:Button ID="Button7" runat="server" Text="Group查询" OnClick="Button7_Click" />
    <pre>
    //using CRL以获取扩展方法
    var query = Code.ProductDataManage.Instance.GetLambdaQuery();
    query.Where(b => b.Id > 0);
    query.Top(10);
    //选择GROUP字段
    query.Select(b => new
    {
        b.BarCode,
        sum2 = b.SUM(x => x.Number * x.Id),//等效为 sum(Number*Id) as sum2
        total = b.BarCode.COUNT(),//等效为count(BarCode) as total
        sum1 = b.Number.SUM(),//等效为sum(Number) as sum1
        b.ProductName
    });
    //GROUP条件
    query.GroupBy(b => new { b.BarCode, b.ProductName });
    //having
    query.GroupHaving(b => b.Number.SUM() >= 0);
    //设置排序
    query.OrderBy(b => b.BarCode.Count(), true);//等效为 order by count(BarCode) desc
    txtOutput.Visible = true;
    txtOutput.Text = query.PrintQuery();
    var list = query.ToDynamic();
    foreach (dynamic item in list)
    {
        var str = string.Format("{0}______{1} {2} {3}<br>", item.BarCode, item.ProductName, item.total, item.sum1);//动态对象
        Response.Write(str);
    }
    </pre>
    <asp:Button ID="Button8" runat="server" Text="Distinct" OnClick="Button8_Click" />
    <pre>
    var query = Code.ProductDataManage.Instance.GetLamadaQuery();
    query.Where(b => b.Id > 0);
    query.DistinctBy(b => new { b.ProductName, b.BarCode });
    //query.DistinctCount();//表示count Distinct 结果名为Total
    var list = query.ToDynamic();
    foreach (dynamic item in list)
    {
        var str = string.Format("{0}______{1}<br>", item.ProductName, item.BarCode);//动态对象
        Response.Write(str);
    }
    </pre>
</asp:Content>
