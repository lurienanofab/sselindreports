<%@ Page Title="" Language="C#" MasterPageFile="~/IndReportsMaster.Master" AutoEventWireup="true" CodeBehind="DatOrganization.aspx.cs" Inherits="sselIndReports.DatOrganization" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
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
                <asp:Button ID="btnReport" runat="server" Text="Retrieve Data" OnClick="btnReport_Click" CssClass="report-button" />
                <asp:LinkButton runat="server" ID="btnBack" Text="&larr; Back to Main Page" OnClick="BackButton_Click"></asp:LinkButton>
            </div>
        </div>
    </div>
    <div class="section">
        <asp:GridView runat="server" ID="gv" CssClass="gridview highlight" GridLines="None" AutoGenerateColumns="false" AllowSorting="True" OnRowDataBound="gv_RowDataBound">
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
            </Columns>
            <EmptyDataTemplate>
                There is not active account with this organization
            </EmptyDataTemplate>
        </asp:GridView>
        <asp:ObjectDataSource ID="odsGrid" runat="server" SelectMethod="GetAccountDetailsByOrgID" TypeName="sselIndReports.AppCode.DAL.AccountDA">
            <SelectParameters>
                <asp:ControlParameter ControlID="ddlOrg" DefaultValue="0" Name="OrgID" PropertyName="SelectedValue" Type="Int32" />
                <asp:ControlParameter ControlID="pp1" Name="Year" PropertyName="SelectedYear" Type="Int32" />
                <asp:ControlParameter ControlID="pp1" Name="Month" PropertyName="SelectedMonth" Type="Int32" />
            </SelectParameters>
        </asp:ObjectDataSource>
    </div>
</asp:Content>
