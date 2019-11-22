<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ViewCookieWriter.ascx.cs" Inherits="Brierley.LWModules.CookieWriter.ViewCookieWriter" %>

<div id="CookieWriterContainer">

	<h2 id="Title" runat="server">
		<asp:Label runat="server" ID="lblModuleTitle" />
	</h2>

	<div id="CookieWriter">
		<asp:PlaceHolder ID="phFieldsToShow" runat="server" />
	</div>
</div>
