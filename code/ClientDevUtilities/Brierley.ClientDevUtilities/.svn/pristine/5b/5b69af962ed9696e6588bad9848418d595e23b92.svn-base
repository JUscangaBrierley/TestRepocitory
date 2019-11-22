using Brierley.FrameWork.Data.ModelAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Brierley.FrameWork.Data.DomainModel
{
	[Serializable]
	[PetaPoco.ExplicitColumns]
	[PetaPoco.PrimaryKey("Id", sequenceName = "hibernate_sequence")]
	[PetaPoco.TableName("LW_UserAgentMap")]
	public class UserAgentMap : LWCoreObjectBase
	{
        [PetaPoco.Column(IsNullable = false)]
		public long Id { get; set; }

        [PetaPoco.Column(Length = 2000, IsNullable = false)]
        [UniqueIndex(RequiresLowerFunction = false)]
		public string UserAgent { get; set; }

        [PetaPoco.Column(Length = 255)]
		public string Suffix { get; set; }

        [PetaPoco.Column(Length = 10)]
		public string Channel { get; set; }

		public UserAgentMap Clone()
		{
			return Clone(new UserAgentMap());
		}

		public UserAgentMap Clone(UserAgentMap other)
		{
			other.UserAgent = UserAgent;
			other.Suffix = Suffix;
			other.Channel = Channel;
			return (UserAgentMap)base.Clone(other);
		}
	}
}
