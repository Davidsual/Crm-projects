<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TestSetLead.aspx.cs" Inherits="Reply.Iveco.LeadManagement.Web.UI.Test.TestSetLead" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
     <center>
        <h1>
            SetLead Test Page</h1>
    </center>
    <div>
        <asp:Panel runat="server" ID="pnlCall" GroupingText="Test - Call SetLead"
            Style="border: solid 1px black;">
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
                        City</td>
                    <td>
                        <asp:TextBox ID="txtCity" runat="server" Text="Torino" Width="150"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        Country</td>
                    <td>
                        <asp:TextBox ID="txtCountry" runat="server" Text="Italy" Width="150"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        BusinessType</td>
                    <td>
                        <asp:TextBox ID="txtBT" runat="server" Text="07.29" Width="150"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        Campaign</td>
                    <td>
                        <asp:TextBox ID="txtCampaign" runat="server" Text="campagna prova" Width="150"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        Privacy</td>
                    <td>
                        <asp:TextBox ID="txtPrivacy" runat="server" Text="true" Width="150"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        InterlocutorRoleCode</td>
                    <td>
                        <asp:TextBox ID="txtRoleCode" runat="server" Text="2.000000" Width="150"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        NotaProdotto</td>
                    <td>
                        <asp:TextBox ID="txtNotaProdotto" runat="server" Text="Bla bla bla" Width="150"></asp:TextBox>
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
                        &nbsp;</td>
                    <td>
                        &nbsp;</td>
                </tr>
                <tr>
                    <td>
                        <asp:Button ID="Button1" runat="server" Text="Call SetLead CRMDealer" 
                            Width="171px" onclick="Button1_Click" />
                    </td>
                    <td>
                        &nbsp;</td>
                </tr>
            </table>
        </asp:Panel>
    </div>
    </form>
</body>
</html>
