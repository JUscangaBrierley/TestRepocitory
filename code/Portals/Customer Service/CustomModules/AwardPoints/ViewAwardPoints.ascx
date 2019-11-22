<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ViewAwardPoints.ascx.cs"
    Inherits="Brierley.AEModules.AwardPoints.ViewAwardPoints" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<style type="text/css">
    .banner
    {
        text-indent: 10px;
        background-color: #c0c0c0;
    }
</style>
<asp:Panel ID="pnlBanner" runat="server" CssClass="banner" Width="100%">
    <h1>
        <asp:Label ID="lblBanner" runat="server" Width="100%"></asp:Label></h1>
</asp:Panel>
<br />
<br />
<asp:Panel ID="pnlMain" runat="server">
    <table width="100%">
        <tr>
            <td align="left">
                 <%--AEO Redesign 2015 Begin  --%>
                <asp:Button ID="btnAwardBonus" runat="server" Font-Bold="true" Text="Point/Credit Adjustments"
                    OnClick="btnAwardBonus_Click" />
                 <%--AEO Redesign 2015 End--%>
            </td>
        </tr>
    </table>
</asp:Panel>
<br />
<asp:Panel ID="pnlForm" runat="server">
    <table width="100%">
        <asp:Panel ID="pnlPointType" runat="server">
            <tr>
                <td align="left">
                    <asp:Label ID="lblPointType" runat="server" Font-Bold="true" Text="Point Type"></asp:Label>
                </td>
            </tr>
            <tr>
                <td align="left">
                    <asp:DropDownList ID="ddlPointType" runat="server">
                    </asp:DropDownList>
                    <asp:RequiredFieldValidator ID="rqPointType" runat="server" ControlToValidate="ddlPointType"
                        SetFocusOnError="true" ForeColor="Red" ErrorMessage="Please select a Bonus Type."
                        InitialValue="0"  ValidationGroup="valSave"></asp:RequiredFieldValidator>
                </td>
            </tr>
        </asp:Panel>
        <asp:Panel ID="pnlEvent" runat="server">
            <tr>
                  <td align="left">
                    <asp:Label ID="Label1" runat="server" Font-Bold="true" Text="Adjustment Type"></asp:Label>
                </td>
                
            </tr>
            <tr>
                <td align="left">
                   
                <%--AEO Redesign 2015 Begin --%>
                    <asp:DropDownList ID="ddlPointEvent" runat="server"  AutoPostBack="true" OnSelectedIndexChanged ="ddlPointEvent_SelectedIndexChanged">
                    </asp:DropDownList>
                    <asp:RequiredFieldValidator ID="rqPointEvent" runat="server" ForeColor="Red" ControlToValidate="ddlPointEvent"
                        ErrorMessage="Please select a Bonus Type." SetFocusOnError="true" InitialValue="0" 
                        ValidationGroup="valSave"></asp:RequiredFieldValidator>
                    <%--AEO Redesign 2015 end --%>
                </td>
                 <%--AEO Redesign 2015 Begin --%>
                <td align="left">
                    <asp:Label ID="lblBraMessage" runat="server" Font-Bold="true" Text=""></asp:Label>
                </td>
                <%--AEO Redesign 2015 end --%>
            </tr>
        </asp:Panel>
        <asp:Panel ID="pnlTransDate" runat="server">
            <tr>
                  <%--AEO Redesign 2015 BEGIN --%>
                <td align="left">
                    <asp:Label ID="lblTxnDate" runat="server" Font-Bold="true" Text="Point Award Date"></asp:Label>
                </td>
                  <%--AEO Redesign 2015 END --%>
            </tr>
            <tr>
                <td align="left">
                    <telerik:RadDatePicker ID="rdpMonthYear" runat="server" Width="140px" DateInput-EmptyMessage="Select Date"
                        MinDate="01/01/1000" MaxDate="01/01/3000">
                        <Calendar runat="server">
                            <SpecialDays>
                                <telerik:RadCalendarDay Repeatable="Today" />
                            </SpecialDays>
                        </Calendar>
                    </telerik:RadDatePicker>
                    <asp:RequiredFieldValidator ID="rqAwardDate" runat="server" ControlToValidate="rdpMonthYear"
                        ErrorMessage="Please select point award date." SetFocusOnError="true" ForeColor="Red"
                        ValidationGroup="valSave"></asp:RequiredFieldValidator>
                </td>
            </tr>
        </asp:Panel>
        <tr>
            <%-- AEO-Redesing 2015 begin--%>
            <td align="left">
                <asp:Label ID="lblPoints" runat="server" Font-Bold="true" Text="Number of Points"></asp:Label>
            </td>
            <%-- AEO-Redesing 2015 end--%>
        </tr>
        <tr>
            <td align="left">
                <asp:TextBox ID="txtPoints" runat="server" MaxLength="10"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rqPoints" runat="server" ControlToValidate="txtPoints"
                    SetFocusOnError="true" ForeColor="Red" ErrorMessage="Please enter a valid number of Points."
                    ValidationGroup="valSave" Display="Dynamic"></asp:RequiredFieldValidator>
                <asp:RangeValidator ID="rngPoints" runat="server" ControlToValidate="txtPoints" ValidationGroup="valSave"
                    Display="Dynamic" Type="Double" SetFocusOnError="true" ForeColor="Red" MinimumValue="-9999999999"
                    MaximumValue="9999999999"></asp:RangeValidator>
                <asp:RegularExpressionValidator ID="rePoints" runat="server" ErrorMessage="Please provide whole numbers only."
                    ControlToValidate="txtPoints" SetFocusOnError="true" Display="Dynamic" ValidationGroup="valSave"
                    ValidationExpression="^-?\d+$" ForeColor="Red"></asp:RegularExpressionValidator>
            </td>
           
               <%-- AEO-Redesing 2015 begin--%>
            <td align="left">
                <asp:Label ID="lblJeanMessage" runat="server" Font-Bold="true" Text=""></asp:Label>
            </td>
            <%-- AEO-Redesing 2015 end--%>
           
        </tr>
        <asp:Panel ID="pnlNotes" runat="server">
            <tr>
                <td align="left">
                    <asp:Label ID="Label4" runat="server" Font-Bold="true" Text="Additional Notes"></asp:Label>
                </td>
            </tr>
            <tr>
                <td align="left">
                    <asp:TextBox ID="txtNotes" runat="server" TextMode="MultiLine" Height="66px" Width="435px"></asp:TextBox>
                </td>
            </tr>
        </asp:Panel>
        <tr>
            <td align="left">
                <asp:Label ID="lblSuccess" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td align="left">
                <asp:LinkButton ID="bntSave" runat=server Text="SUBMIT" ValidationGroup="valSave" OnClick="btnSave_Click"></asp:LinkButton>
                &nbsp;&nbsp;
                <asp:LinkButton ID="btnCancel" runat=server Text="CANCEL" OnClick="btnCancel_Click"></asp:LinkButton>

<%--                <asp:Button ID="btnSave" runat="server" Text="SUBMIT" ValidationGroup="valSave" OnClick="btnSave_Click" />
                &nbsp;&nbsp;
                <asp:Button ID="btnCancel" runat="server" Text="CANCEL" OnClick="btnCancel_Click" />
--%>            </td>
        </tr>
    </table>
</asp:Panel>
