using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Newtonsoft.Json;
using Brierley.FrameWork.Data.ModelAttributes;

namespace Brierley.FrameWork.Data.DomainModel
{
	[PetaPoco.ExplicitColumns]
	[PetaPoco.PrimaryKey("SetId", sequenceName = "hibernate_sequence")]
	[PetaPoco.TableName("LW_PromoTestSet")]
	public class PromoTestSet : LWCoreObjectBase
	{
		[PetaPoco.Column("SetId", IsNullable = false)]
		public long Id { get; set; }

		[PetaPoco.Column("SetName", Length = 50)]
		public string Name { get; set; }

		[PetaPoco.Column("SetDescription", Length = 500)]
		public string Description { get; set; }

		[PetaPoco.Column("SetConfig")]
		public string Config { get; set; }

        [PetaPoco.Column]
        [ColumnIndex]
		public long? FolderId { get; set; }

		public PromoTestSetConfig GetConfig()
        {
            PromoTestSetConfig config = null;
            if (!string.IsNullOrEmpty(Config))
            {
                config = JsonConvert.DeserializeObject<PromoTestSetConfig>(Config);
            }
            return config;
        }
        
		public PromoTestSet Clone()
		{
			return Clone(new PromoTestSet());
		}

		public PromoTestSet Clone(PromoTestSet dest)
		{
			dest.Name = Name;
			dest.Description = Description;
			dest.Config = Config;
            dest.FolderId = FolderId;
			return (PromoTestSet)base.Clone(dest);
		}
	}
}
