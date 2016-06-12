<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="Update.aspx.cs" Inherits="WebTest.Update" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h4>以下演示了如何更新数据</h4>
    <blockquote>
    表示值被更改有以下几种方式
    </blockquote>
    <asp:Button ID="Button1" runat="server" OnClick="Button1_Click" class="btn" Text="更新测试" />
    <pre>
    //更新传值有多种方式

    //字典传参的形式
    CRL.ParameCollection c = new CRL.ParameCollection();
    c["ProductName"] = "product1";
    Code.ProductDataManage.Instance.Update(b => b.Id == 4, c);

    //按匿名对象
    Code.ProductDataManage.Instance.Update(b => b.Id == 4, new { ProductName = "product1" });

    //按对象差异更新
    var p = new Code.ProductData() { Id = 4 };
    //手动修改值时,指定修改属性以在Update时识别,分以下几种形式
    p.Change(b => b.BarCode);//表示值被更改了
    p.Change(b => b.BarCode, "123");//通过参数赋值
    p.Change(b => b.BarCode == "123");//通过表达式赋值
    Code.ProductDataManage.Instance.Update(b => b.Id == 4, p);//指定查询更新

    //当对象是查询创建则能自动识别
    p = Code.ProductDataManage.Instance.QueryItem(b => b.Id > 0);
    p.UserId += 1;//只会更新UserId
    Code.ProductDataManage.Instance.Update(p);//按主键更新,主键值是必须的
    </pre>
    以上是需要通过方法修改属性值,若要实现直接给属性赋值时也能更新,则需要按以下格式实现属性构造
    <pre>
        string name;
        public string Name
        {
            get { return name; }
            set {
                name = value;
                SetChanges("name", value);
            }
        }
        var p = new Code.ProductData() { Id = 4 };
        p.Name="name2";
        Code.ProductDataManage.Instance.Update(p)</pre>
    那么这样更新是被支持的,如果更新的项为0,则抛出异常<br />
    <br />
    两表进行关联更新
    <pre>
        //关联更新
        c = new CRL.ParameCollection();
        //参数会按拼接处理
        c["UserId"] = "$UserId";//order.userid=product.userid
        c["Remark"] = "2222";//order.remark=2222
        Code.OrderManage.Instance.Update&lt;Code.ProductData>((a, b) => a.Id == b.Id && b.Number > 10, c);
        //等效语句为 update order set userid=ProductData.userid,remark='2222' from ProductData where order.id=ProductData.id and ProductData.number<10
    </pre>
</asp:Content>
