using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Data.ModelAttributes;

namespace Brierley.FrameWork.Data.DomainModel
{
	[PetaPoco.ExplicitColumns]
	[PetaPoco.PrimaryKey("ID", sequenceName = "hibernate_sequence")]
	[PetaPoco.TableName("LW_PromoDataFile")]
	public class PromoDataFile : LWCoreObjectBase
    {
        Dictionary<string, List<string>> _columnMap = new Dictionary<string, List<string>>();

        [PetaPoco.Column(IsNullable = false)]
        public virtual long ID { get; set; }

        [PetaPoco.Column(Length = 50, IsNullable = false)]
        [UniqueIndex(RequiresLowerFunction = false)]
        public virtual string Name { get; set; }

        [PetaPoco.Column(Length = 500)]
		public virtual string Description { get; set; }

        [PetaPoco.Column(IsNullable = false)]
        public virtual bool IsTakenInOrder { get; set; }

        [PetaPoco.Column(Length = 50, IsNullable = false)]
		public virtual string ColumnDelimiter { get; set; }

        [PetaPoco.Column]
		public virtual string ColumnNames { get; set; }

        [PetaPoco.Column]
		public virtual string Content { get; set; }


        public virtual List<string> GetColumnValues(string columnName)
        {            
            if (!_columnMap.ContainsKey(columnName))
            {
                string[] tokens = ColumnNames.Split(ColumnDelimiter.ToCharArray());
                int tokenPos = -1;
                for (int i = 0; i < tokens.Length; i++)
                {
                    if (tokens[i] == columnName)
                    {
                        tokenPos = i;
                        break;
                    }
                }
                if (tokenPos != -1)
                {
                    MemoryStream mem = new MemoryStream(UTF8Encoding.UTF8.GetBytes(Content));
                    List<string> columnValues = new List<string>();
                    using (StreamReader reader = new StreamReader(mem))
                    {
                        while (true)
                        {
                            string line = reader.ReadLine();
                            if (string.IsNullOrWhiteSpace(line))
                            {
                                break;
                            }
                            string[] lineTokens = line.Split(ColumnDelimiter.ToCharArray());
                            if (lineTokens.Length != tokens.Length)
                            {
                                // this column name is not found.
                                string errMsg = string.Format("Line {0} in file {1} does not have the correct numner of columns.", line, Name);
                                throw new LWException(errMsg) { };
                            }
                            columnValues.Add(lineTokens[tokenPos]);
                        }
                    }
                    _columnMap.Add(columnName, columnValues);
                }
                else
                {
                    // this column name is not found.
                    string errMsg = string.Format("Column {0} does not exist in file {1}.", columnName, Name);
                    throw new LWException(errMsg) { };
                }
            }
            return _columnMap.ContainsKey(columnName) ? _columnMap[columnName] : null;
        }
        
		public virtual PromoDataFile Clone()
		{
			return Clone(new PromoDataFile());
		}

		public virtual PromoDataFile Clone(PromoDataFile dest)
		{
			dest.Name = Name;
			dest.Description = Description;
			dest.IsTakenInOrder = IsTakenInOrder;
			dest.ColumnDelimiter = ColumnDelimiter;
			dest.ColumnNames = ColumnNames;
			dest.Content = Content;
			return (PromoDataFile)base.Clone(dest);
		}
	}
}
