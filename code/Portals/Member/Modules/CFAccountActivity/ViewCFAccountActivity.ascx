<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ViewCFAccountActivity.ascx.cs" Inherits="Brierley.LWModules.CFAccountActivity.ViewCFAccountActivity" %>

<script  language="javascript">
</script>
<div id="CFAccountActivityContainer">

	<h2 id="Title">
		<asp:Label runat="server" ID="lblModuleTitle" />
	</h2>

	<div id="CFAccountActivity">
		<asp:PlaceHolder runat="server" ID="pchTxnHeaderData"></asp:PlaceHolder>
        <asp:PlaceHolder runat="server" ID="pchOrphansDate"></asp:PlaceHolder>
	</div>
</div>
