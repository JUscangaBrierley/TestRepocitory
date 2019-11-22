<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ViewMTouchProcessing.ascx.cs" Inherits="Brierley.LWModules.MTouchProcessing.ViewMTouchProcessing" %>

<div id="MTouchProcessingContainer">
	<asp:MultiView runat="server" ID="mvMain">
		<asp:View runat="server" ID="EmptyView">
		</asp:View>
		<asp:View runat="server" ID="SuccessView">
			<div class="Positive">
				<asp:Label runat="server" ID="lblSuccess"></asp:Label>
			</div>
		</asp:View>
		<asp:View runat="server" ID="ErrorView">
			<div class="Negative">
				<asp:Label runat="server" id="lblError"></asp:Label>
			</div>
		</asp:View>
		<asp:View runat="server" ID="QuotaMetView">
			<asp:Literal runat="server" ID="litQuotaMet"></asp:Literal>
		</asp:View>
	</asp:MultiView>



</div>