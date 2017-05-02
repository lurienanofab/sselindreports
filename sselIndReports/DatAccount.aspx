<%@ Page Title="" Language="C#" MasterPageFile="~/IndReportsMaster.Master" AutoEventWireup="true" CodeBehind="DatAccount.aspx.cs" Inherits="sselIndReports.DatAccount" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="section">
        <h2>Account Report</h2>
        <div class="criteria">
            <div class="criteria-item">
                <asp:LinkButton runat="server" ID="btnBack" Text="&larr; Back to Main Page" OnClick="BackButton_Click"></asp:LinkButton>
            </div>
        </div>
    </div>

    <div class="section" style="width: 60%;">
        <div style="border: solid 1px #ccc; padding: 10px; margin-bottom: 20px; border-radius: 4px; background-color: #f5f5f5;">
            This report shows all accounts that were active as of
            <asp:Literal runat="server" ID="litCurrentTime"></asp:Literal>. Both internal and external accounts are included. Accounts with multiple managers will display one row per manager. For external accounts, the ShortCode and Project Number columns are blank.
        </div>

        <div style="margin-bottom: 20px;">
            <asp:HyperLink runat="server" ID="hypExportCSV" NavigateUrl="~/DatAccount.aspx?export=csv">Export CSV</asp:HyperLink>
        </div>

        <asp:Repeater runat="server" ID="rptAccount">
            <HeaderTemplate>
                <table class="gridview highlight" style="border-collapse: collapse; width: 100%;">
                    <tbody>
                        <tr class="header">
                            <th>Account Name</th>
                            <th>ShortCode</th>
                            <th>Project Number</th>
                            <th>Manager</th>
                            <th>Technical Field</th>
                            <th>Funding Source</th>
                        </tr>
            </HeaderTemplate>
            <ItemTemplate>
                <tr class="row">
                    <td><%#Eval("AccountName")%></td>
                    <td><%#Eval("ShortCode")%></td>
                    <td><%#Eval("ProjectNumber")%></td>
                    <td><%#Eval("ManagerDisplayName")%></td>
                    <td><%#Eval("TechnicalFieldName")%></td>
                    <td><%#Eval("FundingSourceName")%></td>
                </tr>
            </ItemTemplate>
            <AlternatingItemTemplate>
                <tr class="altrow">
                    <td><%#Eval("AccountName")%></td>
                    <td><%#Eval("ShortCode")%></td>
                    <td><%#Eval("ProjectNumber")%></td>
                    <td><%#Eval("ManagerDisplayName")%></td>
                    <td><%#Eval("TechnicalFieldName")%></td>
                    <td><%#Eval("FundingSourceName")%></td>
                </tr>
            </AlternatingItemTemplate>
            <FooterTemplate>
                </tbody>
                </table>
            </FooterTemplate>
        </asp:Repeater>
    </div>
</asp:Content>
