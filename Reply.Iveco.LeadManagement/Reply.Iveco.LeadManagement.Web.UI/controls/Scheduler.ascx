<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Scheduler.ascx.cs" Inherits="Reply.Iveco.LeadManagement.Web.UI.Scheduler" %>
<asp:Repeater ID="rpdCalendar" runat="server" OnItemDataBound="rpdCalendar_ItemDataBound">
    <HeaderTemplate>
        <table border="0" cellpadding="0" cellspacing="0" style="table-layout:fixed;">
            <tr>
                <td rowspan="2" align="center" valign="middle" class="ms-crm-List-Sortable" style="width: 100px;background-color: #f0f0f0;">
                    &nbsp;
                </td>
                <asp:Repeater ID="rpdCalendarHeader" runat="server">
                    <ItemTemplate>
                        <td  class="ms-crm-List-Sortable" style="background-color: #f0f0f0;text-align:center;">
                            &nbsp;<%# ((DateTime)Container.DataItem).Day + "/" +((DateTime)Container.DataItem).Month%>&nbsp;
                        </td>
                    </ItemTemplate>
                </asp:Repeater>
            </tr>
            <tr>
                <td colspan="<%#CurrentColumnCount %>" class="HeaderTitle" align="center" style="width: 100px;">
                    &nbsp;
                </td>
            </tr>
    </HeaderTemplate>
    <ItemTemplate>
        <tr>
            <td style="width: 100px; padding-top: 20px; border: solid 1px #C0C0C0;
                background-color: #f0f0f0;">
                <table width="100px" cellpadding="0" cellspacing="0" border="0">
                <tr>
                <td style="text-align: center;" valign="middle"><span><b>
                    <asp:Label ID="lblRowSlotDescr" runat="server" Text=""></asp:Label>
                    </b></span></td>
                </tr>
                </table>
                
            </td>
            <asp:Repeater ID="rpdCalendarRow" OnItemDataBound="rpdCalendarRow_ItemDataBound"
                runat="server">
                <ItemTemplate>
                    <td id="tbCell" runat="server" class="RowContent" style="width: 50px;table-layout:fixed;">
                        <table border="0" cellpadding="0" cellspacing="0" width="100%">
                            <tr>
                                <td style="text-align:center;padding:2px;" colspan="2">
                                    <span><b><%# ((Reply.Iveco.LeadManagement.Presenter.DataSchedulerRowCell)Container.DataItem).AvailableSlot.ToString()%></b>
                                    </span>
                                </td>
                            </tr>
                            <tr>
                                <td style="width:30%;font-size:8px;">
                                    &nbsp;&nbsp;ASAP
                                </td>
                                <td style="text-align:center;font-size:8px;">
                                    <asp:Label ID="lblOccupationAsap" runat="server" Text=""></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td style="font-size:8px;">
                                    &nbsp;&nbsp;CSI
                                </td>
                                <td style="text-align:center;font-size:8px;">
                                    <asp:Label ID="lblOccupationCSI" runat="server" Text=""></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td style="font-size:8px;">
                                    &nbsp;&nbsp;BOOK
                                </td>
                                <td style="text-align:center;font-size:8px;">
                                    <asp:Label ID="lblOccupationBook" runat="server" Text=""></asp:Label>
                                </td>
                            </tr>
                        </table>
                    </td>
                </ItemTemplate>
            </asp:Repeater>
        </tr>
    </ItemTemplate>
    <FooterTemplate>
        </table>
    </FooterTemplate>
</asp:Repeater>
