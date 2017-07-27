<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TcpTest.aspx.cs" Inherits="WebTest.TcpTest" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
        SERVER<asp:TextBox ID="TextBox1" runat="server"></asp:TextBox>
        Num<asp:TextBox ID="TextBox2" runat="server" Width="46px">10</asp:TextBox>
        <asp:Button ID="Button1" runat="server" OnClick="Button1_Click" Text="Button" />
    
        <asp:Button ID="Button2" runat="server" OnClick="Button2_Click" Text="Button" />
    
    </div>
    </form>
</body>
</html>
