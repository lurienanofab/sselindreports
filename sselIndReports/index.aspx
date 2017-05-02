<%@ Page Title="" Language="C#" MasterPageFile="~/IndReportsMaster.Master" AutoEventWireup="true" CodeBehind="index.aspx.cs" Inherits="sselIndReports.index" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="section">
        <table class="button-table">
            <tr>
                <td colspan="3" class="PageHeader">User Reports</td>
            </tr>
            <tr>
                <td colspan="3">You are logged in as
                <asp:Label ID="lblName" runat="server"></asp:Label></td>
            </tr>
            <tr>
                <td class="ButtonGroupHeader">
                    <asp:Label runat="server" ID="lblInd" Visible="false">Individual User Reports</asp:Label>
                </td>
                <td class="ButtonGroupHeader">
                    <asp:Label runat="server" ID="lblAgg" Visible="false">Aggregate User Reports</asp:Label>
                </td>
                <td class="ButtonGroupHeader">
                    <asp:Label runat="server" ID="lblDat" Visible="false">Database Reports</asp:Label>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Button runat="server" ID="btnIndDetUsage" CssClass="CommandButton" Visible="false" Text="User Detailed Time in Lab" OnCommand="Button_Command" CommandName="IndDetUsage" CommandArgument="" />
                </td>
                <td>
                    <asp:Button runat="server" ID="btnAggSumUsage" CssClass="CommandButton" Visible="false" Text="Lab Usage Summary" OnCommand="Button_Command" CommandName="AggSumUsage" CommandArgument="" />
                </td>
                <td>
                    <asp:Button runat="server" ID="btnDatClient" CssClass="CommandButton" Visible="false" Text="Client Database Report" OnCommand="Button_Command" CommandName="DatClient" CommandArgument="" />
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Button runat="server" ID="btnIndSumUsage" CssClass="CommandButton" Visible="false" Text="User Usage Summary - June 2009 and Before" OnCommand="Button_Command" CommandName="IndSumUsage" CommandArgument="" />
                    <asp:Button runat="server" ID="btnIndUserUsageSummary" CssClass="CommandButton" Visible="false" Text="User Usage Summary" OnCommand="Button_Command" CommandName="IndUserUsageSummary" CommandArgument="" />
                    <asp:Button runat="server" ID="btnIndUserUsageSummary20100701" CssClass="CommandButton" Visible="false" Text="User Usage Summary" OnCommand="Button_Command" CommandName="IndUserUsageSummary20100701" CommandArgument="" />
                    <asp:Button runat="server" ID="btnIndUserUsageSummary20110401" CssClass="CommandButton" Visible="false" Text="User Usage Summary" OnCommand="Button_Command" CommandName="IndUserUsageSummary20110401" CommandArgument="" />
                    <asp:Button runat="server" ID="btnIndUserUsageSummary20111101" CssClass="CommandButton" Visible="false" Text="User Usage Summary" OnCommand="Button_Command" CommandName="IndUserUsageSummary20111101" CommandArgument="" />
                </td>
                <td>
                    <asp:Button runat="server" ID="btnAggDemographic" CssClass="CommandButton" Visible="false" Text="Demographic Usage" OnCommand="Button_Command" CommandName="AggDemographic" CommandArgument="" />
                </td>
                <td>
                    <asp:Button runat="server" ID="btnDatAccount" CssClass="CommandButton" Visible="false" Text="Account Database Report" OnCommand="Button_Command" CommandName="DatAccount" CommandArgument="" />
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Button runat="server" ID="btnIndAuthTools" CssClass="CommandButton" Width="209" Text="User Tool Authorization" Visible="false" OnCommand="Button_Command" CommandName="IndAuthTools" CommandArgument="" />
                </td>
                <td>
                    <asp:Button runat="server" ID="btnAggNNIN" CssClass="CommandButton" Visible="false" Text="NNIN Report" OnCommand="Button_Command" CommandName="AggNNIN" CommandArgument="" />
                </td>
                <td>
                    <asp:Button runat="server" ID="btnDatOrganization" CssClass="CommandButton" Visible="false" Text="Organization Database Report" OnCommand="Button_Command" CommandName="DatOrganization" CommandArgument="" />
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Button runat="server" ID="btnIndClientAccount" CssClass="CommandButton" Visible="false" Text="Account & Client Report" OnCommand="Button_Command" CommandName="IndClientAccount" CommandArgument="" />
                </td>
                <td>
                    <asp:Button runat="server" ID="btnAggNNIN2" CssClass="CommandButton" Visible="false" Text="NNIN Report 2" OnCommand="Button_Command" CommandName="AggNNIN2" CommandArgument="" />
                </td>
                <td>
                    <asp:Button runat="server" ID="btnDatHistory" CssClass="CommandButton" Visible="false" Text="Historical Database Report" OnCommand="Button_Command" CommandName="navigate" CommandArgument="/data/dispatch/historical-database-report?returnTo=/sselindreports" />
                </td>
            </tr>
            <tr>
                <td>&nbsp;</td>
                <td>
                    <asp:Button runat="server" ID="btnAggFeeComparison" CssClass="CommandButton" Visible="false" Text="Fee Comparison" OnCommand="Button_Command" CommandName="AggFeeComparison" CommandArgument="" />
                </td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td>&nbsp;</td>
                <td>
                    <asp:Button runat="server" ID="btnAggSubsidyByFacultyGroup" CssClass="CommandButton" Visible="false" Text="Subsidy by Faculty" OnCommand="Button_Command" CommandName="AggSubsidyByFacultyGroup" CommandArgument="" />
                </td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td>&nbsp;</td>
                <td>&nbsp;</td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td class="ButtonGroupHeader">Application Control</td>
                <td>&nbsp;</td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td>
                    <asp:Button runat="server" ID="btnLogout" CssClass="CommandButton" Text="Exit Application" OnClick="btnLogout_Click" />
                </td>
                <td>&nbsp;</td>
                <td>&nbsp;</td>
            </tr>
        </table>
    </div>
</asp:Content>
