<%@ Page Title="Aggregate User Summary Report" Language="C#" MasterPageFile="~/IndReportsMaster.Master" AutoEventWireup="true" CodeBehind="AggSumUsage.aspx.cs" Inherits="sselIndReports.AggSumUsage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .report-table td {
            padding: 3px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="section">
        <h2>Aggregate User Summary Report</h2>
        <div class="criteria">
            <table class="report-table" border="1">
                <tr>
                    <td>Select period:</td>
                    <td>
                        <lnf:PeriodPicker runat="server" ID="pp1" AutoPostBack="true" OnSelectedPeriodChanged="Pp1_SelectedPeriodChanged" />
                    </td>
                </tr>
                <tr>
                    <td rowspan="2">Users to include in report:</td>
                    <td>
                        <asp:RadioButtonList runat="server" ID="rblUserOrg" Width="295" AutoPostBack="True" RepeatDirection="Horizontal" TextAlign="Left" OnSelectedIndexChanged="RbUserOrg_SelectedIndexChanged">
                            <asp:ListItem Value="1" Selected="True">UMich Only</asp:ListItem>
                            <asp:ListItem Value="0">External Only</asp:ListItem>
                            <asp:ListItem Value="-1">All</asp:ListItem>
                        </asp:RadioButtonList>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:CheckBoxList runat="server" ID="cblPriv" Width="600" AutoPostBack="True" RepeatDirection="Horizontal" TextAlign="Left" RepeatColumns="4" OnSelectedIndexChanged="CblPriv_SelectedIndexChanged">
                        </asp:CheckBoxList>
                    </td>
                </tr>
            </table>
            <div class="criteria-item">
                <asp:LinkButton runat="server" ID="btnBack" Text="&larr; Back to Main Page" OnClick="BackButton_Click" CssClass="back-link"></asp:LinkButton>
            </div>
        </div>
    </div>
    <div class="section">
        <asp:GridView runat="server" ID="dgAccess" CssClass="gridview highlight" GridLines="None" EnableViewState="false" AutoGenerateColumns="false" OnRowDataBound="DgAccess_RowDataBound">
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
