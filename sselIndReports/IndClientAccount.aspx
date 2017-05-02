<%@ Page Title="" Language="C#" MasterPageFile="~/IndReportsMaster.Master" AutoEventWireup="true" CodeBehind="IndClientAccount.aspx.cs" Inherits="sselIndReports.IndClientAccount" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="/static/scripts/jquery/jquery.assignaccounts/css/jquery.assignaccounts.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="section">
        <h2>Client and Accounts Mapping by Manager</h2>
        <div style="padding-bottom: 10px;">
            <table style="border-collapse: separate;">
                <tr>
                    <td style="padding-bottom: 5px; padding-right: 5px;">
                        <strong>Manager:</strong>
                    </td>
                    <td colspan="4" style="padding-bottom: 5px; padding-left: 0;">
                        <asp:DropDownList runat="server" ID="ddlManager" DataTextField="DisplayName" DataValueField="ClientOrgID" CssClass="report-select" AutoPostBack="true">
                        </asp:DropDownList>
                        <asp:LinkButton runat="server" ID="btnBack" Text="&larr; Back to Main Page" OnClick="BackButton_Click" CssClass="back-link"></asp:LinkButton>
                    </td>
                </tr>
                <tr>
                    <td style="padding: 5px; border: none; text-align: right;">
                        <strong>Display:</strong>
                    </td>
                    <td style="padding: 5px; border: solid 1px #AAAAAA; background-color: #EAEAFF;">
                        <label>
                            Name
                                <input type="radio" runat="server" id="rdoAcctDisplayByName" name="acct_display" class="acct-display-by" data-key="name" />
                        </label>
                    </td>
                    <td style="padding: 5px; border: solid 1px #AAAAAA; background-color: #EAEAFF;">
                        <label>
                            Numbers
                                <input type="radio" runat="server" id="rdoAcctDisplayByNumber" name="acct_display" class="acct-display-by" data-key="number" />
                        </label>
                    </td>
                    <td style="padding: 5px; border: solid 1px #AAAAAA; background-color: #EAEAFF;">
                        <label>
                            Project
                                <input type="radio" runat="server" id="rdoAcctDisplayByProject" name="acct_display" class="acct-display-by" data-key="project" />
                        </label>
                    </td>
                    <td style="padding: 5px; border: solid 1px #AAAAAA; background-color: #EAEAFF;">
                        <label>
                            ShortCode
                                <input type="radio" runat="server" id="rdoAcctDisplayByShortCode" name="acct_display" class="acct-display-by" data-key="shortcode" />
                        </label>
                    </td>
                </tr>
            </table>
        </div>
        <div style="border-top: solid 1px #A0A0A0; padding-top: 10px;">
            <asp:Literal runat="server" ID="litMessage"></asp:Literal>
            <asp:Literal runat="server" ID="litMatrix"></asp:Literal>
        </div>
    </div>
</asp:Content>

<asp:Content runat="server" ID="Content3" ContentPlaceHolderID="scripts">
    <script src="/static/scripts/jquery/jquery.assignaccounts/js/jquery.assignaccounts.js"></script>

    <script>
        $('.section').assignaccounts({
            'init': function (matrix) {
                return;
                $('input[type="checkbox"]', matrix).each(function () {
                    var chk = $(this);
                    var cell = chk.closest('td');
                    var text = chk.is(':checked') ? 'True' : '';
                    cell.html(text);
                });
            }
        });
    </script>
</asp:Content>
