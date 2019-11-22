<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ViewMessages.ascx.cs"
    Inherits="Brierley.LWModules.Messages.ViewMessages" %>
<%@ Register Assembly="Brierley.WebFrameWork" Namespace="Brierley.WebFrameWork.Controls.Mobile" TagPrefix="Mobile" %>
<%@ Register Src="~/Controls/DateRangeFilter.ascx" TagName="DateFilter" TagPrefix="Controls" %>
<%@ Import Namespace="Brierley.WebFrameWork.Portal.Configuration.Modules" %>

<div id="MessagesContainer">
    <h2 id="Title">
        <asp:Literal runat="server" ID="litTitle" meta:resourcekey="Title"></asp:Literal>
    </h2>

    <div id="MessageList">
		<div class="row">
			<div class="col-lg-8 col-sm-7 col-xs-12">
				<div id="StatusFilter" class="form-inline text-right">
					<label>
						<asp:literal runat="server" meta:resourcekey="show"></asp:literal></label>
					<label class="checkbox">
						<input type="checkbox" name="chkUnread" id="chkUnread" <%= (Config.DefaultFilter & MessagesConfig.DefaultFilters.Unread) != 0 ? " checked" : string.Empty %>/>
						<%=GetResource("Unread.Text") %>
					</label>
					<label class="checkbox">
						<input type="checkbox" name="chkRead" id="chkRead" <%= (Config.DefaultFilter & MessagesConfig.DefaultFilters.Read) != 0 ? " checked" : string.Empty %>/>
						<%=GetResource("Read.Text") %>
					</label>
					<% if (Config.AllowViewDeleted) { %>
					<label class="checkbox">
						<input type="checkbox" name="chkDeleted" id="chkDeleted" <%= (Config.DefaultFilter & MessagesConfig.DefaultFilters.Deleted) != 0 ? " checked" : string.Empty %>/>
						<%=GetResource("Deleted.Text") %>
					</label>
					<% } %>
				</div>
			</div>

			<div class="col-lg-4 col-sm-5 col-xs-12">
				<div id="MessageSorting" class="form-inline message-filter text-right">
					<label><asp:Literal runat="server" meta:resourcekey="OrderBy"></asp:Literal></label>
					<span>
						<label class="radio">
							<input id="rdoNewest" name="rdoOrder" type="radio" <%= Config.DefaultOrder == MessagesConfig.MessageDefaultOrder.Newest ? " checked" : string.Empty %>>
							<%=GetResource("Newest") %>
						</label>
						<label class="radio">
							<input id="rdoOldest" name="rdoOrder" type="radio"<%= Config.DefaultOrder == MessagesConfig.MessageDefaultOrder.Oldest ? " checked" : string.Empty %>>
							<%=GetResource("Oldest") %>
						</label>
					</span>
				</div>
			</div>
		</div>	
		<div class="row">
			<div class="DateRangeFilter text-right">
				<Controls:DateFilter runat="server" id="DateFilter"></Controls:DateFilter>
			</div>
		</div>

        <div class="wait-animation"></div>

		<label id="lblNoMessages" style="display:none;">
			<asp:literal runat="server" ID="litNoResults" meta:resourcekey="EmptyMessage"></asp:literal>
		</label>
		
		<div id="Messages">
			<asp:ListView runat="server" id="lstMessages">
					<LayoutTemplate>
						<asp:PlaceHolder runat="server" ID="ItemPlaceholder"></asp:PlaceHolder>
					</LayoutTemplate>
					<ItemTemplate>
						<a href="#" class="message <%# Eval("Status").ToString().ToLower()%>" data-messageid="<%#Eval("Id")%>" data-messagedefid="<%# Eval("MessageDefId")%>">
							<div class="row">
								<div class="col-sm-10 col-xs-12 message-description">
									<span class="icon"></span>
									<%#Eval("Description")%>
								</div>
								<div class="col-sm-2 col-xs-12 message-date">
									<%#Eval("DateIssued")%>
								</div>
							</div>
						</a>
						<span class="message-separator"></span>
					</ItemTemplate>
			</asp:ListView>
		</div>
		
		<div class="pager">
			<label><%=GetResource("Page")%></label>
			<label id="lblPage"></label>
			<label><%=GetResource("Of")%></label>
			<label id="lblPages"></label>
			<a href="#" id="btnPrevious" class="btn"><%=GetResource("btnPrevious.Text")%></a>
			<a href="#" id="btnNext" class="btn"><%=GetResource("btnNext.Text")%></a>
		</div>

	</div>

	<div id="MessageDetail">
        <div class="wait-animation"></div>
		<h3></h3>
		<div id="MessageContent"></div>
		<div class="buttons">
			<a class="btn" id="btnDone"><%=GetResource("btnDone.Text") %></a>
			<% if(Config.AllowDelete) { %>
			<a class="btn pull-right" id="btnDelete"><%=GetResource("btnDelete.Text") %></a>
			<% } %>
			<a class="btn pull-right" id="btnMarkRead"><%=GetResource("btnMarkRead.Text") %></a>
			<a class="btn pull-right" id="btnMarkUnread"><%=GetResource("btnMarkUnread.Text") %></a>
			<a class="btn pull-right" id="btnUndelete"><%=GetResource("btnUndelete.Text") %></a>
		</div>
	</div>

	<script type="text/javascript">

		$(document).ready(function () {
			var autoRead = 0;
			<% 
			if(Config.AutoMarkRead == MessagesConfig.MessageAutoMarkRead.Immediate) 
			{ 
				%> autoRead = 10; <% 
			}
			else if (Config.AutoMarkRead == MessagesConfig.MessageAutoMarkRead.Delayed)
			{
				%> autoRead = 5000; <%
			}
			%>

			messages.init(autoRead, <%=Config.MessagesPerPage%>, '<%=GetResource("ConfirmDelete") %>');
			rangeFilter.init();
			rangeFilter.onChange = function (range) {
				messages.getMessages();
			};
			messages.getMessages();
		});
	</script>
</div>
