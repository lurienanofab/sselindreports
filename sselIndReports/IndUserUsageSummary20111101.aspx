<%@ Page Title="User Usage Summary" Language="C#" MasterPageFile="~/IndReportsMaster.Master" AutoEventWireup="true" CodeBehind="IndUserUsageSummary20111101.aspx.cs" Inherits="sselIndReports.IndUserUsageSummary20111101" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .detail-link {
            color: #000000;
            text-decoration: none;
        }

        div.ui-dialog, div.ui-dialog div {
            margin: 0 !important;
        }

        div.ui-widget {
            font-size: 10pt !important;
        }

        .error {
            color: #FF0000;
        }

        .message {
            background-color: #ffff00;
            border: solid 1px #000000;
            color: #ff0000;
            font-weight: bold;
            padding: 5px;
            margin: 5px 5px 10px 5px;
        }

        .uus-detail-table {
            border-collapse: collapse;
        }

            .uus-detail-table th, .uus-detail-table td {
                padding: 3px;
                border: solid 1px #AAAAAA;
            }

            .uus-detail-table th {
                background-color: #DCDCDC;
            }

            .uus-detail-table td.numeric-cell {
                text-align: right;
            }

        .striped > tbody > tr:nth-child(odd) > td {
            background-color: #eee;
        }

        .gridview > tbody > tr.child > td:first-child {
            padding-left: 20px;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="user-usage-summary">
        <input runat="server" id="hidAjaxUrl" type="hidden" class="ajax-url" />
        <div class="section">
            <h3>Individual User Usage Summary</h3>
            <asp:Panel runat="server" ID="panDisclaimerConfig" Visible="false">
                <asp:CheckBox runat="server" ID="chkShowDisclaimer" Text="Show Disclaimer" OnCheckedChanged="chkShowDisclaimer_CheckedChanged" AutoPostBack="true" />
            </asp:Panel>
            <asp:Panel runat="server" ID="panDisclaimer" CssClass="message" Visible="false">
                <asp:Literal runat="server" ID="litDisclaimerText"></asp:Literal>
            </asp:Panel>
            <div class="criteria">
                <table class="criteria-table">
                    <tr>
                        <td>Select period:</td>
                        <td class="disable">
                            <lnf:PeriodPicker runat="server" ID="pp1" AutoPostBack="true" OnSelectedPeriodChanged="pp1_SelectedPeriodChanged" />
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
        <div runat="server" id="divReportInfo" class="report section" visible="false">
            <h4>User Usage Summary Report</h4>
            <asp:Repeater runat="server" ID="rptReportInfo">
                <HeaderTemplate>
                    <table class="report-info-table">
                        <tbody>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr>
                        <th><%#Eval("Label")%></th>
                        <td><%#Eval("Value")%></td>
                    </tr>
                </ItemTemplate>
                <FooterTemplate>
                    </tbody>
                </table>
                </FooterTemplate>
            </asp:Repeater>
            <asp:Literal runat="server" ID="litSummaryDisclaimerText"></asp:Literal>
        </div>
        <div id="divReportContent" runat="server" visible="false">
            <asp:Label ID="lblSummaryApproximate" runat="server" Text="The numbers are approximate before the third business day of this month" Visible="false"></asp:Label>
            <div id="divAggReports" style="margin-left: 0px;" runat="server">
                <div id="divAggByOrg" class="section">
                    <h4 id="h4AggByOrg">Aggregate By Organization</h4>
                    Room:<br />
                    <asp:GridView runat="server" ID="gvAggByOrgRoom" CssClass="gridview" GridLines="None" AllowSorting="true" AutoGenerateColumns="false">
                        <HeaderStyle CssClass="header" />
                        <RowStyle CssClass="row" />
                        <AlternatingRowStyle CssClass="altrow" />
                        <SelectedRowStyle CssClass="selected" />
                        <FooterStyle CssClass="footer" />
                        <PagerStyle CssClass="pager" />
                        <PagerSettings Mode="NumericFirstLast" FirstPageText="First" LastPageText="Last" />
                        <Columns>
                            <asp:BoundField DataField="OrgName" HeaderText="Org">
                                <HeaderStyle Width="150" />
                                <ItemStyle HorizontalAlign="Center" />
                            </asp:BoundField>
                            <asp:TemplateField HeaderText="Room Cost">
                                <HeaderStyle Width="90" />
                                <ItemStyle HorizontalAlign="Right" />
                                <ItemTemplate>
                                    <a href="#" class="detail-link" onclick="showDetail({'type': 'agg-by-org', 'subtype': 'room', 'column': 'room-cost', 'orgId':'<%#Eval("OrgID") %>', 'clientId': '<%#SelectedClientID %>', 'period': '<%#SelectedPeriod.ToString("yyyy-MM-dd") %>'})">
                                        <%#Convert.ToDouble(Eval("RoomCharge")).ToString("$#,##0.00")%>
                                    </a>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="RoomMisc" HeaderText="Misc Charge" HtmlEncode="false" DataFormatString="{0:$#,##0.00}">
                                <HeaderStyle Width="95" />
                                <ItemStyle HorizontalAlign="Right" />
                            </asp:BoundField>
                            <asp:BoundField DataField="TotalUsageCharge" HeaderText="Total<br />(w/o Subsidy)" HtmlEncode="false" DataFormatString="{0:$#,##0.00}">
                                <HeaderStyle Width="95" />
                                <ItemStyle HorizontalAlign="Right" />
                            </asp:BoundField>
                            <asp:BoundField DataField="SubsidyDiscount" HeaderText="Subsidy" HtmlEncode="false" DataFormatString="{0:$#,##0.00}">
                                <HeaderStyle Width="95" />
                                <ItemStyle HorizontalAlign="Right" />
                            </asp:BoundField>
                            <asp:BoundField DataField="TotalCharge" HeaderText="Total<br />Room Cost" HtmlEncode="false" DataFormatString="{0:$#,##0.00}">
                                <HeaderStyle Width="95" />
                                <ItemStyle HorizontalAlign="Right" />
                            </asp:BoundField>
                        </Columns>
                        <EmptyDataTemplate>
                            <div class="empty-data">-- There is no record of room usage for this period --</div>
                        </EmptyDataTemplate>
                    </asp:GridView>
                    <%-- /room by org --%>
                    <br />
                    Tool:<br />
                    <asp:GridView runat="server" ID="gvToolOrg20110701" CssClass="gridview" GridLines="None" AllowSorting="true" AutoGenerateColumns="false" OnRowDataBound="gvToolOrg20110701_RowDataBound">
                        <HeaderStyle CssClass="header" />
                        <RowStyle CssClass="row" />
                        <AlternatingRowStyle CssClass="altrow" />
                        <SelectedRowStyle CssClass="selected" />
                        <FooterStyle CssClass="footer" />
                        <PagerStyle CssClass="pager" />
                        <PagerSettings Mode="NumericFirstLast" FirstPageText="First" LastPageText="Last" />
                        <Columns>
                            <asp:BoundField DataField="OrgName" HeaderText="Org" Visible="true" ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="150" />
                            <asp:BoundField DataField="UsageFeeCharged" HeaderText="Usage Charge" Visible="false" ItemStyle-HorizontalAlign="Right" HtmlEncode="false" DataFormatString="{0:$#,##0.00}" HeaderStyle-Width="95" />
                            <asp:BoundField DataField="UsageFeeDisplay" HeaderText="Scheduled" Visible="true" ItemStyle-HorizontalAlign="Right" HtmlEncode="false" DataFormatString="{0:$#,##0.00}" HeaderStyle-Width="150" />
                            <asp:BoundField DataField="OverTimePenaltyFee" HeaderText="Overtime Charge" Visible="true" ItemStyle-HorizontalAlign="Right" HtmlEncode="false" DataFormatString="{0:$#,##0.00}" HeaderStyle-Width="95" />
                            <asp:BoundField DataField="BookingFee" HeaderText="Booking Fee" Visible="true" ItemStyle-HorizontalAlign="Right" HtmlEncode="false" DataFormatString="{0:$#,##0.00}" HeaderStyle-Width="95" />
                            <asp:BoundField DataField="TransferredFee" HeaderText="Transferred Fee" Visible="true" ItemStyle-HorizontalAlign="Right" HtmlEncode="false" DataFormatString="{0:$#,##0.00}" HeaderStyle-Width="95" />
                            <asp:BoundField DataField="ForgivenFee" HeaderText="Forgiven Fee" Visible="true" ItemStyle-HorizontalAlign="Right" HtmlEncode="false" DataFormatString="{0:$#,##0.00}" HeaderStyle-Width="95" />
                            <asp:BoundField DataField="ToolCharge" HeaderText="Tool Cost" Visible="false" ItemStyle-HorizontalAlign="Right" HtmlEncode="false" DataFormatString="{0:$#,##0.00}" HeaderStyle-Width="95" />
                            <asp:BoundField DataField="ToolMisc" HeaderText="Misc Charge" Visible="true" ItemStyle-HorizontalAlign="Right" HtmlEncode="false" DataFormatString="{0:$#,##0.00}" HeaderStyle-Width="95" />
                            <asp:BoundField DataField="TotalUsageCharge" HeaderText="Total<br />(w/o Subsidy)" Visible="true" ItemStyle-HorizontalAlign="Right" HtmlEncode="false" DataFormatString="{0:$#,##0.00}" HeaderStyle-Width="95" />
                            <asp:BoundField DataField="SubsidyDiscount" HeaderText="Subsidy" Visible="true" ItemStyle-HorizontalAlign="Right" HtmlEncode="false" DataFormatString="{0:$#,##0.00}" HeaderStyle-Width="95" />
                            <asp:BoundField DataField="TotalCharge" HeaderText="Line Total" Visible="true" ItemStyle-HorizontalAlign="Right" HtmlEncode="false" DataFormatString="{0:$#,##0.00}" HeaderStyle-Width="95" />
                        </Columns>
                        <EmptyDataTemplate>
                            <div class="empty-data">-- There is no record of tool usage for this period --</div>
                        </EmptyDataTemplate>
                    </asp:GridView>
                    <%-- /tool by org --%>
                    <br />
                    Subsidy (room and tool charges only):<br />
                    <asp:GridView runat="server" ID="gvSubsidy" CssClass="gridview" GridLines="None" AllowSorting="true" AutoGenerateColumns="false">
                        <HeaderStyle CssClass="header" />
                        <RowStyle CssClass="row" />
                        <AlternatingRowStyle CssClass="altrow" />
                        <SelectedRowStyle CssClass="selected" />
                        <FooterStyle CssClass="footer" />
                        <PagerStyle CssClass="pager" />
                        <PagerSettings Mode="NumericFirstLast" FirstPageText="First" LastPageText="Last" />
                        <Columns>
                            <asp:BoundField DataField="OrgName" HeaderText="Org" ItemStyle-HorizontalAlign="Center">
                                <HeaderStyle Width="125" />
                            </asp:BoundField>
                            <asp:BoundField DataField="RoomSum" HeaderText="Room Cost" ItemStyle-HorizontalAlign="Center" HtmlEncode="false" DataFormatString="{0:$#,##0.00}" Visible="false">
                                <ItemStyle HorizontalAlign="Right" />
                                <HeaderStyle Width="90" />
                            </asp:BoundField>
                            <asp:BoundField DataField="RoomMiscSum" HeaderText="Room Misc" ItemStyle-HorizontalAlign="Center" HtmlEncode="false" DataFormatString="{0:$#,##0.00}" Visible="false">
                                <ItemStyle HorizontalAlign="Right" />
                                <HeaderStyle Width="90" />
                            </asp:BoundField>
                            <asp:BoundField DataField="ToolSum" HeaderText="Tool Cost" ItemStyle-HorizontalAlign="Center" HtmlEncode="false" DataFormatString="{0:$#,##0.00}" Visible="false">
                                <ItemStyle HorizontalAlign="Right" />
                                <HeaderStyle Width="90" />
                            </asp:BoundField>
                            <asp:BoundField DataField="ToolMiscSum" HeaderText="Tool Misc" ItemStyle-HorizontalAlign="Center" HtmlEncode="false" DataFormatString="{0:$#,##0.00}" Visible="false">
                                <ItemStyle HorizontalAlign="Right" />
                                <HeaderStyle Width="90" />
                            </asp:BoundField>
                            <asp:BoundField DataField="UserTotalSum" HeaderText="Usage Charges" ItemStyle-HorizontalAlign="Center" HtmlEncode="false" DataFormatString="{0:$#,##0.00}" Visible="true">
                                <ItemStyle HorizontalAlign="Right" />
                                <HeaderStyle Width="90" />
                            </asp:BoundField>
                            <asp:BoundField DataField="UserPaymentSum" HeaderText="Billed Charges" ItemStyle-HorizontalAlign="Center" HtmlEncode="false" DataFormatString="{0:$#,##0.00}" Visible="true">
                                <ItemStyle HorizontalAlign="Right" />
                                <HeaderStyle Width="90" />
                            </asp:BoundField>
                            <asp:BoundField DataField="Accumulated" HeaderText="Previous Balance" Visible="false" ItemStyle-HorizontalAlign="Center" HtmlEncode="false" DataFormatString="{0:$#,##0.00}">
                                <ItemStyle HorizontalAlign="Right" />
                                <HeaderStyle Width="90" />
                            </asp:BoundField>
                            <asp:BoundField DataField="StartingPeriod" HeaderText="Current Subsidy Year" ItemStyle-HorizontalAlign="Center" HtmlEncode="false" DataFormatString="{0:yyyy/MM/dd}" Visible="true">
                                <ItemStyle HorizontalAlign="Right" />
                                <HeaderStyle Width="90" />
                            </asp:BoundField>
                        </Columns>
                        <EmptyDataTemplate>
                            <div class="empty-data">-- There is no record of lab usage for this period OR this user is not an internal user --</div>
                        </EmptyDataTemplate>
                    </asp:GridView>
                    <%-- /subsidy by org --%>
                    <br />
                    Store:<br />
                    <asp:GridView runat="server" ID="gvStoreOrg" CssClass="gridview" GridLines="None" AllowSorting="true" AutoGenerateColumns="false">
                        <HeaderStyle CssClass="header" />
                        <RowStyle CssClass="row" />
                        <AlternatingRowStyle CssClass="altrow" />
                        <SelectedRowStyle CssClass="selected" />
                        <FooterStyle CssClass="footer" />
                        <PagerStyle CssClass="pager" />
                        <PagerSettings Mode="NumericFirstLast" FirstPageText="First" LastPageText="Last" />
                        <Columns>
                            <asp:BoundField DataField="OrgName" HeaderText="Org" ItemStyle-HorizontalAlign="Center">
                                <HeaderStyle Width="125" />
                            </asp:BoundField>
                            <asp:BoundField DataField="TotalChargeNoMisc" HeaderText="Store Cost" ItemStyle-HorizontalAlign="Right" HtmlEncode="false" DataFormatString="{0:$#,##0.00}">
                                <HeaderStyle Width="90" />
                            </asp:BoundField>
                            <asp:BoundField DataField="StoreMisc" HeaderText="Misc Charge" ItemStyle-HorizontalAlign="Right" HtmlEncode="false" DataFormatString="{0:$#,##0.00}">
                                <HeaderStyle Width="90" />
                            </asp:BoundField>
                            <asp:BoundField DataField="TotalCharge" HeaderText="Total Store Cost" ItemStyle-HorizontalAlign="Right" HtmlEncode="false" DataFormatString="{0:$#,##0.00}" Visible="true">
                                <HeaderStyle Width="90" />
                            </asp:BoundField>
                        </Columns>
                        <EmptyDataTemplate>
                            <div class="empty-data">-- There is no record of store usage for this period --</div>
                        </EmptyDataTemplate>
                    </asp:GridView>
                    <%-- /store by org --%>
                </div>
                <div id="divAggByAcc" class="section">
                    <h4 id="h4AggByAcc">Aggregate by Accounts</h4>
                    Room:<br />
                    <asp:GridView runat="server" ID="gvRoomAccount" CssClass="gridview" GridLines="None" AllowSorting="true" AutoGenerateColumns="false">
                        <HeaderStyle CssClass="header" />
                        <RowStyle CssClass="row" />
                        <AlternatingRowStyle CssClass="altrow" />
                        <SelectedRowStyle CssClass="selected" />
                        <FooterStyle CssClass="footer" />
                        <PagerStyle CssClass="pager" />
                        <PagerSettings Mode="NumericFirstLast" FirstPageText="First" LastPageText="Last" />
                        <Columns>
                            <asp:BoundField DataField="AccountName" HeaderText="Account" ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="125" />
                            <asp:BoundField DataField="ShortCode" HeaderText="Short Code" ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="90" />
                            <asp:BoundField DataField="BillingTypeName" HeaderText="Billing Type" ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="125" />
                            <asp:BoundField DataField="RoomCharge" HeaderText="Room Charge" ItemStyle-HorizontalAlign="Right" HtmlEncode="false" DataFormatString="{0:$#,##0.00}" Visible="true" HeaderStyle-Width="90" />
                            <asp:BoundField DataField="EntryCharge" HeaderText="Entry Charge" ItemStyle-HorizontalAlign="Right" HtmlEncode="false" DataFormatString="{0:$#,##0.00}" Visible="true" HeaderStyle-Width="90" />
                            <asp:BoundField DataField="RoomMisc" HeaderText="Misc Charge" ItemStyle-HorizontalAlign="Right" HtmlEncode="false" DataFormatString="{0:$#,##0.00}" Visible="true" HeaderStyle-Width="90" />
                            <asp:BoundField DataField="TotalChargePreSubsidy" HeaderText="Total (w/o Subsidy)" ItemStyle-HorizontalAlign="Right" HtmlEncode="false" DataFormatString="{0:$#,##0.00}" Visible="true" HeaderStyle-Width="90" />
                            <asp:BoundField DataField="SubsidyDiscount" HeaderText="Subsidy" ItemStyle-HorizontalAlign="Right" HtmlEncode="false" DataFormatString="{0:$#,##0.00}" Visible="true" HeaderStyle-Width="90" />
                            <asp:BoundField DataField="TotalCharge" HeaderText="Line Total" ItemStyle-HorizontalAlign="Right" HtmlEncode="false" DataFormatString="{0:$#,##0.00}" Visible="true" HeaderStyle-Width="90" />
                        </Columns>
                        <EmptyDataTemplate>
                            <div class="empty-data">-- There is no record of room usage for this period --</div>
                        </EmptyDataTemplate>
                    </asp:GridView>
                    <br />
                    Tool:<br />
                    <asp:Literal runat="server" ID="litToolAccountDebug"></asp:Literal>
                    <asp:GridView runat="server" ID="gvToolAccount20110701" CssClass="gridview" GridLines="None" AllowSorting="true" AutoGenerateColumns="false" OnRowDataBound="gvToolAccount20110701_RowDataBound">
                        <HeaderStyle CssClass="header" />
                        <RowStyle CssClass="row" />
                        <AlternatingRowStyle CssClass="altrow" />
                        <SelectedRowStyle CssClass="selected" />
                        <FooterStyle CssClass="footer" />
                        <PagerStyle CssClass="pager" />
                        <PagerSettings Mode="NumericFirstLast" FirstPageText="First" LastPageText="Last" />
                        <Columns>
                            <asp:BoundField DataField="AccountName" HeaderText="Account" ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="125" />
                            <asp:BoundField DataField="ShortCode" HeaderText="Short Code" ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="90" />
                            <asp:BoundField DataField="BillingTypeName" HeaderText="Billing Type" ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="125" />
                            <asp:BoundField DataField="UsageFeeCharged" HeaderText="Usage Charge" ItemStyle-HorizontalAlign="Right" HtmlEncode="false" DataFormatString="{0:$#,##0.00}" Visible="false" HeaderStyle-Width="125" />
                            <asp:BoundField DataField="UsageFeeDisplay" HeaderText="Scheduled" ItemStyle-HorizontalAlign="Right" HtmlEncode="false" DataFormatString="{0:$#,##0.00}" Visible="true" HeaderStyle-Width="125" />
                            <asp:BoundField DataField="OverTimePenaltyFee" HeaderText="Overtime Charge" ItemStyle-HorizontalAlign="Right" HtmlEncode="false" DataFormatString="{0:$#,##0.00}" Visible="true" HeaderStyle-Width="90" />
                            <asp:BoundField DataField="BookingFee" HeaderText="Booking Fee" ItemStyle-HorizontalAlign="Right" HtmlEncode="false" DataFormatString="{0:$#,##0.00}" Visible="true" HeaderStyle-Width="90" />
                            <asp:BoundField DataField="TransferredFee" HeaderText="Transferred Fee" ItemStyle-HorizontalAlign="Right" HtmlEncode="false" DataFormatString="{0:$#,##0.00}" Visible="true" HeaderStyle-Width="90" />
                            <asp:BoundField DataField="ForgivenFee" HeaderText="Forgiven Fee" ItemStyle-HorizontalAlign="Right" HtmlEncode="false" DataFormatString="{0:$#,##0.00}" Visible="true" HeaderStyle-Width="90" />
                            <asp:BoundField DataField="ToolMisc" HeaderText="Misc Charge" ItemStyle-HorizontalAlign="Right" HtmlEncode="false" DataFormatString="{0:$#,##0.00}" Visible="true" HeaderStyle-Width="90" />
                            <asp:BoundField DataField="TotalChargePreSubsidy" HeaderText="Total (w/o Subsidy)" ItemStyle-HorizontalAlign="Right" HtmlEncode="false" DataFormatString="{0:$#,##0.00}" Visible="true" HeaderStyle-Width="90" />
                            <asp:BoundField DataField="SubsidyDiscount" HeaderText="Subsidy" ItemStyle-HorizontalAlign="Right" HtmlEncode="false" DataFormatString="{0:$#,##0.00}" Visible="true" HeaderStyle-Width="90" />
                            <asp:BoundField DataField="TotalCharge" HeaderText="Line Total" ItemStyle-HorizontalAlign="Right" HtmlEncode="false" DataFormatString="{0:$#,##0.00}" Visible="true" HeaderStyle-Width="90" />
                        </Columns>
                        <EmptyDataTemplate>
                            <div class="empty-data">-- There is no record of tool usage for this period --</div>
                        </EmptyDataTemplate>
                    </asp:GridView>
                    <br />
                    Store:<br />
                    <asp:GridView runat="server" ID="gvStoreAccount" CssClass="gridview" GridLines="None" AllowSorting="true" AutoGenerateColumns="false">
                        <HeaderStyle CssClass="header" />
                        <RowStyle CssClass="row" />
                        <AlternatingRowStyle CssClass="altrow" />
                        <SelectedRowStyle CssClass="selected" />
                        <FooterStyle CssClass="footer" />
                        <PagerStyle CssClass="pager" />
                        <PagerSettings Mode="NumericFirstLast" FirstPageText="First" LastPageText="Last" />
                        <Columns>
                            <asp:BoundField DataField="AccountName" HeaderText="Account" ItemStyle-HorizontalAlign="Center">
                                <HeaderStyle Width="125" />
                            </asp:BoundField>
                            <asp:BoundField DataField="ShortCode" HeaderText="Short Code" ItemStyle-HorizontalAlign="Center">
                                <HeaderStyle Width="90" />
                            </asp:BoundField>
                            <asp:BoundField DataField="TotalCost" HeaderText="Store Cost" ItemStyle-HorizontalAlign="Center" HtmlEncode="false" DataFormatString="{0:$#,##0.00}" Visible="true">
                                <ItemStyle HorizontalAlign="Right" />
                                <HeaderStyle Width="90" />
                            </asp:BoundField>
                            <asp:BoundField DataField="StoreMisc" HeaderText="Misc Charge" ItemStyle-HorizontalAlign="Center" HtmlEncode="false" DataFormatString="{0:$#,##0.00}" Visible="true">
                                <ItemStyle HorizontalAlign="Right" />
                                <HeaderStyle Width="90" />
                            </asp:BoundField>
                            <asp:BoundField DataField="TotalCharge" HeaderText="Line Total" ItemStyle-HorizontalAlign="Center" HtmlEncode="false" DataFormatString="{0:$#,##0.00}" Visible="true">
                                <ItemStyle HorizontalAlign="Right" />
                                <HeaderStyle Width="90" />
                            </asp:BoundField>
                        </Columns>
                        <EmptyDataTemplate>
                            <div class="empty-data">-- There is no record of store usage for this period --</div>
                        </EmptyDataTemplate>
                    </asp:GridView>
                </div>
            </div>
            <div id="divDetailAll" class="section">
                <h4 id="h4BillingDetail">Billing Details</h4>
                <h5>
                    <asp:Label ID="lblRoom" runat="server"></asp:Label>
                    <asp:Label ID="lblRoomHours" runat="server"></asp:Label>
                </h5>
                <div class="lab-usage" style="display: inline-block;">

                    <div class="room-billing-detail">
                        <asp:Repeater runat="server" ID="rptParentRoomDetail">
                            <ItemTemplate>
                                <div class="parent-room">
                                    <h3><%#Eval("Room")%></h3>
                                    <asp:Repeater runat="server" ID="rptParentRoomAccounts">
                                        <HeaderTemplate>
                                            <table class="gridview">
                                                <thead>
                                                    <tr>
                                                        <td>Days</td>
                                                        <td>Entries</td>
                                                        <td>Account</td>
                                                        <td>ShortCode</td>
                                                        <td>BillingType</td>
                                                        <td>DailyFee</td>
                                                        <td>EntryFee</td>
                                                        <td>LineTotal</td>
                                                    </tr>
                                                </thead>
                                                <tbody>
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <tr>
                                                <td><%#Eval("Days")%></td>
                                                <td><%#Eval("Entries")%></td>
                                                <td><%#Eval("Account")%></td>
                                                <td><%#Eval("ShortCode")%></td>
                                                <td><%#Eval("BillingType")%></td>
                                                <td><%#Eval("DailyFee")%></td>
                                                <td><%#Eval("EntryFee")%></td>
                                                <td><%#Eval("LineTotal")%></td>
                                            </tr>
                                        </ItemTemplate>
                                        <FooterTemplate>
                                            </tbody>
                                            </table>

                                            <asp:Repeater runat="server" ID="rptChildRoomDetail">
                                                <ItemTemplate>
                                                    <div class="child-room">
                                                        <h3><%#Eval("Room")%></h3>
                                                        <asp:Repeater runat="server" ID="rptChildRoomAccounts">
                                                            <HeaderTemplate>
                                                                <table class="gridview">
                                                                    <thead>
                                                                        <tr>
                                                                            <td>Days</td>
                                                                            <td>Entries</td>
                                                                            <td>Account</td>
                                                                            <td>ShortCode</td>
                                                                            <td>BillingType</td>
                                                                            <td>DailyFee</td>
                                                                            <td>EntryFee</td>
                                                                            <td>LineTotal</td>
                                                                        </tr>
                                                                    </thead>
                                                                    <tbody>
                                                            </HeaderTemplate>
                                                            <ItemTemplate>
                                                                <tr>
                                                                    <td><%#Eval("Days")%></td>
                                                                    <td><%#Eval("Entries")%></td>
                                                                    <td><%#Eval("Account")%></td>
                                                                    <td><%#Eval("ShortCode")%></td>
                                                                    <td><%#Eval("BillingType")%></td>
                                                                    <td><%#Eval("DailyFee")%></td>
                                                                    <td><%#Eval("EntryFee")%></td>
                                                                    <td><%#Eval("LineTotal")%></td>
                                                                </tr>
                                                            </ItemTemplate>
                                                            <FooterTemplate>
                                                                </tbody>
                                                                </table>
                                                            </FooterTemplate>
                                                        </asp:Repeater>
                                                    </div>
                                                </ItemTemplate>
                                            </asp:Repeater>

                                        </FooterTemplate>
                                    </asp:Repeater>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>
                    </div>
                    <!-- /room-billing-detail -->

                    <asp:Repeater runat="server" ID="rptRoomDetail">
                        <HeaderTemplate>
                            <table class="gridview striped" style="border-collapse: collapse;">
                                <thead>
                                    <tr class="header">
                                        <th style="width: 125px;">Room</th>
                                        <th style="width: 80px;">Days</th>
                                        <th style="width: 80px;">Entries</th>
                                        <th style="width: 200px;">Account</th>
                                        <th style="width: 80px;">Short Code</th>
                                        <th>Billing Type</th>
                                        <th>Daily Fee</th>
                                        <th>Entry Fee</th>
                                        <th>Line Total</th>
                                    </tr>
                                </thead>
                                <tbody>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <tr class="<%#Eval("RowCssClass")%>">
                                <td><%#Eval("Room")%></td>
                                <td style="text-align: center;"><%#Eval("ChargeDays", "{0:F2}")%></td>
                                <td style="text-align: center;"><%#Eval("Entries", "{0:F2}")%></td>
                                <td style="text-align: center;"><%#Eval("Name")%></td>
                                <td style="text-align: center;"><%#Eval("ShortCode")%></td>
                                <td style="text-align: center;"><%#Eval("BillingTypeName")%></td>
                                <td style="text-align: right;"><%#Eval("DailyFee", "{0:C}")%></td>
                                <td style="text-align: right;"><%#Eval("EntryFee", "{0:C}")%></td>
                                <td style="text-align: right;"><%#Eval("LineCost", "{0:C}")%></td>
                            </tr>
                        </ItemTemplate>
                        <FooterTemplate>
                            </tbody>
                            </table>
                        </FooterTemplate>
                    </asp:Repeater>

                    <asp:GridView runat="server" ID="gvRoomDetail" CssClass="gridview" GridLines="None" AutoGenerateColumns="false">
                        <HeaderStyle CssClass="header" />
                        <RowStyle CssClass="row" />
                        <AlternatingRowStyle CssClass="altrow" />
                        <SelectedRowStyle CssClass="selected" />
                        <FooterStyle CssClass="footer" />
                        <PagerStyle CssClass="pager" />
                        <PagerSettings Mode="NumericFirstLast" FirstPageText="First" LastPageText="Last" />
                        <Columns>
                            <asp:BoundField DataField="Room" ItemStyle-HorizontalAlign="Center" HeaderText="Room">
                                <HeaderStyle Width="125" />
                            </asp:BoundField>
                            <asp:BoundField DataField="ChargeDays" HeaderText="Days" ItemStyle-HorizontalAlign="Center" HtmlEncode="false" DataFormatString="{0:F2}">
                                <HeaderStyle Width="80" />
                            </asp:BoundField>
                            <asp:BoundField DataField="Entries" HeaderText="Entries" ItemStyle-HorizontalAlign="Center" HtmlEncode="false" DataFormatString="{0:F2}">
                                <HeaderStyle Width="80" />
                            </asp:BoundField>
                            <asp:BoundField DataField="Hours" HeaderText="Duration (hours)" ItemStyle-HorizontalAlign="Center" HtmlEncode="false" Visible="false" DataFormatString="{0:F4}">
                                <HeaderStyle Width="100" />
                            </asp:BoundField>
                            <asp:BoundField DataField="LineCost" HeaderText="Cost" ItemStyle-HorizontalAlign="Center" HtmlEncode="false" DataFormatString="{0:$#,##0.00}" Visible="false">
                                <ItemStyle HorizontalAlign="Right" />
                                <HeaderStyle Width="90" />
                            </asp:BoundField>
                            <asp:BoundField DataField="Name" HeaderText="Account" ItemStyle-HorizontalAlign="Center">
                                <HeaderStyle Width="250" />
                            </asp:BoundField>
                            <asp:BoundField DataField="ShortCode" HeaderText="Short Code" ItemStyle-HorizontalAlign="Center">
                                <HeaderStyle Width="90" />
                            </asp:BoundField>
                            <asp:BoundField DataField="BillingTypeName" HeaderText="Billing Type" ItemStyle-HorizontalAlign="Center" />
                            <asp:BoundField DataField="OrgName" Visible="false" HeaderText="Org" ItemStyle-HorizontalAlign="Center" />
                            <asp:BoundField DataField="DailyFee" DataFormatString="{0:C}" HeaderText="Daily Fee" HtmlEncode="false" ItemStyle-HorizontalAlign="Right" />
                            <asp:BoundField DataField="EntryFee" DataFormatString="{0:C}" HeaderText="Entry Fee" HtmlEncode="false" ItemStyle-HorizontalAlign="Right" />
                            <asp:BoundField DataField="LineCost" DataFormatString="{0:C}" HeaderText="Line Total" HtmlEncode="false" ItemStyle-HorizontalAlign="Right" />
                        </Columns>
                    </asp:GridView>
                    <div style="text-align: right; padding-right: 5px; font-size: small; color: Maroon;">
                        <asp:Label ID="lblRoomsSum" CssClass="NormalText" runat="server" Visible="false" />
                    </div>
                </div>
                <h5>
                    <asp:Label ID="lblTool" runat="server"></asp:Label>
                </h5>
                <div class="tool-usage" style="display: inline-block;">
                    <asp:GridView runat="server" ID="gvToolDetail" CssClass="gridview" GridLines="None" AutoGenerateColumns="false" OnRowDataBound="gvToolDetail_RowDataBound">
                        <HeaderStyle CssClass="header" />
                        <RowStyle CssClass="row" />
                        <AlternatingRowStyle CssClass="altrow" />
                        <SelectedRowStyle CssClass="selected" />
                        <FooterStyle CssClass="footer" />
                        <PagerStyle CssClass="pager" />
                        <PagerSettings Mode="NumericFirstLast" FirstPageText="First" LastPageText="Last" />
                        <Columns>
                            <asp:BoundField DataField="ResourceName" HeaderText="Tool" HeaderStyle-Width="220" ItemStyle-HorizontalAlign="Center" />
                            <asp:BoundField DataField="RoomName" HeaderText="Lab" HeaderStyle-Width="100" ItemStyle-HorizontalAlign="Center" />
                            <asp:BoundField DataField="TotalChargeDuration" Visible="false" HeaderText="Charged (hours)" HtmlEncode="false" DataFormatString="{0:F2}" HeaderStyle-Width="60" ItemStyle-HorizontalAlign="Center" />
                            <asp:BoundField DataField="TotalActDuration" Visible="false" HeaderText="Activated Used (hours)" HtmlEncode="false" DataFormatString="{0:F2}" HeaderStyle-Width="80" ItemStyle-HorizontalAlign="Center" />
                            <asp:BoundField DataField="ActivatedUsed" Visible="true" HeaderText="Activated Used (hours)" HtmlEncode="false" DataFormatString="{0:F2}" HeaderStyle-Width="80" ItemStyle-HorizontalAlign="Center" />
                            <asp:BoundField DataField="ActivatedUnused" Visible="true" HeaderText="Activated Unused (hours)" HtmlEncode="false" DataFormatString="{0:F2}" HeaderStyle-Width="80" ItemStyle-HorizontalAlign="Center" />
                            <asp:BoundField DataField="TotalOverTime" HeaderText="Overtime (hours)" HtmlEncode="false" DataFormatString="{0:F2}" HeaderStyle-Width="80" ItemStyle-HorizontalAlign="Center" />
                            <asp:BoundField DataField="OverTimePenaltyFee" HeaderText="Overtime Fee" HtmlEncode="false" DataFormatString="{0:C}" HeaderStyle-Width="80" ItemStyle-HorizontalAlign="Right" />
                            <asp:BoundField DataField="UnstartedUnused" Visible="true" HeaderText="Unactivated (hours)" HtmlEncode="false" DataFormatString="{0:F2}" HeaderStyle-Width="80" ItemStyle-HorizontalAlign="Center" />
                            <asp:BoundField DataField="BookingFee" HeaderText="Booking Fee" HtmlEncode="false" DataFormatString="{0:C}" ItemStyle-HorizontalAlign="Right" />
                            <asp:BoundField DataField="TotalTransferredDuration" HeaderText="Transferred (hours)" HtmlEncode="false" DataFormatString="{0:F2}" HeaderStyle-Width="60" ItemStyle-HorizontalAlign="Center" />
                            <asp:BoundField DataField="TotalForgivenDuration" Visible="true" HeaderText="Forgiven (hours)" HtmlEncode="false" DataFormatString="{0:F2}" HeaderStyle-Width="80" ItemStyle-HorizontalAlign="Center" />
                            <asp:BoundField DataField="ResourceRate" HeaderText="Rate" HtmlEncode="false" DataFormatString="{0:C}" ItemStyle-HorizontalAlign="Right" />
                            <asp:BoundField DataField="AccountName" HeaderText="Account" HeaderStyle-Width="250" ItemStyle-HorizontalAlign="Center" />
                            <asp:BoundField DataField="ShortCode" HeaderText="Short Code" HeaderStyle-Width="70" ItemStyle-HorizontalAlign="Center" />
                            <asp:BoundField DataField="OrgName" Visible="false" HeaderText="Org" ItemStyle-HorizontalAlign="Center"></asp:BoundField>
                            <asp:BoundField DataField="LineCost" HeaderText="Line Total" HtmlEncode="false" DataFormatString="{0:C}" ItemStyle-HorizontalAlign="Right" />
                        </Columns>
                        <EmptyDataTemplate>
                            <div class="empty-data">-- There is no record of activated reservation for this period --</div>
                        </EmptyDataTemplate>
                    </asp:GridView>
                    <div style="text-align: right; padding-right: 5px; font-size: small; color: Maroon;">
                        <asp:Label ID="lbl20110401RoomSum" CssClass="NormalText" runat="server"></asp:Label>
                        <asp:Label ID="lbl20110401ResFee" CssClass="NormalText" runat="server"></asp:Label>
                    </div>
                </div>
                <h5>
                    <asp:Label ID="lblStore" runat="server"></asp:Label>
                </h5>
                <div class="store-usage" style="display: inline-block;">
                    <asp:GridView runat="server" ID="gvStoreDetail" CssClass="gridview" GridLines="None" AutoGenerateColumns="false">
                        <HeaderStyle CssClass="header" />
                        <RowStyle CssClass="row" />
                        <AlternatingRowStyle CssClass="altrow" />
                        <SelectedRowStyle CssClass="selected" />
                        <FooterStyle CssClass="footer" />
                        <PagerStyle CssClass="pager" />
                        <PagerSettings Mode="NumericFirstLast" FirstPageText="First" LastPageText="Last" />
                        <Columns>
                            <asp:BoundField DataField="Description" HeaderText="Item" ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="220" />
                            <asp:BoundField DataField="TotalQuantity" HeaderText="Quantity" DataFormatString="{0:F2}" HtmlEncode="false" ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="80" />
                            <asp:BoundField DataField="Name" HeaderText="Account" ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="200" />
                            <asp:BoundField DataField="ShortCode" HeaderText="Short Code" HeaderStyle-Width="90" ItemStyle-HorizontalAlign="Center" />
                            <asp:BoundField DataField="OrgName" Visible="false" HeaderText="Org" ItemStyle-HorizontalAlign="Center" />
                            <asp:BoundField DataField="LineCost" HeaderText="Line Cost" ItemStyle-HorizontalAlign="Right" DataFormatString="{0:$#,##0.00}" HtmlEncode="false" HeaderStyle-Width="90" />
                        </Columns>
                    </asp:GridView>
                </div>
                <h5>Miscellaneous charges</h5>
                <div class="misc-usage" style="display: inline-block;">
                    <asp:GridView runat="server" ID="gvMisc" CssClass="gridview" GridLines="None" AutoGenerateColumns="false">
                        <HeaderStyle CssClass="header" />
                        <RowStyle CssClass="row" />
                        <AlternatingRowStyle CssClass="altrow" />
                        <SelectedRowStyle CssClass="selected" />
                        <FooterStyle CssClass="footer" />
                        <PagerStyle CssClass="pager" />
                        <PagerSettings Mode="NumericFirstLast" FirstPageText="First" LastPageText="Last" />
                        <Columns>
                            <asp:BoundField DataField="AccountName" HeaderText="Account" ItemStyle-HorizontalAlign="Center">
                                <HeaderStyle Width="125" />
                            </asp:BoundField>
                            <asp:BoundField DataField="SUBType" HeaderText="Type" ItemStyle-HorizontalAlign="Center">
                                <HeaderStyle Width="90" />
                            </asp:BoundField>
                            <asp:BoundField DataField="Description" HeaderText="Description" ItemStyle-HorizontalAlign="Center">
                                <HeaderStyle Width="90" />
                            </asp:BoundField>
                            <asp:BoundField DataField="MiscCost" HeaderText="Line Total" ItemStyle-HorizontalAlign="Center" HtmlEncode="false" DataFormatString="{0:$#,##0.00}" Visible="true">
                                <ItemStyle HorizontalAlign="Center" />
                                <HeaderStyle Width="90" />
                            </asp:BoundField>
                        </Columns>
                        <EmptyDataTemplate>
                            <div class="empty-data">-- There is no record of miscellaneous charges for this period --</div>
                        </EmptyDataTemplate>
                    </asp:GridView>
                </div>
            </div>
        </div>
        <div class="uus-detail-dialog" style="display: none;">
            <div class="uus-detail-content">
            </div>
        </div>
    </div>
</asp:Content>

<asp:Content runat="server" ID="Content3" ContentPlaceHolderID="scripts">
    <script type="text/javascript">
        $('.detail-link').click(function (event) {
            event.preventDefault();
        });

        function getAjaxUrl() {
            return $('.ajax-url').val();
        }

        function showDetail(args) {
            getData(args, function (data) {
                $('.uus-detail-content').html(data);
                $('.uus-detail-dialog').dialog(getDialogOptions(args));
            });
        }

        function getData(args, callback) {
            args.command = 'uus-detail';
            args.dataType = 'html';
            $.ajax({
                'url': getAjaxUrl(),
                'type': 'POST',
                'data': args,
                'success': function (data) {
                    callback(data);
                },
                'error': function (err) {
                    alert(err);
                }
            });
        }

        function getDialogOptions(args) {
            var result = { 'title': '', 'width': '500px' };
            switch (args.type) {
                case 'agg-by-org':
                    result.title = 'Aggregate By Organization';
                    break;
            }
            switch (args.subtype) {
                case 'room':
                    result.title += ' : Room';
                    break;
            }
            switch (args.column) {
                case 'room-cost':
                    result.title += ' : Room Cost';
                    result.width = '700px';
                    break;
            }
            return result;
        }
    </script>
</asp:Content>
