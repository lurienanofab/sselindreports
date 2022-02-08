<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ToolCharges.aspx.cs" Inherits="sselIndReports.ToolCharges" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Tool Detail</title>

    <link rel="stylesheet" href="//ssel-apps.eecs.umich.edu/static/lib/bootstrap4/css/bootstrap.min.css" />

    <style>
        .lnf-table > .thead-dark > tr > th {
            background-color: #dcdcdc;
            font-weight: bold;
            color: #000000;
            border-color: #dcdcdc;
        }

        .lnf-table > thead > tr > th {
            text-align: center;
            border: solid 1px;
        }

        .lnf-table > tbody > tr > td {
            border-top: none;
            border-right: solid 1px #dcdcdc;
            border-bottom: none;
            border-left: solid 1px #dcdcdc;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container-fluid mt-4">
            <div class="row">
                <div class="col-9 mx-auto">
                    <asp:Repeater runat="server" ID="rptToolCharges">
                        <HeaderTemplate>
                            <table class="lnf-table table table-striped">
                                <thead class="thead-dark">
                                    <tr>
                                        <th scope="col" style="width: 220px;">Tool</th>
                                        <th scope="col" style="width: 120px;">Lab</th>
                                        <%--<th style="width: 80px;" title="Actual start time - actual end time">Activated Used (hours)</th>
                                <th style="width: 80px;" title="Time for unused, but activated reservation">Activated Unused (hours)</th>
                                <th style="width: 80px;" title="Overtime - used time after original end time">Overtime (hours)</th>
                                <th style="width: 80px;" title="Reserved reservation that has not been used">Overtime Fee</th>
                                <th style="width: 80px;" title="10% of all cancellations before 2 hours of starting time">Unactivated (hours)</th>
                                <th style="width: 80px;" title="Time used by subsequent users">Booking Fee</th>
                                <th style="width: 60px;">Transferred (hours)</th>
                                <th style="width: 80px;">Forgiven (hours)</th>--%>
                                        <th scope="col" style="width: 90px;">Per Use Rate</th>
                                        <th scope="col" style="width: 90px;">Hourly Rate</th>
                                        <th scope="col" style="min-width: 250px;">Account</th>
                                        <th scope="col" style="width: 80px;">Short<br />Code</th>
                                        <th scope="col" style="width: 90px;">Duration</th>
                                        <th scope="col" style="width: 90px;">Booking<br />Fee</th>
                                        <th scope="col" style="width: 90px;">Per Use<br />Charge</th>
                                        <th scope="col" style="width: 90px;">Hourly<br />Charge</th>
                                        <th scope="col" style="width: 90px;">Line<br />Total</th>
                                    </tr>
                                </thead>
                                <tbody>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <tr>
                                <td style="text-align: center;"><%#Eval("ResourceName")%></td>
                                <td style="text-align: center;"><%#Eval("RoomName")%></td>
                                <%--<td style="text-align: center;"><%#Eval("ActivatedUsed", "{0:F2}")%></td>
                        <td style="text-align: center;"><%#Eval("ActivatedUnused", "{0:F2}")%></td>
                        <td style="text-align: center;"><%#Eval("TotalOverTime", "{0:F2}")%></td>
                        <td style="text-align: right;"><%#Eval("OverTimePenaltyFee", "{0:C}")%></td>
                        <td style="text-align: center;"><%#Eval("UnstartedUnused", "{0:F2}")%></td>
                        <td style="text-align: center;"><%#Eval("TotalTransferredDuration", "{0:F2}")%></td>
                        <td style="text-align: center;"><%#Eval("TotalForgivenDuration", "{0:F2}")%></td>--%>
                                <td style="text-align: right;"><%#Eval("PerUseRate", "{0:C}")%></td>
                                <td style="text-align: right;"><%#Eval("HourlyRate", "{0:C}")%></td>
                                <td style="text-align: center;"><%#Eval("AccountName")%></td>
                                <td style="text-align: center;"><%#Eval("ShortCode")%></td>
                                <td style="text-align: center;"><%#Eval("TotalDuration", "{0:0.0000}")%></td>
                                <td style="text-align: right;"><%#Eval("BookingFee", "{0:C}")%></td>
                                <td style="text-align: right;"><%#Eval("PerUseCharge", "{0:C}")%></td>
                                <td style="text-align: right;"><%#Eval("HourlyCharge", "{0:C}")%></td>
                                <td style="text-align: right;"><%#Eval("LineTotal", "{0:C}")%></td>
                            </tr>
                        </ItemTemplate>
                        <FooterTemplate>
                            </tbody>
                    </table>
                        </FooterTemplate>
                    </asp:Repeater>
                </div>
            </div>
        </div>
    </form>

    <script src="//ssel-apps.eecs.umich.edu/static/lib/jquery/jquery.min.js"></script>
    <script src="//ssel-apps.eecs.umich.edu/static/lib/popper/2.9.0/umd/popper.min.js"></script>
    <script src="//ssel-apps.eecs.umich.edu/static/lib/bootstrap4/js/bootstrap.min.js"></script>
</body>
</html>
