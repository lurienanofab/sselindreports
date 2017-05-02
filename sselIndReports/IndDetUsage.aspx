<%@ Page Title="" Language="C#" MasterPageFile="~/IndReportsMaster.Master" AutoEventWireup="true" CodeBehind="IndDetUsage.aspx.cs" Inherits="sselIndReports.IndDetUsage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .report-table {
            width: 400px;
        }

        .display-name {
            height: 22px;
            padding-top: 2px;
            padding-left: 4px;
            background-color: #d4d0c8;
            font-weight: bold;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="section">
        <h2>Individual User Detailed Time-in-Lab Report</h2>
        <div class="criteria">
            <table class="criteria-table">
                <tr>
                    <td>Select month and year:</td>
                    <td class="disable">
                        <lnf:PeriodPicker runat="server" ID="pp1" AutoPostBack="true" OnSelectedPeriodChanged="pp1_SelectedPeriodChanged" />
                    </td>
                </tr>
                <tr>
                    <td>Select user:</td>
                    <td>
                        <asp:DropDownList runat="server" ID="ddlUser" CssClass="report-select" AutoPostBack="true"  DataTextField="DisplayName" DataValueField="ClientID" >
                        </asp:DropDownList>
                    </td>
                </tr>
            </table>
            <div class="criteria-item">
                <asp:Button ID="btnReport" Text="Retrieve Data" runat="server" CssClass="report-button" OnClick="ReportButton_Click" />
                <asp:LinkButton runat="server" ID="btnBack" Text="&larr; Back to Main Page" OnClick="BackButton_Click" CssClass="back-link"></asp:LinkButton>
            </div>

            <input type="hidden" runat="server" id="hidSelectedUser" class="selected-user" />

        </div>
    </div>
    <div class="section">
        <table class="report-table" border="1">
            <tr>
                <td style="vertical-align: middle; padding: 0;">
                    <asp:Literal runat="server" ID="litMessage"></asp:Literal>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:DataGrid runat="server" ID="dgActDate" Width="632" AutoGenerateColumns="false" OnItemDataBound="dgActDate_ItemDataBound">
                        <AlternatingItemStyle BackColor="Azure" />
                        <HeaderStyle Font-Bold="true" />
                        <Columns>
                            <asp:BoundColumn DataField="Date" HeaderText="Date" DataFormatString="{0: dd MMM yyyy}">
                                <HeaderStyle Font-Bold="true" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" Width="150" VerticalAlign="Middle" />
                            </asp:BoundColumn>
                            <asp:TemplateColumn HeaderText="Activity Detail">
                                <ItemStyle BackColor="White" />
                                <ItemTemplate>
                                    <asp:DataGrid runat="server" ID="dgActivity" AutoGenerateColumns="false" HeaderStyle-BackColor="#D4D0C8">
                                        <AlternatingItemStyle BackColor="Azure" />
                                        <Columns>
                                            <asp:BoundColumn DataField="ActivityTime" HeaderText="Time" ItemStyle-Width="75" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" HeaderStyle-Font-Italic="true" DataFormatString="{0: dd MMM yyyy}"></asp:BoundColumn>
                                            <asp:BoundColumn DataField="Description" HeaderText="Activity" ItemStyle-Width="450" HeaderStyle-Font-Italic="true"></asp:BoundColumn>
                                        </Columns>
                                    </asp:DataGrid>
                                </ItemTemplate>
                            </asp:TemplateColumn>
                        </Columns>
                    </asp:DataGrid>
                    <asp:Literal runat="server" ID="litNoData"></asp:Literal>
                </td>
            </tr>
        </table>
    </div>
</asp:Content>
