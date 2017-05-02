<%@ Page Title="Aggregate User Summary Report" Language="C#" MasterPageFile="~/IndReportsMaster.Master" AutoEventWireup="true" CodeBehind="AggNNIN.aspx.cs" Inherits="sselIndReports.AggNNIN" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="section">
        <h2>Aggregate User Summary Report</h2>
        <div class="criteria">
            <table class="criteria-table">
                <tr>
                    <td>Create report for period:</td>
                    <td>
                        <lnf:PeriodPicker runat="server" ID="ppRep" />
                    </td>
                </tr>
                <tr>
                    <td>Start aggregation period:</td>
                    <td>
                        <lnf:PeriodPicker runat="server" ID="ppAgg" AutoPostBack="true" OnSelectedPeriodChanged="ppAgg_SelectedPeriodChanged" />
                    </td>
                </tr>
            </table>
            <div class="criteria-item">
                <asp:Button runat="server" ID="btnReport" Text="Retrieve Data" CommandArgument="0" OnCommand="btnReport_Command" CssClass="report-button"></asp:Button>
                <asp:LinkButton runat="server" ID="btnBack" Text="&larr; Back to Main Page" OnClick="BackButton_Click"></asp:LinkButton>
                <div>
                    <asp:Label runat="server" ID="lblWarning" CssClass="WarningText" Visible="False"></asp:Label>
                </div>
            </div>
        </div>
    </div>
    <div class="section">
        <asp:GridView runat="server" ID="gv1" CssClass="gridview" GridLines="None">
            <HeaderStyle CssClass="header" />
            <RowStyle CssClass="row" />
            <AlternatingRowStyle CssClass="altrow" />
            <SelectedRowStyle CssClass="selected" />
            <FooterStyle CssClass="footer" />
            <PagerStyle CssClass="pager" />
            <PagerSettings Mode="NumericFirstLast" FirstPageText="First" LastPageText="Last" />
        </asp:GridView>
        <asp:GridView runat="server" ID="gv2" CssClass="gridview" GridLines="None">
            <HeaderStyle CssClass="header" />
            <RowStyle CssClass="row" />
            <AlternatingRowStyle CssClass="altrow" />
            <SelectedRowStyle CssClass="selected" />
            <FooterStyle CssClass="footer" />
            <PagerStyle CssClass="pager" />
            <PagerSettings Mode="NumericFirstLast" FirstPageText="First" LastPageText="Last" />
        </asp:GridView>
        <asp:GridView runat="server" ID="gv3" CssClass="gridview" GridLines="None">
            <HeaderStyle CssClass="header" />
            <RowStyle CssClass="row" />
            <AlternatingRowStyle CssClass="altrow" />
            <SelectedRowStyle CssClass="selected" />
            <FooterStyle CssClass="footer" />
            <PagerStyle CssClass="pager" />
            <PagerSettings Mode="NumericFirstLast" FirstPageText="First" LastPageText="Last" />
        </asp:GridView>
    </div>
</asp:Content>
