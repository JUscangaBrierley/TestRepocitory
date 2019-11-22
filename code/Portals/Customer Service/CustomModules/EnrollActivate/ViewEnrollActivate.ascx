<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ViewEnrollActivate.ascx.cs"
    Inherits="Brierley.AEModules.EnrollActivate.ViewEnrollActivate" %>
<style type="text/css">
    .style2
    {
        width: 172px;
    }
    .style3
    {
        width: 109px;
    }
    .style4
    {
        width: 210px;
    }
    .style7
    {
        width: 203px;
    }
    .style8
    {
        width: 149px;
    }
</style>
<asp:Panel ID = "pnlEnroll" runat = "server" Visible = "true">
 <table cellpadding="3" width= "60%" cellspacing="0" border="0"> 
       <tr>
            <td class="style4">AEREWARD$ Card Number:</td>
            <td>
                    <asp:TextBox ID="txtLoyaltyIDNumber" MaxLength="14" runat="server" tabIndex="1" AutoPostBack="True"></asp:TextBox>  
            </td>
        </tr>
       <tr>
            <td class="style4">Select Brand:</td>
            <td>
                 <asp:RadioButtonList ID="radioBaseBrand" runat="server"
                     RepeatDirection="Horizontal" tabIndex="2">
                     <asp:ListItem Value="1" Selected="True">AE</asp:ListItem>
                     <asp:ListItem Value="2">aerie</asp:ListItem>
                 </asp:RadioButtonList>
             </td>
        </tr>
        <caption>
            <br />
       </caption>
 </table>
 <table>
        <tr><td width="70%"><font color = "blue">Please enter a valid AEREWARD$ number.  If you do not have an AEREWARD$ number, leave the field blank, and a temporary ID will be issued.</font><br/></td></tr>
 </table>
 <table>
        <tr><td width="70%">Note: If you make changes and then navigate to another page without pressing the submit button, your changes will be lost.<br/></td></tr>
 </table>
 <table cellpadding="3" cellspacing="0" border="0"> 
    <tr><td>
    <div style="background-color: #ccc; width: 100%; text-indent: 10px;"><h1>MEMBER</h1><div style="float:right;"><span style="color: "red";display:none;font-weight: bold;" id="spnMandatoryErrorMessage1"></span></div></div>
    </td></tr>
 </table>

 <table cellpadding="3"  width= "70%" cellspacing="0" border="0"> 
 	<tr>
		<td>FIRST NAME<font color = "red">*</font></td>
		<td>
				<asp:textbox id="txtFirstName" runat="server" MaxLength="15" tabIndex="3"></asp:textbox>
				<asp:requiredfieldvalidator id="RequiredFieldValidator1" runat="server" ErrorMessage="Please enter first name." ForeColor="Red"
						ControlToValidate="txtFirstName" Display="Dynamic" DESIGNTIMEDRAGDROP="1188">*Please enter first name.</asp:requiredfieldvalidator>
                                <br />
				                <font style="COLOR: #666666; FONT-FAMILY: arial, san-serif">up to 15 characters </font>
               </td>
	</tr>
	<tr>
		<td>LAST NAME<font color = "red">*</font></td>
		<td>
				<asp:textbox id="txtLastName" runat="server" MaxLength="15" tabIndex="4"></asp:textbox>
				<asp:requiredfieldvalidator id="RequiredFieldValidator2" runat="server" ErrorMessage="Please enter last name."
						ControlToValidate="txtLastName" Display="Dynamic" DESIGNTIMEDRAGDROP="1188" 
                    ForeColor="Red">*Please enter last name.</asp:requiredfieldvalidator>
                                <br />
				                <font style="COLOR: #666666; FONT-FAMILY: arial, san-serif">up to 15 characters </font>
               </td>
	</tr>
 </table>
 <table cellpadding="3"  width= "100%" cellspacing="0" border="0"> 
        <tr>
            <td>
                <asp:Label ID="LblBirthday" runat="server">BIRTHDAY <font color = "red">*</font></asp:Label>
            </td>
            <td>
                <asp:TextBox ID="txtBirthday" runat="server" MaxLength="10" tabIndex="5"></asp:TextBox>
                	<asp:requiredfieldvalidator id="RequiredFieldValidator3" runat="server" ErrorMessage="Please enter birthday."
						ControlToValidate="txtBirthday" Display="Dynamic" DESIGNTIMEDRAGDROP="1188" 
                    ForeColor="Red">*Please enter birthday.</asp:requiredfieldvalidator> 
                <asp:RegularExpressionValidator ID="dateValRegex" runat="server" 
                    ControlToValidate="txtBirthday" 
                    ErrorMessage="Please enter a valid date in the correct format." 
                    ValidationExpression="^(0[1-9]|1[012])[- /.](0[1-9]|[12][0-9]|3[01])[- /.](19|20)\d\d$" 
                    ForeColor="Red"></asp:RegularExpressionValidator>
             </td>
        </tr>
    <tr><td width="25%">&nbsp;</td><td>MM/DD/YYYY</td><td>&nbsp;</td><td>&nbsp;</td></tr>
  </table>
  <table cellpadding="3"  width= "70%" cellspacing="0" border="0"> 
	 <caption>
         &nbsp;
         <tr>
             <td height="17" valign="top">
                 GENDER</td>
             <td height="17">
                 <asp:RadioButtonList ID="radioGender" runat="server"
                     RepeatDirection="Horizontal" tabIndex="6">
                     <asp:ListItem Value="1">Male</asp:ListItem>
                     <asp:ListItem Value="2">Female</asp:ListItem>
                 </asp:RadioButtonList>
             </td>
         </tr>
         <tr>
             <td align="left" height="17" valign="top">
                 LANGUAGE PREFERENCE:</td>
             <td height="17">
                 <asp:RadioButtonList ID="radioLanguage" runat="server"
                     RepeatDirection="Horizontal" tabIndex="7">
                     <asp:ListItem Value="0">English</asp:ListItem>
                     <asp:ListItem Value="2">French</asp:ListItem>
                     <asp:ListItem Value="1">Spanish</asp:ListItem>
                 </asp:RadioButtonList>
             </td>
         </tr>
     </caption>
</table>

<table cellpadding="3" cellspacing="0" border="0"> 
    <tr><td>
    <div style="background-color: #ccc; width: 100%; text-indent: 10px;"><h1>E-MAIL</h1><div style="float:right;"><span style="color: "red";display:none;font-weight: bold;" id="spnMandatoryErrorMessage2"></span></div></div>
    </td></tr>
</table>

 <table cellpadding="3" width= "100%" cellspacing="0" border="0"> 
        <tr>
            <td>
                <asp:Label ID="LblEmailAddress" runat="server">E-MAIL ADDRESS<font color = "red">*</font></asp:Label>
            </td>
            <td>
                <asp:TextBox ID="txtEmailAddress" runat="server" MaxLength="50" tabIndex="8"></asp:TextBox>
                <asp:requiredfieldvalidator id="RequiredFieldValidator4" runat="server" ErrorMessage="Please enter emailaddress."
						ControlToValidate="txtEmailAddress" Display="Dynamic" DESIGNTIMEDRAGDROP="1188" 
                    ForeColor="Red">*Please enter email address.</asp:requiredfieldvalidator>
  
                <br />
	        <font style="COLOR: #666666; FONT-FAMILY: arial, san-serif">up to 50 characters</font></td>
           <td>
                <asp:Label ID="LblEmailConfirm" runat="server" text = "CONFIRM E-MAIL"></asp:Label>
            </td>
            <td>
                <asp:TextBox ID="txtEmailConfirm" runat="server" MaxLength="50" tabIndex="9"></asp:TextBox>
                <asp:CompareValidator ID="validatorCompareEmail" runat="server" 
                    ControlToCompare="txtEmailAddress" ControlToValidate="txtEmailConfirm" 
                    ErrorMessage="*Email Addresses do not match" ForeColor="Red"></asp:CompareValidator>
            </td>
        </tr>
        <tr><td>&nbsp;&nbsp;</td></tr>
</table>
<table cellpadding="3" cellspacing="0" border="0"> 
    <tr><td>
    <div style="background-color: #ccc; width: 100%; text-indent: 10px;"><h1>MAIL</h1><div style="float:right;"><span style="color: red; display:none;font-weight: bold;" id="spnMandatoryErrorMessage3"></span></div></div>
    </td></tr>
</table>

 <table cellpadding="3" cellspacing="0" border="0" 
        style="width: 75%; margin-right: 82px"> 
 	<tr>
		<td height="18" class="style3">COUNTRY<font color = "red">*</font></td>
		<td height="18" class="style7">
			<asp:dropdownlist id="selCountry" runat="server" tabIndex="10" 
                onselectedindexchanged="selCountry_SelectedIndexChanged" 
                AutoPostBack="True">
			<asp:ListItem Value="USA">USA</asp:ListItem>
			<asp:ListItem Value="CAN">CAN</asp:ListItem>
			</asp:dropdownlist></td>
	</tr>

	<tr>
		<td class="style3">ADDRESS 1<font color = "red">*</font></td>
		<td class="style7">
		<asp:textbox id="txtAddress1" runat="server" MaxLength="50" tabIndex="11"></asp:textbox>
		<asp:requiredfieldvalidator id="RequiredFieldValidator6" runat="server" ErrorMessage="Please enter address 1." 
				ControlToValidate="txtAddress1" Display="Dynamic" DESIGNTIMEDRAGDROP="1188" 
                ForeColor="Red">*Please enter address 1.</asp:requiredfieldvalidator>
		    <br />
		<font style="COLOR: #666666; FONT-FAMILY: arial, san-serif">up to 50 characters</font>
		</td>
	</tr>
	<tr>
		<td class="style3">ADDRESS 2</td>
		<td class="style7">
		<asp:textbox id="txtAddress2" runat="server" MaxLength="50" tabIndex="12"></asp:textbox>
			<br />
			<font style="COLOR: #666666; FONT-FAMILY: arial, san-serif">up to 50 characters</font></td>
	</tr>
	<tr>
		<td class="style3">CITY<font color = "red">*</font></td>
		<td class="style7">
			<asp:textbox id="txtCity" runat="server" MaxLength="30" tabIndex="13"></asp:textbox>
			<asp:requiredfieldvalidator id="RequiredFieldValidator7" runat="server" ErrorMessage="Please enter the city."
				ControlToValidate="txtCity" Display="Dynamic" ForeColor="Red">*Please enter city.</asp:requiredfieldvalidator>
            <br />
			<font style="COLOR: #666666; FONT-FAMILY: arial, san-serif">up to 30 characters</font></td> 
        <td>&nbsp;</td>  
		<td align="right" height="25">STATE<font color = "red">*</font></td>
		<td height="25">
				<asp:dropdownlist id="selState" runat="server" tabIndex="14"></asp:dropdownlist>    
                                <asp:requiredfieldvalidator id="RequiredFieldValidator8" 
                    runat="server" ErrorMessage="Please select Country and State."
						ControlToValidate="selState" EnableViewState="False" Font-Italic="True" ForeColor="Red">*Please select Country and State.</asp:requiredfieldvalidator></td>
	</tr>
	<tr>
		<td class="style3">ZIP/POSTAL CODE<font color = "red">*</font></td>
		<td class="style7">
				<asp:textbox id="txtPostalCode" runat="server" MaxLength="10" tabIndex="15"></asp:textbox>
				<asp:requiredfieldvalidator id="RequiredFieldValidator9" runat="server" ErrorMessage="Please enter postal code."
						ControlToValidate="txtPostalCode" Display="Dynamic" DESIGNTIMEDRAGDROP="1188" 
                    ForeColor="Red">*Please enter postal code.</asp:requiredfieldvalidator>
                        <br />
				<font style="COLOR: #666666; FONT-FAMILY: arial, san-serif">5 to 10 characters <br/> Canada Format: A1B 2C3</font></td>
	</tr>

</table>
<table cellpadding="3" cellspacing="0" border="0"> 
<tr><td>
<div style="background-color: #ccc; width: 100%; text-indent: 10px;"><h1>PHONE</h1><div style="float:right;"><span style="color: red;display:none;font-weight: bold;" id="spnMandatoryErrorMessage4"></span></div></div>
</td></tr>
</table>

 <table cellpadding="3" width= "60%" cellspacing="0" border="0"> 
         <tr>
            <td class="style8">
                <asp:Label ID="LblHomePhone" runat="server" text ="HOME PHONE" ></asp:Label>
            </td>
            <td>
                <asp:TextBox ID="txtHomePhone" runat="server" MaxLength="12" tabIndex="16"></asp:TextBox>
            </td>
        </tr>
        <tr><td class="style8">&nbsp;</td><td width="25%">###-###-####</td><td></td><td>&nbsp;</td><td>&nbsp;</td></tr>
        <tr>
            <td class="style8">
                <asp:Label ID="LblMobilePhone" runat="server" text ="MOBILE PHONE" ></asp:Label>
            </td>
            <td>
                <asp:TextBox ID="txtMobilePhone" runat="server" MaxLength="12" tabIndex="17"></asp:TextBox>
            </td>
        </tr>
      <tr><td class="style8">&nbsp;</td><td width="25%">###-###-####</td><td></td><td>&nbsp;</td><td>&nbsp;</td></tr>
	<tr runat="server" id="SMSRow">
		<td align="left" height="26" class="style8"></td>
		<td height="26"><asp:checkbox id="chkSMSOptIn" tabindex="18" runat="server" Text="I want to receive special text message promotions and alerts from American Eagle Outfitters. *"></asp:checkbox>
		</td>
	</tr>
  <tr><td class="style8"> &nbsp;&nbsp;</td></tr>
 </table>
   <table cellpadding="3" width= "100%" cellspacing="0" border="0"> 
    <tr><td align="left" width="100%">*Message and data rates may apply. AEO respects your privacy and will never sell or share your mobile number. Subscribers will receive messages from short code 32453(EAGLE)<br/><br/> <br/></td></tr>
  </table>
   <table>
       <tr><td> &nbsp;&nbsp;</td></tr>
       <tr>
            <td align="left">
                <asp:LinkButton ID="BtnSubmit" runat="server" Text="SUBMIT"  Visible="true" tabIndex="19"
                    onclick="BtnSubmit_Click" ></asp:LinkButton>
            </td>
        </tr>
    </table>
</asp:Panel>

<asp:Panel ID = "pnlConfirm" runat = "server" Visible = "false" >

<table>
<tr>
<td>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</td>
<td>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</td>
<td>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</td>
<td>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</td>
<td>
   <table cellpadding="2" cellspacing="0" width="300px" border="0" style="border-left: thick solid Silver;        border-top: thick solid Silver; border-right: thick solid Silver; border-bottom: thick solid Silver;">        
	<tr>            
		<td width="150" style="font-size: 14px; font-weight: bold;">                Member Name:            </td>            
		<td width="150" align="right" style="font-size: 14px; white-space:nowrap;">                
              <asp:Label ID="lblMemberName" runat="server"></asp:Label>         
        </td>        
	</tr>        
	<tr>            
		<td style="font-size: 14px; font-weight: bold;">                AEREWARD$#:            </td>            
		<td align="right" style="font-size: 14px;white-space:nowrap;">
             <asp:Label ID="lblMemberLoyaltyNumber" runat="server"></asp:Label>                           
        </td>        
	</tr>        
	<tr>            
		<td style="font-size: 14px; font-weight: bold;" valign="top">                Notes:            </td>            
		<td align="right">                
			<table cellpadding="0" cellspacing="0" border="0" width="100%">                    
				<tr>                        
				<td align="right" style="font-size: 14px;white-space:nowrap;">                            UnLinked                        </td>                    
				</tr>                    
			</table>            
		</td>        
	</tr>    
   </table>
</td>
</tr>
</table>
 <table cellpadding="3" width= "60%" cellspacing="0" border="0"> 
        <tr><td align="left" width="60%">Congratulations! </td></tr>
        <caption> 
            <br />       
            <tr>
                <td align="left" width="60%">
                    Your AEREWARD$ card is now registered! Please
                </td>
            </tr>
            <tr>
                <td align="left" width="60%">
                    go to AE.com to view your current point balance.</td>
            </tr>
            <caption>
                <br />
                <tr>
                    <td align="left" width="60%">
                        Your AEREWARD$ number is:</td>
                </tr>
                <tr>
                    <td align="center"width="20%">
                        <asp:Label ID="lblMemberLoyaltyNumber2" runat="server" Font-Bold="true"></asp:Label>    
                    </td>
                </tr>
                <tr>
                    <td align="left" width="60%">
                        You will receive your new AEREWARD$ card in
                    </td>
                </tr>
                <tr>
                    <td align="left" width="60%">
                        the mail in 4-6 weeks.
                    </td>
                </tr>
            </caption>
        </caption>
 </table>
 <br />
 <table runat="server" id="tbUnderAge" visible="false" cellpadding="3" width= "100%" cellspacing="0" border="0"> 
    <tr><td align="left" width="80%"><font color = "blue">"The birth date you provided does not meet our age requirement." </font></td></tr>
    <tr><td align="left" width="80%"><font color = "blue"> "Is a parent available?" </font> </td></tr>
    <tr><td align="left" width="80%"><font color = "blue"> If Yes: Explain to parent rules of the program in regards to members under the age of 13. </font></td></tr>
    <tr><td align="left" width="80%"><font color = "blue"> If No: "Please have your parent or guardian call us back at 800-340-0532 to update the  </font></td></tr>
    <tr><td align="left" width="80%"><font color = "blue">         profile information provided." </font></td></tr>   
 </table>
</asp:Panel>