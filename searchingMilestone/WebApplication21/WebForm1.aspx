<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebForm1.aspx.cs" Inherits="WebApplication21.WebForm1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body style="height: 497px">
    <form id="form1" runat="server">
        <div style="height: 95px">
            <asp:Label ID="Label1" runat="server" Font-Names="Candara" Font-Size="60px" Height="72px" style="margin-left: 568px; margin-top: 36px" Text="Search Engine" Width="411px"></asp:Label>
        </div>
        <p>
            <asp:TextBox ID="TextBox1" runat="server" Height="20px" style="margin-left: 433px; margin-top: 10px; margin-bottom: 33px" Width="633px"></asp:TextBox>
            <asp:Button ID="searchBtn" runat="server" BackColor="#E2241A" Font-Names="Microsoft Sans Serif" ForeColor="White" Height="43px" OnClick="searchBtn_Click" style="margin-left: 649px; margin-top: 0px" Text="Search" Width="220px" />
        </p>
 
       



        <asp:GridView ID="GridView1" runat="server" Height="300px" Width="1443px" style="margin-left: 25px">

        
        </asp:GridView>
 
       



    </form>
</body>
</html>
