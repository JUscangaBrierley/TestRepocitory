using System;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Data.ModelAttributes;

namespace Brierley.FrameWork.Data.DomainModel
{
    [PetaPoco.ExplicitColumns]
    [PetaPoco.PrimaryKey("ID", sequenceName = "hibernate_sequence")]
    [PetaPoco.TableName("LW_Mailing")]
    public class Mailing : LWCoreObjectBase
    {
        #region fields
        private long _ID = -1;
		private long _emailID = -1;
		private DateTime _sendDate = DateTimeUtil.MinValue;
		private long? _listID = null;
		private bool _isTestMailing = true;
		private string _emailXsl = null;		
        #endregion

        #region properties
        [PetaPoco.Column(IsNullable = false)]
        public long ID
		{
			get { return _ID; }
            set { _ID = value; }
		}

        [PetaPoco.Column(IsNullable = false)]
        [ForeignKey(typeof(EmailDocument), "Id")]
        public long EmailID
		{
			get { return _emailID; }
			set { _emailID = value; }
		}

        [PetaPoco.Column(IsNullable = false)]
        public DateTime SendDate 
		{
			get { return _sendDate; }
			set { _sendDate = value; }
		}

        [PetaPoco.Column]
        public long? ListID
		{
			get { return _listID; }
			set { _listID = value; }
		}

        [PetaPoco.Column(IsNullable = false)]
        public bool IsTestMailing 
		{
			get { return _isTestMailing; }
			set { _isTestMailing = value; }
		}

        [PetaPoco.Column]
        public string EmailXsl 
		{
			get { return _emailXsl; }
			set { _emailXsl = value; }
		}
		
        #endregion
    }
}
