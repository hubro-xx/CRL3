<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="pay.aspx.cs" validateRequest="false" Inherits="WebTest.pay" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
        <asp:TextBox ID="TextBox1" runat="server" Height="271px" TextMode="MultiLine"></asp:TextBox>
        <asp:Button ID="Button2" runat="server" OnClick="Button2_Click" Text="支付" />
    
    </div>
    </form>
</body>
</html>
