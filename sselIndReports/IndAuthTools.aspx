<%@ Page Title="" Language="C#" MasterPageFile="~/IndReportsMaster.Master" AutoEventWireup="true" CodeBehind="IndAuthTools.aspx.cs" Inherits="sselIndReports.IndAuthTools" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .report-container {
            width: 350px;
            margin-bottom: 10px;
        }

            .report-container .tool-auth-table {
                width: 100%;
            }

            .lab-name{
                padding-left: 0;
            }

            .proc-tech-name{
                padding-left: 10px;
            }

            .resource-name{
                padding-left: 25px;
            }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="section">
        <h2>User Tool Authorization</h2>
        <div class="criteria">
            <div class="criteria-item">
                <asp:CheckBox ID="chkActive" runat="server" Visible="false" Text="Includes Inactive Users" AutoPostBack="true" OnCheckedChanged="ChkActive_CheckedChanged" />
            </div>
            <div class="criteria-item">
                Select user:
                <asp:DropDownList runat="server" ID="ddlUser" CssClass="report-select"></asp:DropDownList>
                <asp:Label runat="server" ID="lblUserMessage" ForeColor="Red"></asp:Label>
            </div>
            <div class="criteria-item">
                <asp:Button runat="server" ID="btnReport" Text="Retrieve Data" OnClick="BtnReport_Click" CssClass="report-button" />
                <asp:LinkButton runat="server" ID="btnBack" Text="&larr; Back to Main Page" OnClick="BackButton_Click" CssClass="back-link"></asp:LinkButton>
            </div>
        </div>
    </div>
    <div class="section">
        <div class="report-container">
            <asp:Literal runat="server" ID="litNoData"></asp:Literal>
            <asp:DataGrid runat="server" ID="dgAuthTools" AutoGenerateColumns="False" CssClass="outline tool-auth-table" OnItemDataBound="DgAuthTools_ItemDataBound">
                <HeaderStyle CssClass="GridHeader"></HeaderStyle>
                <Columns>
                    <asp:TemplateColumn HeaderText="Authorized Tools">
                        <ItemTemplate>
                            <asp:Literal runat="server" ID="litAuthTool"></asp:Literal>
                        </ItemTemplate>
                    </asp:TemplateColumn>
                </Columns>
            </asp:DataGrid>
        </div>
    </div>
</asp:Content>
