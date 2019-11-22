<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ViewSelectLanguage.ascx.cs" Inherits="Brierley.LWModules.SelectLanguage.ViewSelectLanguage" %>
<div id="SelectLanguageContainer">
	<asp:Label runat="server" ID="lblLanguagePrompt" />
	<asp:DropDownList runat="server" ID="drpLanguages" AutoPostBack="true" />
</div>