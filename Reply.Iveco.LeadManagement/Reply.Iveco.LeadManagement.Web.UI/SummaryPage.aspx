<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SummaryPage.aspx.cs"
    Inherits="Reply.Iveco.LeadManagement.Web.UI.SummaryPage" Culture="auto" meta:resourcekey="PageResource1"
    UICulture="auto" %>

<%@ Register Assembly="MSCaptcha" Namespace="MSCaptcha" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link rel="stylesheet" type="text/css" href="css/Screen_common.css" media="screen" />
    <link rel="stylesheet" type="text/css" href="css/core.css" media="screen" />
    <title>Contact Summary</title>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <asp:UpdatePanel ID="updPnlCapcha" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <div class="body">
            <label id="lblLanguage">
                                        <%= GetLocalResourceObject("lblLanguage.Text")%></label>
                <asp:DropDownList ID="ddlLingua" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlLingua_SelectedIndexChanged">
                    <asp:ListItem Value="it-IT">Italiano</asp:ListItem>
                    <asp:ListItem Value="en-GB">Inglese</asp:ListItem>
                </asp:DropDownList>
                <div class="f_contattaci">
                    <div class="container_form_contatti">
                        <!-- sezione combo -->
                        <div style="display: none; margin-left: 10px">
                            <br />
                            <p>
                                <label id="lblObbligatori1">
                                    Per poter soddisfare la tua richiesta nel modo più rapido ed efficace possibile,
                                    ti preghiamo di compilare il form in tutti i suoi campi</label></p>
                        </div>
                        <!-- sezione 1 -->
                        <div id="Sezione1">
                            <div class="row">
                                <h2 id="lblTitolo1">
                                    <%= GetLocalResourceObject("lblTitolo1.Text")%></h2>
                                <p>
                                    <label id="lblNome">
                                        <%= GetLocalResourceObject("lblNome.Text")%></label>
                                    <input style="background-color: #99ccff" id="firstName" type="text" name="firstName" /></p>
                                <p>
                                    <label id="lblCognome" runat="server">
                                        <%= GetLocalResourceObject("lblCognome.Text") %></label>
                                    <input style="background-color: #99ccff" id="lastName" type="text" name="lastName" /></p>
                                <input id="lblAlert1" value="Una o più informazioni necessarie non sono state inserite"
                                    type="hidden" name="lblAlert1" />
                                <input id="lblAlert2" value="Il numero di telaio deve essere composto da 17 cifre alfanumeriche"
                                    type="hidden" name="lblAlert2" />
                                <input id="lblAlert3" value="Una o più informazioni necessarie non sono state inserite. I campi contrassegnati da * sono obbligatori."
                                    type="hidden" name="lblAlert3" />
                                <input id="lblAlert4" value="Il codice di verifica non è corretto" type="hidden"
                                    name="lblAlert4" />
                            </div>
                            <div class="row">
                                <p>
                                    <label id="lblTelefono">
                                        <%= GetLocalResourceObject("lblTelefono.Text")%></label>
                                    <input style="background-color: #99ccff" id="telephone" type="text" name="telephone" /></p>
                                <p>
                                    <label id="lblEmail">
                                        <%= GetLocalResourceObject("lblEmail.Text")%></label>
                                    <input style="background-color: #99ccff" id="eMail" type="text" name="eMail" /></p>
                            </div>
                        </div>
                        <!-- sezione 2 -->
                        <div id="Sezione2">
                        </div>
                        <!-- sezione 4 -->
                        <div style="float: right;" id="Sezione5" class="row" align="right">
                            <table border="0" cellpadding="1" cellspacing="2">
                                <tr>
                                    <td>
                                        <cc1:CaptchaControl ID="ctrlCaptcha" runat="server" CaptchaLength="3" CaptchaHeight="40"
                                            CaptchaWidth="100" CaptchaBackgroundNoise="Extreme" CaptchaLineNoise="Medium"  />
                                    </td>
                                    <td>
                                        <label id="lblCodiceVerifica" style="float: none;">
                                            <%= GetLocalResourceObject("lblCodiceVerifica.Text")%></label><br />
                                        <asp:TextBox Style="background-color: #99ccff; height: 30px; font-size: 24px" ID="txtCaptcha"
                                            runat="server" />
                                    </td>
                                    <td>
                                        <table border="0" cellpadding="0" cellspacing="0" width="100%">
                                            <tr>
                                                <td>
                                                    <p class="button">
                                                        <asp:ImageButton ID="btnAsap" ImageUrl="image/btn_asap.gif" runat="server" Style="border-bottom: #000066 0px solid;
                                                            border-left: #000066 0px solid; padding-bottom: 3px; background-color: transparent;
                                                            margin: 9px 0px 0px; padding-left: 3px; padding-right: 3px; border-top: #000066 0px solid;
                                                            cursor: hand; border-right: #000066 0px solid; padding-top: 3px" 
                                                            OnClick="btnInvia_Click" CommandName="PARAMS" CommandArgument="ASAP" />
                                                    </p>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <p class="button">
                                                        <asp:ImageButton ID="btnInvia" ImageUrl="image/btn_invia.gif" runat="server" Style="border-bottom: #000066 0px solid;
                                                            border-left: #000066 0px solid; padding-bottom: 3px; background-color: transparent;
                                                            margin: 0px 0px 0px; padding-left: 3px; padding-right: 3px; border-top: #000066 0px solid;
                                                            cursor: hand; border-right: #000066 0px solid; padding-top: 3px" OnClick="btnInvia_Click" />
                                                    </p>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </div>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
    </form>
</body>
</html>
