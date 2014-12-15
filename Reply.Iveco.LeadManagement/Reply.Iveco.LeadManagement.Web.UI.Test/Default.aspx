<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Reply.Iveco.LeadManagement.Web.UI.Test._Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
        <asp:Button ID="btnGetCalendar" runat="server" Text="Get Calendar" 
            onclick="btnGetCalendar_Click" />
        <br />
        <br />
        <asp:Button ID="btnSetAppointment" runat="server" Text="Set Appointment" 
            onclick="btnSetAppointment_Click" />
        <br />
        <br />
        <asp:Button ID="btnSetDealert" runat="server" Text="Set Dealer" 
            onclick="btnSetDealert_Click" />
                
    </div>
    </form>
</body>
</html>
