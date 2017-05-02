<%@ Page Title="" Language="C#" MasterPageFile="~/IndReportsMaster.Master" AutoEventWireup="true" CodeBehind="AggFeeComparison.aspx.cs" Inherits="sselIndReports.AggFeeComparison" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        div.criteria-item {
            margin-bottom: 10px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="section">
        <h2>Actual Fee and Per Usage Fee Comparison</h2>
        <div class="criteria">
            <table class="criteria-table">
                <tr>
                    <td>Starting period: 
                    </td>
                    <td>
                        <lnf:PeriodPicker runat="server" ID="pp1" />
                        <span style="margin-left: 20px;">Number of months:</span>
                        <asp:TextBox runat="server" ID="txtNumMonths" ReadOnly="true" Text="1" Width="30" BackColor="#DDDDDD" BorderColor="#AAAAAA" BorderStyle="Solid" BorderWidth="1" CssClass="report-text"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>Org type:</td>
                    <td>
                        <asp:RadioButtonList ID="rdolist" runat="server" CellPadding="85" AutoPostBack="true" RepeatDirection="Horizontal" RepeatLayout="Flow">
                            <asp:ListItem Text="Internal" Value="5" Selected="true"></asp:ListItem>
                            <asp:ListItem Text="External Academic" Value="15"></asp:ListItem>
                            <asp:ListItem Text="External Non-Academic" Value="25"></asp:ListItem>
                        </asp:RadioButtonList>
                    </td>
                </tr>
                <tr>
                    <td>Manager:</td>
                    <td>
                        <asp:DropDownList ID="ddlManager" DataTextField="DisplayName" DataValueField="ClientOrgID" runat="server" DataSourceID="odsManager" AutoPostBack="true" CssClass="report-select">
                        </asp:DropDownList>
                        <asp:ObjectDataSource ID="odsManager" runat="server" SelectMethod="GetManagersByPeriod" TypeName="sselIndReports.AppCode.BLL.ClientAccountManager">
                            <SelectParameters>
                                <asp:ControlParameter Name="sYear" ControlID="pp1" PropertyName="SelectedYear" Type="Int32" />
                                <asp:ControlParameter Name="sMonth" ControlID="pp1" PropertyName="SelectedMonth" Type="Int32" />
                                <asp:ControlParameter Name="NumMonths" ControlID="txtNumMonths" PropertyName="Text" Type="Int32" />
                                <asp:ControlParameter Name="ChargeTypeID" ControlID="rdolist" PropertyName="SelectedValue" Type="Int32" />
                            </SelectParameters>
                        </asp:ObjectDataSource>
                    </td>
                </tr>
                <tr>
                    <td>Users:</td>
                    <td>
                        <asp:DropDownList ID="ddlUser" runat="server" DataSourceID="odsUser" DataTextField="DisplayName" DataValueField="ClientID" EnableViewState="false" AppendDataBoundItems="true" CssClass="report-select">
                            <asp:ListItem Value="0" Text="All"></asp:ListItem>
                        </asp:DropDownList>
                        <asp:ObjectDataSource ID="odsUser" runat="server" SelectMethod="GetUsersByManagerOrgID" TypeName="sselIndReports.AppCode.BLL.ClientManager">
                            <SelectParameters>
                                <asp:ControlParameter Name="sYear" ControlID="pp1" PropertyName="SelectedYear" Type="Int32" />
                                <asp:ControlParameter Name="sMonth" ControlID="pp1" PropertyName="SelectedMonth" Type="Int32" />
                                <asp:ControlParameter Name="NumMonths" ControlID="txtNumMonths" PropertyName="Text" Type="Int32" />
                                <asp:ControlParameter Name="ManagerOrgID" ControlID="ddlManager" PropertyName="SelectedValue" Type="Int32" />
                            </SelectParameters>
                        </asp:ObjectDataSource>
                    </td>
                </tr>
            </table>
            <div class="criteria-item">
                <asp:Button ID="btnReport" runat="server" Text="Retrieve Data" OnClick="btnReport_Click" CssClass="report-button" />
                <asp:LinkButton runat="server" ID="btnBack" Text="&larr; Back to Main Page" OnClick="BackButton_Click" CssClass="back-link"></asp:LinkButton>
            </div>
        </div>
    </div>
    <div class="section">
        <div>
            <asp:Label ID="lblBillingType" runat="server"></asp:Label>
        </div>
        <div>
            <asp:Label ID="lblRoomCost" runat="server"></asp:Label>
        </div>
        <div>
            <asp:Label ID="lblToolCost" runat="server"></asp:Label>
        </div>
        <div>
            <asp:Label ID="lblCurrentRoomCost" ForeColor="#ff0000" runat="server"></asp:Label>
        </div>
        <div>
            <asp:Label ID="lblPerUseRoomCost" runat="server"></asp:Label>
        </div>
        <div>
            <asp:Label ID="lblPerUseToolCost" runat="server"></asp:Label>
        </div>
        <div>
            <asp:Label ID="lblRealCost" ForeColor="#ff0000" runat="server"></asp:Label>
        </div>
    </div>
    <div class="section">
        <asp:GridView runat="server" ID="gvAllUsers" CssClass="gridview highlight" GridLines="None">
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
