<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ViewCSUserAdministration.ascx.cs" Inherits="Brierley.LWModules.CSUserAdministration.ViewCSUserAdministration" %>
<%@ Register TagPrefix="cc1" Namespace="Brierley.WebFrameWork.Controls" Assembly="Brierley.WebFrameWork" %>

<div id="CSUserAdministrationContainer">

	<h2 id="Title">
		<asp:Literal runat="server" ID="litTitle"></asp:Literal>
	</h2>
	    
	<cc1:LWApplicationPanel runat="server" ID="pnlCSUserAdministration" resourcekey="pnlLeft" Visible="true">
		<Tabs>
			<cc1:Tab ID="Tab1" runat="server" Text="Agents" CommandName="CSAgents" ToolTip="Customer service agents." />
			<cc1:Tab ID="Tab2" runat="server" Text="Roles" CommandName="Roles" ToolTip="Customer service roles." />        
		</Tabs>
		<Content>
			<asp:MultiView runat="server" ID="CSUserAdminView">                                                
				<asp:View runat="server" ID="CSAgentsView">

				<script type="text/javascript">

					$(document).ready(function () {
						var ddl = $('#<%=ddlSearchOptions.ClientID %>');
						ddl.bind('change', function () { SetSearchOptions(true); });
						SetSearchOptions(false);
					}
					);

					function ValidateSearchCriteria(sender, args) {
						var ddl = $('#<%=ddlSearchOptions.ClientID %>');
						var txt = $('#<%=txtSearchValue.ClientID %>');
						if (ddl.val() == '' || ddl.val() == ' ') {
							args.IsValid = false;
							$(sender).text('Please select a property to search on.');
							sender.errormessage = 'Please select a property to search on.';
							return;
						}

						if (ddl.val() != 'Role' && ddl.val() != 'Status' && txt.val() == '') {
							args.IsValid = false;
							$(sender).text('Please enter' + (ddl.val() == 'Email Address' ? ' an ' : ' a ') + ddl.val());
							sender.errormessage = 'Please enter a ' + ddl.val();
							return;
						}
						args.IsValid = true;
					}

					function SetSearchOptions(clear) {
						var ddl = $('#<%=ddlSearchOptions.ClientID %>');
						var txt = $('#<%=txtSearchValue.ClientID %>');
						var role = $('#<%=ddlRole.ClientID %>');
						var status = $('#<%=ddlAccountStatus.ClientID %>');
						if (ddl.val() == 'Role') {
							txt.hide();
							status.hide();
							role.show();
						}
						else if (ddl.val() == 'Status') {
							txt.hide();
							status.show();
							role.hide();
						}
						else {
							txt.show();
							status.hide();
							role.hide();
							if (clear) {
								txt.val('');
								txt.focus();
							}
						}
					}
				</script>

				<table cellspacing="0" cellpadding="2" border="0">
					<tr>
						<td>
							<br />
						</td>
					</tr>
					<tr>
						<td>
							<asp:Label ID="lblSearchRange" runat="server" Text="Search:" Visible="true"></asp:Label>
						</td>
						<td>
							<asp:DropDownList runat="server" ID="ddlSearchOptions">
								<asp:ListItem Value=" "> Select Search Criteria </asp:ListItem>
								<asp:ListItem Value="Role">Role</asp:ListItem>
								<asp:ListItem Value="Name">Name</asp:ListItem>
								<asp:ListItem Value="Email Address">Email Address</asp:ListItem>
								<asp:ListItem Value="Phone Number">Phone Number</asp:ListItem>
								<asp:ListItem Value="Username">Username</asp:ListItem>
								<asp:ListItem Value="Status">Status</asp:ListItem>
							</asp:DropDownList>                        
						</td>
						<td>
							<asp:TextBox ID="txtSearchValue" runat="server" Visible="true" />

							<asp:DropDownList runat="server" ID="ddlRole"></asp:DropDownList>

							<asp:DropDownList runat="server" ID="ddlAccountStatus"></asp:DropDownList>			
						</td>
						<td>
							<asp:Button ID="btnSearch" runat="server" Text="Search" Visible="true" ValidationGroup="Search"/><br />
						</td>
					</tr>
					<tr>
						<td colspan="4">
							<asp:CustomValidator ErrorMessage="" runat="server" ID="vldCustomSearch" Display="Dynamic" EnableClientScript="true" ClientValidationFunction="ValidateSearchCriteria" ValidationGroup="Search" CssClass="Validator"></asp:CustomValidator>
						</td>
					</tr>
				</table>

					<asp:PlaceHolder ID="phCSAgents" runat="server" Visible="false" />
					<br />
					<cc1:LWApplicationPanel runat="server" ID="pnlPwdChange" Visible="false">
						<Content>
							<asp:Panel ID="pnlPassword" runat="server" Visible="true" meta:resourcekey="pnlPassword">
									<table style="margin: 3px; font: bold 12px arial; color: #687e18;">
										<tr>
											<td valign="top" style="width: 140px; padding: 0px 5px 5px 5px;">
												<asp:Label ID="lblPassword1" runat="server" CssClass="radLabelCss_Default" meta:resourcekey="lblPassword1">New Password:</asp:Label>
											</td>
											<td runat="server" style="width: 200px;">                                                                                                                                   
												<asp:TextBox TextMode="Password" ID="txtPassword1" runat="server" Width="200px"></asp:TextBox>                                            
											</td>
											<td>
												<asp:RequiredFieldValidator ID="rqPassword1" runat="server" ControlToValidate="txtPassword1" Display="Dynamic" ErrorMessage="Please enter a New Password" CssClass="Validator" />
												<asp:RegularExpressionValidator ID="rePassword1" runat="server" ControlToValidate="txtPassword1" ValidationExpression="^.{1,200}$" ErrorMessage="New Password: max 200 characters" CssClass="Validator" />
											</td>
										</tr>
										<tr>
											<td valign="top" style="width: 140px; padding: 0px 5px 5px 5px;">
												<asp:Label ID="Label1" runat="server" CssClass="radLabelCss_Default" meta:resourcekey="lblPassword1">Confirm Password:</asp:Label>
											</td>
											<td runat="server" style="width: 200px;">                                                                                                                                   
												<asp:TextBox TextMode="Password" ID="txtPassword2" runat="server" Width="200px"></asp:TextBox>                                            
											</td>
											<td>
												<asp:RequiredFieldValidator ID="rqPassword2" runat="server" ControlToValidate="txtPassword2" Display="Dynamic" ErrorMessage="Please Confirm the password." CssClass="Validator" />
												<asp:RegularExpressionValidator ID="rePassword2" runat="server" ControlToValidate="txtPassword2" Display="Dynamic" ValidationExpression="^.{1,200}$" ErrorMessage="Confirm Password: max 200 characters" CssClass="Validator" />
												<asp:CompareValidator ID="cmpPassword2" runat="server" ControlToValidate="txtPassword2" ControlToCompare="txtPassword1" ErrorMessage="New Password and Confirm Password must match." Type="String" CssClass="Validator" />
											</td>
										</tr>                                    
										<tr>
											<td colspan="3" align="left">
												<cc1:LWLinkButton ButtonType="Submit" ID="btnPasswordSave" Text="Save" OnClick="btnPassword_Save" runat="server" />
												<cc1:LWLinkButton ButtonType="Cancel" ID="btnPasswordCancel" Text="Cancel" OnClick="btnPassword_Cancel" runat="server" CausesValidation="false" />
											</td>
										</tr>
									</table>
								</asp:Panel>
						</Content>
					</cc1:LWApplicationPanel>
				</asp:View>
				<asp:View runat="server" ID="CSRolesView">
                    <script type="text/javascript">
                        function ValidateCheckedFunctions(sender, args) {
                            args.IsValid = false;
                            $('input[id*="AssociatedFunctions"]').each(function () {
                                if ($(this).is(":checked")) {
                                    args.IsValid = true;
                                    return;
                                }
                            });
                        }
                    </script>
					<asp:PlaceHolder ID="phCSRoles" runat="server" Visible="false" />
				</asp:View>
			</asp:MultiView>
		</Content>
	</cc1:LWApplicationPanel>
</div>

<script type="text/javascript">
    function ValidateExpireDate() {
	    var result = true;
	    var dateInput = $('td > span > input[id$="_PasswordExpireDate_dateInput"]');
	    var validationMsg = $('span[controltovalidate$="_PasswordExpireDate"]');

	    if (dateInput.length > 0 &&
            dateInput.val().trim() != "" && 
            isNaN(Date.parse(dateInput.val()))) {
		    if (validationMsg.length > 0 ) {
			    validationMsg.css("display", "block");
		    }
		    result = false;
	    }
	    return result;
    }
</script>