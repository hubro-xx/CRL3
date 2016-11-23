<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="Cache.aspx.cs" Inherits="WebTest.Cache" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h4>以下演示了如何创建使用缓存</h4>
    <blockquote>
        使用缓存会减少数据库查询消耗,大大提高程序效率,使用合理的情况下,可解决对象关联问题<br />
        首次查询后会创建缓存,下次再查询时,会按查询条件找到对应的缓存<br />
        以下缓存为异步更新,过期后会按条件异步重新查询最新数据,有线程单独维护
     
        <br />
        缓存在两个周期未使用后,会自动清理
    </blockquote>
    <div>
        <h4>此缓存为前端缓存,直接缓存在WEB服务器内存中,重启时丢失失</h4>
    </div>
    <asp:Button ID="Button1" runat="server" OnClick="Button1_Click" class="btn" Text="创建查询缓存" />
    <pre>
    var query = Code.ProductDataManage.Instance.GetLamadaQuery();
    //缓存会按条件不同缓存不同的数据,条件不固定时,慎用
    query.Where(b => b.Id < 700);
    int exp = 10;//过期分钟
    query.Expire(exp);
    var list = Code.ProductDataManage.Instance.QueryList(query);
    </pre>
    <asp:Button ID="Button2" runat="server" OnClick="Button2_Click" class="btn" Text="使用当前对象缓存" />
    <pre>
    //默认过期时间为5分钟
    //AllCache可重写条件和过期时间,在业务类中实现即可
    //当插入或更新当前类型对象时,此缓存中对应的项也会更新
    var list = Code.ProductDataManage.Instance.QueryFromCache(b => b.Id < 700);
    </pre>
    <asp:Button ID="Button3" runat="server" class="btn" Text="创建动态表达式查询缓存,减少计算次数" OnClick="Button3_Click" />

    <pre>
    var list = Code.ProductDataManage.Instance.AllCache;//指定一个数据源
    #region 常规查找 多次计算和内存操作,增加成本
    var list2 = list.Where(b => b.Id > 0);//执行一次内存查找
    bool a = false;
    if (a)
    {
        list2 = list.Where(b => b.Number > 10);//执行第二次内存查找
    }
    #endregion

    #region 优化后查找 只需一次
    var query = new CRL.ExpressionJoin&lt;Code.ProductData>(list, b => b.Id > 0);
    if (a)
    {
        query.And(b => b.Number > 10);//and 一个查询条件
    }
    list2 = query.ToList();//返回查询结果 只作一次内存查找
    #endregion
    </pre>
</asp:Content>
