<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="Cache2.aspx.cs" Inherits="WebTest.Page.Cache2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h4>以下演示了如何创建使用分布式缓存服务</h4>
    <blockquote>
        
        <ul>
            <li><strong>和传统缓存系统区别:</strong><br />
                像Memcached采用键值的存储方式,如果是一个对象集合,就只能把整个集合放入键值内存储,调用时需要把整个集合拿出来,再按业务要求进行过滤处理,或就没法过滤<br />
                而CRL框架能直接以Lambda查询从服务器缓存集合中查询数据,根本地缓存调用一样,通过实现Lambda的序列化和反序列化处理,解决了查询条件传输的问题<br />
                通过此技术,能快速实现数据缓存和查询需求,不需要重复开发业务查询接口
            </li>
            <li><strong>性能:</strong>目前采用了长连接的TCP连接池实现通讯,也可以实现HTTP通讯的方式,暂没有作其它性能上的优化</li>
            <li><strong>扩展方法支持:</strong><br />
                除支持基本的比较运算 等于,不等于,大于,小于,大于等于,小于等于<br />
                支持以下扩展方法(更多的开发中)
                <pre>
    string.Contains
    string.StartsWith
    string.EndsWith
    string.Substring
    string.IndexOf</pre></li>
        </ul>
        整体架构<br />
        <img  src="/img/cache.png"/><br />
        分布式结构<br />
        <img  src="/img/cache2.png"/>
    </blockquote>
    <h3>部署实现方式:</h3>
&nbsp;<strong>配置服务端</strong><br />
    在Global中配置数据源和服务监听
    <pre>
    //增加处理规则
    CRL.CacheServerSetting.AddCacheServerDealDataRule(typeof(Code.CacheDataTest), Code.CacheDataTestManage.Instance.DeaCacheCommand);
    //启动服务端
    var cacheServer = new CRL.CacheServer.TcpServer(1129);
    cacheServer.Start();
    </pre>
    <strong>配置客户端</strong>
    <pre>
    //实现客户端调用
    //有多个服务器添加多个
    CRL.CacheServerSetting.AddTcpServerListen("127.0.0.1", 1129);
    CRL.CacheServerSetting.Init();
    </pre>
    启用远端缓存查询,重写CacheDataTestManage属性
    <pre>
    //是否从远程查询缓存
    protected override bool QueryCacheFromRemote
    {
        get
        {
            return true;
        }
    }
    </pre>
    <strong>调用</strong><asp:Button ID="Button1" runat="server" OnClick="Button1_Click" Text="测试" />
&nbsp;<strong><asp:Button ID="Button2" runat="server" OnClick="Button2_Click" Text="更新测试" />
    <asp:Button ID="Button3" runat="server" OnClick="Button3_Click" Text="Button" />
    </strong>
    <pre>
    var list = Code.CacheDataTestManage.Instance.QueryItemFromCache(b => b.Id > 0 && b.Name.Contains("name"));
    Response.Write(list.ToString());
    </pre>
</asp:Content>
