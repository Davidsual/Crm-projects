<%@ Page Language="C#" AutoEventWireup="true" EnableViewState="true"  EnableEventValidation="false" CodeBehind="TestPage.aspx.cs" Inherits="Reply.Iveco.LeadManagement.Web.UI.TestPage" %>

<%@ Register Src="controls/Scheduler.ascx" TagName="Scheduler" TagPrefix="UserControl" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link rel="stylesheet" type="text/css" href="css/Screen_common.css" media="screen" />
    <link rel="stylesheet" type="text/css" href="css/core.css" media="screen" />
    <link rel="stylesheet" type="text/css" href="css/Style.css" media="screen" />
    <title>LeadManagement Test Page</title>
</head>
<body style="overflow: auto;">
    <form id="form1" runat="server">

    <center>
        <h1>
            Lead Management Test Page</h1>
    </center>
    <asp:Menu ID="Menu1" Width="168px" runat="server" Orientation="Horizontal" StaticEnableDefaultPopOutImage="False"
        OnMenuItemClick="Menu1_MenuItemClick" BorderStyle="None" DynamicHorizontalOffset="10">
        <DynamicMenuItemStyle BorderStyle="Solid" HorizontalPadding="10px" ItemSpacing="10px" />
        <Items>
            <asp:MenuItem Text="-   Set Appointment   -" Value="0"></asp:MenuItem>
            <asp:MenuItem Text="-   Set Dealer   -" Value="1"></asp:MenuItem>
            <asp:MenuItem Text="-   Get Dealer   -" Value="2"></asp:MenuItem>
        </Items>
    </asp:Menu>
    <asp:MultiView ID="MultiView1" runat="server" ActiveViewIndex="0">
        <asp:View ID="Tab1" runat="server">
            <div>
                <asp:Panel runat="server" ID="pnlChiamata" GroupingText="Test - Chiamata ASAP / BOOKING"
                    Style="border: solid 1px black;">
                    <table border="0" cellpadding="1" cellspacing="1" width="70%">
                        <tr>
                            <td style="width: 35%;">
                                Nome
                            </td>
                            <td>
                                <asp:TextBox ID="txtNome" runat="server" Text="Davide" Width="150"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Cognome
                            </td>
                            <td>
                                <asp:TextBox ID="txtCognome" runat="server" Text="Trotta" Width="150"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Numero Telefono
                            </td>
                            <td>
                                <asp:TextBox ID="txtTelNum" runat="server" Text="0214564132" Width="150"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Language
                            </td>
                            <td>
                                <asp:DropDownList ID="ddlLanguage" runat="server" Width="150">
                                    <asp:ListItem Value="" Text="" Selected="True"></asp:ListItem>
                                    <asp:ListItem Value="francese" Text="francese"></asp:ListItem>
                                    <asp:ListItem Value="italiano" Text="italiano"></asp:ListItem>
                                    <asp:ListItem Value="Spagnolo" Text="Spagnolo"></asp:ListItem>
                                    <asp:ListItem Value="Tedesco" Text="Tedesco"></asp:ListItem>
                                    <asp:ListItem Value="Polacco" Text="Polacco"></asp:ListItem>
                                    <asp:ListItem Value="Inglese" Text="Inglese"></asp:ListItem>
                                    <asp:ListItem Value="Portoghese" Text="Portoghese"></asp:ListItem>
                                    <asp:ListItem Value="Olandese" Text="Olandese"></asp:ListItem>
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Country
                            </td>
                            <td>
                                <asp:DropDownList ID="ddlCountry" runat="server" Width="150">
                                    <asp:ListItem Value="" Text="" Selected="True"></asp:ListItem>
                                    <asp:ListItem Value="Francia" Text="Francia"></asp:ListItem>
                                    <asp:ListItem Value="Italia" Text="Italia"></asp:ListItem>
                                    <asp:ListItem Value="Spagna" Text="Spagna"></asp:ListItem>
                                    <asp:ListItem Value="Germania" Text="Germania"></asp:ListItem>
                                    <asp:ListItem Value="Polonia " Text="Polonia "></asp:ListItem>
                                    <asp:ListItem Value="Austria" Text="Austria"></asp:ListItem>
                                    <asp:ListItem Value="UK" Text="UK"></asp:ListItem>
                                    <asp:ListItem Value="Marocco" Text="Marocco"></asp:ListItem>
                                    <asp:ListItem Value="South Africa" Text="South Africa"></asp:ListItem>
                                    <asp:ListItem Value="Algeria" Text="Algeria"></asp:ListItem>
                                    <asp:ListItem Value="Tunisia" Text="Tunisia"></asp:ListItem>
                                    <asp:ListItem Value="Svizzera" Text="Svizzera"></asp:ListItem>
                                    <asp:ListItem Value="Portogallo" Text="Portogallo"></asp:ListItem>
                                    <asp:ListItem Value="Netherland" Text="Netherland"></asp:ListItem>
                                    <asp:ListItem Value="Belgium" Text="Belgium"></asp:ListItem>
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                InitiativeSource
                            </td>
                            <td>
                                <asp:TextBox ID="txtInitiativeSource" runat="server" Width="150" Text="web.iveco.com/France"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                InitiativeSourceReport
                            </td>
                            <td>
                                <asp:TextBox ID="txtInitiativeSourceReport" runat="server" Width="150" Text="Trouver véhicule"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                InitiativeSourceReportDetail
                            </td>
                            <td>
                                <asp:TextBox ID="txtInitiativeSourceReportDetail" runat="server" Width="150" Text="35C11 - Empt 3000"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Inizio Schedulazione dd/mm/yyyy / hh:mm:ss
                            </td>
                            <td>
                                <asp:TextBox ID="txtStartDate" runat="server"></asp:TextBox>&nbsp;/&nbsp;<asp:TextBox
                                    ID="txtStartTime" runat="server"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Fine Schedulazione dd/mm/yyyy / hh:mm:ss
                            </td>
                            <td>
                                <asp:TextBox ID="txtEndDate" runat="server"></asp:TextBox>&nbsp;/&nbsp;<asp:TextBox
                                    ID="txtEndTime" runat="server"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Data Lead Creation
                            </td>
                            <td>
                                <asp:TextBox ID="txtDataLeadCreation" runat="server" Width="150" Text=""></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Indirizzo
                            </td>
                            <td>
                                <asp:TextBox ID="txtIndirizzo" runat="server" Width="150" Text=""></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                CAP
                            </td>
                            <td>
                                <asp:TextBox ID="txtCap" runat="server" Width="150" Text=""></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Città
                            </td>
                            <td>
                                <asp:TextBox ID="txtCitta" runat="server" Width="150" Text=""></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Provincia
                            </td>
                            <td>
                                <asp:TextBox ID="txtProvincia" runat="server" Width="150" Text=""></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                E-Mail
                            </td>
                            <td>
                                <asp:TextBox ID="txtEMail" runat="server" Width="150" Text=""></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Num. Tel Cellulare
                            </td>
                            <td>
                                <asp:TextBox ID="txtNumCellPhone" runat="server" Width="150" Text=""></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Brand
                            </td>
                            <td>
                                <asp:DropDownList ID="ddlBrand" runat="server" Width="150">
                                    <asp:ListItem Value="" Text="" Selected="True"></asp:ListItem>
                                    <asp:ListItem Value="Iveco" Text="Iveco"></asp:ListItem>
                                    <asp:ListItem Value="Iveco Used" Text="Iveco Used"></asp:ListItem>
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Model
                            </td>
                            <td>
                                <asp:TextBox ID="txtModel" runat="server" Width="150" Text=""></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Type
                            </td>
                            <td>
                                <asp:TextBox ID="txtType" runat="server" Width="150" Text=""></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                GVW
                            </td>
                            <td>
                                <asp:TextBox ID="txtGVW" runat="server" Width="150" Text=""></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                WheelType
                            </td>
                            <td>
                                <asp:TextBox ID="txtWheelType" runat="server" Width="150" Text=""></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Fuel
                            </td>
                            <td>
                                <asp:TextBox ID="txtFuel" runat="server" Width="150" Text=""></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                idCare
                            </td>
                            <td>
                                <asp:TextBox ID="txtIdCare" runat="server" Width="150" Text=""></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                CompanyName
                            </td>
                            <td>
                                <asp:TextBox ID="txtCompanyName" runat="server" Width="150" Text=""></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Power
                            </td>
                            <td>
                                <asp:TextBox ID="txtPower" runat="server" Width="150" Text=""></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                CabType
                            </td>
                            <td>
                                <asp:TextBox ID="txtCabType" runat="server" Width="150" Text=""></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Suspension
                            </td>
                            <td>
                                <asp:TextBox ID="txtSuspension" runat="server" Width="150" Text=""></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                CommentWebForm
                            </td>
                            <td>
                                <asp:TextBox ID="txtCommentWebForm" runat="server" Width="150" Text=""></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                idLeadSite
                            </td>
                            <td>
                                <asp:TextBox ID="txtidLeadSite" runat="server" Width="150" Text=""></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Title
                            </td>
                            <td>
                                <asp:TextBox ID="txtTitle" runat="server" Width="150" Text=""></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                CodePromotion
                            </td>
                            <td>
                                <asp:TextBox ID="txtCodePromotion" runat="server" Width="150" Text=""></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Model of Interest
                            </td>
                            <td>
                                <asp:TextBox ID="txtModelOfInterest" runat="server" Width="150" Text=""></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                DesideredData
                            </td>
                            <td>
                                <asp:TextBox ID="txtDesideredData" runat="server" Width="150" Text=""></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Canale
                            </td>
                            <td>
                                <asp:DropDownList ID="ddlCanale" runat="server" Width="150">
                                    <asp:ListItem Value="" Text="" Selected="True"></asp:ListItem>
                                    <asp:ListItem Value="20" Text="Other"></asp:ListItem>
                                    <asp:ListItem Value="6" Text="Web search engine"></asp:ListItem>
                                    <asp:ListItem Value="9" Text="Iveco Website"></asp:ListItem>
                                    <asp:ListItem Value="21" Text="Iveco used web site"></asp:ListItem>
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                StockSearchModel
                            </td>
                            <td>
                                <asp:TextBox ID="txtStockSearched" runat="server" Width="150" Text=""></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                DueDate
                            </td>
                            <td>
                                <asp:TextBox ID="txtDueDate" runat="server" Width="150" Text=""></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                TypeContact
                            </td>
                            <td>
                                <asp:DropDownList ID="ddlTypeContact" runat="server" Width="150">
                                    <asp:ListItem Value="" Text="" Selected="True"></asp:ListItem>
                                    <asp:ListItem Value="New" Text="New"></asp:ListItem>
                                    <asp:ListItem Value="Used" Text="Used"></asp:ListItem>
                                </asp:DropDownList>
                            </td>
                        </tr>
                    </table>
                    <br />
                    <br />
                    <table border="0" cellpadding="0" cellspacing="0" width="100%">
                        <tr>
                            <td align="center">
                                <asp:Button ID="Button1" runat="server" Text="ASAP" OnClick="Button1_Click" />
                            </td>
                            <td align="center">
                                <asp:Button ID="Button2" runat="server" Text="BOOKING" OnClick="Button2_Click" />
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
                <br />
                <br />
                <asp:Panel runat="server" ID="Panel1" GroupingText="Test - Proposta CRM prenotazione (assegnazione all'operatore)"
                    Style="border: solid 1px black;">
                    <table border="0" cellpadding="1" cellspacing="1" width="70%">
                        <tr>
                            <td>
                                Language
                            </td>
                            <td>
                                <asp:DropDownList ID="ddlLanguagePro" runat="server" Width="150">
                                    <asp:ListItem Value="" Text="" Selected="True"></asp:ListItem>
                                    <asp:ListItem Value="francese" Text="francese"></asp:ListItem>
                                    <asp:ListItem Value="italiano" Text="italiano"></asp:ListItem>
                                    <asp:ListItem Value="Spagnolo" Text="Spagnolo"></asp:ListItem>
                                    <asp:ListItem Value="Tedesco" Text="Tedesco"></asp:ListItem>
                                    <asp:ListItem Value="Polacco" Text="Polacco"></asp:ListItem>
                                    <asp:ListItem Value="Inglese" Text="Inglese"></asp:ListItem>
                                    <asp:ListItem Value="Portoghese" Text="Portoghese"></asp:ListItem>
                                    <asp:ListItem Value="Olandese" Text="Olandese"></asp:ListItem>
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Country
                            </td>
                            <td>
                                <asp:DropDownList ID="ddlCountryPro" runat="server" Width="150">
                                    <asp:ListItem Value="" Text="" Selected="True"></asp:ListItem>
                                    <asp:ListItem Value="Francia" Text="Francia"></asp:ListItem>
                                    <asp:ListItem Value="Italia" Text="Italia"></asp:ListItem>
                                    <asp:ListItem Value="Spagna" Text="Spagna"></asp:ListItem>
                                    <asp:ListItem Value="Germania" Text="Germania"></asp:ListItem>
                                    <asp:ListItem Value="Polonia " Text="Polonia "></asp:ListItem>
                                    <asp:ListItem Value="Austria" Text="Austria"></asp:ListItem>
                                    <asp:ListItem Value="UK" Text="UK"></asp:ListItem>
                                    <asp:ListItem Value="Marocco" Text="Marocco"></asp:ListItem>
                                    <asp:ListItem Value="South Africa" Text="South Africa"></asp:ListItem>
                                    <asp:ListItem Value="Algeria" Text="Algeria"></asp:ListItem>
                                    <asp:ListItem Value="Tunisia" Text="Tunisia"></asp:ListItem>
                                    <asp:ListItem Value="Svizzera" Text="Svizzera"></asp:ListItem>
                                    <asp:ListItem Value="Portogallo" Text="Portogallo"></asp:ListItem>
                                    <asp:ListItem Value="Netherland" Text="Netherland"></asp:ListItem>
                                    <asp:ListItem Value="Belgium" Text="Belgium"></asp:ListItem>
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Inizio Schedulazione dd/mm/yyyy / hh:mm:ss
                            </td>
                            <td>
                                <asp:TextBox ID="txtStartSchedDatePro" runat="server"></asp:TextBox>&nbsp;/&nbsp;<asp:TextBox
                                    ID="txtStartSchedTimePro" runat="server"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Fine Schedulazione dd/mm/yyyy / hh:mm:ss
                            </td>
                            <td>
                                <asp:TextBox ID="txtEndSchedDatePro" runat="server"></asp:TextBox>&nbsp;/&nbsp;<asp:TextBox
                                    ID="txtEndSchedTimePro" runat="server"></asp:TextBox>
                            </td>
                        </tr>
                    </table>
                    <br />
                    <table border="0" cellpadding="0" cellspacing="0" width="100%">
                        <tr>
                            <td align="center">
                                <asp:Button ID="btnPrenotAsap" runat="server" Text="ASAP" OnClick="btnPrenotAsap_Click" />
                            </td>
                            <td align="center">
                                <asp:Button ID="btnPrenotBooking" runat="server" Text="BOOKING" OnClick="btnPrenotBooking_Click" />
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
                <br />
                <asp:Panel runat="server" ID="Panel2" GroupingText="Test - Proposta CRM prenotazione (assegnazione all'operatore)"
                    Style="border: solid 1px black;">
                    <asp:DropDownList ID="ddlLanguageCalendar" runat="server" Width="150">
                        <asp:ListItem Value="" Text="" Selected="True"></asp:ListItem>
                        <asp:ListItem Value="francese" Text="francese"></asp:ListItem>
                        <asp:ListItem Value="italiano" Text="italiano"></asp:ListItem>
                        <asp:ListItem Value="Spagnolo" Text="Spagnolo"></asp:ListItem>
                        <asp:ListItem Value="Tedesco" Text="Tedesco"></asp:ListItem>
                        <asp:ListItem Value="Polacco" Text="Polacco"></asp:ListItem>
                        <asp:ListItem Value="Inglese" Text="Inglese"></asp:ListItem>
                        <asp:ListItem Value="Portoghese" Text="Portoghese"></asp:ListItem>
                        <asp:ListItem Value="Olandese" Text="Olandese"></asp:ListItem>
                    </asp:DropDownList>
                    <br />
                    <asp:DropDownList ID="ddlCountryCalendar" runat="server" Width="150">
                        <asp:ListItem Value="" Text="" Selected="True"></asp:ListItem>
                        <asp:ListItem Value="Francia" Text="Francia"></asp:ListItem>
                        <asp:ListItem Value="Italia" Text="Italia"></asp:ListItem>
                        <asp:ListItem Value="Spagna" Text="Spagna"></asp:ListItem>
                        <asp:ListItem Value="Germania" Text="Germania"></asp:ListItem>
                        <asp:ListItem Value="Polonia " Text="Polonia "></asp:ListItem>
                        <asp:ListItem Value="Austria" Text="Austria"></asp:ListItem>
                        <asp:ListItem Value="UK" Text="UK"></asp:ListItem>
                        <asp:ListItem Value="Marocco" Text="Marocco"></asp:ListItem>
                        <asp:ListItem Value="South Africa" Text="South Africa"></asp:ListItem>
                        <asp:ListItem Value="Algeria" Text="Algeria"></asp:ListItem>
                        <asp:ListItem Value="Tunisia" Text="Tunisia"></asp:ListItem>
                        <asp:ListItem Value="Svizzera" Text="Svizzera"></asp:ListItem>
                        <asp:ListItem Value="Portogallo" Text="Portogallo"></asp:ListItem>
                        <asp:ListItem Value="Netherland" Text="Netherland"></asp:ListItem>
                        <asp:ListItem Value="Belgium" Text="Belgium"></asp:ListItem>
                    </asp:DropDownList>
                    <br />
                    <br />
                    <div style="width: 100%; overflow: auto;">
                        <UserControl:Scheduler ID="MyScheduler" runat="server" />
                    </div>
                    <br />
                    <table border="0" cellpadding="0" cellspacing="0" width="100%">
                        <tr>
                            <td align="center">
                                <asp:Button ID="btnGetCalendar" runat="server" Text="GetCalendar" Height="24px" OnClick="btnGetCalendar_Click" />
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
                <br />
                <asp:Label runat="server" ID="lblErrore"></asp:Label>
            </div>
        </asp:View>
        <asp:View ID="Tab2" runat="server">
            <div>
                <table border="0" cellpadding="0" cellspacing="0" width="100%">
                    <tr>
                        <td>
                            Id Lead Crm
                        </td>
                        <td>
                            <asp:TextBox ID="txtIdLeadCrm" runat="server" Width="150" Text="LEAD-"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Dealer Company Name
                        </td>
                        <td>
                            <asp:TextBox ID="txtDealerCompanyName" runat="server" Width="150" Text=""></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Dealer Code
                        </td>
                        <td>
                            <asp:TextBox ID="txtDealerCode" runat="server" Width="150" Text=""></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Dealer Responsible
                        </td>
                        <td>
                            <asp:TextBox ID="txtDealerResponsible" runat="server" Width="150" Text=""></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Dealer Email
                        </td>
                        <td>
                            <asp:TextBox ID="txtDealerEmail" runat="server" Width="150" Text=""></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Marketing Account
                        </td>
                        <td>
                            <asp:TextBox ID="txtMarketingAccount" runat="server" Width="150" Text=""></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Email Marketing Account
                        </td>
                        <td>
                            <asp:TextBox ID="txtEmailMarketingAccount" runat="server" Width="150" Text=""></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Is DealerAgree
                        </td>
                        <td>
                            <asp:CheckBox ID="chkIsDealerAgree" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Is Critical Customer
                        </td>
                        <td>
                            <asp:CheckBox ID="chkIsCriticalCustomer" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Critical ReasonCode
                        </td>
                        <td>
                            <asp:TextBox ID="txtCriticalReasonCode" runat="server" Width="150" Text=""></asp:TextBox>
                        </td>
                    </tr>
                </table>
                <br />
                <table border="0" cellpadding="0" cellspacing="0" width="100%">
                    <tr>
                        <td>
                            <asp:Button ID="btnSetDealer" runat="server" Text="Set Dealer" OnClick="btnSetDealer_Click" />
                        </td>
                    </tr>
                </table>
                <br />
                <asp:Label runat="server" ID="lblSetDealer"></asp:Label>
            </div>
        </asp:View>
        <asp:View ID="Tab3" runat="server">
            <div>
                <table border="0" cellpadding="0" cellspacing="0" width="100%">
                    <tr>
                        <td>
                            Country
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlCountryDealer" runat="server" Width="150">
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Is Flag Critico
                        </td>
                        <td>
                            <asp:CheckBox ID="chkIsFlagCriticoDealer" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Lead Type
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlLeadType" runat="server" Width="150">
                                <asp:ListItem Value="" Text="" Selected="True"></asp:ListItem>
                                <asp:ListItem Value="1" Text="New"></asp:ListItem>
                                <asp:ListItem Value="2" Text="Used"></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Zip Code
                        </td>
                        <td>
                            <asp:TextBox ID="txtZipCode" runat="server" Width="150" Text=""></asp:TextBox>
                        </td>
                    </tr>
                </table>
                <br />
                <table border="0" cellpadding="0" cellspacing="0" width="100%">
                    <tr>
                        <td>
                            <asp:Button ID="btnGetDealer" runat="server" Text="Gets Dealer" OnClick="btnGetDealer_Click" />
                        </td>
                    </tr>
                </table>
                <br />
                <asp:Label runat="server" ID="lblResultGetDealer"></asp:Label>
            </div>
        </asp:View>
    </asp:MultiView>
    </form>
</body>
</html>
