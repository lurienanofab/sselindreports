<%@ Page Title="" Language="C#" MasterPageFile="~/IndReportsMaster.Master" AutoEventWireup="true" CodeBehind="IndUserUsageSummary.aspx.cs" Inherits="sselIndReports.IndUserUsageSummary" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        body {
            background-color: #E3E3E3;
        }

        .section {
            background-color: #FFFFFF;
            border: solid 1px #808080;
            padding: 3px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="NormalText">
        <div class="section">
            <h3>Individual User Usage Summary</h3>
            <div class="criteria">
                <table class="criteria-table">
                    <tr>
                        <td>Select period:</td>
                        <td>
                            <lnf:PeriodPicker runat="server" ID="pp1" AutoPostBack="true" OnSelectedPeriodChanged="pp1_SelectedPeriodChanged" />
                        </td>
                    </tr>
                    <tr>
                        <td>Select user:</td>
                        <td>
                            <asp:DropDownList runat="server" ID="ddlUser" DataTextField="DisplayName" DataValueField="ClientID" CssClass="report-select"></asp:DropDownList>
                        </td>
                    </tr>
                </table>
                <div class="criteria-item">
                    <asp:Button runat="server" ID="btnReport" Text="Retrieve Data" CssClass="report-button" OnClick="ReportButton_Click" />
                    <asp:LinkButton runat="server" ID="btnBack" Text="&larr; Back to Main Page" OnClick="BackButton_Click" CssClass="back-link"></asp:LinkButton>
                    <div runat="server" id="divBillingSystemLinks" visible="false">
                        <asp:LinkButton runat="server" ID="btnCurrent" Visible="false">Current Billing System</asp:LinkButton>
                        &nbsp;&nbsp;&nbsp;&nbsp;
                        <asp:LinkButton runat="server" ID="btnFuture" Visible="false" OnClick="btnFuture_Click">Future Billing System</asp:LinkButton>
                    </div>
                </div>
            </div>
        </div>
        <div class="section">
            <asp:DataList ID="dlSummary" runat="server">
                <HeaderTemplate>
                    <span style="font-weight: bold;">Summary by Organizations - The numbers are approximate only.  Effective Nov 1, 2009, expenses covered by monthly fee will be capped, please refer to the "User Fees" in the Help Menu above for more details.</span>
                </HeaderTemplate>
                <ItemTemplate>
                    <br />
                    <asp:Label ID="Label3" ForeColor="Blue" Font-Bold="true" Text='<%#Eval("OrgName")%>' runat="server"></asp:Label>
                    <br />
                    Room:
                        <asp:Label ID="Label4" Text='<%# Eval("RoomTotal", "{0:C}") %>' runat="server"></asp:Label>
                    <br />
                    Tool:
                        <asp:Label ID="Label5" Text='<%# Eval("ToolTotal", "{0:C}") %>' runat="server"></asp:Label>
                    <br />
                    Store:
                        <asp:Label ID="Label6" Text='<%# Eval("StoreTotal", "{0:C}") %>' runat="server"></asp:Label>
                    <br />
                </ItemTemplate>
            </asp:DataList>
        </div>
        <div class="section">
            <h4>
                <asp:Label ID="lblRoom" ForeColor="Red" runat="server">Lab Usage</asp:Label></h4>
            <div class="lab-usage" style="display: inline-block;">
                <asp:GridView runat="server" ID="gvRoom" CssClass="gridview" GridLines="None" AutoGenerateColumns="False" OnRowDataBound="gvRoom_RowDataBound">
                    <HeaderStyle CssClass="header" />
                    <RowStyle CssClass="row" />
                    <AlternatingRowStyle CssClass="altrow" />
                    <SelectedRowStyle CssClass="selected" />
                    <FooterStyle CssClass="footer" />
                    <PagerStyle CssClass="pager" />
                    <PagerSettings Mode="NumericFirstLast" FirstPageText="First" LastPageText="Last" />
                    <Columns>
                        <asp:BoundField DataField="Room" ItemStyle-HorizontalAlign="Center" HeaderText="Room">
                            <HeaderStyle Width="125" />
                        </asp:BoundField>
                        <asp:BoundField DataField="ChargeDays" HeaderText="Days" ItemStyle-HorizontalAlign="Center" HtmlEncode="false">
                            <HeaderStyle Width="80" />
                        </asp:BoundField>
                        <asp:BoundField DataField="Entries" HeaderText="Entries" ItemStyle-HorizontalAlign="Center" HtmlEncode="false" DataFormatString="{0:0.00}">
                            <HeaderStyle Width="80" />
                        </asp:BoundField>
                        <asp:BoundField DataField="Hours" HeaderText="Duration (hours)" ItemStyle-HorizontalAlign="Center" HtmlEncode="false" DataFormatString="{0:0.0000}">
                            <HeaderStyle Width="100" />
                        </asp:BoundField>
                        <asp:BoundField DataField="LineCost" HeaderText="Cost" ItemStyle-HorizontalAlign="Center" HtmlEncode="false" DataFormatString="{0:$#,##0.00}" Visible="false">
                            <ItemStyle HorizontalAlign="Right" />
                            <HeaderStyle Width="90" />
                        </asp:BoundField>
                        <asp:BoundField DataField="Name" HeaderText="Account" ItemStyle-HorizontalAlign="Center">
                            <HeaderStyle Width="250" />
                        </asp:BoundField>
                        <asp:BoundField DataField="ShortCode" HeaderText="Short Code" ItemStyle-HorizontalAlign="Center">
                            <HeaderStyle Width="90" />
                        </asp:BoundField>
                        <asp:BoundField DataField="BillingTypeName" HeaderText="Billing Type" ItemStyle-HorizontalAlign="Center"></asp:BoundField>
                        <asp:BoundField DataField="OrgName" Visible="false" HeaderText="Org" ItemStyle-HorizontalAlign="Center"></asp:BoundField>
                        <asp:BoundField DataField="LineCost" DataFormatString="{0:c2}" HeaderText="Line Cost" HtmlEncode="false" ItemStyle-HorizontalAlign="Center"></asp:BoundField>
                    </Columns>
                </asp:GridView>
                <div style="text-align: right; padding-right: 5px; font-size: small; color: Maroon;">
                    <asp:Label ID="lblRoomsSum" CssClass="NormalText" runat="server" />
                </div>
            </div>
        </div>
        <div class="section">
            <h4>
                <asp:Label ID="lblTool" ForeColor="Red" runat="server"></asp:Label>
            </h4>
            <div runat="server" id="divTool" style="margin-left: 0px;">
                <h5 style="margin-left: 20px;">Activated Reservations</h5>
                <div class="activated-reservations" style="display: inline-block;">
                    <asp:GridView runat="server" ID="gvTool" CssClass="gridview" GridLines="None" AutoGenerateColumns="false">
                        <HeaderStyle CssClass="header" />
                        <RowStyle CssClass="row" />
                        <AlternatingRowStyle CssClass="altrow" />
                        <SelectedRowStyle CssClass="selected" />
                        <FooterStyle CssClass="footer" />
                        <PagerStyle CssClass="pager" />
                        <PagerSettings Mode="NumericFirstLast" FirstPageText="First" LastPageText="Last" />
                        <Columns>
                            <asp:BoundField DataField="ResourceName" HeaderText="Tool" HeaderStyle-Width="220" ItemStyle-HorizontalAlign="Center" />
                            <asp:BoundField DataField="Room" HeaderText="Lab" HeaderStyle-Width="100" ItemStyle-HorizontalAlign="Center" />
                            <asp:BoundField DataField="TotalUses" HeaderText="Uses" HtmlEncode="false" DataFormatString="{0:0.00}" HeaderStyle-Width="60" ItemStyle-HorizontalAlign="Center" />
                            <asp:BoundField DataField="TotalActDuration" HeaderText="Duration (hours)" HtmlEncode="false" DataFormatString="{0:0.00}" HeaderStyle-Width="80" ItemStyle-HorizontalAlign="Center" />
                            <asp:BoundField DataField="TotalOverTime" HeaderText="Overtime (hours)" HtmlEncode="false" DataFormatString="{0:0.00}" HeaderStyle-Width="80" ItemStyle-HorizontalAlign="Center" />
                            <asp:BoundField DataField="Name" HeaderText="Account" HeaderStyle-Width="250" ItemStyle-HorizontalAlign="Center" />
                            <asp:BoundField DataField="ShortCode" HeaderText="Short Code" HeaderStyle-Width="70" ItemStyle-HorizontalAlign="Center" />
                            <asp:BoundField DataField="OrgName" Visible="false" HeaderText="Org" ItemStyle-HorizontalAlign="Center"></asp:BoundField>
                            <asp:BoundField DataField="LineCost" HeaderText="Line Cost" HtmlEncode="false" DataFormatString="{0:$0.00}" ItemStyle-HorizontalAlign="Center" />
                        </Columns>
                        <EmptyDataTemplate>
                            &nbsp;&nbsp;-- There is no record of activated reservation for this period --&nbsp;&nbsp;
                        </EmptyDataTemplate>
                    </asp:GridView>
                    <div style="text-align: right; padding-right: 5px; font-size: small; color: Maroon;">
                        <asp:Label ID="lblRoomsSumActivated" CssClass="NormalText" runat="server"></asp:Label>
                        <asp:Label ID="lblActivatedToolFee" CssClass="NormalText" runat="server"></asp:Label>
                    </div>
                </div>
                <h5 style="margin-left: 20px;">Uncancelled Reservations</h5>
                <div class="uncancelled-reservations" style="display: inline-block;">
                    <asp:GridView runat="server" ID="gvToolCancelled" CssClass="gridview" GridLines="None" AutoGenerateColumns="False">
                        <HeaderStyle CssClass="header" />
                        <RowStyle CssClass="row" />
                        <AlternatingRowStyle CssClass="altrow" />
                        <SelectedRowStyle CssClass="selected" />
                        <FooterStyle CssClass="footer" />
                        <PagerStyle CssClass="pager" />
                        <PagerSettings Mode="NumericFirstLast" FirstPageText="First" LastPageText="Last" />
                        <Columns>
                            <asp:BoundField DataField="ResourceName" HeaderText="Tool" HeaderStyle-Width="220" ItemStyle-HorizontalAlign="Center" />
                            <asp:BoundField DataField="Room" HeaderText="Lab" HeaderStyle-Width="100" ItemStyle-HorizontalAlign="Center" />
                            <asp:BoundField DataField="TotalUses" HeaderText="Uses" DataFormatString="{0:0.00}" HtmlEncode="false" HeaderStyle-Width="60" ItemStyle-HorizontalAlign="Center" />
                            <asp:BoundField DataField="TotalSchedDuration" HeaderText="Duration (hours)" DataFormatString="{0:0.00}" HtmlEncode="false" HeaderStyle-Width="80" ItemStyle-HorizontalAlign="Center" />
                            <asp:BoundField DataField="Name" HeaderText="Account" HeaderStyle-Width="250" ItemStyle-HorizontalAlign="Center" />
                            <asp:BoundField DataField="ShortCode" HeaderText="Short Code" HeaderStyle-Width="70" ItemStyle-HorizontalAlign="Center" />
                            <asp:BoundField DataField="OrgName" Visible="false" HeaderText="Org" ItemStyle-HorizontalAlign="Center"></asp:BoundField>
                            <asp:BoundField DataField="LineCost" HeaderText="Line Cost" DataFormatString="{0:$0.00}" HtmlEncode="false" ItemStyle-HorizontalAlign="Center" />
                        </Columns>
                        <EmptyDataTemplate>
                            &nbsp;&nbsp;-- There is no record of uncancelled reservation for this period --&nbsp;&nbsp;
                        </EmptyDataTemplate>
                    </asp:GridView>
                    <div style="text-align: right; padding-right: 5px; font-size: small; color: Maroon;">
                        <asp:Label ID="lblRoomSumUnCancelled" CssClass="NormalText" runat="server"></asp:Label>
                        <asp:Label ID="lblCancelledToolFee" runat="server"></asp:Label>
                    </div>
                </div>
                <h5 style="margin-left: 20px;">Forgiven Reservations</h5>
                <div class="forgiven-reservations" style="display: inline-block;">
                    <asp:GridView runat="server" ID="gvToolForgiven" CssClass="gridview" GridLines="None" AutoGenerateColumns="False">
                        <HeaderStyle CssClass="header" />
                        <RowStyle CssClass="row" />
                        <AlternatingRowStyle CssClass="altrow" />
                        <SelectedRowStyle CssClass="selected" />
                        <FooterStyle CssClass="footer" />
                        <PagerStyle CssClass="pager" />
                        <PagerSettings Mode="NumericFirstLast" FirstPageText="First" LastPageText="Last" />
                        <Columns>
                            <asp:BoundField DataField="ResourceName" HeaderText="Tool" HeaderStyle-Width="220" ItemStyle-HorizontalAlign="Center" />
                            <asp:BoundField DataField="Room" HeaderText="Lab" HeaderStyle-Width="100" ItemStyle-HorizontalAlign="Center" />
                            <asp:BoundField DataField="TotalUses" HeaderText="Uses" DataFormatString="{0:0.00}" HtmlEncode="false" HeaderStyle-Width="60" ItemStyle-HorizontalAlign="Center" />
                            <asp:BoundField DataField="TotalActDuration" HeaderText="Duration (hours)" DataFormatString="{0:0.00}" HtmlEncode="false" HeaderStyle-Width="80" ItemStyle-HorizontalAlign="Center" />
                            <asp:BoundField DataField="Name" HeaderText="Account" HeaderStyle-Width="250" ItemStyle-HorizontalAlign="Center" />
                            <asp:BoundField DataField="ShortCode" HeaderText="Short Code" HeaderStyle-Width="70" ItemStyle-HorizontalAlign="Center" />
                            <asp:BoundField DataField="OrgName" Visible="false" HeaderText="Org" ItemStyle-HorizontalAlign="Center"></asp:BoundField>
                            <asp:BoundField DataField="LineCost" HeaderText="Line Cost" DataFormatString="{0:$0.00}" HtmlEncode="false" ItemStyle-HorizontalAlign="Center" />
                        </Columns>
                        <EmptyDataTemplate>
                            &nbsp;&nbsp;-- There is no record of forgiven reservation for this period --&nbsp;&nbsp;
                        </EmptyDataTemplate>
                    </asp:GridView>
                    <div style="text-align: right; padding-right: 5px; font-size: small; color: Maroon;">
                        <asp:Label ID="lblForgivenToolFee" runat="server"></asp:Label>
                    </div>
                </div>
            </div>
        </div>
        <div class="section">
            <h4>
                <asp:Label ID="lblStore" ForeColor="Red" runat="server">Store Usage</asp:Label></h4>
            <div class="store-usage" style="display: inline-block;">
                <asp:GridView runat="server" ID="gvStore" CssClass="gridview" GridLines="None" AutoGenerateColumns="False">
                    <HeaderStyle CssClass="header" />
                    <RowStyle CssClass="row" />
                    <AlternatingRowStyle CssClass="altrow" />
                    <SelectedRowStyle CssClass="selected" />
                    <FooterStyle CssClass="footer" />
                    <PagerStyle CssClass="pager" />
                    <PagerSettings Mode="NumericFirstLast" FirstPageText="First" LastPageText="Last" />
                    <Columns>
                        <asp:BoundField DataField="Description" HeaderText="Item" ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="220" />
                        <asp:BoundField DataField="TotalQuantity" HeaderText="Quantity" ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="80" />
                        <asp:BoundField DataField="Name" HeaderText="Account" ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="200" />
                        <asp:BoundField DataField="ShortCode" HeaderText="Short Code" HeaderStyle-Width="90" ItemStyle-HorizontalAlign="Center" />
                        <asp:BoundField DataField="OrgName" Visible="false" HeaderText="Org" ItemStyle-HorizontalAlign="Center"></asp:BoundField>
                        <asp:BoundField DataField="LineCost" HeaderText="Line Cost" ItemStyle-HorizontalAlign="Center" DataFormatString="{0:$#,##0.00}" HtmlEncode="false">
                            <HeaderStyle Width="90" />
                        </asp:BoundField>
                    </Columns>
                </asp:GridView>
            </div>
        </div>
    </div>
</asp:Content>
