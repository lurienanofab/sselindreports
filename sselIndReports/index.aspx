<%@ Page Title="" Language="C#" MasterPageFile="~/IndReportsMaster.Master" AutoEventWireup="true" CodeBehind="index.aspx.cs" Inherits="sselIndReports.Index" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .container {
            display: flex;
            margin-bottom: 10px;
            margin-left: 5px;
        }

            .container > .col {
                margin-right: 10px;
                padding: 10px;
            }

            .container .CommandButton {
                display: block;
                margin-top: 15px;
            }

        .user-info {
            margin: 10px 0 20px 0;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="section">

        <div class="PageHeader">User Reports</div>

        <div class="user-info">
            You are logged in as
            <asp:Label ID="lblName" runat="server"></asp:Label>
        </div>

        <div class="container">
            <div runat="server" id="divInd" visible="false" class="col">
                <div class="ButtonGroupHeader">
                    <asp:Label runat="server" ID="lblInd" Visible="false">Individual User Reports</asp:Label>
                </div>
                <asp:Button runat="server" ID="btnIndDetUsage" CssClass="CommandButton" Visible="false" Text="User Detailed Time in Lab" OnCommand="Button_Command" CommandName="IndDetUsage" CommandArgument="" />
                <asp:Button runat="server" ID="btnIndSumUsage" CssClass="CommandButton" Visible="false" Text="User Usage Summary - June 2009 and Before" OnCommand="Button_Command" CommandName="IndSumUsage" CommandArgument="" />
                <asp:Button runat="server" ID="btnIndUserUsageSummary" CssClass="CommandButton" Visible="false" Text="User Usage Summary" OnCommand="Button_Command" CommandName="IndUserUsageSummary" CommandArgument="" />
                <asp:Button runat="server" ID="btnIndUserUsageSummary20100701" CssClass="CommandButton" Visible="false" Text="User Usage Summary" OnCommand="Button_Command" CommandName="IndUserUsageSummary20100701" CommandArgument="" />
                <asp:Button runat="server" ID="btnIndUserUsageSummary20110401" CssClass="CommandButton" Visible="false" Text="User Usage Summary" OnCommand="Button_Command" CommandName="IndUserUsageSummary20110401" CommandArgument="" />
                <asp:Button runat="server" ID="btnIndUserUsageSummary20111101" CssClass="CommandButton" Visible="false" Text="User Usage Summary (old)" OnCommand="Button_Command" CommandName="IndUserUsageSummary20111101" CommandArgument="" />
                <asp:Button runat="server" ID="btnIndUserUsageSummary20220301" CssClass="CommandButton" Visible="false" Text="User Usage Summary" OnCommand="Button_Command" CommandName="IndUserUsageSummary20220301" CommandArgument="" />
                <asp:Button runat="server" ID="btnIndUserUsageSummaryAudit" CssClass="CommandButton" Visible="false" Text="User Usage Summary Audit" OnCommand="Button_Command" CommandName="IndUserUsageSummaryAudit" CommandArgument="" />
                <asp:Button runat="server" ID="btnIndAuthTools" CssClass="CommandButton" Width="209" Text="User Tool Authorization" Visible="false" OnCommand="Button_Command" CommandName="IndAuthTools" CommandArgument="" />
                <asp:Button runat="server" ID="btnIndClientAccount" CssClass="CommandButton" Visible="false" Text="Account & Client Report" OnCommand="Button_Command" CommandName="IndClientAccount" CommandArgument="" />
            </div>
            <div runat="server" id="divAgg" visible="false" class="col">
                <div class="ButtonGroupHeader">
                    <asp:Label runat="server" ID="lblAgg" Visible="false">Aggregate User Reports</asp:Label>
                </div>
                <asp:Button runat="server" ID="btnAggSumUsage" CssClass="CommandButton" Visible="false" Text="Lab Usage Summary" OnCommand="Button_Command" CommandName="AggSumUsage" CommandArgument="" />
                <asp:Button runat="server" ID="btnAggDemographic" CssClass="CommandButton" Visible="false" Text="Demographic Usage" OnCommand="Button_Command" CommandName="AggDemographic" CommandArgument="" />
                <asp:Button runat="server" ID="btnAggNNIN" CssClass="CommandButton" Visible="false" Text="NNIN Report" OnCommand="Button_Command" CommandName="AggNNIN" CommandArgument="" />
                <asp:Button runat="server" ID="btnAggNNIN2" CssClass="CommandButton" Visible="false" Text="NNIN Report 2" OnCommand="Button_Command" CommandName="AggNNIN2" CommandArgument="" />
                <asp:Button runat="server" ID="btnAggFeeComparison" CssClass="CommandButton" Visible="false" Text="Fee Comparison" OnCommand="Button_Command" CommandName="AggFeeComparison" CommandArgument="" />
                <asp:Button runat="server" ID="btnAggSubsidyByFacultyGroup" CssClass="CommandButton" Visible="false" Text="Subsidy by Faculty" OnCommand="Button_Command" CommandName="AggSubsidyByFacultyGroup" CommandArgument="" />
            </div>
            <div runat="server" id="divDat" visible="false" class="col">
                <div class="ButtonGroupHeader">
                    <asp:Label runat="server" ID="lblDat" Visible="false">Database Reports</asp:Label>
                </div>
                <asp:Button runat="server" ID="btnDatClient" CssClass="CommandButton" Visible="false" Text="Client Database Report" OnCommand="Button_Command" CommandName="DatClient" CommandArgument="" />
                <asp:Button runat="server" ID="btnDatAccount" CssClass="CommandButton" Visible="false" Text="Account Database Report" OnCommand="Button_Command" CommandName="DatAccount" CommandArgument="" />
                <asp:Button runat="server" ID="btnDatOrganization" CssClass="CommandButton" Visible="false" Text="Organization Database Report" OnCommand="Button_Command" CommandName="DatOrganization" CommandArgument="" />
                <asp:Button runat="server" ID="btnDatHistory" CssClass="CommandButton" Visible="false" Text="Historical Database Report" OnCommand="Button_Command" CommandName="DatHistory" CommandArgument="" />
            </div>
        </div>
        <div class="container">
            <div class="col">
                <div class="ButtonGroupHeader">Application Control</div>
                <asp:Button runat="server" ID="btnLogout" CssClass="CommandButton" Text="Exit Application" OnClick="BtnLogout_Click" />
            </div>
        </div>
    </div>
</asp:Content>
