<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ViewRewardChoiceHistory.ascx.cs" Inherits="LoyaltyWareWebsite.Controls.Modules.RewardChoiceHistory.ViewRewardChoiceHistory" %>
<div id="RewardChoiceHistoryContainer">
	<h2 id="Title">
		<asp:Literal runat="server" ID="litTitle" meta:resourcekey="Title"></asp:Literal>
	</h2>
	<div class="table_wrapper">
		<div class="table_wrapper_inner">
			<asp:GridView runat="server" ID="grdHistory" AutoGenerateColumns="false" GridLines="none" AllowPaging="true" PageSize="10" meta:resourcekey="grid" PagerStyle-CssClass="pager">
				<EmptyDataTemplate>
					<asp:Literal runat="server" ID="litNoData" meta:resourcekey="NoData"></asp:Literal>
				</EmptyDataTemplate>
				<RowStyle CssClass="first" />
				<AlternatingRowStyle CssClass="second" />
				<PagerSettings Mode="NumericFirstLast" PageButtonCount="6" FirstPageText="First" LastPageText="Last" PreviousPageText="Previous" NextPageText="Next" />
				<Columns>
					<asp:BoundField DataField="Date" runat="server" meta:resourcekey="Date" HeaderStyle-HorizontalAlign="Left" SortExpression="Date" />
					<asp:BoundField DataField="Reward" runat="server" meta:resourcekey="Reward" HeaderStyle-HorizontalAlign="Left" SortExpression="Reward" />
					<asp:BoundField DataField="ChangedBy" runat="server" meta:resourcekey="ChangedBy" HeaderStyle-HorizontalAlign="Left" SortExpression="ChangedBy" />
				</Columns>
			</asp:GridView>
		</div>
	</div>
</div>
