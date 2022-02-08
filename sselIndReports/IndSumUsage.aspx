<%@ Page Title="" Language="C#" MasterPageFile="~/IndReportsMaster.Master" AutoEventWireup="true" CodeBehind="IndSumUsage.aspx.cs" Inherits="sselIndReports.IndSumUsage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        body {
            background-color: #e3ffff;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <div class="NormalText">
                <div class="section">
                    <h3>Individual User Usage Summary</h3>
                    <div class="criteria">
                        <table class="criteria-table">
                            <tr>
                                <td>Select period:</td>
                                <td>
                                    <lnf:PeriodPicker runat="server" ID="pp1" AutoPostBack="true" OnSelectedPeriodChanged="Pp1_SelectedPeriodChanged" />
                                </td>
                            </tr>
                            <tr>
                                <td>Select user:</td>
                                <td>
                                    <asp:DropDownList runat="server" ID="ddlUser" DataTextField="DisplayName" DataValueField="ClientID" CssClass="report-select">
                                    </asp:DropDownList>
                                </td>
                            </tr>
                        </table>
                        <div class="criteria-item">
                            <asp:Button runat="server" ID="btnReport" Text="Retrieve Data" CssClass="StoreButton" OnClick="ReportButton_Click" />
                            <asp:Button runat="server" ID="btnDiscard" Text="Return to Main Page" CausesValidation="False" CssClass="QuitButton" OnClick="BackButton_Click"></asp:Button>
                        </div>
                    </div>
                </div>
                <!-- tab -->
                <asp:Panel runat="server" ID="panTabs" CssClass="section">
                    <div id="tabs1">
                        <ul>
                            <li><a href="#tabs-1">Current Billing</a></li>
                            <li><a href="#tabs-2">New Billing</a></li>
                        </ul>
                        <div id="tabs-1">
                            <div>
                                <asp:DataList ID="dlSummary" runat="server">
                                    <HeaderTemplate>
                                        <span style="font-weight: bold;">Summary by Organizations - The numbers are approximate only</span>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <br />
                                        <asp:Label ForeColor="Blue" Font-Bold="true" Text='<%#Eval("OrgName")%>' runat="server"></asp:Label>
                                        <br />
                                        Room:
                                        <asp:Label Text='<%# Eval("RoomTotal", "{0:c}") %>' runat="server"></asp:Label>
                                        <br />
                                        Tool:
                                        <asp:Label Text='<%# Eval("ToolTotal", "{0:c}") %>' runat="server"></asp:Label>
                                        <br />
                                        Store:
                                        <asp:Label Text='<%# Eval("StoreTotal", "{0:c}") %>' runat="server"></asp:Label>
                                        <br />
                                    </ItemTemplate>
                                </asp:DataList>
                            </div>
                            <div style="margin-left: 0px">
                                <h4>
                                    <asp:Label ID="lblRoom" runat="server">Lab Usage</asp:Label></h4>
                                <asp:GridView runat="server" ID="dgRoom" CssClass="gridview" GridLines="None" AutoGenerateColumns="False">
                                    <HeaderStyle CssClass="header" />
                                    <RowStyle CssClass="row" />
                                    <AlternatingRowStyle CssClass="altrow" />
                                    <SelectedRowStyle CssClass="selected" />
                                    <FooterStyle CssClass="footer" />
                                    <PagerStyle CssClass="pager" />
                                    <PagerSettings Mode="NumericFirstLast" FirstPageText="First" LastPageText="Last" />
                                    <Columns>
                                        <asp:BoundField DataField="Room" ItemStyle-HorizontalAlign="Center" HeaderText="Room">
                                            <HeaderStyle Width="125px"></HeaderStyle>
                                        </asp:BoundField>
                                        <asp:BoundField DataField="TotalEntries" HeaderText="Entries" ItemStyle-HorizontalAlign="Center" HtmlEncode="false" DataFormatString="{0:0.00}">
                                            <HeaderStyle Width="80px"></HeaderStyle>
                                        </asp:BoundField>
                                        <asp:BoundField DataField="TotalHours" HeaderText="Duration (hours)" ItemStyle-HorizontalAlign="Center" HtmlEncode="false" DataFormatString="{0:0.0000}">
                                            <HeaderStyle Width="100px"></HeaderStyle>
                                        </asp:BoundField>
                                        <asp:BoundField DataField="TotalCalcCost" HeaderText="Cost" ItemStyle-HorizontalAlign="Center" HtmlEncode="false" DataFormatString="{0:$#,##0.00}" Visible="false">
                                            <ItemStyle HorizontalAlign="Right"></ItemStyle>
                                            <HeaderStyle Width="90px"></HeaderStyle>
                                        </asp:BoundField>
                                        <asp:BoundField DataField="Name" HeaderText="Account" ItemStyle-HorizontalAlign="Center">
                                            <HeaderStyle Width="250px"></HeaderStyle>
                                        </asp:BoundField>
                                        <asp:BoundField DataField="ShortCode" HeaderText="Short Code" ItemStyle-HorizontalAlign="Center">
                                            <HeaderStyle Width="90px"></HeaderStyle>
                                        </asp:BoundField>
                                        <asp:BoundField DataField="BillingTypeName" HeaderText="Billing Type" ItemStyle-HorizontalAlign="Center"></asp:BoundField>
                                        <asp:BoundField DataField="OrgName" Visible="false" HeaderText="Org" ItemStyle-HorizontalAlign="Center"></asp:BoundField>
                                        <asp:BoundField DataField="LineCost" DataFormatString="{0:c2}" HeaderText="Line Cost" HtmlEncode="false" ItemStyle-HorizontalAlign="Center"></asp:BoundField>
                                    </Columns>
                                </asp:GridView>
                            </div>
                            <div style="margin-left: 0px">
                                <h4>
                                    <asp:Label ID="lblTool" runat="server">Tool Usage</asp:Label></h4>
                                <div id="divTool" runat="server" style="width: 810px; margin-left: 0px;">
                                    <h5 style="margin-left: 20px">Activated Reservations</h5>
                                    <asp:GridView runat="server" ID="gvTool" CssClass="gridview" GridLines="None" AutoGenerateColumns="false">
                                        <HeaderStyle CssClass="header" />
                                        <RowStyle CssClass="row" />
                                        <AlternatingRowStyle CssClass="altrow" />
                                        <SelectedRowStyle CssClass="selected" />
                                        <FooterStyle CssClass="footer" />
                                        <PagerStyle CssClass="pager" />
                                        <PagerSettings Mode="NumericFirstLast" FirstPageText="First" LastPageText="Last" />
                                        <Columns>
                                            <asp:BoundField DataField="ResourceName" HeaderText="Tool" HeaderStyle-Width="220px" ItemStyle-HorizontalAlign="Center" />
                                            <asp:BoundField DataField="TotalUses" HeaderText="Uses" HtmlEncode="false" DataFormatString="{0:0.00}" HeaderStyle-Width="100px" ItemStyle-HorizontalAlign="Center" />
                                            <asp:BoundField DataField="TotalActDuration" HeaderText="Duration (hours)" HtmlEncode="false" DataFormatString="{0:0.00}" HeaderStyle-Width="100px" ItemStyle-HorizontalAlign="Center" />
                                            <asp:BoundField DataField="AccountName" HeaderText="Account" HeaderStyle-Width="200px" ItemStyle-HorizontalAlign="Center" />
                                            <asp:BoundField DataField="ShortCode" HeaderText="Short Code" HeaderStyle-Width="90px" ItemStyle-HorizontalAlign="Center" />
                                            <asp:BoundField DataField="OrgName" Visible="false" HeaderText="Org" ItemStyle-HorizontalAlign="Center"></asp:BoundField>
                                            <asp:BoundField DataField="TotalCalcCost" HeaderText="Line Cost" HtmlEncode="false" DataFormatString="{0:$0.00}" HeaderStyle-Width="90px" ItemStyle-HorizontalAlign="Center" />
                                        </Columns>
                                        <EmptyDataTemplate>
                                            &nbsp;&nbsp;-- There is no record of activated reservation for this period --&nbsp;&nbsp;
                                        </EmptyDataTemplate>
                                    </asp:GridView>
                                    <span style="float: right; padding-right: 30px; font-size: small;">
                                        <asp:Label ID="lblActivatedToolFee" ForeColor="Brown" CssClass="NormalText" runat="server"></asp:Label></span>
                                    <h5 style="margin-left: 20px">Uncancelled Reservations</h5>
                                    <asp:GridView runat="server" ID="gvToolCancelled" CssClass="gridview" GridLines="None" AutoGenerateColumns="False">
                                        <RowStyle CssClass="row" />
                                        <AlternatingRowStyle CssClass="altrow" />
                                        <SelectedRowStyle CssClass="selected" />
                                        <FooterStyle CssClass="footer" />
                                        <PagerStyle CssClass="pager" />
                                        <PagerSettings Mode="NumericFirstLast" FirstPageText="First" LastPageText="Last" />
                                        <Columns>
                                            <asp:BoundField HeaderText="Tool" DataField="ResourceName" HeaderStyle-Width="220px" ItemStyle-HorizontalAlign="Center" />
                                            <asp:BoundField HeaderText="Uses" DataField="TotalUses" DataFormatString="{0:0.00}" HtmlEncode="false" HeaderStyle-Width="100px" ItemStyle-HorizontalAlign="Center" />
                                            <asp:BoundField HeaderText="Duration (hours)" DataField="TotalActDuration" DataFormatString="{0:0.00}" HtmlEncode="false" HeaderStyle-Width="100px" ItemStyle-HorizontalAlign="Center" />
                                            <asp:BoundField HeaderText="Account" DataField="AccountName" HeaderStyle-Width="200px" ItemStyle-HorizontalAlign="Center" />
                                            <asp:BoundField DataField="ShortCode" HeaderText="Short Code" HeaderStyle-Width="90px" ItemStyle-HorizontalAlign="Center" />
                                            <asp:BoundField DataField="OrgName" Visible="false" HeaderText="Org" ItemStyle-HorizontalAlign="Center"></asp:BoundField>
                                            <asp:BoundField HeaderText="Line Cost" DataField="TotalCalcCost" DataFormatString="{0:$0.00}" HtmlEncode="false" HeaderStyle-Width="90px" ItemStyle-HorizontalAlign="Center" />
                                        </Columns>
                                        <EmptyDataTemplate>
                                            &nbsp;&nbsp;-- There is no record of uncancelled reservation for this period --&nbsp;&nbsp;
                                        </EmptyDataTemplate>
                                    </asp:GridView>
                                    <span style="float: right; padding-right: 30px; font-size: small;">
                                        <asp:Label ID="lblCancelledToolFee" ForeColor="Brown" runat="server"></asp:Label></span>
                                    <h5 style="margin-left: 20px">Forgiven Reservations</h5>
                                    <asp:GridView runat="server" ID="gvToolForgiven" CssClass="gridview" GridLines="None" AutoGenerateColumns="False">
                                        <HeaderStyle CssClass="header" />
                                        <RowStyle CssClass="row" />
                                        <AlternatingRowStyle CssClass="altrow" />
                                        <SelectedRowStyle CssClass="selected" />
                                        <FooterStyle CssClass="footer" />
                                        <PagerStyle CssClass="pager" />
                                        <PagerSettings Mode="NumericFirstLast" FirstPageText="First" LastPageText="Last" />
                                        <Columns>
                                            <asp:BoundField HeaderText="Tool" DataField="ResourceName" HeaderStyle-Width="220px" ItemStyle-HorizontalAlign="Center" />
                                            <asp:BoundField HeaderText="Uses" DataField="TotalUses" DataFormatString="{0:0.00}" HtmlEncode="false" HeaderStyle-Width="100px" ItemStyle-HorizontalAlign="Center" />
                                            <asp:BoundField HeaderText="Duration (hours)" DataField="TotalActDuration" DataFormatString="{0:0.00}" HtmlEncode="false" HeaderStyle-Width="100px" ItemStyle-HorizontalAlign="Center" />
                                            <asp:BoundField HeaderText="Account" DataField="AccountName" HeaderStyle-Width="200px" ItemStyle-HorizontalAlign="Center" />
                                            <asp:BoundField DataField="ShortCode" HeaderText="Short Code" HeaderStyle-Width="90px" ItemStyle-HorizontalAlign="Center" />
                                            <asp:BoundField DataField="OrgName" Visible="false" HeaderText="Org" ItemStyle-HorizontalAlign="Center"></asp:BoundField>
                                            <asp:BoundField HeaderText="Line Cost" DataField="TotalCalcCost" DataFormatString="{0:$0.00}" HtmlEncode="false" HeaderStyle-Width="90px" ItemStyle-HorizontalAlign="Center" />
                                        </Columns>
                                        <EmptyDataTemplate>
                                            &nbsp;&nbsp;-- There is no record of forgiven reservation for this period --&nbsp;&nbsp;
                                        </EmptyDataTemplate>
                                    </asp:GridView>
                                    <span style="float: right; padding-right: 30px; font-size: small;">
                                        <asp:Label ID="lblForgivenToolFee" ForeColor="Brown" runat="server"></asp:Label></span>
                                </div>
                            </div>
                            <div style="margin-left: 0px">
                                <h4>
                                    <asp:Label ID="lblStore" runat="server">Store Usage</asp:Label></h4>
                                <asp:GridView runat="server" ID="dgStore" CssClass="gridview" GridLines="None" AutoGenerateColumns="False">
                                    <HeaderStyle CssClass="header" />
                                    <RowStyle CssClass="row" />
                                    <AlternatingRowStyle CssClass="altrow" />
                                    <SelectedRowStyle CssClass="selected" />
                                    <FooterStyle CssClass="footer" />
                                    <PagerStyle CssClass="pager" />
                                    <PagerSettings Mode="NumericFirstLast" FirstPageText="First" LastPageText="Last" />
                                    <Columns>
                                        <asp:BoundField DataField="Item" HeaderText="Item" ItemStyle-HorizontalAlign="Center">
                                            <HeaderStyle Width="220px"></HeaderStyle>
                                        </asp:BoundField>
                                        <asp:BoundField DataField="Name" HeaderText="Account" ItemStyle-HorizontalAlign="Center">
                                            <HeaderStyle Width="200px"></HeaderStyle>
                                        </asp:BoundField>
                                        <asp:BoundField DataField="ShortCode" HeaderText="Short Code" HeaderStyle-Width="90px" ItemStyle-HorizontalAlign="Center" />
                                        <asp:BoundField DataField="OrgName" Visible="false" HeaderText="Org" ItemStyle-HorizontalAlign="Center"></asp:BoundField>
                                        <asp:BoundField DataField="CalcCost" HeaderText="Line Cost" ItemStyle-HorizontalAlign="Center" DataFormatString="{0:$#,##0.00}" HtmlEncode="false">
                                            <HeaderStyle Width="90px"></HeaderStyle>
                                        </asp:BoundField>
                                    </Columns>
                                </asp:GridView>
                            </div>
                        </div>
                        <div id="tabs-2">
                            <div>
                                <asp:DataList ID="dlSummary2" runat="server">
                                    <HeaderTemplate>
                                        <span style="font-weight: bold;">Summary by Organizations - The numbers are approximate only</span>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <br />
                                        <asp:Label ForeColor="Blue" Font-Bold="true" Text='<%#Eval("OrgName")%>' runat="server"></asp:Label>
                                        <br />
                                        Room:
                                        <asp:Label Text='<%# Eval("RoomTotal", "{0:c}") %>' runat="server"></asp:Label>
                                        <br />
                                        Tool:
                                        <asp:Label Text='<%# Eval("ToolTotal", "{0:c}") %>' runat="server"></asp:Label>
                                        <br />
                                        Store:
                                        <asp:Label Text='<%# Eval("StoreTotal", "{0:c}") %>' runat="server"></asp:Label>
                                        <br />
                                    </ItemTemplate>
                                </asp:DataList>
                            </div>
                            <div style="margin-left: 0px">
                                <h4>
                                    <asp:Label ID="lblRoom2" ForeColor="Red" runat="server">Lab Usage</asp:Label></h4>
                                <asp:GridView runat="server" ID="gvRoom" CssClass="gridview" GridLines="None" AutoGenerateColumns="False">
                                    <HeaderStyle CssClass="header" />
                                    <RowStyle CssClass="row" />
                                    <AlternatingRowStyle CssClass="altrow" />
                                    <SelectedRowStyle CssClass="selected" />
                                    <FooterStyle CssClass="footer" />
                                    <PagerStyle CssClass="pager" />
                                    <PagerSettings Mode="NumericFirstLast" FirstPageText="First" LastPageText="Last" />
                                    <Columns>
                                        <asp:BoundField DataField="Room" ItemStyle-HorizontalAlign="Center" HeaderText="Room">
                                            <HeaderStyle Width="125px"></HeaderStyle>
                                        </asp:BoundField>
                                        <asp:BoundField DataField="TotalEntries" HeaderText="Entries" ItemStyle-HorizontalAlign="Center" HtmlEncode="false" DataFormatString="{0:0.00}">
                                            <HeaderStyle Width="80px"></HeaderStyle>
                                        </asp:BoundField>
                                        <asp:BoundField DataField="TotalHours" HeaderText="Duration (hours)" ItemStyle-HorizontalAlign="Center" HtmlEncode="false" DataFormatString="{0:0.0000}">
                                            <HeaderStyle Width="100px"></HeaderStyle>
                                        </asp:BoundField>
                                        <asp:BoundField DataField="TotalDays" HeaderText="Days" ItemStyle-HorizontalAlign="Center" HtmlEncode="false" DataFormatString="{0:0}">
                                            <HeaderStyle Width="100px"></HeaderStyle>
                                        </asp:BoundField>
                                        <asp:BoundField DataField="TotalCalcCost" HeaderText="Cost" ItemStyle-HorizontalAlign="Center" HtmlEncode="false" DataFormatString="{0:$#,##0.00}" Visible="false">
                                            <ItemStyle HorizontalAlign="Right"></ItemStyle>
                                            <HeaderStyle Width="90px"></HeaderStyle>
                                        </asp:BoundField>
                                        <asp:BoundField DataField="Name" HeaderText="Account" ItemStyle-HorizontalAlign="Center">
                                            <HeaderStyle Width="250px"></HeaderStyle>
                                        </asp:BoundField>
                                        <asp:BoundField DataField="ShortCode" HeaderText="Short Code" HeaderStyle-Width="90px" ItemStyle-HorizontalAlign="Center" />
                                        <asp:BoundField DataField="TotalCalcCost" DataFormatString="{0:c2}" HeaderText="Line Cost" HtmlEncode="false" ItemStyle-HorizontalAlign="Center"></asp:BoundField>
                                    </Columns>
                                    <EmptyDataTemplate>
                                        &nbsp;&nbsp;-- There is no record of room usage for this period --&nbsp;&nbsp;
                                    </EmptyDataTemplate>
                                </asp:GridView>
                            </div>
                            <!-- Tool -->
                            <div style="margin-left: 0px">
                                <h4>
                                    <asp:Label ID="lblTool2" ForeColor="Red" runat="server">Tool Usage</asp:Label></h4>
                                <div id="div1" runat="server" style="width: 810px; margin-left: 0px;">
                                    <h5 style="margin-left: 20px">Activated Reservations</h5>
                                    <asp:GridView runat="server" ID="gvTool2" CssClass="gridview" GridLines="None" AutoGenerateColumns="false">
                                        <HeaderStyle CssClass="header" />
                                        <RowStyle CssClass="row" />
                                        <AlternatingRowStyle CssClass="altrow" />
                                        <SelectedRowStyle CssClass="selected" />
                                        <FooterStyle CssClass="footer" />
                                        <PagerStyle CssClass="pager" />
                                        <PagerSettings Mode="NumericFirstLast" FirstPageText="First" LastPageText="Last" />
                                        <Columns>
                                            <asp:BoundField DataField="ResourceName" HeaderText="Tool" HeaderStyle-Width="220px" ItemStyle-HorizontalAlign="Center" />
                                            <asp:BoundField DataField="TotalUses" HeaderText="Uses" HtmlEncode="false" DataFormatString="{0:0.00}" HeaderStyle-Width="100px" ItemStyle-HorizontalAlign="Center" />
                                            <asp:BoundField DataField="TotalActDuration" HeaderText="Duration (hours)" HtmlEncode="false" DataFormatString="{0:0.00}" HeaderStyle-Width="100px" ItemStyle-HorizontalAlign="Center" />
                                            <asp:BoundField DataField="AccountName" HeaderText="Account" HeaderStyle-Width="200px" ItemStyle-HorizontalAlign="Center" />
                                            <asp:BoundField DataField="ShortCode" HeaderText="Short Code" HeaderStyle-Width="90px" ItemStyle-HorizontalAlign="Center" />
                                            <asp:BoundField DataField="OrgName" Visible="false" HeaderText="Org" ItemStyle-HorizontalAlign="Center"></asp:BoundField>
                                            <asp:BoundField DataField="TotalCalcCost" HeaderText="Line Cost" HtmlEncode="false" DataFormatString="{0:$0.00}" HeaderStyle-Width="90px" ItemStyle-HorizontalAlign="Center" />
                                        </Columns>
                                        <EmptyDataTemplate>
                                            &nbsp;&nbsp;-- There is no record of activated reservation for this period --&nbsp;&nbsp;
                                        </EmptyDataTemplate>
                                    </asp:GridView>
                                    <span style="float: right; padding-right: 30px; font-size: small;">
                                        <asp:Label ID="lblActivatedToolFee2" ForeColor="Brown" CssClass="NormalText" runat="server"></asp:Label></span>
                                    <h5 style="margin-left: 20px">Uncancelled Reservations</h5>
                                    <asp:GridView runat="server" ID="gvToolCancelled2" CssClass="gridview" GridLines="None" AutoGenerateColumns="False">
                                        <HeaderStyle CssClass="header" />
                                        <RowStyle CssClass="row" />
                                        <AlternatingRowStyle CssClass="altrow" />
                                        <SelectedRowStyle CssClass="selected" />
                                        <FooterStyle CssClass="footer" />
                                        <PagerStyle CssClass="pager" />
                                        <PagerSettings Mode="NumericFirstLast" FirstPageText="First" LastPageText="Last" />
                                        <Columns>
                                            <asp:BoundField HeaderText="Tool" DataField="ResourceName" HeaderStyle-Width="220px" ItemStyle-HorizontalAlign="Center" />
                                            <asp:BoundField HeaderText="Uses" DataField="TotalUses" DataFormatString="{0:0.00}" HtmlEncode="false" HeaderStyle-Width="100px" ItemStyle-HorizontalAlign="Center" />
                                            <asp:BoundField HeaderText="Duration (hours)" DataField="TotalActDuration" DataFormatString="{0:0.00}" HtmlEncode="false" HeaderStyle-Width="100px" ItemStyle-HorizontalAlign="Center" />
                                            <asp:BoundField HeaderText="Account" DataField="AccountName" HeaderStyle-Width="200px" ItemStyle-HorizontalAlign="Center" />
                                            <asp:BoundField DataField="ShortCode" HeaderText="Short Code" HeaderStyle-Width="90px" ItemStyle-HorizontalAlign="Center" />
                                            <asp:BoundField DataField="OrgName" Visible="false" HeaderText="Org" ItemStyle-HorizontalAlign="Center"></asp:BoundField>
                                            <asp:BoundField HeaderText="Line Cost" DataField="TotalCalcCost" DataFormatString="{0:$0.00}" HtmlEncode="false" HeaderStyle-Width="90px" ItemStyle-HorizontalAlign="Center" />
                                        </Columns>
                                        <EmptyDataTemplate>
                                            &nbsp;&nbsp;-- There is no record of uncancelled reservation for this period --&nbsp;&nbsp;
                                        </EmptyDataTemplate>
                                    </asp:GridView>
                                    <span style="float: right; padding-right: 30px; font-size: small;">
                                        <asp:Label ID="lblCancelledToolFee2" ForeColor="Brown" runat="server"></asp:Label></span>
                                    <h5 style="margin-left: 20px">Forgiven Reservations</h5>
                                    <asp:GridView runat="server" ID="gvToolForgiven2" CssClass="gridview" GridLines="None" AutoGenerateColumns="False">
                                        <HeaderStyle CssClass="header" />
                                        <RowStyle CssClass="row" />
                                        <AlternatingRowStyle CssClass="altrow" />
                                        <SelectedRowStyle CssClass="selected" />
                                        <FooterStyle CssClass="footer" />
                                        <PagerStyle CssClass="pager" />
                                        <PagerSettings Mode="NumericFirstLast" FirstPageText="First" LastPageText="Last" />
                                        <Columns>
                                            <asp:BoundField HeaderText="Tool" DataField="ResourceName" HeaderStyle-Width="220px" ItemStyle-HorizontalAlign="Center" />
                                            <asp:BoundField HeaderText="Uses" DataField="TotalUses" DataFormatString="{0:0.00}" HtmlEncode="false" HeaderStyle-Width="100px" ItemStyle-HorizontalAlign="Center" />
                                            <asp:BoundField HeaderText="Duration (hours)" DataField="TotalActDuration" DataFormatString="{0:0.00}" HtmlEncode="false" HeaderStyle-Width="100px" ItemStyle-HorizontalAlign="Center" />
                                            <asp:BoundField HeaderText="Account" DataField="AccountName" HeaderStyle-Width="200px" ItemStyle-HorizontalAlign="Center" />
                                            <asp:BoundField DataField="ShortCode" HeaderText="Short Code" HeaderStyle-Width="90px" ItemStyle-HorizontalAlign="Center" />
                                            <asp:BoundField DataField="OrgName" Visible="false" HeaderText="Org" ItemStyle-HorizontalAlign="Center"></asp:BoundField>
                                            <asp:BoundField HeaderText="Line Cost" DataField="TotalCalcCost" DataFormatString="{0:$0.00}" HtmlEncode="false" HeaderStyle-Width="90px" ItemStyle-HorizontalAlign="Center" />
                                        </Columns>
                                        <EmptyDataTemplate>
                                            &nbsp;&nbsp;-- There is no record of forgiven reservation for this period --&nbsp;&nbsp;
                                        </EmptyDataTemplate>
                                    </asp:GridView>
                                    <span style="float: right; padding-right: 30px; font-size: small;">
                                        <asp:Label ID="lblForgivenToolFee2" ForeColor="Brown" runat="server"></asp:Label></span>
                                </div>
                            </div>
                            <!-- Store -->
                            <div style="margin-left: 0px">
                                <h4>
                                    <asp:Label ID="lblStore2" ForeColor="Red" runat="server">Store Usage</asp:Label></h4>
                                <asp:GridView runat="server" ID="gvStore2" CssClass="gridview" GridLines="None" AutoGenerateColumns="False">
                                    <HeaderStyle CssClass="header" />
                                    <RowStyle CssClass="row" />
                                    <AlternatingRowStyle CssClass="altrow" />
                                    <SelectedRowStyle CssClass="selected" />
                                    <FooterStyle CssClass="footer" />
                                    <PagerStyle CssClass="pager" />
                                    <PagerSettings Mode="NumericFirstLast" FirstPageText="First" LastPageText="Last" />
                                    <Columns>
                                        <asp:BoundField DataField="Item" HeaderText="Item" ItemStyle-HorizontalAlign="Center">
                                            <HeaderStyle Width="220px"></HeaderStyle>
                                        </asp:BoundField>
                                        <asp:BoundField DataField="Name" HeaderText="Account" ItemStyle-HorizontalAlign="Center">
                                            <HeaderStyle Width="200px"></HeaderStyle>
                                        </asp:BoundField>
                                        <asp:BoundField DataField="OrgName" Visible="false" HeaderText="Org" ItemStyle-HorizontalAlign="Center"></asp:BoundField>
                                        <asp:BoundField DataField="ShortCode" HeaderText="Short Code" HeaderStyle-Width="90px" ItemStyle-HorizontalAlign="Center" />
                                        <asp:BoundField DataField="CalcCost" HeaderText="Line Cost" ItemStyle-HorizontalAlign="Center" DataFormatString="{0:$#,##0.00}" HtmlEncode="false">
                                            <HeaderStyle Width="90px"></HeaderStyle>
                                        </asp:BoundField>
                                    </Columns>
                                </asp:GridView>
                            </div>
                        </div>
                    </div>
                </asp:Panel>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

<asp:Content runat="server" ID="Content3" ContentPlaceHolderID="scripts">
    <script type="text/javascript">
        //this is because of the UpdatePanel control
        Sys.Application.add_load(function () {
            $('#tabs1').tabs();
        });
    </script>
</asp:Content>
