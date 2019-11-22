<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ViewRewardCatalog.ascx.cs" Inherits="Brierley.LWModules.RewardCatalog.ViewRewardCatalog" %>
<%@ Register Assembly="Brierley.WebFrameWork" Namespace="Brierley.WebFrameWork.Controls.Mobile" TagPrefix="Mobile" %>
<%@ Register TagPrefix="cc1" Namespace="Brierley.WebFrameWork.Controls" Assembly="Brierley.WebFrameWork" %>

<script type="text/javascript">

	var _parentLists = null;
	var _relations = null;
	var _orderPlaced = 0;

	$(document).ready(function () {
		BindParentEvents();
	});


	function BindParentEvents() {
		if (_parentLists == null) {
			return;
		}
		for (var i = 0; i < _parentLists.length; i++) {
			var list = $('#' + _parentLists[i]);
			if (list != null && list.length > 0) {
				list.bind('change', function () { ParentChanged($(this)); });
				ParentChanged(list);
			}
		}
	}

	function ParentChanged(parent) {
		if (parent != null && parent.length > 0) {
			var parentId = $(parent).attr('id');
			for (var i = 0; i < _relations.length; i++) {
				if (_relations[i].parentId == parentId) {
					var relation = _relations[i];
					var children = relation.childValues;
					var child = $('#' + relation.childId);
					var currentVal = child.val();
					var parentVal = $(parent).val();
					var hasValue = false;

					$('#' + relation.childId + ' > option').remove();

					for (var i = 0; i < children.length; i++) {
						if (children[i].parentKey == parentVal || children[i].parentKey == '') {
							child.append($('<option></option').val(children[i].key).html(children[i].text));
							if (children[i].key == currentVal) {
								hasValue = true;
							}
						}
					}
					child.val(hasValue ? currentVal : '');
					if ($.inArray(child.attr('id'), _parentLists)) {
						ParentChanged(child);
					}
				}
			}
		}
	}


	function Relation(parentId, childId, childValues) {
		this.parentId = parentId;
		this.childId = childId;
		this.childValues = childValues;
		return true;
	}

	function Item(parentKey, key, text) {
		this.parentKey = parentKey;
		this.key = key;
		this.text = text;
	}

    function CheckDuplicateSubmit(){
        if (_orderPlaced == 0) {
            _orderPlaced = 1;
            return true;
        }
        else {
            alert('<%= GetLocalResourceObject("RequestAlreadySubmitted.Text") %>');
            return false;
        }
    }

</script>

<div id="RewardCatalogContainer">

	<h2 id="Title">
		<asp:Literal runat="server" ID="litTitle"></asp:Literal>
	</h2>

	<asp:MultiView runat="server" ID="mvMain">
		<asp:View runat="server" ID="RewardListView">
			<div id="divLanguageChannel" class="form-inline">
				<label><asp:Literal ID="lblLanguage" runat="server" /></label>
				<asp:DropDownList runat="server" ID="drpLanguage" AutoPostBack="true" CssClass="form-control">
					<asp:ListItem Value="Default" Text="Default"></asp:ListItem>
				</asp:DropDownList>
				<label><asp:Literal ID="lblChannel" runat="server" /></label>
				<asp:DropDownList runat="server" ID="drpChannel" AutoPostBack="true" CssClass="form-contorl">
					<asp:ListItem Value="Default" Text="Default"></asp:ListItem>
				</asp:DropDownList>
			</div>

			<div id="RewardFilter">
				<asp:PlaceHolder runat="server" ID="phRewardFilter" />
			</div>

			<div id="RewardCategories">
				<div>
					<asp:PlaceHolder runat="server" ID="pchCategories"></asp:PlaceHolder>
				</div>
				<div class="clearfix"></div>
			</div>

			<div id="NoResults">
				<asp:Label ID="lblNoResults" runat="server" Visible="false"></asp:Label>
			</div>

			<asp:PlaceHolder runat="server" ID="pchRewardList">
				<div id="RewardSorting" class="form-inline reward-filter text-center">
					<asp:Label ID="lblOrderBy" runat="server" />
					<span>
						<Mobile:RadioButtonList runat="server" id="ddlOrder" autopostback="true">
							<asp:ListItem Value="NameAsc" meta:resourcekey="rdoNameAsc"></asp:ListItem>
							<asp:ListItem Value="NameDesc" meta:resourcekey="rdoNameDesc"></asp:ListItem>
							<asp:ListItem Value="PriceAsc" meta:ResourceKey="rdoPriceAsc"></asp:ListItem>
							<asp:ListItem Value="PriceDesc" meta:resourceKey="rdoPriceDesc"></asp:ListItem>
						</Mobile:RadioButtonList>
					</span>
				</div>
				<div class="clearfix"></div>
				

				<cc1:LWApplicationPanel runat="server" ID="pnlRewards" resourcekey="pnlRewards" Visible="true">
					<Content>
						<div class="DynamicList">
							<ul>
							<asp:ListView runat="server" ID="lstRewards">
								<ItemTemplate>
									<li>
										<div class="row">
											<div class="col-sm-3 col-xs-4 list-left">
												<asp:Image runat="server" ID="imgThumbnail" CssClass="RewardThumbnail" />
												<br>
												<br>
												<asp:LinkButton runat="server" ID="lnkReward" class="btn btn-sm" meta:resourcekey="btnView" />
											</div>
											<div class="col-sm-6 col-xs-8 list-middle">
												<h3 class="RewardName">
													<%#Eval("Name")%>
												</h3>
												<p>
													<%#Eval("ShortDescription")%>
												</p>
												<p>
													<asp:Literal runat="server" ID="litRewardDescription"></asp:Literal>
												</p>
											</div>
											<div class="col-md-3 col-sm-2 hidden-xs list-right">
													<span class="RewardCost">
														<asp:Literal runat="server" ID="litRewardCost"></asp:Literal>
													</span>
													<span class="RewardQuantity">
														<asp:Literal runat="server" ID="litRewardQuantity"></asp:Literal>
													</span>
											</div>
										</div>
										</li>
								</ItemTemplate>
							</asp:ListView>
							</ul>
						</div>

						<div class="pager">
							<asp:DataPager runat="server" ID="Pager" PagedControlID="lstRewards" PageSize="10">
								<Fields>
									<asp:NextPreviousPagerField 
										ShowFirstPageButton="true" 
										ShowPreviousPageButton="true"
										ShowNextPageButton="false"
										ShowLastPageButton="false"
										ButtonCssClass="" />
									<asp:NumericPagerField 
										ButtonType="Link" 
										NumericButtonCssClass=""
										CurrentPageLabelCssClass="current" />
									<asp:NextPreviousPagerField
										ButtonType="Link" 
										ButtonCssClass=""
										ShowFirstPageButton="false"
										ShowPreviousPageButton="false" 
										ShowNextPageButton="true" 
										ShowLastPageButton="true" />
								</Fields>
							</asp:DataPager>
						</div>

					</Content>
				</cc1:LWApplicationPanel>



			</asp:PlaceHolder>
		
		</asp:View>

		<asp:View runat="server" ID="RewardDetailView">

			<div class="RewardDetail">
				<div class="row">
					<div class="col-md-5 col-sm-12 list-left">
						<asp:Image runat="server" id="RewardImageDetail" CssClass="RewardImage" />
					</div>
					<div class="col-md-5 col-sm-12 list-middle">
						<h3>
							<asp:Literal runat="server" ID="litRewardNameDetail"></asp:Literal>
						</h3>
						<p>
							<asp:Literal runat="server" ID="litRewardDescriptionDetail"></asp:Literal>
						</p>
						<p>
							<asp:Literal runat="server" ID="litProductDescriptionDetail"></asp:Literal>
						</p>
					</div>
					<div class="col-md-2 col-sm-12 list-right">
						<asp:Literal runat="server" ID="litRewardCostDetail"></asp:Literal>
					</div>
				</div>
			</div>
			<div class="buttons">
				<asp:LinkButton runat="server" ID="lnkDone" Text="Done" cssclass="btn" meta:resourcekey="btnViewDone"></asp:LinkButton>
				<asp:LinkButton runat="server" ID="lnkOrder" Text="Order" cssclass="btn" meta:resourcekey="btnViewOrder"></asp:LinkButton>
			</div>

		</asp:View>

		<asp:View runat="server" ID="ShippingInformationView">
		
			<div id="OrderDetail">
				<h2>
					Shipping Information
				</h2>

				<div class="RewardDetail">

					<h4 id="DetailRewardName">
						<asp:Literal runat="server" ID="litRewardNameShipping"></asp:Literal>
					</h4>

					<asp:Image runat="server" id="RewardImageShipping" CssClass="RewardImage" />

					<span id="DetailRewardDescription">
						<asp:Literal runat="server" ID="litRewardDescriptionShipping"></asp:Literal>
					</span>

					<span id="DetailRewardCost">
						<asp:Literal runat="server" ID="litRewardCostShipping"></asp:Literal>
					</span>

				</div>

				<asp:PlaceHolder runat="server" ID="pchShippingFields"></asp:PlaceHolder>
								
				<div class="buttons">
					<asp:LinkButton runat="server" ID="lnkCancelShipping" Text="Cancel" cssclass="btn" CausesValidation="false" meta:resourcekey="btnShippingCancel"></asp:LinkButton>
					<asp:LinkButton runat="server" ID="lnkOrderShipping" Text="Order" cssclass="btn" meta:resourcekey="btnShippingOrder"></asp:LinkButton>
				</div>

			</div>

		</asp:View>

		<asp:View runat="server" ID="OrderReviewView">
		
			<div id="OrderReview">
				<h2>
					Order Review
				</h2>

				<div class="RewardDetail">
					<div class="row">
						<div class="col-md-5 col-sm-12 list-left">
							<asp:Image runat="server" id="RewardImageReview" CssClass="RewardImage" />
						</div>
						
						<div class="col-md-4 col-sm-12 list-middle">
							<h3 id="DetailRewardName">
								<asp:Literal runat="server" ID="litRewardNameReview"></asp:Literal>
							</h3>
							<p>
								<asp:Literal runat="server" ID="litRewardDescriptionReview"></asp:Literal>
							</p>
						</div>

						<div class="col-md-3 col-sm-12 list-right">
							<p>
								Current Balance:
								<asp:Literal runat="server" ID="litCurrentBalanceReview"></asp:Literal>
							</p>
							<p>
								Deduct:
								<asp:Literal runat="server" ID="litRewardCostReview"></asp:Literal>
							</p>
							<p>
								Remaining Balance:
								<asp:Literal runat="server" ID="litRemainingBalanceReview"></asp:Literal>
							</p>
						</div>
					</div>

					<asp:PlaceHolder runat="server" ID="pchReviewShipping" Visible="false">
						<h3>Shipping Information</h3>
						<asp:PlaceHolder runat="server" ID="pchReviewShippingFields"></asp:PlaceHolder>
					</asp:PlaceHolder>
				</div>
			</div>
			<div class="buttons">
				<asp:LinkButton runat="server" ID="lnkCancelReview" cssclass="btn" Text="Order" meta:resourcekey="btnReviewCancel"></asp:LinkButton>
				<asp:LinkButton runat="server" ID="lnkPlaceOrder" cssclass="btn" Text="Order" meta:resourcekey="btnReviewOrder"></asp:LinkButton>
			</div>

		</asp:View>

		<asp:View runat="server" id="OrderCompleteView">
			<div id="OrderComplete">
				<h2>
					Order Complete
				</h2>
				<asp:Literal runat="server" ID="litOrderComplete"></asp:Literal>
			</div>
		</asp:View>


	</asp:MultiView>

</div>