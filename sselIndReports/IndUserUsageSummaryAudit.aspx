<%@ Page Title="User Usage Summary Audit" Language="C#" MasterPageFile="~/IndReportsMaster.Master" AutoEventWireup="true" CodeBehind="IndUserUsageSummaryAudit.aspx.cs" Inherits="sselIndReports.IndUserUsageSummaryAudit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        body {
            font-family: Calibri, Arial, sans-serif;
            font-size: 11pt;
        }

        h2 {
            font-family: 'Arial Black';
            font-weight: bold;
            font-size: large;
            font-variant: small-caps;
            color: #0033FF;
        }

        .report-container {
            border: double 3px #aaa;
            padding: 1px;
            margin-bottom: 10px;
        }

        .report-table {
            border-collapse: collapse;
            width: 100%;
        }

            .report-table td {
                padding: 4px;
                border: solid 1px #ddd;
            }

            .report-table th {
                padding: 4px;
                border: solid 1px #ddd;
                background-color: #ddd;
                font-weight: bold;
                vertical-align: bottom;
            }

            .report-table a:link,
            .report-table a:active,
            .report-table a:visited {
                color: #000;
                text-decoration: none;
            }

            .report-table a:hover {
                color: #0000ee;
                text-decoration: underline;
            }

            .report-table.striped > tbody > tr:nth-child(even) > td {
                background-color: #eee;
            }

            .report-table > tbody > tr.not-equal > td {
                background-color: #f39696 !important;
            }

        .text-left {
            text-align: left !important;
        }

        .text-right {
            text-align: right !important;
        }

        .text-center {
            text-align: center !important;
        }

        .formula {
            font-family: monospace;
        }

        .help {
            display: none;
            padding: 10px;
            border: solid 1px #ccc;
        }

        .show-help {
            margin-bottom: 10px;
        }

        .subtitle {
            margin: 10px 0 10px 0;
        }

        .subtitle-text {
            font-size: 10pt;
            font-family: 'Palatino Linotype';
            font-weight: bold;
            background-color: #ffcc99;
            color: #000;
            padding: 2px;
        }

        .user-usage-summary div.section {
            margin-bottom: 20px;
        }

        .report-info th {
            width: 70px;
            background-color: #dadada;
            border: solid 1px #bababa;
        }

        .report-info td {
            border: solid 1px #bababa;
        }

        .nodata {
            padding: 10px;
            color: #808080;
            font-style: italic;
        }

        .back-link {
            margin-left: 10px;
        }

        .alert {
            color: #ff0000;
            display: inline-block;
            padding: 5px;
            font-weight: bold;
        }

        .hidden {
            display: none;
        }

        /* these styles control the table column display */
        .tool-billing.detail [data-property='ReservationID'],
        .tool-billing.detail [data-property='IsCancelledBeforeAllowedTime'],
        .tool-billing.detail [data-property='ForgivenPercentage'],
        .tool-billing.aggregate [data-property='ResourceDisplayName'],
        .tool-billing.aggregate [data-property='LabDisplayName'] {
            display: table-cell;
        }

        .tool-billing.detail [data-property='ResourceDisplayName'],
        .tool-billing.detail [data-property='LabDisplayName'],
        .tool-billing.detail [data-property='UsageFeeCharged'],
        .tool-billing.detail [data-property='OverTimePenaltyFee'],
        .tool-billing.aggregate [data-property='ReservationID'],
        .tool-billing.aggregate [data-property='IsCancelledBeforeAllowedTime'],
        .tool-billing.aggregate [data-property='ForgivenPercentage'],
        .tool-billing.aggregate [data-property='BaseDurationHoursForgiven'],
        .tool-billing.aggregate [data-property='OverTimePenaltyHoursForgiven'],
        .tool-billing.aggregate [data-property='UsageFeeCharged'],
        .tool-billing.aggregate [data-property='OverTimePenaltyFee'] {
            display: none;
        }

        .tool-billing [data-property='ReservationID'] {
            text-align: center;
            width: 110px;
        }

        .tool-billing [data-property='ResourceDisplayName'] {
            text-align: left;
            width: 400px;
        }

        .tool-billing [data-property='LabDisplayName'] {
            text-align: center;
            width: 100px;
        }

        .tool-billing [data-property='AccountName'] {
            text-align: left;
        }

        .tool-billing [data-property='ShortCode'] {
            text-align: center;
            width: 80px;
        }

        .tool-billing [data-property='PerUseRate'] {
            text-align: right;
            width: 60px;
        }

        .tool-billing [data-property='Uses'] {
            text-align: center;
            width: 60px;
        }

        .tool-billing [data-property='PerUseCharge'] {
            text-align: right;
            width: 60px;
        }

        .tool-billing [data-property='HourlyRate'] {
            text-align: right;
            width: 60px;
        }

        .tool-billing [data-property='IsCancelledBeforeAllowedTime'] {
            text-align: center;
            width: 100px;
        }

        .tool-billing [data-property='ForgivenPercentage'] {
            text-align: center;
            width: 80px;
        }

        .tool-billing [data-property='BaseDurationHoursForgiven'] {
            text-align: center;
            width: 100px;
        }

        .tool-billing [data-property='OverTimePenaltyHoursForgiven'] {
            text-align: center;
            width: 100px;
        }

        .tool-billing [data-property='BillingDurationHoursForgiven'] {
            text-align: center;
            width: 60px;
        }

        .tool-billing [data-property='BillingCharge'] {
            text-align: right;
            width: 60px;
        }

        .tool-billing [data-property='UsageFeeCharged'] {
            text-align: right;
            width: 80px;
        }

        .tool-billing [data-property='OverTimePenaltyFee'] {
            text-align: right;
            width: 90px;
        }

        .tool-billing [data-property='BookingFee'] {
            text-align: right;
            width: 60px;
        }

        .tool-billing [data-property='LineCost'] {
            text-align: right;
            width: 70px;
        }

        .tool-billing [data-property='ShortCode'],
        .tool-billing [data-property='BookingFee'],
        .tool-billing [data-property='Uses'],
        .tool-billing [data-property='PerUseCharge'],
        .tool-billing [data-property='BillingDurationHoursForgiven'],
        .tool-billing [data-property='BillingCharge'] {
            border-right: solid 2px #808080;
        }

        /* Per Use Group */
        .tool-billing.colorized .report-table > thead > tr > th[data-property='PerUseRate'],
        .tool-billing.colorized .report-table > thead > tr > th[data-property='Uses'],
        .tool-billing.colorized .report-table > thead > tr > th[data-property='PerUseCharge'] {
            background-color: #e6de4c !important;
        }

        .tool-billing.colorized .report-table > tbody > tr:nth-child(odd) > td[data-property='PerUseRate'],
        .tool-billing.colorized .report-table > tbody > tr:nth-child(odd) > td[data-property='Uses'],
        .tool-billing.colorized .report-table > tbody > tr:nth-child(odd) > td[data-property='PerUseCharge'] {
            background-color: #f5f2b8 !important;
        }

        .tool-billing.colorized .report-table > tbody > tr:nth-child(even) > td[data-property='PerUseRate'],
        .tool-billing.colorized .report-table > tbody > tr:nth-child(even) > td[data-property='Uses'],
        .tool-billing.colorized .report-table > tbody > tr:nth-child(even) > td[data-property='PerUseCharge'] {
            background-color: #ede882 !important;
        }

        /* Hourly Group */
        .tool-billing.colorized .report-table > thead > tr > th[data-property='HourlyRate'],
        .tool-billing.colorized .report-table > thead > tr > th[data-property='BillingDurationHoursForgiven'],
        .tool-billing.colorized .report-table > thead > tr > th[data-property='BillingCharge'] {
            background-color: #92bd4c !important;
        }

        .tool-billing.colorized .report-table > tbody > tr:nth-child(odd) > td[data-property='HourlyRate'],
        .tool-billing.colorized .report-table > tbody > tr:nth-child(odd) > td[data-property='BillingDurationHoursForgiven'],
        .tool-billing.colorized .report-table > tbody > tr:nth-child(odd) > td[data-property='BillingCharge'] {
            background-color: #c5dca0 !important;
        }

        .tool-billing.colorized .report-table > tbody > tr:nth-child(even) > td[data-property='HourlyRate'],
        .tool-billing.colorized .report-table > tbody > tr:nth-child(even) > td[data-property='BillingDurationHoursForgiven'],
        .tool-billing.colorized .report-table > tbody > tr:nth-child(even) > td[data-property='BillingCharge'] {
            background-color: #adcd79 !important;
        }

        /* Booking Fee */
        .tool-billing.colorized .report-table > thead > tr > th[data-property='BookingFee'] {
            background-color: #959bb1 !important;
        }

        .tool-billing.colorized .report-table > tbody > tr:nth-child(odd) > td[data-property='BookingFee'] {
            background-color: #dcdee5 !important;
        }

        .tool-billing.colorized .report-table > tbody > tr:nth-child(even) > td[data-property='BookingFee'] {
            background-color: #b9bdcb !important;
        }

        /* Line Cost */
        .tool-billing.colorized .report-table > thead > tr > th[data-property='LineCost'] {
            background-color: #ea805d !important;
        }

        .tool-billing.colorized .report-table > tbody > tr:nth-child(odd) > td[data-property='LineCost'] {
            background-color: #f8d5c9 !important;
        }

        .tool-billing.colorized .report-table > tbody > tr:nth-child(even) > td[data-property='LineCost'] {
            background-color: #f1aa93 !important;
        }

        /*------*/

        .tool-billing .report-table > thead > tr > th[data-property='BookingFee'],
        .tool-billing .report-table > thead > tr > th[data-property='PerUseCharge'],
        .tool-billing .report-table > thead > tr > th[data-property='BillingCharge'] {
            background-color: #ccc !important;
        }

        .tool-billing .report-table > tbody > tr:nth-child(odd) > td[data-property='BookingFee'],
        .tool-billing .report-table > tbody > tr:nth-child(odd) > td[data-property='PerUseCharge'],
        .tool-billing .report-table > tbody > tr:nth-child(odd) > td[data-property='BillingCharge'] {
            background-color: #eee !important;
        }

        .tool-billing .report-table > tbody > tr:nth-child(even) > td[data-property='BookingFee'],
        .tool-billing .report-table > tbody > tr:nth-child(even) > td[data-property='PerUseCharge'],
        .tool-billing .report-table > tbody > tr:nth-child(even) > td[data-property='BillingCharge'] {
            background-color: #ddd !important;
        }

        /*-----*/

        .tool-billing .report-table > thead > tr > th[data-property='LineCost'] {
            background-color: #bbb !important;
            background-color: #e6de4c !important;
            background-color: #b9bfcb !important;
        }

        .tool-billing .report-table > tbody > tr:nth-child(odd) > td[data-property='LineCost'] {
            background-color: #ddd !important;
            background-color: #f5f2b8 !important;
            background-color: #f3f4f6 !important;
        }

        .tool-billing .report-table > tbody > tr:nth-child(even) > td[data-property='LineCost'] {
            background-color: #ccc !important;
            background-color: #ede882 !important;
            background-color: #dcdfe5 !important;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="user-usage-summary">
        <div class="section">
            <h3>User Usage Summary Audit Report</h3>

            <div style="border-top: solid 1px #aaa; border-bottom: solid 1px #aaa; padding: 10px; margin-bottom: 20px;">
                This page was created while developing the new "audit friendly" version of the User Usage Summary report. Table data displayed is the same as what is shown on the latest version of the User Usage Summary, Tool Detail section. You can click a resource name to see reservation details.
            </div>

            <div class="criteria">
                <table class="criteria-table">
                    <tr>
                        <td>Select period:</td>
                        <td>
                            <lnf:PeriodPicker runat="server" ID="pp1" AutoPostBack="true" OnSelectedPeriodChanged="Pp1_SelectedPeriodChanged" />
                            <div style="display: inline-block; margin-left: 10px;">
                                <asp:LinkButton runat="server" ID="btnPrev" Text="prev" OnCommand="DateSelect_Command" CommandName="select" CommandArgument="prev"></asp:LinkButton>
                                |
                                <asp:LinkButton runat="server" ID="btnNext" Text="next" OnCommand="DateSelect_Command" CommandName="select" CommandArgument="next"></asp:LinkButton>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td>Select user:</td>
                        <td>
                            <asp:DropDownList runat="server" ID="ddlUser" DataTextField="DisplayName" DataValueField="ClientID" CssClass="report-select"></asp:DropDownList>
                            <asp:Label ID="lblClientID" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <div class="report-controls">
                                <asp:Button ID="btnReport" Text="Retrieve Data" runat="server" CssClass="report-button" OnClick="ReportButton_Click" />
                                <asp:LinkButton runat="server" ID="LinkButton1" Text="&larr; Back to Main Page" OnClick="BackButton_Click" CssClass="back-link"></asp:LinkButton>
                            </div>
                            <div class="spinner" style="display: none;">&nbsp;</div>
                            <div>
                                <asp:Label ID="lblGlobalMsg" runat="server"></asp:Label>
                            </div>
                        </td>
                    </tr>
                </table>
            </div>
        </div>

        <asp:PlaceHolder runat="server" ID="phHelp" Visible="false">
            <div class="section">
                <div class="show-help">
                    <a href="#">show help</a>
                </div>

                <div class="help">
                    <div style="margin-bottom: 10px;">
                        <strong style="font-size: larger;">Definitions</strong>
                        <ul style="margin-top: 0;">
                            <li>
                                <strong>Cancelled:</strong> True if the reservation was cancelled before the two hour cutoff. False if the reservation was not cancelled OR was cancelled after the cutoff.
                            </li>
                            <li>
                                <strong>ChargeMultiplier:</strong> The percentage to be charged (0 = 100% forgiven, 1 = 0% forgiven). Applies to Booking Fee, Base Charge, and Overtime Fee.
                            </li>
                            <li>
                                <strong>BaseDuration:</strong> The normal reserved hours charged at the hourly rate. Excludes overtime, transferred duration. When cancelled before cutoff scheduled duration is excluded.
                            </li>
                            <li>
                                <strong>Overtime:</strong> The hours used past the scheduled end time.
                            </li>
                            <li>
                                <strong>BaseCharge:</strong> When cancelled before the two hour cutoff: zero (booking fee may apply). Otherwize the base duration (does not include overtime) multipled by the  hourly rate and the forgiven %.
                            </li>
                            <li>
                                <strong>OvertimeFee:</strong> The charge for any time used after the scheduled end time. This time is charged at the hourly rate plus a 25% penalty.
                            </li>
                            <li>
                                <strong>BookingFee:</strong> When cancelled before the two hour cutoff: 10% of the charge for reserved time multipled by the forgiven %. Otherwize zero because all reserved time will be charged. Since May 2020 this has been waived due to COVID-19.
                            </li>
                            <li>
                                <strong>LineCost:</strong> Total amount billed: Booking Fee + Base Charge + Overtime Fee (ChargeMultiplier is already applied to each).
                            </li>
                        </ul>
                    </div>

                    <div>
                        <strong style="font-size: larger;">Formulas</strong>
                        <ul style="margin-top: 0;">
                            <li>
                                <div class="formula">
                                    ChargeMultiplier = 1 - Forgiven%
                                </div>
                            </li>
                            <li>
                                <strong>Not cancelled or cancelled after two hour cutoff:</strong>
                                <div class="formula" style="padding-left: 10px;">
                                    <div>BaseCharge = ChargeMultiplier * BaseDuration * HourlyRate</div>
                                    <div>OvertimeFee = ChargeMultiplier * Overtime * HourlyRate * 1.25</div>
                                    <div>BookingFee = 0</div>
                                    <div>LineCost = PerUse + BaseCharge + OvertimeFee</div>
                                </div>
                            </li>
                            <li>
                                <strong>Cancelled before two hour cutoff:</strong>
                                <div class="formula" style="padding-left: 10px;">
                                    <div>BaseCharge = 0</div>
                                    <div>OvertimeFee = 0</div>
                                    <div>BookingFee = ChargeMultiplier * BaseDuration * HourlyRate * 10%</div>
                                    <div>LineCost = BookingFee</div>
                                </div>
                            </li>
                        </ul>
                    </div>
                </div>
            </div>
        </asp:PlaceHolder>

        <asp:PlaceHolder runat="server" ID="phReport" Visible="false">
            <div class="section">
                <div class="report-container">
                    <asp:Repeater runat="server" ID="rptReportInfo">
                        <HeaderTemplate>
                            <table class="report-table report-info">
                                <tbody>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <tr>
                                <th><%#Eval("Key")%></th>
                                <td><%#Eval("Value")%></td>
                            </tr>
                        </ItemTemplate>
                        <FooterTemplate>
                            </tbody>
                            </table>
                        </FooterTemplate>
                    </asp:Repeater>
                </div>

                <div class="report-container">

                    <asp:Label ID="lblSummaryApproximate" runat="server" Text="The numbers are approximate before the third business day of this month." Visible="false" CssClass="alert"></asp:Label>

                    <asp:PlaceHolder runat="server" ID="phTool" Visible="false">
                        <div class="subtitle">
                            <div class="subtitle-text">
                                <asp:Literal runat="server" ID="litSubtitle"></asp:Literal>
                                <asp:PlaceHolder runat="server" ID="phToolBackLink" Visible="false">
                                    <span>[<asp:HyperLink runat="server" ID="hypToolBackLink">back</asp:HyperLink>]</span>
                                </asp:PlaceHolder>
                            </div>
                        </div>

                        <div runat="server" id="divToolBilling" class="tool-billing">
                            <asp:Repeater runat="server" ID="rptToolBilling">
                                <HeaderTemplate>
                                    <table class="report-table striped">
                                        <thead>
                                            <tr>
                                                <th data-property="ReservationID">ReservationID</th>
                                                <th data-property="ResourceDisplayName">Resource</th>
                                                <th data-property="LabDisplayName">Lab</th>
                                                <th data-property="AccountName">Account</th>
                                                <th data-property="ShortCode">Short Code</th>
                                                <th data-property="BookingFee">Booking Fee</th>
                                                <th data-property="PerUseRate">Per Use Rate</th>
                                                <th data-property="Uses">Uses</th>
                                                <th data-property="PerUseCharge">Per Use Charge</th>
                                                <th data-property="HourlyRate">Usage Rate</th>
                                                <th data-property="IsCancelledBeforeAllowedTime">Cancelled Before Cutoff</th>
                                                <th data-property="ForgivenPercentage">Forgiven %</th>
                                                <th data-property="BaseDurationHoursForgiven">Base Hours</th>
                                                <th data-property="OverTimePenaltyHoursForgiven">Overtime Penalty Hours</th>
                                                <th data-property="BillingDurationHoursForgiven">Usage Hours</th>
                                                <th data-property="BillingCharge">Usage Charge</th>
                                                <th data-property="UsageFeeCharged">Base Charge</th>
                                                <th data-property="OverTimePenaltyFee">Overtime Fee</th>
                                                <th data-property="LineCost">Line Cost</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <tr>
                                        <td data-property="ReservationID" data-value='<%#Eval("ReservationID")%>'><%#Eval("ReservationID")%></td>
                                        <td data-property="ResourceDisplayName" data-value='<%#Eval("ResourceDisplayName")%>'><%#GetResourceLink(Container.DataItem)%></td>
                                        <td data-property="LabDisplayName" data-value='<%#Eval("LabDisplayName")%>'><%#Eval("LabDisplayName")%></td>
                                        <td data-property="AccountName" data-value='<%#Eval("AccountName")%>'><%#Eval("AccountName")%></td>
                                        <td data-property="ShortCode" data-value='<%#Eval("ShortCode")%>'><%#Eval("ShortCode")%></td>
                                        <td data-property="BookingFee" data-value='<%#Eval("BookingFee")%>'><%#Eval("BookingFee", "{0:C}")%></td>
                                        <td data-property="PerUseRate" data-value='<%#Eval("PerUseRate")%>'><%#Eval("PerUseRate", "{0:C}")%></td>
                                        <td data-property="Uses" data-value='<%#Eval("Uses")%>'><%#Eval("Uses", "{0:0.0}")%></td>
                                        <td data-property="PerUseCharge" data-value='<%#Eval("PerUseCharge")%>'><%#Eval("PerUseCharge", "{0:C}")%></td>
                                        <td data-property="HourlyRate" data-value='<%#Eval("HourlyRate")%>'><%#Eval("HourlyRate", "{0:C}")%></td>
                                        <td data-property="IsCancelledBeforeAllowedTime" data-value='<%#Eval("IsCancelledBeforeAllowedTime").ToString().ToLower()%>'><%#Eval("IsCancelledBeforeAllowedTime")%></td>
                                        <td data-property="ForgivenPercentage" data-value='<%#Eval("ForgivenPercentage")%>'><%#Eval("ForgivenPercentage", "{0:0.0%}")%></td>
                                        <td data-property="BaseDurationHoursForgiven" data-value='<%#Eval("BaseDurationHoursForgiven")%>'><%#Eval("BaseDurationHoursForgiven", "{0:0.0000}")%></td>
                                        <td data-property="OverTimePenaltyHoursForgiven" data-value='<%#Eval("OverTimePenaltyHoursForgiven")%>'><%#Eval("OverTimePenaltyHoursForgiven", "{0:0.0000}")%></td>
                                        <td data-property="BillingDurationHoursForgiven" data-value='<%#Eval("BillingDurationHoursForgiven")%>'><%#Eval("BillingDurationHoursForgiven", "{0:0.0000}")%></td>
                                        <td data-property="BillingCharge" data-value='<%#Eval("BillingCharge")%>'><%#Eval("BillingCharge", "{0:C}")%></td>
                                        <td data-property="UsageFeeCharged" data-value='<%#Eval("UsageFeeCharged")%>'><%#Eval("UsageFeeCharged", "{0:C}")%></td>
                                        <td data-property="OverTimePenaltyFee" data-value='<%#Eval("OverTimePenaltyFee")%>'><%#Eval("OverTimePenaltyFee", "{0:C}")%></td>
                                        <td data-property="LineCost" data-value='<%#Eval("LineCost")%>'><%#Eval("LineCost", "{0:C}")%></td>
                                    </tr>
                                </ItemTemplate>
                                <FooterTemplate>
                                    </tbody>
                                </table>
                                </FooterTemplate>
                            </asp:Repeater>
                        </div>
                    </asp:PlaceHolder>
                    <asp:PlaceHolder runat="server" ID="phToolNoData" Visible="true">
                        <div class="nodata">
                            There are no tool charges for this period.
                        </div>
                    </asp:PlaceHolder>
                </div>
            </div>
        </asp:PlaceHolder>

    </div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="scripts" runat="server">
    <script>
        $(".show-help").on("click", "a", function (e) {
            e.preventDefault();
            var help = $(".help");
            if (help.is(":visible")) {
                help.hide();
                $(this).html("show help");
            } else {
                help.show();
                $(this).html("hide help");
            }
        });

        const OVERTIME_PENALTY = 1.25;

        function getLineCost(item) {
            var result = (item.PerUseRate * item.Uses)
                + (item.BaseDurationHoursForgiven * item.HourlyRate)
                + (item.OverTimeHoursForgiven * item.HourlyRate * OVERTIME_PENALTY);

            result *= item.IsCancelledBeforeAllowedTime ? 0 : 1;

            result += item.BookingFee;

            return result;
        }

        function auditToolBilling() {
            var items = $.makeArray($(".tool-billing > tbody > tr").map(function () {
                var row = $(this);
                var result = {};
                $("[data-property]", row).each(function () {
                    var cell = $(this);

                    var val = cell.data("value");
                    var num = parseFloat(val);

                    if (val == "true")
                        result[cell.data('property')] = true;
                    else if (val == "false")
                        result[cell.data('property')] = false;
                    else if (isNaN(num))
                        result[cell.data('property')] = val;
                    else
                        result[cell.data('property')] = num;
                });

                result.row = row;

                return result;
            }));

            var formatter = new Intl.NumberFormat('en-US', {
                style: 'currency',
                currency: 'USD'
            });

            var audit = items.map(function (item) {
                var lc1 = getLineCost(item);
                var flc1 = formatter.format(lc1);

                var lc2 = item.LineCost;
                var flc2 = formatter.format(lc2);

                return {
                    row: item.row,
                    reservationId: item.ReservationID,
                    resource: item.ResourceDisplayName,
                    account: item.AccountName,
                    lineCost1: lc1,
                    formattedLineCost1: flc1,
                    lineCost2: lc2,
                    formattedLineCost2: flc2,
                    areEqual: flc1 == flc2
                };
            });

            var problems = audit.filter(x => !x.areEqual);

            if (problems.length > 0)
                console.log('problems found: ' + problems.length);
            else
                console.log('no problems found');

            audit.forEach(x => {
                var rsvId = x.reservationId ? x.reservationId + ': ' : '';
                var res = x.resource ? x.resource + ': ' : '';
                var c = x.areEqual ? 'equal' : 'not-equal';
                x.row.addClass(c);
                console.log('[' + c + ']' + rsvId + res + x.account + ': ' + x.formattedLineCost1 + (x.areEqual ? ' == ' : ' != ') + x.formattedLineCost2 + ' [diff = ' + (x.lineCost1 - x.lineCost2).toFixed(4) + ']');
            });
        }
    </script>
</asp:Content>
