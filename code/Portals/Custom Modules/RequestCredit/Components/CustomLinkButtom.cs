using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
// AEO-74 Upgrade 4.5 New class added-----------SCJ
namespace Brierley.AEModules.RequestCredit.Components
{
    //[ToolboxData("<{0}:WebCustomControl1 runat=server></{0}:WebCustomControl1>")]
    //[DefaultProperty("Text")]
    //public class CustomLinkButtom : LinkButton
    //{
    ////    public CustomLinkButtom()
    ////    { 
    ////    }
    ////    [Category("Appearance")]
    ////    public CustomLinkButtom.ButtonTypes ButtonType { get; set; }

    ////    public override void RenderBeginTag(HtmlTextWriter writer)
    ////    { 
    ////    }
    ////    protected override void RenderContents(HtmlTextWriter output)
    ////    { 
    ////    }
    ////    public override void RenderEndTag(HtmlTextWriter writer)
    ////    { 
    ////    }

    ////    public enum ButtonTypes
    ////    {
    ////        Submit = 0,
    ////        Cancel = 1,
    ////        AddNew = 2,
    ////        Orange = 3,
    ////        Red = 4,
    ////        Grey = 5,
    ////    }
    //}

    [DefaultProperty("Text"), System.Web.UI.ToolboxData("<{0}:WebCustomControl1 runat=server></{0}:WebCustomControl1>")]
    public class CustomLinkButtom : System.Web.UI.WebControls.LinkButton
    {
        public enum ButtonTypes
        {
            Submit,
            Cancel,
            AddNew,
            Orange,
            Red,
            Grey
        }
        private CustomLinkButtom.ButtonTypes _buttonType = CustomLinkButtom.ButtonTypes.Submit;
        [Category("Appearance")]
        public CustomLinkButtom.ButtonTypes ButtonType
        {
            get
            {
                return this._buttonType;
            }
            set
            {
                this._buttonType = value;
            }
        }
        public override void RenderBeginTag(System.Web.UI.HtmlTextWriter writer)
        {
            base.RenderBeginTag(writer);
        }
        public override void RenderEndTag(System.Web.UI.HtmlTextWriter writer)
        {
            base.RenderEndTag(writer);
        }
        protected override void RenderContents(System.Web.UI.HtmlTextWriter output)
        {
            string str = string.Empty;
            if (!base.IsEnabled)
            {
                str = "grey_btn";
            }
            else
            {
                switch (this._buttonType)
                {
                    case CustomLinkButtom.ButtonTypes.Submit:
                        str = "send_form_btn";
                        break;
                    case CustomLinkButtom.ButtonTypes.Cancel:
                        str = "send_form_btn";
                        break;
                    case CustomLinkButtom.ButtonTypes.AddNew:
                        str = "add_new_btn";
                        break;
                    case CustomLinkButtom.ButtonTypes.Orange:
                        str = "orange_btn";
                        break;
                    case CustomLinkButtom.ButtonTypes.Red:
                        str = "red_btn";
                        break;
                    case CustomLinkButtom.ButtonTypes.Grey:
                        str = "grey_btn";
                        break;
                }
            }
            output.Write("<span class=\"button " + str + "\"><span><span>");
            output.Write(this.Text);
            output.Write("</span></span></span>");
        }
    }
}