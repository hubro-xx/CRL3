<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="DataVerification.aspx.cs" Inherits="WebTest.DataVerification" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h4>以下演示了数据有效性检测</h4>
    <blockquote>
    数据校验可以保证数据在插入或更新不符合规则时,终止操作,保证数据完整性
    <br />区别于前台验证,能实现业务逻辑验证,更灵活
    </blockquote>
<asp:Button ID="Button1" runat="server" OnClick="Button1_Click" Text="非法数据提交测试" />
<pre>
    //ProductData 限定了 BarCode不能为空
    public override string CheckData()
    {
        if (string.IsNullOrEmpty(BarCode))
        {
            return "BarCode不能为空";
        }
        if (Number < 0)
        {
            return "Number不能小于0";
        }
        return "";
    }

    //这里设为空,提交时会抛出异常
    var item = new Code.ProductData() { InterFaceUser = "2222", ProductName = "product2", BarCode = "" };
    var msg = item.CheckData();
    if (!string.IsNullOrEmpty(msg))//手动判断对象数据是否合法
    {
        Response.Write(msg);
    }
    try
    {
        Code.ProductDataManage.Instance.Add(item);
    }
    catch(Exception ero)//捕获异常
    {
        Response.Write(ero.Message);
    }
</pre>
</asp:Content>
