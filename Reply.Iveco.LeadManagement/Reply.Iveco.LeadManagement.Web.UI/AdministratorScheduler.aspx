<%@ Page Language="C#" AutoEventWireup="true" EnableViewState="true" CodeBehind="AdministratorScheduler.aspx.cs"
    Inherits="Reply.Iveco.LeadManagement.Web.UI.AdministratorScheduler" %>

<%@ Register Src="controls/Scheduler.ascx" TagName="Scheduler" TagPrefix="UserControl" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="X-UA-Compatible" content="IE=7" />
    <link rel="stylesheet" type="text/css" href="css/Screen_common.css" media="screen" />
    <link rel="stylesheet" type="text/css" href="css/core.css" media="screen" />
    <link rel="stylesheet" type="text/css" href="css/Style.css" media="screen" />
    <title>Booking Page</title>
    <style lang="it" media="screen">
        .CrmButton
        {
            border-right: #3366cc 1px solid;
            padding-right: 5px;
            border-top: #3366cc 1px solid;
            padding-left: 5px;
            font-size: 11px;
            background-position: center;
            background-image: url('./Image/btn_rest.gif');
            border-left: #3366cc 1px solid;
            width: 84px;
            cursor: pointer;
            line-height: 18px;
            border-bottom: #3366cc 1px solid;
            background-repeat: repeat-x;
            font-family: Tahoma;
            height: 23px;
            background-color: #cee7ff;
            text-align: center;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <asp:UpdatePanel ID="updPnlScheduler" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <table border="0" cellpadding="0" cellspacing="0" width="100%" style="margin-left: 5px;">
                <tr>
                    <td style="width: 15%;">
                        <span><b>Country</b></span><br />
                        <asp:DropDownList ID="ddlCountry" runat="server" Style="width: 150px;" AutoPostBack="True"
                            OnSelectedIndexChanged="ddlCountry_SelectedIndexChanged">
                        </asp:DropDownList>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="ddlCountry"
                            ValidationGroup="SCHEDULER" ErrorMessage="Select a country">!</asp:RequiredFieldValidator>
                    </td>
                    <td style="width: 15%;">
                        <span><b>Language</b></span><br />
                        <asp:DropDownList ID="ddlLanguage" runat="server" Style="width: 150px;">
                        </asp:DropDownList>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="ddlLanguage"
                            ValidationGroup="SCHEDULER" ErrorMessage="Select a language">!</asp:RequiredFieldValidator>
                    </td>
                    <td style="width: 70%;">
                        <asp:Button ID="btnShowCalendar" CssClass="CrmButton" Style="margin-top: 10px; width: 150px;"
                            runat="server" Text="Show calendar" OnClientClick="if(Page_ClientValidate('SCHEDULER'))document.getElementById('divWait').style.display = 'block';"
                            ValidationGroup="SCHEDULER" OnClick="btnShowCalendar_Click" /><asp:ValidationSummary
                                ID="ValidationSummary1" runat="server" ValidationGroup="SCHEDULER" ShowMessageBox="true"
                                ShowSummary="false" />
                    </td>
                </tr>
            </table>
            <table border="0" cellpadding="0" cellspacing="0" width="100%">
                <tr>
                    <td>
                        <div style="overflow: hidden; margin: 20px 5px 5px 5px;">
                            <UserControl:Scheduler ID="MyScheduler" runat="server" />
                        </div>
                    </td>
                </tr>
            </table>
            <div id="divWait" style="display: none; width: 100%; height: 100%; background-image: url(image/progress.gif);
                background-position: center; background-repeat: no-repeat; position: absolute;
                z-index: 9999; top: 0px; left: 0px; background-color: Black; opacity: 0.4; filter: alpha(opacity=40);">
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
    </form>
</body>
</html>
