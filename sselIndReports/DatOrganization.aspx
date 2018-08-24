﻿<%@ Page Title="" Language="C#" MasterPageFile="~/IndReportsMaster.Master" AutoEventWireup="true" CodeBehind="DatOrganization.aspx.cs" Inherits="sselIndReports.DatOrganization" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .debug {
            color: #bbb;
            margin-top: 20px;
            font-size: 10pt;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="section">
        <h2>Organization Report</h2>
        <div class="criteria">
            <table class="criteria-table">
                <tr>
                    <td>Select period:</td>
                    <td>
                        <lnf:PeriodPicker runat="server" ID="pp1" />
                    </td>
                </tr>
                <tr>
                    <td>Select organization:</td>
                    <td>
                        <asp:DropDownList runat="server" ID="ddlOrg" CssClass="report-select">
                            <asp:ListItem Text="" Value="0" Selected="true"></asp:ListItem>
                        </asp:DropDownList>
                    </td>
                </tr>
            </table>
            <div class="criteria-item">
                <asp:Button ID="btnReport" runat="server" Text="Retrieve Data" OnClick="BtnReport_Click" CssClass="report-button" />
                <asp:LinkButton runat="server" ID="btnBack" Text="&larr; Back to Main Page" OnClick="BackButton_Click"></asp:LinkButton>
            </div>
        </div>
    </div>
    <div class="section">
        <asp:GridView runat="server" ID="gvReport" CssClass="gridview highlight" GridLines="None" AutoGenerateColumns="false" AllowSorting="True" OnRowDataBound="GvReport_RowDataBound">
            <HeaderStyle CssClass="header" />
            <RowStyle CssClass="row" />
            <AlternatingRowStyle CssClass="altrow" />
            <SelectedRowStyle CssClass="selected" />
            <FooterStyle CssClass="footer" />
            <PagerStyle CssClass="pager" />
            <PagerSettings Mode="NumericFirstLast" FirstPageText="First" LastPageText="Last" />
            <Columns>
                <asp:BoundField DataField="Name" HeaderText="Account Name" ReadOnly="True" SortExpression="Name" />
                <asp:BoundField DataField="ShortCode" HeaderText="Short Code" ReadOnly="True" SortExpression="ShortCode" />
                <asp:BoundField DataField="Project Number" HeaderText="Project/Grant" ReadOnly="True" SortExpression="Project Number" />
                <asp:BoundField DataField="DisplayName" HeaderText="Client Name" ReadOnly="True" SortExpression="DisplayName" />
                <asp:BoundField DataField="Phone" HeaderText="Phone" ReadOnly="True" />
                <asp:BoundField DataField="Email" HeaderText="Email" ReadOnly="True" />
                <asp:BoundField DataField="IsManager" HeaderText="Is Manager" ReadOnly="True" SortExpression="IsManager" />
                <asp:BoundField DataField="IsFinManager" HeaderText="Is Fin Manager" ReadOnly="True" SortExpression="IsFinManager" />
                <asp:BoundField DataField="FundingSourceName" HeaderText="Funding Source" ReadOnly="True" SortExpression="FundingSourceName" />
                <asp:BoundField DataField="TechnicalFieldName" HeaderText="Technical Field" ReadOnly="True" SortExpression="TechnicalFieldName" />
            </Columns>
            <EmptyDataTemplate>
                There are no active accounts with this organization.
            </EmptyDataTemplate>
        </asp:GridView>

        <asp:Literal runat="server" ID="litDebug"></asp:Literal>
    </div>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="scripts">
</asp:Content>
