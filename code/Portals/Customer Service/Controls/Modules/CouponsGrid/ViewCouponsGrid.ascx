<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ViewCouponsGrid.ascx.cs"
    Inherits="Brierley.LWModules.CouponsGrid.ViewCouponsGrid" %>
    <%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Assembly="Brierley.WebFrameWork" Namespace="Brierley.WebFrameWork.Controls"
    TagPrefix="WebFramework" %>
<%@ Import Namespace="Brierley.WebFrameWork.Portal" %>
<div id="CouponsContainer">
    <h2 id="Title">
        <asp:Literal runat="server" ID="litTitle"></asp:Literal>
    </h2>
    <div id="CouponList">
        <asp:PlaceHolder ID="phCouponsList" runat="server" />
        <asp:PlaceHolder runat="server" ID="pchNoCoupons" Visible="false"></asp:PlaceHolder>
    </div>
</div>
