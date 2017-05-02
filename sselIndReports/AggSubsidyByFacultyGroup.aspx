<%@ Page Title="" Language="C#" MasterPageFile="~/IndReportsMaster.Master" AutoEventWireup="true" CodeBehind="AggSubsidyByFacultyGroup.aspx.cs" Inherits="sselIndReports.AggSubsidyByFacultyGroup" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="section">
        <h2>Aggregate Subsidy Report</h2>
        <div class="criteria">
            <table class="criteria-table">
                <tr>
                    <td>Start period:</td>
                    <td>
                        <lnf:PeriodPicker runat="server" ID="ppStart" />
                    </td>
                </tr>
                <tr>
                    <td>End period:</td>
                    <td>
                        <lnf:PeriodPicker runat="server" ID="ppEnd" />
                    </td>
                </tr>
                <tr>
                    <td>Manager:</td>
                    <td>
                        <asp:DropDownList ID="ddlManager" DataTextField="DisplayName" DataValueField="ClientOrgID" runat="server" CssClass="report-select">
                        </asp:DropDownList>
                    </td>
                </tr>
            </table>
            <div class="criteria-item">
                <asp:Button ID="btnReport" runat="server" Text="Retrieve Data" OnClick="btnReport_Click" CssClass="report-button"></asp:Button>
                <asp:LinkButton runat="server" ID="btnBack" Text="&larr; Back to Main Page" OnClick="BackButton_Click" CssClass="back-link"></asp:LinkButton>
            </div>
        </div>
    </div>
    <div class="section">
        <asp:GridView runat="server" ID="gv" CssClass="gridview highlight" GridLines="None" AutoGenerateColumns="false" OnDataBound="gv_DataBound">
            <HeaderStyle CssClass="header" />
            <RowStyle CssClass="row" />
            <AlternatingRowStyle CssClass="altrow" />
            <SelectedRowStyle CssClass="selected" />
            <FooterStyle CssClass="footer" />
            <PagerStyle CssClass="pager" />
            <PagerSettings Mode="NumericFirstLast" FirstPageText="First" LastPageText="Last" />
            <Columns>
                <asp:BoundField DataField="ClientName" HeaderText="Name" ItemStyle-HorizontalAlign="Center">
                    <HeaderStyle Width="125" />
                </asp:BoundField>
                <asp:BoundField DataField="TotalCharge" HeaderText="Total Charge" ItemStyle-HorizontalAlign="Center" HtmlEncode="false" DataFormatString="{0:$#,##0.00}" Visible="true">
                    <ItemStyle HorizontalAlign="Right" />
                    <HeaderStyle Width="90" />
                </asp:BoundField>
                <asp:BoundField DataField="Billed" HeaderText="Billed Amount" ItemStyle-HorizontalAlign="Center" HtmlEncode="false" DataFormatString="{0:$#,##0.00}" Visible="true">
                    <ItemStyle HorizontalAlign="Right" />
                    <HeaderStyle Width="90" />
                </asp:BoundField>
                <asp:BoundField DataField="Subsidy" HeaderText="Subsidy Amount" ItemStyle-HorizontalAlign="Center" HtmlEncode="false" DataFormatString="{0:$#,##0.00}" Visible="true">
                    <ItemStyle HorizontalAlign="Right" />
                    <HeaderStyle Width="90" />
                </asp:BoundField>
                <asp:BoundField DataField="Tier" HeaderText="Tier" ItemStyle-HorizontalAlign="Center" Visible="true" />
                <asp:BoundField DataField="StartDate" HeaderText="Start Date" ItemStyle-HorizontalAlign="Center" HtmlEncode="false" DataFormatString="{0:MM/dd/yyyy}" Visible="true">
                    <HeaderStyle Width="90" />
                </asp:BoundField>
            </Columns>
            <EmptyDataTemplate>
                <div class="empty-data">-- There is no record of subsidy and lab usage --</div>
            </EmptyDataTemplate>
        </asp:GridView>
    </div>
    <div id="divGridDetail">
    </div>
</asp:Content>
