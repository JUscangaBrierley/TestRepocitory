<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ViewMessages.ascx.cs"
    Inherits="Brierley.LWModules.Messages.ViewMessages" %>
<div id="MessagesContainer">
    <h2 id="Title">
        <asp:Literal runat="server" ID="litTitle"></asp:Literal>
    </h2>
    <div id="MessageList">
        <asp:PlaceHolder ID="phMessagesList" runat="server" />        
    </div>    
</div>
