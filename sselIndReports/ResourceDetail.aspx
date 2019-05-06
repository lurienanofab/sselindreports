<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ResourceDetail.aspx.cs" Inherits="sselIndReports.ResourceDetail" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <div style="color: #cc6600; font-family: Verdana; font-size: 8pt; padding: 5px; font-weight: bold; margin-bottom: 10px;">
                <span>Resource Detail: </span>
                <asp:Literal runat="server" ID="litHeaderResource"></asp:Literal>,
                <span>Account: </span>
                <asp:Literal runat="server" ID="litHeaderAccount"></asp:Literal>
            </div>

            <asp:Repeater runat="server" ID="rptResourceDetail" OnItemDataBound="RptResourceDetail_ItemDataBound">
                <HeaderTemplate>
                    <table class="gridview striped" style="border-collapse: collapse;">
                        <thead>
                            <tr class="header">
                                <th style="width: 80px;">ReservationID</th>
                                <th>Date</th>
                                <th style="width: 80px;">Started</th>
                                <th style="width: 80px;">Cancelled</th>
                                <th style="width: 80px;">Cancelled Before Cutoff</th>
                                <th style="width: 80px;">Activated Used (hours)</th>
                                <th style="width: 80px;">Activated Unused (hours)</th>
                                <th style="width: 80px;">Overtime (hours)</th>
                                <th style="width: 80px;">Overtime Fee</th>
                                <th style="width: 80px;">Unactivated (hours)</th>
                                <th style="width: 80px;">Booking Fee</th>
                                <th style="width: 80px;">Transferred (hours)</th>
                                <th style="width: 80px;">Forgiven (hours)</th>
                                <th style="width: 80px;">Rate</th>
                                <th style="width: 80px;">Line Total</th>
                            </tr>
                        </thead>
                        <tbody>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr>
                        <td><%#Eval("ReservationID")%></td>
                        <td><%#Eval("ActDate", "{0:MM/dd/yyyy}")%></td>
                        <td style="text-align: center;"><%#Eval("Started")%></td>
                        <td style="text-align: center;"><%#Eval("Cancelled")%></td>
                        <td style="text-align: center;"><%#Eval("CancelledBeforeCutoff")%></td>
                        <td style="text-align: right;"><%#Eval("ActivatedUsed", "{0:0.00}")%></td>
                        <td style="text-align: right;"><%#Eval("ActivatedUnused", "{0:0.00}")%></td>
                        <td style="text-align: right;"><%#Eval("Overtime", "{0:0.00}")%></td>
                        <td style="text-align: right;"><%#Eval("OvertimeFee", "{0:C}")%></td>
                        <td style="text-align: right;"><%#Eval("UnstartedUnused", "{0:0.00}")%></td>
                        <td style="text-align: right;"><%#Eval("BookingFee", "{0:C}")%></td>
                        <td style="text-align: right;"><%#Eval("Transferred", "{0:0.00}")%></td>
                        <td style="text-align: right;"><%#Eval("Forgiven", "{0:0.00}")%></td>
                        <td style="text-align: right;"><%#Eval("ResourceRate", "{0:C}")%></td>
                        <td style="text-align: right;"><%#Eval("LineTotal", "{0:C}")%></td>
                    </tr>
                </ItemTemplate>
                <FooterTemplate>
                    </tbody>
                    <tfoot>
                        <tr class="footer" style="font-weight: bold;">
                            <td colspan="5" style="text-align: right;">Total:
                            </td>
                            <td style="text-align: right;">
                                <asp:Literal runat="server" ID="litTotalActivatedUsed"></asp:Literal>
                            </td>
                            <td style="text-align: right;">
                                <asp:Literal runat="server" ID="litTotalActivatedUnused"></asp:Literal>
                            </td>
                            <td style="text-align: right;">
                                <asp:Literal runat="server" ID="litTotalOvertime"></asp:Literal>
                            </td>
                            <td style="text-align: right;">
                                <asp:Literal runat="server" ID="litTotalOvertimeFee"></asp:Literal>
                            </td>
                            <td style="text-align: right;">
                                <asp:Literal runat="server" ID="litTotalUnstartedUnused"></asp:Literal>
                            </td>
                            <td style="text-align: right;">
                                <asp:Literal runat="server" ID="litTotalBookingFee"></asp:Literal>
                            </td>
                            <td style="text-align: right;">
                                <asp:Literal runat="server" ID="litTotalTransferred"></asp:Literal>
                            </td>
                            <td style="text-align: right;">
                                <asp:Literal runat="server" ID="litTotalForgiven"></asp:Literal>
                            </td>
                            <td style="text-align: right;">&nbsp;
                            </td>
                            <td style="text-align: right;">
                                <asp:Literal runat="server" ID="litTotalLineTotal"></asp:Literal>
                            </td>
                        </tr>
                    </tfoot>
                    </table>
                </FooterTemplate>
            </asp:Repeater>
            <asp:Literal runat="server" ID="litDebug"></asp:Literal>
        </div>
    </form>
</body>
</html>
