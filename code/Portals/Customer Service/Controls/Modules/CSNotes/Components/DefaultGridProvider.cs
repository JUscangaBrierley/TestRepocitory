using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.WebFrameWork.Controls.Grid;
using Brierley.WebFrameWork.Portal;
using Brierley.WebFrameWork.Portal.Configuration.Modules;

namespace Brierley.LWModules.CSNotes.Components
{
	public class DefaultGridProvider : AspGridProviderBase
    {
		private const string _className = "DefaultGridProvider";
        private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_PORTALMODULES);
        private List<CSNote> _notes = null;
		private long _ipcode = 0;
        private DateTime searchFrom = DateTimeUtil.MinValue;
        private DateTime searchTo = DateTimeUtil.MaxValue;
		private int portalId = 0;
		private int userId = 0;
        private CSNotesConfig _config = null;
        private Dictionary<long, string> agentUsernames = null;

        #region Helpers
        
        protected void LoadCSNotes()
        {
            _notes = CSService.GetNotesByMember(_ipcode, searchFrom, searchTo);
            if(_config != null)
            {
                int direction = _config.IsSortedAscending ? 1 : -1;
                switch(_config.DefaultSortColumn)
                {
                    case "CreateDate":
                        _notes.Sort((a, b) => a.CreateDate.CompareTo(b.CreateDate) * direction);
                        break;

                    case "CreatedBy":
                        agentUsernames = new Dictionary<long, string>();
                        _notes.Sort((a, b) => GetCSAgentUsername(a.CreatedBy).CompareTo(GetCSAgentUsername(b.CreatedBy)) * direction);
                        break;

                    case "Note":
                        _notes.Sort((a, b) => a.Note.CompareTo(b.Note) * direction);
                        break;
                }
            }
        }

		private object GetData(CSNote note, DynamicGridColumnSpec column)
		{
			string methodName = "GetData";

			object value = null;
			if (column.Name == "NoteId")
			{
				value = note.Id;
			}
			else if (column.Name == "CreateDate")
			{
				value = note.CreateDate;
			}
			else if (column.Name == "CreatedBy")
			{
				long id = note.CreatedBy;
				CSAgent agent = CSService.GetCSAgentById(id);
				if (agent != null)
				{
					value = agent.Username;
				}
				else
				{
					value = string.Empty;
					_logger.Error(_className, methodName, "No CSAgent could be retrieved with id = " + id);
				}
			}
			else if (column.Name == "Note")
			{
				value = note.Note;
			}			
			return value;
		}

        private string GetCSAgentUsername(long id)
        {
            if (!agentUsernames.ContainsKey(id))
                agentUsernames.Add(id, CSService.GetCSAgentById(id).Username);
            return agentUsernames[id];
        }

        #endregion

        #region Grid Properties

		public override string Id
		{
			get { return "grdCSNotes"; }
		}

        protected override string GetGridName()
        {
            return "CSNotes";
        }

        public override DynamicGridColumnSpec[] GetColumnSpecs()
        {
            DynamicGridColumnSpec[] columns = new DynamicGridColumnSpec[4];

			int idx = 0;
            DynamicGridColumnSpec c = new DynamicGridColumnSpec();
            c.Name = "NoteId";
            c.DisplayText = ResourceUtils.GetLocalWebResource(ParentControl, Id + "-NoteId.Text", "ID");
            c.DataType = typeof(long);
            c.IsKey = true;
            c.IsEditable = false;
            c.IsVisible = false;            
            columns[idx++] = c;

            c = new DynamicGridColumnSpec();
			c.Name = "CreateDate";
            c.DisplayText = ResourceUtils.GetLocalWebResource(ParentControl, Id + "-CreateDate.Text", "Date");
            c.DataType = typeof(DateTime);
			c.IsEditable = false;
            c.IsSortable = true;
            columns[idx++] = c;            

            c = new DynamicGridColumnSpec();
			c.Name = "CreatedBy";
            c.DisplayText = ResourceUtils.GetLocalWebResource(ParentControl, Id + "-CreatedBy.Text", "User ID");
            c.DataType = typeof(string);
			c.IsEditable = false;
            c.IsSortable = true;
			columns[idx++] = c;

            c = new DynamicGridColumnSpec();
            c.Name = "Note";
            c.DisplayText = ResourceUtils.GetLocalWebResource(ParentControl, Id + "-Note.Text", "Note");
            c.DataType = typeof(string);
			c.EditControlType = DynamicGridColumnSpec.TEXTAREA;
			c.Columns = 50;
			c.Rows = 10;
            c.IsVisible = true;
            c.IsSortable = true;
			c.Validators.Add(new RequiredFieldValidator() { ErrorMessage = ResourceUtils.GetLocalWebResource(ParentControl, Id + "-NoNoteMessage.Text", "Please enter a note."), ValidationGroup = this.ValidationGroup });
			var cv = new CustomValidator() { ClientValidationFunction = "ValidateNoteLength", ErrorMessage = ResourceUtils.GetLocalWebResource(ParentControl, Id + "-NoteLengthMessage.Text", "Note cannot exceed 512 characters."), ValidationGroup = this.ValidationGroup, CssClass = "Validator" };
			cv.ServerValidate += new ServerValidateEventHandler(cv_ServerValidate);
			c.Validators.Add(cv);
            columns[idx++] = c;
            
            return columns;
        }

		void cv_ServerValidate(object source, ServerValidateEventArgs args)
		{
			args.IsValid = args.Value.Length <= 512;
		}

        public override bool IsGridEditable()
        {
            //var agent = PortalState.GetLoggedInCSAgent();
            //return agent.HasPermission(CSFunction.LW_CSFUNCTION_CREATECSNOTES);            
            // See the comment in issue PI 25144.
            return _config != null ? !_config.IsReadOnly : true;                        
        }

        public override string GetGridInsertLabel()
        {
            return ResourceUtils.GetLocalWebResource(ParentControl, "AddNote.Text", "Add Note");
        }

		public override bool IsButtonVisible(string commandName)
		{
			if (commandName == AspDynamicGrid.EDIT_ROW_COMMAND ||
				commandName == AspDynamicGrid.DELETE_ROW_COMMAND)
			{
				return false;
			}
			else
			{
				return base.IsButtonVisible(commandName);
			}
		}

		public override string GetEmptyGridMessage()
		{
			return ResourceUtils.GetLocalWebResource(ParentControl, "NoNotesFound.Text", "No notes found.");
		}

        public override bool IsActionColumnVisible()
        {
            return false;
        }
        #endregion

        #region Data Source

		public override void SetSearchParm(string parmName, object parmValue)
        {
			if (parmName == "IpCode")
			{
				_ipcode = (long)parmValue;
			}
            else if (parmName == "SearchFrom")
            {
                searchFrom = (DateTime)parmValue;
            }
            else if (parmName == "SearchTo")
            {
                searchTo = (DateTime)parmValue;
            }
			else if (parmName == "PortalId")
			{
				portalId = (int)parmValue;
			}
			else if (parmName == "UserId")
			{
				userId = (int)parmValue;
			}
            else if (parmName == "Configuration")
            {
                _config = (CSNotesConfig)parmValue;
            }
        }

        public override void LoadGridData()
        {
			LoadCSNotes();          
        }

        public override void SaveGridData(DynamicGridColumnSpec[] columns, AspDynamicGrid.GridAction gridAction)
        {
			string methodName = "SaveGridData";

			try
			{
				CSNote note = new CSNote();
				Member m = PortalState.CurrentMember;
				if (m != null)
				{
					note.MemberId = m.IpCode;
				}
				else
				{
					_logger.Error(_className, methodName, "No member has yet been selected.");
					throw new LWException("No member has yet been selected.");
				}
				CSAgent agent = PortalState.GetLoggedInCSAgent();
				if (agent == null)
				{
					string errMsg = string.Format(ResourceUtils.GetLocalWebResource(ParentControl, "NoCSAgent.Text", "Unable to get CSAGent.  PortalId = {0}, UserId = {1}"),
						portalId, userId);
					_logger.Error(_className, methodName, errMsg);
					throw new LWDynamicGridException(errMsg);
				}
				else
				{
					note.CreatedBy = agent.Id;
				}

				foreach (DynamicGridColumnSpec column in columns)
				{
					if (column.Name == "Note")
					{
						note.Note = StringUtils.DeHTML(StringUtils.FriendlyString(column.Data));
					}
				}

                CSService.CreateNote(note);
			}
			catch (Exception ex)
			{
				_logger.Error(_className, methodName, "Error savind CSNotes.", ex);
				throw;
			}
        }

        public override int GetNumberOfRows()
        {
			return (_notes != null ? _notes.Count : 0);            
        }

        public override object GetColumnData(int rowIndex, DynamicGridColumnSpec column)
        {
            CSNote note = _notes[rowIndex];
			return GetData(note, column);            
        }
                
        #endregion
    }
}
