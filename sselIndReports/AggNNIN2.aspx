<%@ Page Title="Aggregate User Summary Report" Language="C#" MasterPageFile="~/IndReportsMaster.Master" AutoEventWireup="true" CodeBehind="AggNNIN2.aspx.cs" Inherits="sselIndReports.AggNNIN2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .agg-nnin-report {
            font-size: 10pt;
            font-family: Arial;
        }

        .current-files {
            border-collapse: collapse;
            margin-top: 5px;
        }

            .current-files th, .current-files td {
                padding: 5px;
                font-family: Arial;
                font-size: 10pt;
                border: solid 1px #cccccc;
            }

            .current-files th {
                background-color: #dddddd;
            }

        .control-header, .control-item {
            text-align: center;
        }

        .warning {
            color: #ff0000;
            padding-top: 2px;
        }

        .file-error {
            color: #ff0000;
            font-weight: bold;
            padding-top: 5px;
            margin: 0;
        }

        .nodata {
            font-style: italic;
            font-weight: bold;
            color: #808080;
            padding: 4px;
            border: solid 1px #808080;
            margin-left: 0;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="agg-nnin-report">
        <div class="section">
            <h2>Aggregate User Summary Report</h2>
            <div class="criteria">
                <table class="criteria-table">
                    <tr>
                        <td><strong>Start aggregation period:</strong></td>
                        <td>
                            <lnf:PeriodPicker runat="server" ID="ppAgg" StartPeriod="2008-01-01" AutoPostBack="true" OnSelectedPeriodChanged="PpAgg_SelectedPeriodChanged" />
                        </td>
                    </tr>
                    <tr>
                        <td><strong>Create report for period:</strong></td>
                        <td>
                            <lnf:PeriodPicker runat="server" ID="ppRep" StartPeriod="2008-01-01" />
                        </td>
                    </tr>
                    <tr>
                        <td><strong>Current threshold:</strong></td>
                        <td>
                            <asp:Literal runat="server" ID="litCurrentThreshold"></asp:Literal>
                            minutes
                        </td>
                    </tr>
                    <tr>
                        <td><strong>Change threshold to:</strong></td>
                        <td>
                            <asp:TextBox ID="txtMinMinutes" Text="" runat="server" Width="60"></asp:TextBox>
                            minutes
                            <asp:Button runat="server" ID="btnChangeThreshold" Text="Update" OnClick="BtnChangeThreshold_Click" />
                        </td>
                    </tr>
                    <tr>
                        <td><strong>Force cumulative data refresh:</strong></td>
                        <td>
                            <asp:CheckBox runat="server" ID="chkForceRefresh" />
                        </td>
                    </tr>
                </table>
                <div class="criteria-item">
                    <div style="margin-bottom: 10px;"><asp:Button runat="server" ID="btnReport" Text="Retrieve Data" CommandName="ViewReport" CommandArgument="0" OnCommand="Report_Command" /></div>
                    <div style="margin-bottom: 10px;"><asp:LinkButton runat="server" ID="btnBack" Text="&larr; Back to Main Page" OnClick="BackButton_Click" CssClass="back-link"></asp:LinkButton></div>
                    <asp:Literal ID="litWarning" runat="server"></asp:Literal>
                </div>
            </div>
        </div>
        <div class="section">
            <asp:LinkButton runat="server" ID="btnDeleteCheckedFiles" Text="Delete Checked" OnClick="BtnDeleteCheckedFiles_Click"></asp:LinkButton>
            <asp:Literal runat="server" ID="litFileError"></asp:Literal>
            <asp:Repeater runat="server" ID="rptCurrentFiles">
                <HeaderTemplate>
                    <table class="current-files">
                        <tr>
                            <th class="files-name-header">Name</th>
                            <th class="files-lastmod-header">Last Modified</th>
                            <th class="control-header">
                                <input type="checkbox" class="check-all-files" />
                            </th>
                        </tr>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr>
                        <td class="files-name-item">
                            <asp:HyperLink runat="server" ID="hypFileName" NavigateUrl='<%#GetFileURL(Eval("Name"))%>' Text='<%#Eval("Name")%>'></asp:HyperLink>
                        </td>
                        <td class="files-lastmod-item">
                            <%#Eval("LastWriteTime")%>
                        </td>
                        <td class="control-item">
                            <asp:CheckBox runat="server" ID="chkCheckFile" CssClass="check-file" />
                        </td>
                    </tr>
                </ItemTemplate>
                <FooterTemplate>
                    </table>
                </FooterTemplate>
            </asp:Repeater>
        </div>
        <asp:Literal runat="server" ID="litDebug"></asp:Literal>
    </div>
</asp:Content>

<asp:Content runat="server" ID="Content3" ContentPlaceHolderID="scripts">
    <script type="text/javascript">
        $('.agg-nnin-report').on('change', '.check-all-files', function (event) {
            $('.check-file').find('input[type="checkbox"]').attr('checked', $(this).is(':checked'));
        });
    </script>
</asp:Content>
