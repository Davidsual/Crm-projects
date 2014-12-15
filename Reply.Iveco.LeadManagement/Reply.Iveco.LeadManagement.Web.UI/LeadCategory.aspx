<%@ Page Language="C#" AutoEventWireup="true" EnableViewState="true" CodeBehind="LeadCategory.aspx.cs"
    Inherits="Reply.Iveco.LeadManagement.Web.UI.LeadCategory" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <base target="_self" />
    <link rel="stylesheet" type="text/css" href="css/Screen_common.css" media="screen" />
    <link rel="stylesheet" type="text/css" href="css/core.css" media="screen" />
    <link rel="stylesheet" type="text/css" href="css/Style.css" media="screen" />
    <title>Lead Category</title>
</head>
<body>
    <form id="form1" runat="server">
    <table border="0" cellpadding="3" cellspacing="0" width="100%" style="margin: 20px 0 0 20px;">
        <tr>
            <td>
                <asp:DropDownList ID="ddlLeadCategory" runat="server">
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td>
                <div id="divMail" runat="server" visible="false" style="margin-top:5px;">
                    <table border="0" cellpadding="0" cellspacing="0" width="100%">
                        <tr>
                            <td style="width:30%;">
                                <asp:Label ID="lblMail" runat="server" Text="E-Mail Sent"></asp:Label>
                            </td>
                            <td style="width:70%;">
                                <asp:CheckBox ID="chkMail" runat="server" Checked="false"/>
                            </td>
                        </tr>
                        <tr>
                            <td style="width:30%;">
                                <asp:Label ID="lblSms" runat="server" Text="SMS Sent"></asp:Label>
                            </td>
                            <td style="width:70%;">
                                <asp:CheckBox ID="chkSms" runat="server" Checked="false"/>
                            </td>
                        </tr>
                    </table>
                </div>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:Button ID="btnLoad" runat="server" Text="Select" OnClientClick="if(document.getElementById('ddlLeadCategory').selectedIndex == 0) {alert('Fill Lead category');return false;}javascript:window.opener.GENERIC_fCloseLead(document.getElementById('ddlLeadCategory').options[document.getElementById('ddlLeadCategory').selectedIndex].value,(document.getElementById('chkMail') != null)?document.getElementById('chkMail').checked:false,(document.getElementById('chkSms') != null)?document.getElementById('chkSms').checked:false);window.close();"
                    Style="margin: 10px 0 10px 0;" />
            </td>
        </tr>
    </table>
    </form>
</body>
</html>
