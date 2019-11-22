<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ViewCSUserAdministration.ascx.cs" Inherits="Brierley.LWModules.CSUserAdministration.ViewCSUserAdministration" %>
<%@ Register TagPrefix="cc1" Namespace="Brierley.WebFrameWork.Controls" Assembly="Brierley.WebFrameWork" %>

<div id="CSUserAdministrationContainer">

	<h2 id="Title">
		<asp:Literal runat="server" ID="litTitle"></asp:Literal>
	</h2>
	    
	<cc1:LWApplicationPanel runat="server" ID="pnlCSUserAdministration" resourcekey="pnlLeft" Visible="true">
		<Tabs>
			<cc1:Tab ID="Tab1" runat="server" CommandName="CSAgents" meta:resourcekey="tabAgents" /> <%--Text="Agents" ToolTip="Customer service agents."--%>
			<cc1:Tab ID="Tab2" runat="server" CommandName="Roles" meta:resourcekey="tabRoles" /> <%--Text="Roles" ToolTip="Customer service roles."--%>
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
							$(sender).text('<%= GetLocalResourceObject("PropertySearchMessage.Text") %>');
						    sender.errormessage = '<%= GetLocalResourceObject("PropertySearchMessage.Text") %>';
							return;
						}

						if (ddl.val() != 'Role' && ddl.val() != 'Status' && txt.val() == '') {
						    args.IsValid = false;
						    var message = ddl.val() == 'Email Address' ? '<%= GetLocalResourceObject("PleaseEnterAn.Text") %>' : '<%= GetLocalResourceObject("PleaseEnterA.Text") %>';
						    $(sender).text(message + ' ' + ddl.val());
						    sender.errormessage = message + ' ' + ddl.val();
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
							<asp:Label ID="lblSearchRange" runat="server" meta:resourcekey="lblSearch" Visible="true"></asp:Label>
						</td>
						<td>
							<asp:DropDownList runat="server" ID="ddlSearchOptions">
								<asp:ListItem Value=" " meta:resourcekey="optSelectSearchCriteria"></asp:ListItem>
								<asp:ListItem Value="Role" meta:resourcekey="optRole"></asp:ListItem>
								<asp:ListItem Value="Name" meta:resourcekey="optName"></asp:ListItem>
								<asp:ListItem Value="Email Address" meta:resourcekey="optEmailAddress"></asp:ListItem>
								<asp:ListItem Value="Phone Number" meta:resourcekey="optPhoneNumber"></asp:ListItem>
								<asp:ListItem Value="Username" meta:resourcekey="optUsername"></asp:ListItem>
								<asp:ListItem Value="Status" meta:resourcekey="optStatus"></asp:ListItem>
							</asp:DropDownList>                        
						</td>
						<td>
							<asp:TextBox ID="txtSearchValue" runat="server" Visible="true" />

							<asp:DropDownList runat="server" ID="ddlRole"></asp:DropDownList>

							<asp:DropDownList runat="server" ID="ddlAccountStatus"></asp:DropDownList>			
						</td>
						<td>
							<asp:Button ID="btnSearch" runat="server" meta:resourcekey="btnSearch" Visible="true" ValidationGroup="Search"/><br />
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
												<asp:Label ID="lblNewPassword" runat="server" CssClass="radLabelCss_Default" meta:resourcekey="lblNewPassword"></asp:Label>
											</td>
											<td runat="server" style="width: 200px;">                                                                                                                                   
												<asp:TextBox TextMode="Password" ID="txtNewPassword" runat="server" Width="200px"></asp:TextBox>                                            
											</td>
											<td>
												<asp:RequiredFieldValidator ID="rqPassword1" runat="server"   ControlToValidate="txtNewPassword" Display="Dynamic" meta:resourcekey="reqNewPassword" CssClass="Validator"   />
												<asp:RegularExpressionValidator ID="rePassword1" runat="server"  ControlToValidate="txtNewPassword" Display="Dynamic" ValidationExpression="^.{1,200}$" meta:resourcekey="vldNewPassword" CssClass="Validator" />
											</td>
										</tr>
										<tr>
											<td valign="top" style="width: 140px; padding: 0px 5px 5px 5px;">
												<asp:Label ID="lblConfirmPassword" runat="server" CssClass="radLabelCss_Default" meta:resourcekey="lblConfirmPassword"></asp:Label>
											</td>
											<td runat="server" style="width: 200px;">                                                                                                                                   
												<asp:TextBox TextMode="Password" ID="txtConfirmPassword" runat="server" Width="200px"></asp:TextBox>                                            
											</td>
											<td>
												<asp:RequiredFieldValidator ID="rqPassword2" runat="server" ControlToValidate="txtConfirmPassword" Display="Dynamic" meta:resourcekey="reqConfirmPassword" CssClass="Validator"   />
												<asp:RegularExpressionValidator ID="rePassword2" runat="server" ControlToValidate="txtConfirmPassword" Display="Dynamic" ValidationExpression="^.{1,200}$" meta:resourcekey="vldConfirmPassword" CssClass="Validator"  />
												<asp:CompareValidator ID="cmpPassword2" runat="server"  ControlToValidate="txtConfirmPassword" Display="Dynamic" ControlToCompare="txtNewPassword" meta:resourcekey="PasswordsDoNotMatch" Type="String" CssClass="Validator"  />
											</td>
										</tr>                                    
										<tr>
											<td colspan="3" align="left">
												<cc1:LWLinkButton ButtonType="Submit" ID="btnPasswordSave" meta:resourcekey="btnSave" OnClick="btnPassword_Save" CausesValidation="true" runat="server" />
												<cc1:LWLinkButton ButtonType="Cancel" ID="btnPasswordCancel" meta:resourcekey="btnCancel" OnClick="btnPassword_Cancel" runat="server" CausesValidation="false" />
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