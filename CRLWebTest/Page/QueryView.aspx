<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="QueryView.aspx.cs" Inherits="WebTest.Page.ViewQuery" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h4>使用嵌套查询</h4>
        <blockquote>
            4.2版本解决了匿名对象解析和子查询的问题,现在查询可以任意组合了<br />
            为了兼容以前的写法,增加了LambdaQuery.SelectV方法,返回LambdaQueryResultSelect类型<br />
            同时增加了以下几种方式
            <ul>
                <li>关联子查询<br />
                    public LambdaQueryViewJoin&lt;T, TJoinResult> Join&lt;TJoinResult>(LambdaQueryResultSelect&lt;TJoinResult> resultSelect, Expression&lt;Func&lt;T, TJoinResult, bool>&gt; expression, JoinType joinType = JoinType.Inner) </li>
                <li>联合查询<br />
                    public LambdaQueryResultSelect&lt;TResult> Union&lt;TResult2>(LambdaQueryResultSelect&lt;TResult2> resultSelect, UnionType unionType = UnionType.UnionAll)</li>
                <li>
                    In,Exists查询也改为子查询形式
                </li>
            </ul>
        </blockquote>
    <div>
        为了使子查询有关联性,需要调用CreateQuery方法创建查询<br />
        使用嵌套查询过程表示为:<br />
        <blockquote>
            <p></p>
        主查询 => CreateQuery子查询 => 返回匿名对象筛选LambdaQueryResultSelect&nbsp;
        => 主查询嵌套子查询 => 返回结果

        </blockquote>
        <pre>var query = Code.ProductDataManage.Instance.GetLambdaQuery();
var query2 = query.CreateQuery&lt;Code.Order>();</pre>
        <strong>返回匿名结果
        </strong>
        <pre>var result1 = query.SelectV(b => new { id = b.Id, name = b.CategoryName }).ToList();
</pre>
        <strong>关联一个子查询</strong><br />
        只要是LambdaQuery创建的语法都支持,如GROUP,DISTINCT,或者已经是一个关联查询
        <pre>var viewJoin = query2.Where(b => b.Id > 10).SelectV(b => b);
var result2 = query.Join(viewJoin, (a, b) => a.UserId == b.UserId).Select((a, b) => new { a.CategoryName, b.OrderId }).ToList();
</pre>
        <strong>联合查询
        </strong><br />支持N个
        <pre>var view1 = query.SelectV(b => new { a1 = b.Id, a2 = b.ProductName });
var view2 = query2.SelectV(b => new { a1 = b.Id, a2 = b.Remark });
var result3 = view1.Union(view2).OrderBy(b => b.a1).OrderBy(b => b.a2, false).ToList();</pre>
        <strong>按IN查询</strong>
        <pre>var view3 = query2.Where(b => b.Remark == "123").SelectV(b => b.Id);
query.In(view3, b => b.UserId);</pre>
        来一个复杂的
        <pre>
var q1 = Code.OrderManage.Instance.GetLambdaQuery();
var q2 = q1.CreateQuery<&lt;Code.ProductData>();
q2.Where(b => b.Id > 0);
var view = q2.CreateQuery&lt;Code.Member>().GroupBy(b => b.Name).Where(b => b.Id > 0).SelectV(b => new { b.Name, aa = b.Id.COUNT() });//GROUP查询
var view2 = q2.Join(view, (a, b) => a.CategoryName == b.Name).Select((a, b) => new { ss1 = a.UserId, ss2 = b.aa });//关联GROUP
q1.Join(view2, (a, b) => a.Id == b.ss1).Select((a, b) => new { a.Id, b.ss1 });//再关联
//var result = view2.ToList();
var sql = q1.ToString();</pre>
        输出语句
         
        <pre>
SELECT t1.[Id] AS Id,
       t2.[ss1] AS ss1
FROM [OrderProduct] t1 with(nolock)
INNER JOIN
  (SELECT t2.[UserId] AS ss1,
          t3.[aa] AS ss2
   FROM [ProductData] t2 with(nolock)
   INNER JOIN
     (SELECT t3.[Name] AS Name,
             COUNT(t3.Id) AS aa
      FROM [Member] t3 with(nolock)
      WHERE (t3.[Id]>@par1)
      GROUP BY t3.[Name]) t3 ON (t2.[CategoryName]=t3.[Name])
   WHERE (t2.[Id]>@par0) ) t2 ON (t1.[Id]=t2.[ss1])
        </pre>
    </div>
</asp:Content>
