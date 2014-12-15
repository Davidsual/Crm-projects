<%@ Page Language="C#" AutoEventWireup="true" EnableViewState="true" CodeBehind="UploadCallBackData.aspx.cs" 
    Inherits="Reply.Iveco.LeadManagement.Web.UI.UploadCallBackData" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<base target="_self" />
    <link rel="stylesheet" type="text/css" href="css/Screen_common.css" media="screen" />
    <link rel="stylesheet" type="text/css" href="css/core.css" media="screen" />
    <link rel="stylesheet" type="text/css" href="css/Style.css" media="screen" />
    <title>Upload CallBack Data</title>
</head>
<body>
    <form id="form1" runat="server">
    <table border="0" cellpadding="3" cellspacing="0" width="100%" style="margin:20px 0 0 20px;">
        <tr>
            <td>
                <asp:FileUpload
                    ID="flUpload" runat="server" Width="400px" />
            </td>

        </tr>
        <tr>
            <td align="center">
                <asp:Button ID="btnLoad" runat="server" Text="Load" OnClientClick="if(document.getElementById('flUpload').value == ''){alert('Select file to Upload');return false;} document.getElementById('divWait').style.display = 'block';"
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
