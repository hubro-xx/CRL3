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
    //要更新属性集合
    CRL.ParameCollection c = new CRL.ParameCollection();
    c["ProductName"] = "product1";
    Code.ProductDataManage.Instance.Update(b => b.Id == 4, c);
    //按对象差异更新
    var p = new Code.ProductData() { Id = 4 };
    //手动修改值时,指定修改属性以在Update时识别,分以下几种形式
    p.Change(b => b.BarCode);//表示值被更改了
    p.Change(b => b.BarCode,"123");//通过参数赋值
    p.Change(b => b.BarCode == "123");//通过表达式赋值
    Code.ProductDataManage.Instance.Update(b => b.Id == 4, p);//指定查询更新
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
    那么这样更新是被支持的,如果更新的项为0,则抛出异常
</asp:Content>
