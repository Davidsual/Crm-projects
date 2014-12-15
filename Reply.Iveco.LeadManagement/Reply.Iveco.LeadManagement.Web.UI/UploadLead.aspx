<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UploadLead.aspx.cs" EnableViewState="true"
    Inherits="Reply.Iveco.LeadManagement.Web.UI.UploadLead" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<base target="_self" />
    <link rel="stylesheet" type="text/css" href="css/Screen_common.css" media="screen" />
    <link rel="stylesheet" type="text/css" href="css/core.css" media="screen" />
    <link rel="stylesheet" type="text/css" href="css/Style.css" media="screen" />
    <title>Importa lista</title>
</head>
<body>
    <form id="form1" runat="server">
    <table border="0" cellpadding="0" cellspacing="0" width="100%" style="margin:20px 0 0 20px;">
        <tr>
            <td>
                <asp:Label ID="lblDescrLoadingFile" runat="server" Text="Selezionare un file in formato CSV separato da punto e virgola con il tracciato previsto:" style="font-weight:bold;"></asp:Label>
                <br />
                <asp:FileUpload
                    ID="flUpload" runat="server" Width="400px" />
            </td>

        </tr>
        <tr>
            <td align="center">
                <asp:Button ID="btnLoad" runat="server" Text="Carica"
                    onclick="btnLoad_Click" style="margin:10px 0 10px 0;"/>
            </td>
        </tr>
    </table>
    <div id="divWait" style="display:none;width:100%;height:100%;background-image: url(image/progress.gif);
	background-position: center;
	background-repeat: no-repeat; 
	position:absolute;z-index:9999;top:0px;left:0px;background-color:Black;opacity:0.4;filter:alpha(opacity=40);">
    
    </div>
    </form>
</body>
</html>
