<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ConfirmPage.aspx.cs" Inherits="Reply.Iveco.LeadManagement.Web.UI.ConfirmPage" meta:resourcekey="PageResource1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link rel="stylesheet" type="text/css" href="css/Screen_common.css" media="screen" />
    <link rel="stylesheet" type="text/css" href="css/core.css" media="screen" />
    <title>Confirm Page</title>
</head>
<body>
    <form id="form1" runat="server">
    <div class="body">
        <div class="f_contattaci">
            <div class="container_form_contatti">
                <div class="row grey" style="background-color: #eeeeee;">
                    <h1 style="font-size: large">
                        <%= GetLocalResourceObject("h1Title.Text")%></h1>
                </div>
                <div id="Sezione1">
                    <div class="row">
                        <%= GetLocalResourceObject("lblPrenotazioneAvvenuta.Text")%>
                    </div>
                    <div class="row">
                        <%= GetLocalResourceObject("lblDettaglioPrenotazione.Text")%> <b>25/12/2010</b> <%= GetLocalResourceObject("From.Text")%> <b>10.00</b> <%= GetLocalResourceObject("To.Text")%> <b>11.00</b>.
                    </div>
                    <div class="row">
                        <%= GetLocalResourceObject("lblSendMail.Text")%> <b>utente@iveco.com</b>.
                    </div>
                    <div class="row">
                        <%= GetLocalResourceObject("lblSaluti.Text")%>
                    </div>
                    <div class="row">
                        Iveco.com
                    </div>
                </div>
            </div>
        </div>
    </div>
    </form>
</body>
</html>
