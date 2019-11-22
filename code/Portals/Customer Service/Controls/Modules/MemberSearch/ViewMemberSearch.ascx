<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ViewMemberSearch.ascx.cs" Inherits="Brierley.LWModules.MemberSearch.ViewMemberSearch" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<div id="MemberSearchContainer">

	<asp:PlaceHolder runat="server">

	<h2 id="Title">
        <%= GetLocalResourceObject("ModuleTitle.Text") %>
	</h2>

	<script type="text/javascript">
		$(document).ready(
			function () {
				$('#MemberSearchControls').keypress(function (event) {
					if (event.which == '13') {
						event.preventDefault();
						<%= this.Page.ClientScript.GetPostBackEventReference(_btnSearch, string.Empty) %>;
					}
				});
			}
		);

		function ClearSearch(){
			$('#MemberSearch input:checkbox').each(function(){
				$(this).removeAttr('checked');
			});
			$('#MemberSearch input[type=text]').each(function(){
				$(this).val('');
			});
			return false;
		}
	</script>

	</asp:PlaceHolder>

	<div id="MemberSearch">



		<div id="MemberSearchControls">
			<asp:table id="pnlSearchFormTable" summary="Search Form" runat="server" Visible="false">
			</asp:table>

			<asp:table id="tblSearchCtrls" summary="Search Result" runat="server">
			</asp:table>
		</div>


		<div id="MemberSearchResults">
			<asp:PlaceHolder id="phCSMemberSearch" runat="server" Visible="true"/>
		</div>


	</div>

</div>