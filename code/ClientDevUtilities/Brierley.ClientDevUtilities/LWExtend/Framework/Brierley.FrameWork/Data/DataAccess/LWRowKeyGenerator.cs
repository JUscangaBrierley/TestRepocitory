//-----------------------------------------------------------------------
//(C) 2008 Brierley & Partners.  All Rights Reserved
//THIS IS UNPUBLISHED PROPRIETARY SOURCE CODE OF Brierley & Partners.
//-----------------------------------------------------------------------


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;

using NHibernate.Engine;
using NHibernate.Exceptions;
using NHibernate.SqlCommand;
using NHibernate.SqlTypes;
using NHibernate.Type;
using NHibernate.Util;
using NHibernate.Dialect;
using NHibernate.Id;

namespace Brierley.FrameWork.Data.DataAccess
{
	/*
	 * This class shoudl have really been derived from SequenceGenerator and only its 
	 * SqlDropString method should have been redefined.  However, since in nHibernate,
	 * implementation, that method is not virtual, we are forced to copy the entire 
	 * class and then change the implementation of that method.
	 * */
	//public class LWRowKeyGenerator : SequenceGenerator
	//{
	//    public LWRowKeyGenerator()
	//    {
	//        Console.WriteLine("LWRowKeyGenerator being created...");
	//    }

	//    public new string[] SqlDropString(NHibernate.Dialect.Dialect dialect)
	//    {
	//        return new string[0];
	//    } 
	//}
	public class LWRowKeyGenerator : IPersistentIdentifierGenerator, NHibernate.Id.IConfigurable
	{
		/////// Source copied from NHibernate.Id.SequenceGenerator ////////////////
		/// <summary>
		/// The name of the sequence parameter.
		/// </summary>
		public const string Sequence = "sequence";

		/// <summary> 
		/// The parameters parameter, appended to the create sequence DDL.
		/// For example (Oracle): <tt>INCREMENT BY 1 START WITH 1 MAXVALUE 100 NOCACHE</tt>.
		/// </summary>
		public const string Parameters = "parameters";

		private string sequenceName;
		private IType identifierType;
		private SqlString sql;
		private string parameters;

		public string SequenceName
		{
			get { return sequenceName; }
		}

		#region IConfigurable Members

		/// <summary>
		/// Configures the SequenceGenerator by reading the value of <c>sequence</c> and
		/// <c>schema</c> from the <c>parms</c> parameter.
		/// </summary>
		/// <param name="type">The <see cref="IType"/> the identifier should be.</param>
		/// <param name="parms">An <see cref="IDictionary"/> of Param values that are keyed by parameter name.</param>
		/// <param name="dialect">The <see cref="Dialect.Dialect"/> to help with Configuration.</param>
		public virtual void Configure(IType type, IDictionary<string, string> parms, Dialect dialect)
		{
			var nativeSequenceName = PropertiesHelper.GetString(Sequence, parms, "hibernate_sequence");
			bool needQuote = StringHelper.IsBackticksEnclosed(nativeSequenceName);
			bool isQuelified = nativeSequenceName.IndexOf('.') > 0;
			if (isQuelified)
			{
				string qualifier = StringHelper.Qualifier(nativeSequenceName);
				nativeSequenceName = StringHelper.Unqualify(nativeSequenceName);
				nativeSequenceName = StringHelper.PurgeBackticksEnclosing(nativeSequenceName);
				sequenceName = qualifier + '.' + (needQuote ? dialect.QuoteForTableName(nativeSequenceName) : nativeSequenceName);
			}
			else
			{
				nativeSequenceName = StringHelper.PurgeBackticksEnclosing(nativeSequenceName);
				sequenceName = needQuote ? dialect.QuoteForTableName(nativeSequenceName) : nativeSequenceName;
			}
			string schemaName;
			string catalogName;
			parms.TryGetValue(Parameters, out parameters);
			parms.TryGetValue(PersistentIdGeneratorParmsNames.Schema, out schemaName);
			parms.TryGetValue(PersistentIdGeneratorParmsNames.Catalog, out catalogName);

			if (!isQuelified)
			{
				sequenceName = dialect.Qualify(catalogName, schemaName, sequenceName);
			}

			identifierType = type;
			sql = new SqlString(dialect.GetSequenceNextValString(sequenceName));
		}

		#endregion

		#region IIdentifierGenerator Members

		/// <summary>
		/// Generate an <see cref="Int16"/>, <see cref="Int32"/>, or <see cref="Int64"/> 
		/// for the identifier by using a database sequence.
		/// </summary>
		/// <param name="session">The <see cref="ISessionImplementor"/> this id is being generated in.</param>
		/// <param name="obj">The entity for which the id is being generated.</param>
		/// <returns>The new identifier as a <see cref="Int16"/>, <see cref="Int32"/>, or <see cref="Int64"/>.</returns>
		public virtual object Generate(ISessionImplementor session, object obj)
		{
			try
			{
				IDbCommand cmd = session.Batcher.PrepareCommand(CommandType.Text, sql, SqlTypeFactory.NoTypes);
				IDataReader reader = null;
				try
				{
					reader = session.Batcher.ExecuteReader(cmd);
					try
					{
						reader.Read();
						return IdentifierGeneratorFactory.Get(reader, identifierType, session);												
					}
					finally
					{
						reader.Close();
					}
				}
				finally
				{
					session.Batcher.CloseCommand(cmd, reader);
				}
			}
			catch (DbException sqle)
			{
				throw ADOExceptionHelper.Convert(session.Factory.SQLExceptionConverter, sqle, "could not get next sequence value");
			}
		}

		#endregion

		#region IPersistentIdentifierGenerator Members

		/// <summary>
		/// The SQL required to create the database objects for a SequenceGenerator.
		/// </summary>
		/// <param name="dialect">The <see cref="Dialect.Dialect"/> to help with creating the sql.</param>
		/// <returns>
		/// An array of <see cref="String"/> objects that contain the Dialect specific sql to 
		/// create the necessary database objects for the SequenceGenerator.
		/// </returns>
		public string[] SqlCreateStrings(Dialect dialect)
		{
			//string baseDDL = dialect.GetCreateSequenceString(sequenceName);
			//string paramsDDL = null;
			//if (parameters != null)
			//{
			//    paramsDDL = ' ' + parameters;
			//}
			//return new string[] { string.Concat(baseDDL, paramsDDL) };
			return new string[0];
		}

		/// <summary>
		/// The SQL required to remove the underlying database objects for a SequenceGenerator.
		/// </summary>
		/// <param name="dialect">The <see cref="Dialect.Dialect"/> to help with creating the sql.</param>
		/// <returns>
		/// A <see cref="String"/> that will drop the database objects for the SequenceGenerator.
		/// </returns>
		public string[] SqlDropString(Dialect dialect)
		{
			//return new string[] { dialect.GetDropSequenceString(sequenceName) };
			return new string[0];
		}

		/// <summary>
		/// Return a key unique to the underlying database objects for a SequenceGenerator.
		/// </summary>
		/// <returns>
		/// The configured sequence name.
		/// </returns>
		public string GeneratorKey()
		{
			return sequenceName;
		}

		#endregion
	}
}
