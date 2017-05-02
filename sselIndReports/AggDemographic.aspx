<%@ Page Title="Demographic Report" Language="C#" MasterPageFile="~/IndReportsMaster.Master" AutoEventWireup="true" CodeBehind="AggDemographic.aspx.cs" Inherits="sselIndReports.AggDemographic" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="section">
        <h2>Demographic Report</h2>
        <div class="criteria">
            <div class="criteria-item">
                Select period:
                <lnf:PeriodPicker runat="server" ID="pp1" AutoPostBack="true" OnSelectedPeriodChanged="pp1_SelectedPeriodChanged" />
            </div>
            <div class="criteria-item">
                <asp:LinkButton runat="server" ID="btnBack" Text="&larr; Back to Main Page" OnClick="BackButton_Click" CssClass="back-link"></asp:LinkButton>
            </div>
        </div>
    </div>
    <div class="section">
        <table class="report-table" border="1">
            <tr>
                <th>Gender</th>
            </tr>
            <tr>
                <td>
                    <asp:DataGrid ID="dgGender" OnItemDataBound="dgItemBound" runat="server">
                        <AlternatingItemStyle BackColor="Linen"></AlternatingItemStyle>
                        <HeaderStyle Font-Bold="True" BackColor="#FFC0C0"></HeaderStyle>
                    </asp:DataGrid>
                </td>
            </tr>
            <tr>
                <th>Ethnicity</th>
            </tr>
            <tr>
                <td>
                    <asp:DataGrid ID="dgEthnic" OnItemDataBound="dgItemBound" runat="server">
                        <AlternatingItemStyle BackColor="Linen"></AlternatingItemStyle>
                        <HeaderStyle Font-Bold="True" BackColor="#FFC0C0"></HeaderStyle>
                    </asp:DataGrid>
                </td>
            </tr>
            <tr>
                <th>Race</th>
            </tr>
            <tr>
                <td>
                    <asp:DataGrid ID="dgRace" OnItemDataBound="dgItemBound" runat="server">
                        <AlternatingItemStyle BackColor="Linen"></AlternatingItemStyle>
                        <HeaderStyle Font-Bold="True" BackColor="#FFC0C0"></HeaderStyle>
                    </asp:DataGrid>
                </td>
            </tr>
            <tr>
                <th>Disability</th>
            </tr>
            <tr>
                <td>
                    <asp:DataGrid ID="dgDisability" OnItemDataBound="dgItemBound" runat="server">
                        <AlternatingItemStyle BackColor="Linen"></AlternatingItemStyle>
                        <HeaderStyle Font-Bold="True" BackColor="#FFC0C0"></HeaderStyle>
                    </asp:DataGrid>
                </td>
            </tr>
            <tr>
                <th>Citizen</th>
            </tr>
            <tr>
                <td>
                    <asp:DataGrid ID="dgCitizen" OnItemDataBound="dgItemBound" runat="server">
                        <AlternatingItemStyle BackColor="Linen"></AlternatingItemStyle>
                        <HeaderStyle Font-Bold="True" BackColor="#FFC0C0"></HeaderStyle>
                    </asp:DataGrid>
                </td>
            </tr>
        </table>
    </div>
</asp:Content>
