<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="ExportModelInfo.aspx.cs" Inherits="WebTest.ExportModelInfo" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h4>以下演示了导出对象文档</h4>
    <blockquote>
    在数据库里查看表结构,其实是非常不方便,在项目里同样<br />
    通过CRL里提供的方法,导出指定程序集的对象文档,对象结构一目了然
    </blockquote>
    <strong>文档示例</strong><br />
    <img src="/img/model.png" /><p>
    <div class="alert alert-success">编译MODEL所在的DLL时,在项目属性=>生成 选中下面的框,生成XML文档</div><img src="xml.png" /> 
<br />
XML文档地址<asp:TextBox ID="TextBox1" runat="server">/bin/WebTest.xml</asp:TextBox>

<asp:Button ID="Button1" runat="server" OnClick="Button1_Click" Text="导出对象信息" />

<pre>
Type[] types = new Type[] { typeof(Code.ProductData) };
var xmlFiles = new List&lt;string>{ Server.MapPath(TextBox1.Text) };
CRL.SummaryAnalysis.ExportToFile(types, xmlFiles);
</pre>

</asp:Content>
