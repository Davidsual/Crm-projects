<%@ Page Language="C#" AutoEventWireup="true" EnableViewState="true" CodeBehind="Dialog.aspx.cs" Inherits="Reply.Iveco.LeadManagement.CrmDealerLead.Dialog" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD HTML 1.0 Transitional//EN">
<html>
<head id="Head1" runat="server">
    <base target="_self" />
    <title>Dialog</title>
    <link href="CRM_Style.css" rel="Stylesheet" type="text/css" />
    
    <link href="./Styles/common.css" rel="Stylesheet" type="text/css" />
    <link href="./Styles/dialogs.css" rel="Stylesheet" type="text/css" />
    <link href="./Styles/fonts.css" rel="Stylesheet" type="text/css" />
    <link href="./Styles/gridcontrol.css" rel="Stylesheet" type="text/css" />
    <link href="./Styles/lookup.css" rel="Stylesheet" type="text/css" />
    <link href="./Styles/menu.css" rel="Stylesheet" type="text/css" />
    
</head>

<body scroll="no">

<script language="javascript" type="text/javascript">
    
    function button_press(value)
    {
        window.returnValue = value;
        window.close();
    }
    
</script>


    <form id="form1" runat="server">
        <table style="width: 100%; height: 100%" cellspacing="0" cellpadding="0">
        <tr style="height: 51px">
            <td class="mscrm-Dialog-Header" style="">
                <div id="DlgHdTitle" class="mscrm-Dialog-Header-Title">
                    <%=TITLE %></div>
                <div id="DlgHdDesc" class="mscrm-Dialog-Header-Desc"></div>
            </td>
        </tr>
        <tr style="height: 100%; text-align:left;">
            <td style="padding:15px">
                <%=DESCRIPTION %>
            </td>
        </tr>
        <!-- Riga di spaziatura -->
        <tr style="height: 10px">
            <td style="border-bottom: #cccccc 1px solid; height: 10px; font-size: 1px">
                &nbsp;
            </td>
        </tr>
        <tr style="height: 10px">
            <td style="height: 10px; font-size: 1px; border-top: #ffffff 1px solid">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td class="mscrm-Dialog-Footer mscrm-Dialog-Footer-Right" style="padding-left: 5px;
                padding-right: 5px; padding-bottom: 10px; border: 0px">
                <button id="btn_0" class="mscrm-Button" runat="server" visible="false" onclick="button_press(0);"  style="width:auto">
                    <%=button[0] %></button>&nbsp;
                <button id="btn_1" class="mscrm-Button" runat="server" visible="false" onclick="button_press(1);"  style="width:auto">
                    <%=button[1] %></button>&nbsp;
                <button id="btn_2" class="mscrm-Button" runat="server" visible="false" onclick="button_press(2);"  style="width:auto">
                    <%=button[2] %></button>&nbsp;
            </td>
        </tr>
    </table>
    </form>
</body>
</html>
