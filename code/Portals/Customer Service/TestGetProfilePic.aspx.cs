using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Brierley.WebFrameWork.SocialNetwork;
using Brierley.WebFrameWork.Portal;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.bScript;
using Brierley.FrameWork;

public partial class TestGetProfilePic : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
		btnGetProfilePic.Click += new EventHandler(btnGetProfilePic_Click);
    }

	void btnGetProfilePic_Click(object sender, EventArgs e)
	{
		Member member = PortalState.CurrentMember;

		string bscript = "GetSocialMediaProfileImageURL('Gigya', 'gigya_login')";

		ContextObject contextObject = new ContextObject();
		contextObject.Owner = member;
		object obj = ExpressionUtil.Evaluate(bscript, contextObject);

		if (obj != null && obj is string)
		{
			imgProfile.ImageUrl = obj.ToString();
		}
	}
}