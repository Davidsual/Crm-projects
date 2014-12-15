<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TestFindDealer.aspx.cs" Inherits="Reply.Iveco.LeadManagement.Web.UI.Test.TestFindDealer" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
    </div>
            <table border="0" cellpadding="1" cellspacing="1" width="70%">
                <tr>
                    <td style="width:35%;">
                        Nome
                    </td>
                    <td>
                        <asp:TextBox ID="txtName" runat="server" Text="Alessia" Width="150"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        Surname
                    </td>
                    <td>
                        <asp:TextBox ID="txtSurname" runat="server" Text="Alias" Width="150"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        OrgName
                    </td>
                    <td>
                        <asp:TextBox ID="txtOrgName" runat="server" Text="IvecoSvilItalia" Width="150"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        IDLeadCrmLM</td>
                    <td>
                        <asp:TextBox ID="txtIdLead" runat="server" Text="LEAD123" Width="150"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        ZIPCode</td>
                    <td>
                        <asp:TextBox ID="txtZipCode" runat="server" Text="10134" Width="150"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        TypeContact</td>
                    <td>
                        <asp:TextBox ID="txtTypeContact" runat="server" Text="1" Width="150"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        Critical</td>
                    <td>
                        <asp:TextBox ID="txtCritical" runat="server" Text="true" Width="150"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        &nbsp;</td>
                    <td>
                        &nbsp;</td>
                </tr>
                <tr>
                    <td>
                        <asp:Button ID="Button1" runat="server" Text="Call FindDealer CRMDealer" 
                            Width="171px" onclick="Button1_Click" />
                    </td>
                    <td>
                        &nbsp;</td>
                </tr>
            </table>
    </form>
</body>
</html>
