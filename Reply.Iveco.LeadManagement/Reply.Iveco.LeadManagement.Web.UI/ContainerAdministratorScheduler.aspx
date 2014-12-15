<%@ Page Language="C#" AutoEventWireup="true" EnableViewState="true" CodeBehind="ContainerAdministratorScheduler.aspx.cs" 
    Inherits="Reply.Iveco.LeadManagement.Web.UI.ContainerAdministratorScheduler" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="X-UA-Compatible" content="IE=7" />
    <title></title>
</head>
<body style="width: 100%; height: 100%; background-color: #d6e8ff; margin:0px;padding:0px;">
    <form id="form1" runat="server">
    <div>
        <iframe style="position: fixed" id="frm" src="AdministratorScheduler.aspx?<%=Request.QueryString.ToString() %>"
            frameborder='0' height='100%' width='100%'></iframe>
    </div>
    </form>
</body>
</html>
