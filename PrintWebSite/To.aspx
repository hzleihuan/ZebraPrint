﻿<%@ Page Language="C#" AutoEventWireup="true" CodeFile="To.aspx.cs" Inherits="To" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>无标题页</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:TextBox runat="server" ID="tb">
        </asp:TextBox>
        
        <asp:Button runat="server" ID="btn" Text="转换" onclick="btn_Click" />
    </div>
    <div>
        <asp:Literal runat="server" ID="lit"></asp:Literal>
    </div>
    </form>
</body>
</html>
