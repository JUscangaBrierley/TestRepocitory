using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Data.ModelAttributes;

namespace Brierley.FrameWork.Data.DomainModel
{
    [PetaPoco.ExplicitColumns]
    [PetaPoco.PrimaryKey("Id", sequenceName = "hibernate_sequence")]
    [PetaPoco.TableName("LW_EmailFeedback")]
    public class EmailFeedback : LWCoreObjectBase
    {
        [PetaPoco.Column]
        public long Id { get; set; }

        [PetaPoco.Column(Length = 254, IsNullable = false)]
        [ColumnIndex(RequiresLowerFunction = true)]
        public string EmailAddress { get; set; }

        [PetaPoco.Column]
        public DateTime FeedbackDate { get; set; }

        [PetaPoco.Column]
        public EmailFeedbackType FeedbackType { get; set; }

        [PetaPoco.Column]
        public EmailFeedbackSubtype FeedbackSubtype { get; set; }

        [PetaPoco.Column(IsNullable = true)]
        public long? ClearedBy { get; set; }
    }
}
