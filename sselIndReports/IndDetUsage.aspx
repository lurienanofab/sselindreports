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

        .report-table td.pad-left,
        .report-table th.pad-left {
            text-align: left;
            padding-left: 4px;
        }

        .report-table td.pad-right,
        .report-table th.pad-right {
            text-align: right;
            padding-right: 4px;
        }

        .nodata {
            font-style: italic;
            color: #000;
            padding: 2px 0 2px 4px;
            width: 628px;
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
                        <lnf:PeriodPicker runat="server" ID="pp1" AutoPostBack="true" OnSelectedPeriodChanged="Pp1_SelectedPeriodChanged" />
                    </td>
                </tr>
                <tr>
                    <td>Select user:</td>
                    <td>
                        <asp:DropDownList runat="server" ID="ddlUser" CssClass="report-select" AutoPostBack="true" DataTextField="DisplayName" DataValueField="ClientID">
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

    <asp:PlaceHolder runat="server" ID="phAntipassbackSummary">
        <div class="section">
            <table class="report-table" border="1">
                <tbody>
                    <tr>
                        <td>
                            <div class="display-name">Antipassback Room Summary</div>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Repeater runat="server" ID="rptAntipassbackSummary">
                                <HeaderTemplate>
                                    <table border="1" style="width: 632px; border-collapse: collapse;">
                                        <thead>
                                            <tr>
                                                <td class="pad-left"><strong>Room</strong></td>
                                                <td class="pad-right" style="width: 120px;"><strong>Hours</strong></td>
                                            </tr>
                                        </thead>
                                        <tbody>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <tr>
                                        <td class="pad-left"><%#Eval("Key")%></td>
                                        <td class="pad-right"><%#Eval("Value", "{0:0.00}")%></td>
                                    </tr>
                                </ItemTemplate>
                                <FooterTemplate>
                                    </tbody>
                                    </table>
                                </FooterTemplate>
                            </asp:Repeater>
                            <asp:Literal runat="server" ID="litSummaryNoData"></asp:Literal>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
    </asp:PlaceHolder>

    <div class="section">
        <table class="report-table" border="1">
            <tr>
                <td style="vertical-align: middle; padding: 0;">
                    <asp:Literal runat="server" ID="litMessage"></asp:Literal>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:DataGrid runat="server" ID="dgActDate" Width="632" AutoGenerateColumns="false" OnItemDataBound="DgActDate_ItemDataBound">
                        <AlternatingItemStyle BackColor="Azure" />
                        <HeaderStyle Font-Bold="true" />
                        <Columns>
                            <asp:BoundColumn DataField="Date" HeaderText="Date" DataFormatString="{0: dd MMM yyyy}">
                                <HeaderStyle Font-Bold="true" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" Width="150" VerticalAlign="Middle" />
                            </asp:BoundColumn>
                            <asp:TemplateColumn HeaderText="Activity Detail" HeaderStyle-CssClass="pad-left">
                                <ItemStyle BackColor="White" />
                                <ItemTemplate>
                                    <asp:DataGrid runat="server" ID="dgActivity" AutoGenerateColumns="false" HeaderStyle-BackColor="#D4D0C8">
                                        <AlternatingItemStyle BackColor="Azure" />
                                        <Columns>
                                            <asp:BoundColumn DataField="ActivityTime" HeaderText="Time" ItemStyle-Width="75" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" HeaderStyle-Font-Italic="true" DataFormatString="{0: dd MMM yyyy}"></asp:BoundColumn>
                                            <asp:BoundColumn DataField="Description" HeaderText="Activity" HeaderStyle-CssClass="pad-left" ItemStyle-Width="450" HeaderStyle-Font-Italic="true"></asp:BoundColumn>
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
