<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ViewRewardCatalog.ascx.cs" Inherits="Brierley.LWModules.RewardCatalog.ViewRewardCatalog" %>

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
            alert('You already submitted the request!');
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
			<div id="divLanguageChannel">
				<asp:Label ID="lblLanguage" runat="server" />
				<asp:DropDownList runat="server" ID="drpLanguage" AutoPostBack="true">
					<asp:ListItem Value="Default" Text="Default"></asp:ListItem>
				</asp:DropDownList>
				<asp:Label ID="lblChannel" runat="server" />
				<asp:DropDownList runat="server" ID="drpChannel" AutoPostBack="true">
					<asp:ListItem Value="Default" Text="Default"></asp:ListItem>
				</asp:DropDownList>
			</div>

			<div id="RewardFilter">
				<asp:PlaceHolder runat="server" ID="phRewardFilter" />
			</div>

			<div id="RewardCategories">
				<asp:PlaceHolder runat="server" ID="pchCategories"></asp:PlaceHolder>
			</div>

			<div id="NoResults">
				<asp:Label ID="lblNoResults" runat="server" Visible="false"></asp:Label>
			</div>

			<asp:PlaceHolder runat="server" ID="pchRewardList">

				<div id="RewardSorting">
					<table>
						<tr>
							<td>
								<asp:Label ID="lblAvailableRewards" runat="server" />
							</td>
							<td>
								<span>
									<asp:Label ID="lblOrderBy" runat="server" />
									<asp:DropDownList runat="server" ID="ddlOrder" AutoPostBack="true">
										<asp:ListItem Value="NameAsc" Text="Name (Ascending)"></asp:ListItem>
										<asp:ListItem Value="NameDesc" Text="Name (Descending)"></asp:ListItem>
										<asp:ListItem Value="PriceAsc" Text="Price (Ascending)"></asp:ListItem>
										<asp:ListItem Value="PriceDesc" Text="Price (Descending)"></asp:ListItem>
									</asp:DropDownList>
								</span>
							</td>
						</tr>
					</table>
				</div>

				<asp:ListView runat="server" ID="lstRewards">
					<LayoutTemplate>
						<div id="RewardList">
							<ul>
								<asp:PlaceHolder ID="itemPlaceholder" runat="server"></asp:PlaceHolder>
							</ul>
						</div>
					</LayoutTemplate>
				
					<ItemTemplate>
						<li>
							<asp:LinkButton runat="server" ID="lnkReward">
								<asp:Image runat="server" ID="imgThumbnail" CssClass="RewardThumbnail" />
								<span class="RewardName">
									<%#Eval("Name")%>
								</span>
								<span class="RewardDesc">
									<asp:Literal runat="server" ID="litRewardDescription"></asp:Literal>
								</span>
								<span class="RewardCost">
									<asp:Literal runat="server" ID="litRewardCost"></asp:Literal>
								</span>
								<span class="RewardQuantity">
									<asp:Literal runat="server" ID="litRewardQuantity"></asp:Literal>
								</span>
							</asp:LinkButton>
						</li>
					</ItemTemplate>
				</asp:ListView>

				<div id="RewardPager">
					<asp:DataPager runat="server" ID="Pager" PagedControlID="lstRewards" PageSize="10">
						<Fields>
							<asp:NextPreviousPagerField ShowFirstPageButton="True" ShowNextPageButton="False" />
							<asp:NumericPagerField />
							<asp:NextPreviousPagerField ShowLastPageButton="True" ShowPreviousPageButton="False" />
						</Fields>
					</asp:DataPager>
				</div>

			</asp:PlaceHolder>
		
		</asp:View>

		<asp:View runat="server" ID="RewardDetailView">

			<div class="RewardDetail">
			
				<span id="DetailRewardName">
					<asp:Literal runat="server" ID="litRewardNameDetail"></asp:Literal>
				</span>

				<asp:Image runat="server" id="RewardImageDetail" CssClass="RewardImage" />

				<span id="DetailRewardDescription">
					<asp:Literal runat="server" ID="litRewardDescriptionDetail"></asp:Literal>
				</span>

				<span id="DetailRewardCost">
					<asp:Literal runat="server" ID="litRewardCostDetail"></asp:Literal>
				</span>

				<div class="Buttons">
					<asp:LinkButton runat="server" ID="lnkDone" Text="Done"></asp:LinkButton>
					<asp:LinkButton runat="server" ID="lnkOrder" Text="Order"></asp:LinkButton>
				</div>

			</div>

		</asp:View>

		<asp:View runat="server" ID="ShippingInformationView">
		
			<div id="OrderDetail">
				<span id="ShippingTitle">
					Shipping Information
				</span>

				<div class="RewardDetail">

					<span id="DetailRewardName">
						<asp:Literal runat="server" ID="litRewardNameShipping"></asp:Literal>
					</span>

					<asp:Image runat="server" id="RewardImageShipping" CssClass="RewardImage" />

					<span id="DetailRewardDescription">
						<asp:Literal runat="server" ID="litRewardDescriptionShipping"></asp:Literal>
					</span>

					<span id="DetailRewardCost">
						<asp:Literal runat="server" ID="litRewardCostShipping"></asp:Literal>
					</span>

				</div>

				<asp:PlaceHolder runat="server" ID="pchShippingFields"></asp:PlaceHolder>
								
				<div class="Buttons">
					<asp:LinkButton runat="server" ID="lnkCancelShipping" Text="Order" CausesValidation="false">Cancel</asp:LinkButton>
					<asp:LinkButton runat="server" ID="lnkOrderShipping" Text="Order"></asp:LinkButton>
				</div>

			</div>

		</asp:View>

		<asp:View runat="server" ID="OrderReviewView">
		
			<div id="OrderReview">
				<span id="OrderReviewTitle">
					Order Review
				</span>

				<div class="RewardDetail">

					<span id="DetailRewardName">
						<asp:Literal runat="server" ID="litRewardNameReview"></asp:Literal>
					</span>

					<asp:Image runat="server" id="RewardImageReview" CssClass="RewardImage" />

					<span id="DetailRewardDescription">
						<asp:Literal runat="server" ID="litRewardDescriptionReview"></asp:Literal>
					</span>

					<div id="CostSummary">
						<table>
							<tr>
								<td>
									Current Balance:
								</td>
								<td>
									<asp:Literal runat="server" ID="litCurrentBalanceReview"></asp:Literal>
								</td>
							</tr>
							<tr>
								<td>
									Deduct:
								</td>
								<td>
									<asp:Literal runat="server" ID="litRewardCostReview"></asp:Literal>
								</td>
							</tr>
							<tr>
								<td>
									Remaining Balance:
								</td>
								<td>
									<asp:Literal runat="server" ID="litRemainingBalanceReview"></asp:Literal>
								</td>
							</tr>
						</table>
					</div>

				</div>

				<asp:PlaceHolder runat="server" ID="pchReviewShipping" Visible="false">
					<h2>Shipping Information</h2>
					<asp:PlaceHolder runat="server" ID="pchReviewShippingFields"></asp:PlaceHolder>
				</asp:PlaceHolder>

				<div class="Buttons">
					<asp:LinkButton runat="server" ID="lnkCancelReview" Text="Order">Cancel</asp:LinkButton>
					<asp:LinkButton runat="server" ID="lnkPlaceOrder" Text="Order"></asp:LinkButton>
				</div>

			</div>

		</asp:View>

		<asp:View runat="server" id="OrderCompleteView">
			<div id="OrderComplete">
				<span id="OrderCompleteTitle">
					Order Complete
				</span>
				<asp:Literal runat="server" ID="litOrderComplete"></asp:Literal>
			</div>
		</asp:View>


	</asp:MultiView>

</div>