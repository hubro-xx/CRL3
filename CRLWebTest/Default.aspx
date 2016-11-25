<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="WebTest.Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div>
        <h2>CRL <%=WebTest.Code.Setting.GetVersion() %></h2>
        <blockquote>
            <ul class="unstyled" style="font-size: 15px; line-height: 25px;">
                <li>CRL是一个面向对象的轻便型开发框架
            <ul>
                <li>通过抽象数据库架构和数据库实现,支持关系和非关系类型数据库</li>
                <li>数据处理使用了对象/数据映射,采用Lambda表达式来表示条件查询,增加了可编程性和可靠性,出错机率低,同时也能用原生的SQL实现查询或操作</li>
                <li>数据连接以编程方式进行配置,支持多个库,多种数据库类型,参见Global.asax中实现,<strong>首次使用请更改LocalSqlHelper.CreateDbHelper中数据连接</strong></li>
                <li>通过业务对象封装继承,实现业务重用性,比较常用的封装有,会员/账户系统,字典配置,分类系统,在线支付,订单/购物车,权限验证/菜单系统等等,当然也可以写自已的业务封装</li>
                
            </ul>
                </li>
                <li><strong>开发效率</strong>:不需要额外工具生成,不需要繁琐的拼接字符串,通过代码复用,大大提高开发速度</li>
                <li><strong>运行效率</strong>:对象映射作了很大优化,并且CRL对象查询默认是 with(nolock) 
                </li>
                <li><strong>缓存支持</strong>:光操作数据,没缓存怎么能行, 通过对象缓存绑定,很轻松实现缓存创建和调用
            <br />
                    通过内置分布式解决方案,轻松实现分布式缓存服务
                </li>
                <li><strong>数据安全</strong>:所有标准查询都经过参数化处理,无注入风险</li>
                <li><strong>动态编译</strong>:数据表自动创建/动态存储过程支持与查询转换,极大减少了数据库维护工作,增加开发效率</li>
                <li><strong>关联支持</strong>:2.2版支持多表关联查询,返回两种结果,供不同场景使用,见[关联查询]示例</li>
                <li><strong>分布式缓存</strong>:2.3版支持分布式对象缓存,可以跟使用本地缓存一样调用分布式缓存了</li>
                <li>
                    <strong>大数据支持</strong>:
        2.4版内置大数据分库分表解决方案,轻松实现库表拆分,ORM实现管理
                </li>
                <li>
                    <strong>非关系型数据库支持</strong>:
        4.0框架架构重新整理,非关系型数据库也能支持了,目前实现了MongoDB,通过CRL统一查询,不用再写各种风格不同的查询语法了
                </li>
            </ul>
        </blockquote>
        <h5>特点详细示例:</h5>
        <ul>
            <li><strong>新的开发模式(CodeFirst)</strong>
                <ul>
                    <li>传统开发需先设计表,再设计对象,CRL省略的这一步,直接设计对象,对象为主,数据表为辅,更符合面象向对象的开发模式</li>
                    <li>无需额外工具生成实体类,按标准方式写即可,即写即用,运行后自动创建对象对应数据表</li>
                    <li>新增加的对象属性也不用上数据库维护了,它会自动进行检查创建</li>
                </ul>
            </li>
            <li><strong>调用简单</strong>
                通过表达式查询转换为等效SQL语法,如: 
                <br />
                <pre>
    继承实现业务类  
    public class OrderManage : CRL.Order.OrderBusiness
    对象操作不再需要传入T对象类型
    var order = OrderManage.Instance.QueryItem(b=>b.Id==1 &&  b.UserId==2);
    等效为  
    select * from Order where id=1 and UserId=2
    更新删除同理
 </pre>
            </li>

            <li><strong>业务封装控制</strong>
                数据访问对象不会直接暴露在外面,需要通过业务类才能实现操作,通过这个约束,限定业务必须在类务类实现,达到封装的目的
             
                <pre>
    public bool TransactionTest(out string message) 
    { 
        message = ""; 
        var helper = DBExtend; 
        helper.BeginTran(); 
        try { 
        helper.Delete(b => b.Id == 1); 
        var item = new Code.ProductData() { InterFaceUser = "2222", ProductName = "product2", BarCode = "" };
        helper.InsertFromObj(item);
        helper.CommitTran(); 
        message = "事务已提交"; 
        return true; } 
        catch(Exception ero) { message = ero.Message + " 事务已回滚"; helper.RollbackTran(); } 
        return false; 
    }
                 </pre>
            </li>
            <li><strong>缓存绑定</strong> 对任意基本查询可作数据缓存处理,设置过期时间后自动更新缓存,或直接创建当前对象数据缓存
             
                <pre>
    var query = Code.ProductDataManage.Instance.GetLamadaQuery();//创查完整查询
    query.Where(b => b.Id < 700); 
    int exp = 10;//过期分钟 
    var list = Code.ProductDataManage.Instance.QueryList(query, exp);//返回一个查询缓存,条件不一样,缓存也不一样
    调用对象数据缓存
    var list = Code.ProductDataManage.Instance.QueryFromAllCache(b => b.Id < 700);//在内部缓存数据中查找 
                </pre>
            </li>
            <li><strong>动态编译(仅MSSQL)</strong> 将任意查询/更新/删除操作自动编译为数据库等效存储过程,大大提高运行效率<br />
                <pre>
    var query = Code.ProductDataManage.Instance.GetLamadaQuery();
    query.Where(b => b.Id < 700); 
    string name = Request["name"]; 
    query.Where(b => b.InterFaceUser == name);
    var list = Code.ProductDataManage.Instance.QueryList(query, compileSp: true);
    将会创建并调用等效存储过程
    CREATE PROCEDURE [dbo].[ZautoSp_6B517FF62BDE99E6] 
    (@id0 nvarchar(500),@InterFaceUser1 nvarchar(500)) 
    AS set nocount on
    select t1.[AddTime],t1.[BarCode],t1.[CategoryName],t1.[Id],t1.[InterFaceUser],t1.[Number],
    t1.[ProductChannel],t1.[ProductId],t1.[ProductName],t1.[PurchasePrice],t1.[SoldPrice],
    t1.[Style],t1.[SupplierId],t1.[SupplierName],t1.[TransType] 
    from ProductData t1 with(nolock) where (t1.Id<@Id0) and InterFaceUser=@InterFaceUser1 
                </pre>
            </li>
            <li><strong>继承使用业务封装</strong> 通过继承对象或业务类型,调用内置业务封装或实现自已的业务封装,增加开发效率
             
               <pre>
    var user = new User(){Name="test"};
    UserManage.Instance.Login(user,"user",false);//实现Form验证登录,并设定票据
</pre>
            </li>
            <li><strong>多种结果类型返回</strong>个性索引值和Bag取值更方便 关联查询值可存入对象索引器,model["Name"]等效为 model.Bag.Name</li>
            <li><strong>多数据库支持</strong> 通过实现数据库适配器,实现多数据库支持,详见&quot;支持数据库详细&quot;</li>
        </ul>
        <h5>支持数据库详细:</h5>
        <table border="1" cellpadding="4" style="font-size: 13px;" class="table">
            <tr>
                <td>数据库</td>
                <td>基本查询</td>
                <td>自动创建表</td>
                <td>with(nolock)查询</td>
                <td>批量插入</td>
                <td>存储过程</td>
                <td>动态编译存储过程</td>
                <td>自带业务封装</td>
                <td>备注</td>
            </tr>
            <tr>
                <th>MSSQL</th>
                <td>支持</td>
                <td>支持</td>
                <td>支持</td>
                <td>支持</td>
                <td>支持</td>
                <td>支持</td>
                <td>支持</td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <th>MySQL</th>
                <td>支持</td>
                <td>支持</td>
                <td class="notSupported">不支持</td>
                <td class="notSupported">不支持</td>
                <td>支持</td>
                <td class="notSupported">不支持</td>
                <td>支持</td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <th>ORACLE</th>
                <td>支持</td>
                <td>支持(需高级权限)</td>
                <td class="notSupported">不支持</td>
                <td class="notSupported">不支持</td>
                <td>支持</td>
                <td class="notSupported">不支持</td>
                <td>支持</td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <th style="color:blue">MongoDB</th>
                <td>支持</td>
                <td>支持</td>
                <td class="notSupported">不支持</td>
                <td>支持</td>
                <td class="notSupported">不支持</td>
                <td class="notSupported">不支持</td>
                <td>支持</td>
                <td>&nbsp;</td>
            </tr>
        </table>
        <h5>.Net Framework 4.5以上</h5>
        <h5>建议配合MVC使用,面向对象开发会省很多事</h5>
    </div>
    <br />
</asp:Content>
