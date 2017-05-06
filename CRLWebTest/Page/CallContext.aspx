<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="CallContext.aspx.cs" Inherits="WebTest.Page.CallContext" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h4>使用CallContext提升程序性能</h4>
    <blockquote>
    CRL内置了CallContext实现<br />
        <ul>
        <li>使用数据库事务PackageTrans2</li>
        <li>同一方法调用,内部AbsDBExtend实现</li>
        <li>CRLDbConnectionScope实现</li>
        </ul>
    </blockquote>
    <h4>使用数据库事务PackageTrans2</h4>

    使用此方法后,范围内所有数据操作会生成一个事务,并且可以嵌套调用

    <h4>内部AbsDBExtend实现</h4>
    在当前方法调有,多个ProviderOrigin实例只会关联一个AbsDBExtend,如以下代码
    <pre>
protected void Button1_Click(object sender, EventArgs e) {
    var item = Code.ProductDataManage.Instance.QueryItem(2);
    var item2 = Code.ProductDataManage.Instance.QueryItem(2);
        }</pre>
    然虽实现了两个Instance,但在内部只会生成一个AbsDBExtend
    <h4>CRLDbConnectionScope实现</h4>
    使用CRLDbConnectionScope可以把范围内数据访问只用一个数据连接,如:
    <pre>
      using (var context = new CRL.CRLDbConnectionScope())//使用同一个数据连接
            {
                var item = Code.ProductDataManage.Instance.QueryItem(b => b.Id > 0);
                var item2 = Code.ProductDataManage.Instance.QueryItem(2);
            }
    </pre>
    与此关联的DbConnection,只会打开一次
</asp:Content>
