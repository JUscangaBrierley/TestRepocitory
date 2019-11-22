<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ViewPointCalculator.ascx.cs" Inherits="Brierley.AEModules.PointsCalculator.ViewPointCalculator" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<style type="text/css">
    .banner
    {
       
        background-color: #c0c0c0;    
    }
    .right_align  
    {
        float:right; 
        word-wrap: normal ; 
       
         text-align: left;
          margin-right:15px;
    }
     .left_align  
    {
        float:left; 
        word-wrap: normal ; 
        word-break: break-all;
         text-align: left;
         margin-right:10px;         
    }
</style>

<asp:Panel ID="pnlFormPointCalculator"  runat="server"  > 

  <asp:Panel ID="panelBanner"    Cssclass="banner" runat="server" >
    <asp:Label ID="Label4" runat="server"   Text="Point Calculator" Font-Bold="True" Font-Size="Larger"  ></asp:Label>   
    <asp:Label ID="Label6"  runat="server" Text="*Point Calculator includes Base Points Only" CssClass="right_align" Font-Italic="True"></asp:Label>
    </asp:Panel >   
      <br />
    <br />
<asp:Panel ID="pnlFormPointCalculator2"  runat="server"  > 
    <asp:Label ID="Label2"  CssClass="left_align" runat="server" Font-Bold="True" Text="Amount Spent ($):"></asp:Label>
    <asp:TextBox ID="AmountSpentTxtBox" runat="server"></asp:TextBox>
    <asp:RangeValidator ID="rngPoints" runat="server" ControlToValidate="AmountSpentTxtBox" Display="Dynamic" ForeColor="Red" MaximumValue="9999999999" MinimumValue="-9999999999" SetFocusOnError="true" Type="Double" ValidationGroup="valSave"></asp:RangeValidator>
    <asp:RegularExpressionValidator ID="rePoints" runat="server" ControlToValidate="AmountSpentTxtBox" Display="Dynamic" ErrorMessage="Please provide whole numbers only." ForeColor="Red" SetFocusOnError="true" ValidationExpression="^-?\d+$" ValidationGroup="valSave"></asp:RegularExpressionValidator>
    <!--   </asp:Panel> -->
   <br>

    <!--  <asp:Panel ID="Panel2" runat="server" Height="19px" Width="508px"> -->
    <%--<asp:Label ID="Label3" CssClass="left_align"   runat="server" Font-Bold="True" Text="Base Points :  "></asp:Label>
    <asp:Label ID="BasePointslbl" runat="server" Text="Label"></asp:Label>
    <!--  </asp:Panel> -->
    <br /> 
    <asp:Label ID="Label5" CssClass="left_align"  runat="server" Font-Bold="True" Text="Tier Points     :  "></asp:Label>
    <asp:Label ID="TierPointslbl" runat="server" Text="Label"></asp:Label>
   <br /> --%>
    <asp:Label ID="Label7" CssClass="left_align"  runat="server" Font-Bold="True" Text="Total Points :    "></asp:Label>
    <asp:Label ID="TotalPointslbl" runat="server" Text=""></asp:Label>  <asp:Label ID="Totalpointvalidatorlabel" runat="server" Font-Italic="True" ForeColor="Red"></asp:Label>
   <br />
    <br />
    <asp:LinkButton ID="SubmitButton" CssClass="left_align"  runat="server" OnClick="SubmitButton_Click">CALCULATE</asp:LinkButton>
      </asp:Panel>

</asp:Panel>
<br>

