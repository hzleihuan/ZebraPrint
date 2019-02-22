<%@ Page Language="C#" AutoEventWireup="true"  CodeFile="Default.aspx.cs" Inherits="_Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>无标题页</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:Button ID="Button1" runat="server" OnClick="Button1_Click" Text="生成图片" Width="160" Height="90"  /><br />
        <table border="0" cellpadding="0" cellspacing="0">
            <tr>
                <td>条形码</td>
                <td>二维码</td>
            </tr>
              <tr>
                <td><asp:Image ID="Image1" runat="server" ImageUrl="~/icon/blank.png" Width="160" Height="90" /></td>
                <td><asp:Image ID="Image2" runat="server" ImageUrl="~/icon/blank.png" Width="160" Height="90"  /></td>
            </tr>
        </table>
          <br /><br /><br />
        <asp:Button ID="Button2" runat="server" OnClick="Button2_Click" Text="批量打印（ZPL指令）" Width="160" Height="90"   />
        <asp:Button ID="Button3" runat="server" OnClick="Button3_Click" Text="资产标签（40MM*30MM）小票打印（ZPL指令）" Width="160" Height="90"   />
        <br /><br /><br />
        <asp:Label ID="Label1" runat="server" Font-Bold="true" ForeColor="Green" Text="注：生成图片直接打印可选用PrintDocument"></asp:Label>
    </div>
    </form>
</body>
</html>
