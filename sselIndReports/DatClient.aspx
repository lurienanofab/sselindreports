<%@ Page Title="" Language="C#" MasterPageFile="~/IndReportsMaster.Master" AutoEventWireup="true" CodeBehind="DatClient.aspx.cs" Inherits="sselIndReports.DatClient" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="section">
        <h2>Client Report</h2>
        <div class="criteria">
            <table class="criteria-table">
                <tr>
                    <td>Select month and year:</td>
                    <td class="disable">
                        <lnf:PeriodPicker runat="server" ID="pp1" AutoPostBack="true" OnSelectedPeriodChanged="pp1_SelectedPeriodChanged"/>
                    </td>
                </tr>
                <tr>
                    <td>Select user:</td>
                    <td>
                        <asp:DropDownList runat="server" ID="ddlUser" DataTextField="DisplayName" DataValueField="ClientID" CssClass="report-select"  >
                        </asp:DropDownList>
                    </td>
                </tr>
            </table>
            <div class="criteria-item">
                <asp:Button ID="btnReport" Text="Retrieve Data" runat="server" CssClass="report-button" OnClick="ReportButton_Click" />
                <asp:LinkButton runat="server" ID="btnBack" Text="&larr; Back to Main Page" OnClick="BackButton_Click" CssClass="report-button"></asp:LinkButton>
            </div>
        </div>
    </div>
    <div class="section">
        <table runat="server" id="tblCliRep">
            <tr runat="server" id="trowName">
                <td>
                    <asp:Label runat="server" ID="lblClientName" CssClass="ReportHeader"></asp:Label>
                </td>
            </tr>
            <tr runat="server" id="trowPrivs">
                <td>
                    <asp:Label runat="server" ID="lblPrivs" CssClass="LabelText"></asp:Label>
                </td>
            </tr>
            <tr runat="server" id="trowDem">
                <td>
                    <asp:Label runat="server" ID="lblDem" CssClass="LabelText"></asp:Label>
                </td>
            </tr>
            <tr runat="server" id="trowType">
                <td>
                    <asp:Label runat="server" ID="lblUserType" CssClass="LabelText"></asp:Label>
                </td>
            </tr>
        </table>
    </div>
</asp:Content>
