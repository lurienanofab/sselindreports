<%@ Page Title="User Usage Summary" Language="C#" MasterPageFile="~/IndReportsMaster.Master" AutoEventWireup="true" CodeBehind="IndUserUsageSummary20110401.aspx.cs" Inherits="sselIndReports.IndUserUsageSummary20110401" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="section">
        <h3>Individual User Usage Summary</h3>
        <div class="criteria">
            <table class="criteria-table">
                <tr>
                    <td>Select period:</td>
                    <td>
                        <lnf:PeriodPicker runat="server" ID="pp1" AutoPostBack="true" OnSelectedPeriodChanged="pp1_SelectedPeriodChanged" />
                    </td>
                </tr>
                <tr>
                    <td>Select user:</td>
                    <td>
                        <asp:DropDownList runat="server" ID="ddlUser" DataTextField="DisplayName" DataValueField="ClientID" CssClass="report-select"></asp:DropDownList>
                        <asp:Label runat="server" ID="lblClientID"></asp:Label>
                    </td>
                </tr>
            </table>
            <div class="criteria-item">
                <asp:Button runat="server" ID="btnReport" Text="Retrieve Data" CssClass="report-button" OnClick="ReportButton_Click" />
                <asp:LinkButton runat="server" ID="btnBack" Text="&larr; Back to Main Page" OnClick="BackButton_Click" CssClass="back-link"></asp:LinkButton>
            </div>
        </div>
    </div>
    <div id="divReportContent" runat="server" visible="false">
        <asp:Label ID="lblSummaryApproximate" runat="server" Text="The numbers are approximate before the third business day of this month" Visible="false"></asp:Label>
        <div runat="server" id="divAggReports">
            <div id="divAggByOrg" class="section">
                <h4 id="h4AggByOrg">Aggregate By Organization</h4>
                Room:<br />
                <asp:GridView runat="server" ID="gvRoomOrg" CssClass="gridview" GridLines="None" AllowSorting="true" AutoGenerateColumns="false">
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
                        <asp:BoundField DataField="RoomCharge" HeaderText="Room Cost" ItemStyle-HorizontalAlign="Center" HtmlEncode="false" DataFormatString="{0:$#,##0.00}" Visible="true">
                            <ItemStyle HorizontalAlign="Right" />
                            <HeaderStyle Width="90" />
                        </asp:BoundField>
                        <asp:BoundField DataField="RoomMisc" HeaderText="Misc Charge" ItemStyle-HorizontalAlign="Center" HtmlEncode="false" DataFormatString="{0:$#,##0.00}">
                            <ItemStyle HorizontalAlign="Right" />
                            <HeaderStyle Width="90" />
                        </asp:BoundField>
                        <asp:BoundField DataField="TotalUsageCharge" HeaderText="Total Room Cost" ItemStyle-HorizontalAlign="Center" HtmlEncode="false" DataFormatString="{0:$#,##0.00}">
                            <ItemStyle HorizontalAlign="Right" />
                            <HeaderStyle Width="90" />
                        </asp:BoundField>
                    </Columns>
                    <EmptyDataTemplate>
                        <div class="empty-data">-- There is no record of room usage for this period --</div>
                    </EmptyDataTemplate>
                </asp:GridView>
                <br />
                Tool:<br />
                <asp:GridView runat="server" ID="gvToolOrg" CssClass="gridview" GridLines="None" AllowSorting="true" AutoGenerateColumns="false">
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
                        <asp:BoundField DataField="UsageFeeCharged" HeaderText="Usage Charge" ItemStyle-HorizontalAlign="Center" HtmlEncode="false" DataFormatString="{0:$#,##0.00}" Visible="true">
                            <ItemStyle HorizontalAlign="Right" />
                            <HeaderStyle Width="90" />
                        </asp:BoundField>
                        <asp:BoundField DataField="OverTimePenaltyFee" HeaderText="Overtime Charge" ItemStyle-HorizontalAlign="Center" HtmlEncode="false" DataFormatString="{0:$#,##0.00}" Visible="true">
                            <ItemStyle HorizontalAlign="Right" />
                            <HeaderStyle Width="90" />
                        </asp:BoundField>
                        <asp:BoundField DataField="UncancelledPenaltyFee" HeaderText="Uncancelled Charge" ItemStyle-HorizontalAlign="Center" HtmlEncode="false" DataFormatString="{0:$#,##0.00}" Visible="true">
                            <ItemStyle HorizontalAlign="Right" />
                            <HeaderStyle Width="90" />
                        </asp:BoundField>
                        <asp:BoundField DataField="ReservationFee" HeaderText="Reservation Charge" ItemStyle-HorizontalAlign="Center" HtmlEncode="false" DataFormatString="{0:$#,##0.00}" Visible="true">
                            <ItemStyle HorizontalAlign="Right" />
                            <HeaderStyle Width="90" />
                        </asp:BoundField>
                        <asp:BoundField DataField="ToolCharge" HeaderText="Tool Cost" ItemStyle-HorizontalAlign="Center" HtmlEncode="false" DataFormatString="{0:$#,##0.00}" Visible="true">
                            <ItemStyle HorizontalAlign="Right" />
                            <HeaderStyle Width="90" />
                        </asp:BoundField>
                        <asp:BoundField DataField="ToolMisc" HeaderText="Misc Charge" ItemStyle-HorizontalAlign="Center" HtmlEncode="false" DataFormatString="{0:$#,##0.00}">
                            <ItemStyle HorizontalAlign="Right" />
                            <HeaderStyle Width="90" />
                        </asp:BoundField>
                        <asp:BoundField DataField="TotalUsageCharge" HeaderText="Total Tool Cost" ItemStyle-HorizontalAlign="Center" HtmlEncode="false" DataFormatString="{0:$#,##0.00}">
                            <ItemStyle HorizontalAlign="Right" />
                            <HeaderStyle Width="90" />
                        </asp:BoundField>
                    </Columns>
                    <EmptyDataTemplate>
                        <div class="empty-data">-- There is no record of tool usage for this period --</div>
                    </EmptyDataTemplate>
                </asp:GridView>
                <asp:GridView runat="server" ID="gvToolOrg20110401" CssClass="gridview" GridLines="None" AllowSorting="true" AutoGenerateColumns="false">
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
                        <asp:BoundField DataField="UsageFeeCharged" HeaderText="Usage Charge" ItemStyle-HorizontalAlign="Center" HtmlEncode="false" DataFormatString="{0:$#,##0.00}" Visible="true">
                            <ItemStyle HorizontalAlign="Right" />
                            <HeaderStyle Width="90" />
                        </asp:BoundField>
                        <asp:BoundField DataField="OverTimePenaltyFee" HeaderText="Overtime Charge" ItemStyle-HorizontalAlign="Center" HtmlEncode="false" DataFormatString="{0:$#,##0.00}" Visible="true">
                            <ItemStyle HorizontalAlign="Right" />
                            <HeaderStyle Width="90" />
                        </asp:BoundField>
                        <asp:BoundField DataField="BookingFee" HeaderText="Reservation Charge" ItemStyle-HorizontalAlign="Center" HtmlEncode="false" DataFormatString="{0:$#,##0.00}" Visible="true">
                            <ItemStyle HorizontalAlign="Right" />
                            <HeaderStyle Width="90" />
                        </asp:BoundField>
                        <asp:BoundField DataField="ToolCharge" HeaderText="Tool Cost" ItemStyle-HorizontalAlign="Center" HtmlEncode="false" DataFormatString="{0:$#,##0.00}" Visible="true">
                            <ItemStyle HorizontalAlign="Right" />
                            <HeaderStyle Width="90" />
                        </asp:BoundField>
                        <asp:BoundField DataField="ToolMisc" HeaderText="Misc Charge" ItemStyle-HorizontalAlign="Center" HtmlEncode="false" DataFormatString="{0:$#,##0.00}">
                            <ItemStyle HorizontalAlign="Right" />
                            <HeaderStyle Width="90" />
                        </asp:BoundField>
                        <asp:BoundField DataField="TotalUsageCharge" HeaderText="Total Tool Cost" ItemStyle-HorizontalAlign="Center" HtmlEncode="false" DataFormatString="{0:$#,##0.00}">
                            <ItemStyle HorizontalAlign="Right" />
                            <HeaderStyle Width="90" />
                        </asp:BoundField>
                    </Columns>
                    <EmptyDataTemplate>
                        <div class="empty-data">-- There is no record of tool usage for this period --</div>
                    </EmptyDataTemplate>
                </asp:GridView>
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
                        <asp:BoundField DataField="TotalChargeNoMisc" HeaderText="Store Cost" ItemStyle-HorizontalAlign="Center" HtmlEncode="false" DataFormatString="{0:$#,##0.00}">
                            <ItemStyle HorizontalAlign="Right" />
                            <HeaderStyle Width="90" />
                        </asp:BoundField>
                        <asp:BoundField DataField="StoreMisc" HeaderText="Misc Charge" ItemStyle-HorizontalAlign="Center" HtmlEncode="false" DataFormatString="{0:$#,##0.00}">
                            <ItemStyle HorizontalAlign="Right" />
                            <HeaderStyle Width="90" />
                        </asp:BoundField>
                        <asp:BoundField DataField="TotalCharge" HeaderText="Total Store Cost" ItemStyle-HorizontalAlign="Center" HtmlEncode="false" DataFormatString="{0:$#,##0.00}" Visible="true">
                            <ItemStyle HorizontalAlign="Right" />
                            <HeaderStyle Width="90" />
                        </asp:BoundField>
                    </Columns>
                    <EmptyDataTemplate>
                        <div class="empty-data">-- There is no record of store usage for this period --</div>
                    </EmptyDataTemplate>
                </asp:GridView>
                <br />
                Subsidy (room and tool charges only):<br />
                <asp:GridView runat="server" ID="gvSubsidy" CssClass="gridview" GridLines="None" AutoGenerateColumns="false" AllowSorting="true">
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
            </div>
            <div id="divAggByAcc" class="section">
                <h4 id="h4AggByAcc">Aggregate by Accounts</h4>
                Room:<br />
                <asp:GridView runat="server" ID="gvRoomAccount" CssClass="gridview" GridLines="None" AutoGenerateColumns="false" AllowSorting="true">
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
                        <asp:BoundField DataField="BillingTypeName" HeaderText="Billing Type" ItemStyle-HorizontalAlign="Center">
                            <HeaderStyle Width="125" />
                        </asp:BoundField>
                        <asp:BoundField DataField="RoomCharge" HeaderText="Room Charge" ItemStyle-HorizontalAlign="Center" HtmlEncode="false" DataFormatString="{0:$#,##0.00}" Visible="true">
                            <ItemStyle HorizontalAlign="Right" />
                            <HeaderStyle Width="90" />
                        </asp:BoundField>
                        <asp:BoundField DataField="EntryCharge" HeaderText="Entry Charge" ItemStyle-HorizontalAlign="Center" HtmlEncode="false" DataFormatString="{0:$#,##0.00}" Visible="true">
                            <ItemStyle HorizontalAlign="Right" />
                            <HeaderStyle Width="90" />
                        </asp:BoundField>
                        <asp:BoundField DataField="RoomMisc" HeaderText="Misc Charge" ItemStyle-HorizontalAlign="Center" HtmlEncode="false" DataFormatString="{0:$#,##0.00}" Visible="true">
                            <ItemStyle HorizontalAlign="Right" />
                            <HeaderStyle Width="90" />
                        </asp:BoundField>
                        <asp:BoundField DataField="TotalChargePreSubsidy" HeaderText="Total (w/o Subsidy)" ItemStyle-HorizontalAlign="Center" HtmlEncode="false" DataFormatString="{0:$#,##0.00}" Visible="true">
                            <ItemStyle HorizontalAlign="Right" />
                            <HeaderStyle Width="90" />
                        </asp:BoundField>
                        <asp:BoundField DataField="SubsidyDiscount" HeaderText="Subsidy" ItemStyle-HorizontalAlign="Center" HtmlEncode="false" DataFormatString="{0:$#,##0.00}" Visible="true">
                            <ItemStyle HorizontalAlign="Right" />
                            <HeaderStyle Width="90" />
                        </asp:BoundField>
                        <asp:BoundField DataField="TotalCharge" HeaderText="Line Total" ItemStyle-HorizontalAlign="Center" HtmlEncode="false" DataFormatString="{0:$#,##0.00}" Visible="true">
                            <ItemStyle HorizontalAlign="Right" />
                            <HeaderStyle Width="90" />
                        </asp:BoundField>
                    </Columns>
                    <EmptyDataTemplate>
                        <div class="empty-data">-- There is no record of room usage for this period --</div>
                    </EmptyDataTemplate>
                </asp:GridView>
                <br />
                Tool:<br />
                <asp:GridView runat="server" ID="gvToolAccount" CssClass="gridview" GridLines="None" AllowSorting="true" AutoGenerateColumns="false">
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
                        <asp:BoundField DataField="BillingTypeName" HeaderText="Billing Type" ItemStyle-HorizontalAlign="Center">
                            <HeaderStyle Width="125" />
                        </asp:BoundField>
                        <asp:BoundField DataField="UsageFeeCharged" HeaderText="Usage Charge" ItemStyle-HorizontalAlign="Center" HtmlEncode="false" DataFormatString="{0:$#,##0.00}" Visible="true">
                            <ItemStyle HorizontalAlign="Right" />
                            <HeaderStyle Width="90" />
                        </asp:BoundField>
                        <asp:BoundField DataField="OverTimePenaltyFee" HeaderText="Overtime Charge" ItemStyle-HorizontalAlign="Center" HtmlEncode="false" DataFormatString="{0:$#,##0.00}" Visible="true">
                            <ItemStyle HorizontalAlign="Right" />
                            <HeaderStyle Width="90" />
                        </asp:BoundField>
                        <asp:BoundField DataField="UncancelledPenaltyFee" HeaderText="Uncancelled Charge" ItemStyle-HorizontalAlign="Center" HtmlEncode="false" DataFormatString="{0:$#,##0.00}" Visible="true">
                            <ItemStyle HorizontalAlign="Right" />
                            <HeaderStyle Width="90" />
                        </asp:BoundField>
                        <asp:BoundField DataField="ReservationFee" HeaderText="Reservation Charge" ItemStyle-HorizontalAlign="Center" HtmlEncode="false" DataFormatString="{0:$#,##0.00}" Visible="true">
                            <ItemStyle HorizontalAlign="Right" />
                            <HeaderStyle Width="90" />
                        </asp:BoundField>
                        <asp:BoundField DataField="ToolMisc" HeaderText="Misc Charge" ItemStyle-HorizontalAlign="Center" HtmlEncode="false" DataFormatString="{0:$#,##0.00}" Visible="true">
                            <ItemStyle HorizontalAlign="Right" />
                            <HeaderStyle Width="90" />
                        </asp:BoundField>
                        <asp:BoundField DataField="TotalChargePreSubsidy" HeaderText="Total (w/o Subsidy)" ItemStyle-HorizontalAlign="Center" HtmlEncode="false" DataFormatString="{0:$#,##0.00}" Visible="true">
                            <ItemStyle HorizontalAlign="Right" />
                            <HeaderStyle Width="90" />
                        </asp:BoundField>
                        <asp:BoundField DataField="SubsidyDiscount" HeaderText="Subsidy" ItemStyle-HorizontalAlign="Center" HtmlEncode="false" DataFormatString="{0:$#,##0.00}" Visible="true">
                            <ItemStyle HorizontalAlign="Right" />
                            <HeaderStyle Width="90" />
                        </asp:BoundField>
                        <asp:BoundField DataField="TotalCharge" HeaderText="Line Total" ItemStyle-HorizontalAlign="Center" HtmlEncode="false" DataFormatString="{0:$#,##0.00}" Visible="true">
                            <ItemStyle HorizontalAlign="Right" />
                            <HeaderStyle Width="90" />
                        </asp:BoundField>
                    </Columns>
                    <EmptyDataTemplate>
                        <div class="empty-data">-- There is no record of tool usage for this period --</div>
                    </EmptyDataTemplate>
                </asp:GridView>
                <asp:Literal runat="server" ID="litToolAccountDebug"></asp:Literal>
                <asp:GridView runat="server" ID="gvToolAccount20110401" CssClass="gridview" GridLines="None" AllowSorting="true" AutoGenerateColumns="false">
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
                        <asp:BoundField DataField="BillingTypeName" HeaderText="Billing Type" ItemStyle-HorizontalAlign="Center">
                            <HeaderStyle Width="125" />
                        </asp:BoundField>
                        <asp:BoundField DataField="UsageFeeCharged" HeaderText="Usage Charge" ItemStyle-HorizontalAlign="Center" HtmlEncode="false" DataFormatString="{0:$#,##0.00}" Visible="true">
                            <ItemStyle HorizontalAlign="Right" />
                            <HeaderStyle Width="90" />
                        </asp:BoundField>
                        <asp:BoundField DataField="OverTimePenaltyFee" HeaderText="Overtime Charge" ItemStyle-HorizontalAlign="Center" HtmlEncode="false" DataFormatString="{0:$#,##0.00}" Visible="true">
                            <ItemStyle HorizontalAlign="Right" />
                            <HeaderStyle Width="90" />
                        </asp:BoundField>
                        <asp:BoundField DataField="BookingFee" HeaderText="Booking Fee" ItemStyle-HorizontalAlign="Center" HtmlEncode="false" DataFormatString="{0:$#,##0.00}" Visible="true">
                            <ItemStyle HorizontalAlign="Right" />
                            <HeaderStyle Width="90" />
                        </asp:BoundField>
                        <asp:BoundField DataField="ToolMisc" HeaderText="Misc Charge" ItemStyle-HorizontalAlign="Center" HtmlEncode="false" DataFormatString="{0:$#,##0.00}" Visible="true">
                            <ItemStyle HorizontalAlign="Right" />
                            <HeaderStyle Width="90" />
                        </asp:BoundField>
                        <asp:BoundField DataField="TotalChargePreSubsidy" HeaderText="Total (w/o Subsidy)" ItemStyle-HorizontalAlign="Center" HtmlEncode="false" DataFormatString="{0:$#,##0.00}" Visible="true">
                            <ItemStyle HorizontalAlign="Right" />
                            <HeaderStyle Width="90" />
                        </asp:BoundField>
                        <asp:BoundField DataField="SubsidyDiscount" HeaderText="Subsidy" ItemStyle-HorizontalAlign="Center" HtmlEncode="false" DataFormatString="{0:$#,##0.00}" Visible="true">
                            <ItemStyle HorizontalAlign="Right" />
                            <HeaderStyle Width="90" />
                        </asp:BoundField>
                        <asp:BoundField DataField="TotalCharge" HeaderText="Line Total" ItemStyle-HorizontalAlign="Center" HtmlEncode="false" DataFormatString="{0:$#,##0.00}" Visible="true">
                            <ItemStyle HorizontalAlign="Right" />
                            <HeaderStyle Width="90" />
                        </asp:BoundField>
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
        <div id="divDetail" class="section">
            <h4 id="h4BillingDetail">Billing Details</h4>
            <h5>
                <asp:Label ID="lblRoom" runat="server"></asp:Label>
            </h5>
            <div class="lab-usage" style="display: inline-block;">
                <asp:GridView runat="server" ID="gvRoom" CssClass="gridview" GridLines="None" AutoGenerateColumns="False">
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
                        <asp:BoundField DataField="Hours" HeaderText="Duration (hours)" ItemStyle-HorizontalAlign="Center" HtmlEncode="false" DataFormatString="{0:F4}">
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
                        <asp:BoundField DataField="BillingTypeName" HeaderText="Billing Type" ItemStyle-HorizontalAlign="Center"></asp:BoundField>
                        <asp:BoundField DataField="OrgName" Visible="false" HeaderText="Org" ItemStyle-HorizontalAlign="Center"></asp:BoundField>
                        <asp:BoundField DataField="LineCost" DataFormatString="{0:c2}" HeaderText="Line Total" HtmlEncode="false" ItemStyle-HorizontalAlign="Center"></asp:BoundField>
                    </Columns>
                </asp:GridView>
                <div style="text-align: right; padding-right: 5px; font-size: small; color: Maroon;">
                    <asp:Label ID="lblRoomsSum" CssClass="NormalText" runat="server" Visible="false" />
                </div>
            </div>
            <h5>
                <asp:Label ID="lblTool" runat="server"></asp:Label>
            </h5>
            <div runat="server" id="divTool" style="margin-left: 0px;">
                <h5 style="margin-left: 20px;">Activated Reservations</h5>
                <div class="activated-reservations" style="display: inline-block;">
                    <asp:GridView runat="server" ID="gvTool" CssClass="gridview" GridLines="None" AutoGenerateColumns="false">
                        <HeaderStyle CssClass="header" />
                        <RowStyle CssClass="row" />
                        <AlternatingRowStyle CssClass="altrow" />
                        <SelectedRowStyle CssClass="selected" />
                        <FooterStyle CssClass="footer" />
                        <PagerStyle CssClass="pager" />
                        <PagerSettings Mode="NumericFirstLast" FirstPageText="First" LastPageText="Last" />
                        <Columns>
                            <asp:BoundField DataField="ResourceName" HeaderText="Tool" HeaderStyle-Width="220" ItemStyle-HorizontalAlign="Center" />
                            <asp:BoundField DataField="Room" HeaderText="Lab" HeaderStyle-Width="100" ItemStyle-HorizontalAlign="Center" />
                            <asp:BoundField DataField="TotalUses" HeaderText="Uses" HtmlEncode="false" DataFormatString="{0:F2}" HeaderStyle-Width="60" ItemStyle-HorizontalAlign="Center" />
                            <asp:BoundField DataField="TotalActDuration" HeaderText="Duration (hours)" HtmlEncode="false" DataFormatString="{0:F2}" HeaderStyle-Width="80" ItemStyle-HorizontalAlign="Center" />
                            <asp:BoundField DataField="TotalOverTime" HeaderText="Overtime (hours)" HtmlEncode="false" DataFormatString="{0:F2}" HeaderStyle-Width="80" ItemStyle-HorizontalAlign="Center" />
                            <asp:BoundField DataField="Name" HeaderText="Account" HeaderStyle-Width="250" ItemStyle-HorizontalAlign="Center" />
                            <asp:BoundField DataField="ShortCode" HeaderText="Short Code" HeaderStyle-Width="70" ItemStyle-HorizontalAlign="Center" />
                            <asp:BoundField DataField="OrgName" Visible="false" HeaderText="Org" ItemStyle-HorizontalAlign="Center"></asp:BoundField>
                            <asp:BoundField DataField="LineCost" HeaderText="Line Total" HtmlEncode="false" DataFormatString="{0:C}" ItemStyle-HorizontalAlign="Center" />
                        </Columns>
                        <EmptyDataTemplate>
                            <div class="empty-data">-- There is no record of activated reservation for this period --</div>
                        </EmptyDataTemplate>
                    </asp:GridView>
                    <div style="text-align: right; padding-right: 5px; font-size: small; color: Maroon;">
                        <asp:Label ID="lblRoomsSumActivated" CssClass="NormalText" runat="server"></asp:Label>
                        <asp:Label ID="lblActivatedToolFee" CssClass="NormalText" runat="server"></asp:Label>
                    </div>
                </div>
                <h5 style="margin-left: 20px;">Uncancelled Reservations</h5>
                <div class="uncancelled-reservations" style="display: inline-block;">
                    <asp:GridView runat="server" ID="gvToolCancelled" CssClass="gridview" GridLines="None" AutoGenerateColumns="False">
                        <HeaderStyle CssClass="header" />
                        <RowStyle CssClass="row" />
                        <AlternatingRowStyle CssClass="altrow" />
                        <SelectedRowStyle CssClass="selected" />
                        <FooterStyle CssClass="footer" />
                        <PagerStyle CssClass="pager" />
                        <PagerSettings Mode="NumericFirstLast" FirstPageText="First" LastPageText="Last" />
                        <Columns>
                            <asp:BoundField DataField="ResourceName" HeaderText="Tool" HeaderStyle-Width="220" ItemStyle-HorizontalAlign="Center" />
                            <asp:BoundField DataField="Room" HeaderText="Lab" HeaderStyle-Width="100" ItemStyle-HorizontalAlign="Center" />
                            <asp:BoundField DataField="TotalUses" HeaderText="Uses" DataFormatString="{0:F2}" HtmlEncode="false" HeaderStyle-Width="60" ItemStyle-HorizontalAlign="Center" />
                            <asp:BoundField DataField="TotalSchedDuration" HeaderText="Duration (hours)" DataFormatString="{0:F2}" HtmlEncode="false" HeaderStyle-Width="80" ItemStyle-HorizontalAlign="Center" />
                            <asp:BoundField DataField="Name" HeaderText="Account" HeaderStyle-Width="250" ItemStyle-HorizontalAlign="Center" />
                            <asp:BoundField DataField="ShortCode" HeaderText="Short Code" HeaderStyle-Width="70" ItemStyle-HorizontalAlign="Center" />
                            <asp:BoundField DataField="OrgName" Visible="false" HeaderText="Org" ItemStyle-HorizontalAlign="Center"></asp:BoundField>
                            <asp:BoundField DataField="LineCost" HeaderText="Line Total" DataFormatString="{0:C}" HtmlEncode="false" ItemStyle-HorizontalAlign="Center" />
                        </Columns>
                        <EmptyDataTemplate>
                            <div class="empty-data">-- There is no record of uncancelled reservation for this period --</div>
                        </EmptyDataTemplate>
                    </asp:GridView>
                    <div style="text-align: right; padding-right: 5px; font-size: small; color: Maroon;">
                        <asp:Label ID="lblRoomSumUnCancelled" CssClass="NormalText" Visible="false" runat="server"></asp:Label>
                        <asp:Label ID="lblCancelledToolFee" runat="server"></asp:Label>
                    </div>
                </div>
                <h5 style="margin-left: 20px;">Forgiven Reservations</h5>
                <div class="forgiven-reservations" style="display: inline-block;">
                    <asp:GridView runat="server" ID="gvToolForgiven" CssClass="gridview" GridLines="None" AutoGenerateColumns="False">
                        <HeaderStyle CssClass="header" />
                        <RowStyle CssClass="row" />
                        <AlternatingRowStyle CssClass="altrow" />
                        <SelectedRowStyle CssClass="selected" />
                        <FooterStyle CssClass="footer" />
                        <PagerStyle CssClass="pager" />
                        <PagerSettings Mode="NumericFirstLast" FirstPageText="First" LastPageText="Last" />
                        <Columns>
                            <asp:BoundField DataField="ResourceName" HeaderText="Tool" HeaderStyle-Width="220" ItemStyle-HorizontalAlign="Center" />
                            <asp:BoundField DataField="Room" HeaderText="Lab" HeaderStyle-Width="100" ItemStyle-HorizontalAlign="Center" />
                            <asp:BoundField DataField="TotalUses" HeaderText="Uses" DataFormatString="{0:F2}" HtmlEncode="false" HeaderStyle-Width="60" ItemStyle-HorizontalAlign="Center" />
                            <asp:BoundField DataField="TotalActDuration" HeaderText="Duration (hours)" DataFormatString="{0:F2}" HtmlEncode="false" HeaderStyle-Width="80" ItemStyle-HorizontalAlign="Center" />
                            <asp:BoundField DataField="Name" HeaderText="Account" HeaderStyle-Width="250" ItemStyle-HorizontalAlign="Center" />
                            <asp:BoundField DataField="ShortCode" HeaderText="Short Code" HeaderStyle-Width="70" ItemStyle-HorizontalAlign="Center" />
                            <asp:BoundField DataField="OrgName" Visible="false" HeaderText="Org" ItemStyle-HorizontalAlign="Center"></asp:BoundField>
                            <asp:BoundField DataField="LineCost" HeaderText="Line Total" DataFormatString="{0:C}" HtmlEncode="false" ItemStyle-HorizontalAlign="Center" />
                        </Columns>
                        <EmptyDataTemplate>
                            <div class="empty-data">-- There is no record of forgiven reservation for this period --</div>
                        </EmptyDataTemplate>
                    </asp:GridView>
                    <div style="text-align: right; padding-right: 5px; font-size: small; color: Maroon;">
                        <asp:Label ID="lblForgivenToolFee" runat="server" Visible="false"></asp:Label>
                    </div>
                </div>
            </div>
            <div id="divTool20110401" runat="server" style="margin-left: 0px;">
                <h5 style="margin-left: 20px;">Reservations</h5>
                <div class="activated-reservations" style="display: inline-block;">
                    <asp:GridView runat="server" ID="gvTool20110401" CssClass="gridview" GridLines="None" AutoGenerateColumns="false">
                        <HeaderStyle CssClass="header" />
                        <RowStyle CssClass="row" />
                        <AlternatingRowStyle CssClass="altrow" />
                        <SelectedRowStyle CssClass="selected" />
                        <FooterStyle CssClass="footer" />
                        <PagerStyle CssClass="pager" />
                        <PagerSettings Mode="NumericFirstLast" FirstPageText="First" LastPageText="Last" />
                        <Columns>
                            <asp:BoundField DataField="ResourceName" HeaderText="Tool" HeaderStyle-Width="220" ItemStyle-HorizontalAlign="Center" />
                            <asp:BoundField DataField="Room" HeaderText="Lab" HeaderStyle-Width="100" ItemStyle-HorizontalAlign="Center" />
                            <asp:BoundField DataField="TotalChargeDuration" HeaderText="Duration (hours)" HtmlEncode="false" DataFormatString="{0:F2}" HeaderStyle-Width="60" ItemStyle-HorizontalAlign="Center" />
                            <asp:BoundField DataField="TotalTransferredDuration" HeaderText="Transferred (hours)" HtmlEncode="false" DataFormatString="{0:F2}" HeaderStyle-Width="60" ItemStyle-HorizontalAlign="Center" />
                            <asp:BoundField DataField="TotalActDuration" Visible="false" HeaderText="Duration (hours)" HtmlEncode="false" DataFormatString="{0:F2}" HeaderStyle-Width="80" ItemStyle-HorizontalAlign="Center" />
                            <asp:BoundField DataField="TotalOverTime" HeaderText="Overtime (hours)" HtmlEncode="false" DataFormatString="{0:F2}" HeaderStyle-Width="80" ItemStyle-HorizontalAlign="Center" />
                            <asp:BoundField DataField="Name" HeaderText="Account" HeaderStyle-Width="250" ItemStyle-HorizontalAlign="Center" />
                            <asp:BoundField DataField="ShortCode" HeaderText="Short Code" HeaderStyle-Width="70" ItemStyle-HorizontalAlign="Center" />
                            <asp:BoundField DataField="OrgName" Visible="false" HeaderText="Org" ItemStyle-HorizontalAlign="Center"></asp:BoundField>
                            <asp:BoundField DataField="LineCost" HeaderText="Line Total" HtmlEncode="false" DataFormatString="{0:C}" ItemStyle-HorizontalAlign="Center" />
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
                <h5 style="margin-left: 20px;">Cancelled Reservations</h5>
                <div class="cancelled-reservations" style="display: inline-block;">
                    <asp:GridView runat="server" ID="gvToolCancelled20110401" CssClass="gridview" GridLines="None" AutoGenerateColumns="False">
                        <HeaderStyle CssClass="header" />
                        <RowStyle CssClass="row" />
                        <AlternatingRowStyle CssClass="altrow" />
                        <SelectedRowStyle CssClass="selected" />
                        <FooterStyle CssClass="footer" />
                        <PagerStyle CssClass="pager" />
                        <PagerSettings Mode="NumericFirstLast" FirstPageText="First" LastPageText="Last" />
                        <Columns>
                            <asp:BoundField DataField="ResourceName" HeaderText="Tool" HeaderStyle-Width="220" ItemStyle-HorizontalAlign="Center" />
                            <asp:BoundField DataField="Room" HeaderText="Lab" HeaderStyle-Width="100" ItemStyle-HorizontalAlign="Center" />
                            <asp:BoundField DataField="TotalSchedDuration" HeaderText="Duration (hours)" DataFormatString="{0:F2}" HtmlEncode="false" HeaderStyle-Width="80" ItemStyle-HorizontalAlign="Center" />
                            <asp:BoundField DataField="Name" HeaderText="Account" HeaderStyle-Width="250" ItemStyle-HorizontalAlign="Center" />
                            <asp:BoundField DataField="ShortCode" HeaderText="Short Code" HeaderStyle-Width="70" ItemStyle-HorizontalAlign="Center" />
                            <asp:BoundField DataField="OrgName" Visible="false" HeaderText="Org" ItemStyle-HorizontalAlign="Center"></asp:BoundField>
                            <asp:BoundField DataField="LineCost" HeaderText="Line Total" DataFormatString="{0:C}" HtmlEncode="false" ItemStyle-HorizontalAlign="Center" />
                        </Columns>
                        <EmptyDataTemplate>
                            <div class="empty-data">-- There is no record of uncancelled reservation for this period --</div>
                        </EmptyDataTemplate>
                    </asp:GridView>
                    <div style="text-align: right; padding-right: 5px; font-size: small; color: Maroon;">
                        <asp:Label ID="lbl20110401RoomSumCan" CssClass="NormalText" Visible="false" runat="server"></asp:Label>
                        <asp:Label ID="lbl20110401CanFee" runat="server"></asp:Label>
                    </div>
                </div>
                <h5 style="margin-left: 20px;">Forgiven Reservations</h5>
                <div class="cancelled-reservations" style="display: inline-block;">
                    <asp:GridView runat="server" ID="gvToolForgiven20110401" CssClass="gridview" GridLines="None" AutoGenerateColumns="False">
                        <HeaderStyle CssClass="header" />
                        <RowStyle CssClass="row" />
                        <AlternatingRowStyle CssClass="altrow" />
                        <SelectedRowStyle CssClass="selected" />
                        <FooterStyle CssClass="footer" />
                        <PagerStyle CssClass="pager" />
                        <PagerSettings Mode="NumericFirstLast" FirstPageText="First" LastPageText="Last" />
                        <Columns>
                            <asp:BoundField DataField="ResourceName" HeaderText="Tool" HeaderStyle-Width="220" ItemStyle-HorizontalAlign="Center" />
                            <asp:BoundField DataField="Room" HeaderText="Lab" HeaderStyle-Width="100" ItemStyle-HorizontalAlign="Center" />
                            <asp:BoundField DataField="TotalUses" HeaderText="Uses" DataFormatString="{0:F2}" HtmlEncode="false" HeaderStyle-Width="60" ItemStyle-HorizontalAlign="Center" />
                            <asp:BoundField DataField="TotalActDuration" HeaderText="Duration (hours)" DataFormatString="{0:F2}" HtmlEncode="false" HeaderStyle-Width="80" ItemStyle-HorizontalAlign="Center" />
                            <asp:BoundField DataField="Name" HeaderText="Account" HeaderStyle-Width="250" ItemStyle-HorizontalAlign="Center" />
                            <asp:BoundField DataField="ShortCode" HeaderText="Short Code" HeaderStyle-Width="70" ItemStyle-HorizontalAlign="Center" />
                            <asp:BoundField DataField="OrgName" Visible="false" HeaderText="Org" ItemStyle-HorizontalAlign="Center"></asp:BoundField>
                            <asp:BoundField DataField="LineCost" HeaderText="Line Total" DataFormatString="{0:C}" HtmlEncode="false" ItemStyle-HorizontalAlign="Center" />
                        </Columns>
                        <EmptyDataTemplate>
                            <div class="empty-data">-- There is no record of forgiven reservation for this period --</div>
                        </EmptyDataTemplate>
                    </asp:GridView>
                    <div style="text-align: right; padding-right: 5px; font-size: small; color: Maroon;">
                        <asp:Label ID="lbl20110401Forgiven" runat="server" Visible="false"></asp:Label>
                    </div>
                </div>
            </div>
            <h5>
                <asp:Label ID="lblStore" runat="server"></asp:Label>
            </h5>
            <div class="store-usage" style="display: inline-block;">
                <asp:GridView runat="server" ID="gvStore" CssClass="gridview" GridLines="None" AutoGenerateColumns="False">
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
                        <asp:BoundField DataField="OrgName" Visible="false" HeaderText="Org" ItemStyle-HorizontalAlign="Center"></asp:BoundField>
                        <asp:BoundField DataField="LineCost" HeaderText="Line Cost" ItemStyle-HorizontalAlign="Center" DataFormatString="{0:$#,##0.00}" HtmlEncode="false">
                            <HeaderStyle Width="90" />
                        </asp:BoundField>
                    </Columns>
                </asp:GridView>
            </div>
            <h5>Miscellaneous charges</h5>
            <div class="misc-usage" style="display: inline-block;">
                <asp:GridView runat="server" ID="gvMisc" CssClass="gridview" GridLines="None" AllowSorting="true" AutoGenerateColumns="false">
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
</asp:Content>
