using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Brierley.FrameWork.Data.DomainModel
{
	[PetaPoco.ExplicitColumns]
	[PetaPoco.PrimaryKey("Id", sequenceName = "hibernate_sequence")]
	[PetaPoco.TableName("LW_TestList")]
	public class TestList : LWCoreObjectBase
	{
		private string _listString = null;
		private List<Dictionary<string, string>> _list;

		[PetaPoco.Column(IsNullable = false)]
		public long Id { get; set; }

		[PetaPoco.Column(IsNullable = false)]
		public long TemplateId { get; set; }

		[PetaPoco.Column(Length = 50)]
		public string Name { get; set; }

		[PetaPoco.Column(Length = 255)]
		public string Description { get; set; }

		[PetaPoco.Column(Length = 2000)]
		public string DestinationAddress { get; set; }

		//json string
		[PetaPoco.Column("List")]
		public string ListString 
		{
			get
			{
				return JsonConvert.SerializeObject(List);
			}
			set
			{
				_listString = value;
				_list = null;
			}
		}

		public List<Dictionary<string, string>> List
		{
			get
			{
				if (_list == null)
				{
					PopulateListDS();
				}
				return _list;
			}
			set
			{
				_list = value;
			}
		}

		//todo: this had no references in 4.5. safe to delete?
		//[PetaPoco.Column]
		//public FieldCollection Fields { get; set; }

		/// <summary>
		/// Creates and returns a list of each destination address combined with each row in the list
		/// </summary>
		public List<Dictionary<string, string>> CompileFullList()
		{
			var ret = new List<Dictionary<string, string>>();

			if (!string.IsNullOrEmpty(DestinationAddress))
			{
				foreach (string address in DestinationAddress.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
				{
					foreach (var row in List)
					{
						row["recipientemail"] = address;
						ret.Add(row);
					}
				}
			}
			return ret;
		}

		public virtual TestList Clone()
		{
			return Clone(new TestList());
		}

		public virtual TestList Clone(TestList other)
		{
			other.TemplateId = TemplateId;
			other.Name = Name;
			other.Description = Description;
			other.DestinationAddress = DestinationAddress;
			other.ListString = ListString;
			return (TestList)base.Clone(other);
		}

		//todo: this should not be handled here. handle in LN instead
		public void Validate()
		{
			if (string.IsNullOrEmpty(Name))
			{
				throw new Exception("Please provide a list name.");
			}

			if (string.IsNullOrEmpty(DestinationAddress))
			{
				throw new Exception("Please provide a Tester Email Address.");
			}

			if (!Brierley.FrameWork.Email.EmailUtil.ValidateEmailAddress(DestinationAddress))
			{
				throw new Exception("Please provide a valid email address.");
			}
		}

		public void DeleteRow(int index)
		{
			List.RemoveAt(index);
		}

		public void UpdateListMember(int index, Dictionary<string, string> values)
		{
			List[index] = values;
		}

		/// <summary>
		/// Adds a recipient to the Test List
		/// </summary>
		/// <param name="recipientValues">Dictionary of Field/Value pairs</param>
		public void AddRecipient(Dictionary<string, string> recipientValues)
		{
			List.Add(recipientValues);
		}

		public void PopulateFromTemplate()
		{
			using (var content = LWDataServiceUtil.ContentServiceInstance())
			{
				Template template = content.GetTemplate(TemplateId);
				FieldCollection fields = new FieldCollection(template.Fields);
				if (fields.CountFields() > 0)
				{
					List = fields.GenerateList();
				}
			}
		}

		/// <summary>
		/// Updates list fields based on fields in the template.
		/// </summary>
		/// <remarks>
		/// This method compares the current set of fields in the lists's template to the list 
		/// and adds or removes any fields that have been added or removed from the template.
		/// </remarks>
		public void RefreshTemplateFields()
		{
			using (var content = LWDataServiceUtil.ContentServiceInstance())
			{
				Template template = content.GetTemplate(TemplateId);
				PopulateListDS();

				if (List == null || List.Count == 0)
				{
					//if no records exist in the test list, populate from template and get all field permutations
					PopulateFromTemplate();
				}
				else
				{
					List<string> fieldNames = new FieldCollection(template.Fields).FieldNames;
					foreach (string s in fieldNames)
					{
						foreach (var row in List)
						{
							if (!row.ContainsKey(s))
							{
								row.Add(s, string.Empty);
							}
						}
					}
				}
			}
		}

		private void PopulateListDS()
		{
			if (!string.IsNullOrEmpty(_listString))
			{
				_list = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(_listString);
			}
			else
			{
				_list = new List<Dictionary<string, string>>();
			}
		}
	}
}
