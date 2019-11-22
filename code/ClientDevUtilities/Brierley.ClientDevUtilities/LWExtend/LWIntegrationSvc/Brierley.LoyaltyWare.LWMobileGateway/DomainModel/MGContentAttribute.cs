using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using System.ComponentModel;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;

namespace Brierley.LoyaltyWare.LWMobileGateway.DomainModel
{    
    //[DataContract]
    //[TypeConverter(typeof(MGContentAttributeConverter))]
    public class MGContentAttribute
    {
        #region Properties
        //[DataMember]
        public string AttributeName { get; set; }
        //[DataMember]
        public string AttributeValue { get; set; }
        #endregion

        #region Data Transfer Methods
        public static MGContentAttribute Hydrate(Brierley.FrameWork.Data.DomainModel.ContentAttribute att)
        {
			using (var content = LWDataServiceUtil.ContentServiceInstance())
			{
				ContentAttributeDef cadef = content.GetContentAttributeDef(att.ContentAttributeDefId);
				MGContentAttribute mm = new MGContentAttribute()
				{
					AttributeName = cadef.Name,
					AttributeValue = att.Value
				};
				return mm;
			}
        }

        public static MGContentAttribute[] ConvertFromJson(string contentAttsStr)
        {
            MGContentAttribute[] attsList = JsonConvert.DeserializeObject<MGContentAttribute[]>(contentAttsStr);
            return attsList;
        }
        #endregion
    }
    
    //public class MGContentAttributeConverter : TypeConverter
    //{
    //    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
    //    {
    //        return base.CanConvertFrom(context, sourceType);
    //    }

    //    public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
    //    {
    //        return base.ConvertFrom(context, culture, value);
    //    }

    //    public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
    //    {
    //        return base.CanConvertTo(context, destinationType);
    //    }

    //    public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
    //    {
    //        return base.ConvertTo(context, culture, value, destinationType);
    //    }
    //}
}