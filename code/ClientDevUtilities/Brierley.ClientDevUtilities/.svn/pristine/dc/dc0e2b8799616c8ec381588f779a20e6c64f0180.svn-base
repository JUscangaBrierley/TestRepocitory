using System;
using System.Data;
using System.Collections.Generic;
using System.Text;

namespace Brierley.FrameWork.Data.DomainModel
{
    /// <summary>
    /// A collection of structured data, usually the result of evaluating a data filter.
    /// </summary>
    public class StructuredDataRows
    {
        #region private data
        private DataTable _table;
        private int _currentRowIndex;
        #endregion

        #region constructors
        internal StructuredDataRows()
        {
            _currentRowIndex = -1;
            _table = null;
        }
        #endregion

        #region public methods
        /// <summary>
        /// Get a row of data.
        /// </summary>
        /// <param name="index">zero-based index of the row</param>
        /// <returns>System.Data.DataRow containing the row of data</returns>
        public DataRow GetRow(int index)
        {
            DataRow result = null;
            if (_table != null && _table.Rows != null && index >= 0 && index < _table.Rows.Count)
            {
                result = _table.Rows[index];
            }
            return result;
        }

        /// <summary>
        /// Get the current row of data.
        /// </summary>
        /// <returns>System.Data.DataRow containing the row of data</returns>
        public DataRow GetCurrentRow()
        {
            return GetCurrentRow(false);
        }

        /// <summary>
        /// Get the current row of data.
        /// </summary>
        /// <param name="incrementToNextRow">boolean indicating whether row index should be incremented</param>
        /// <returns>System.Data.DataRow containing the row of data</returns>
        public DataRow GetCurrentRow(bool incrementToNextRow)
        {
            DataRow result = GetRow(_currentRowIndex);
            if (incrementToNextRow)
            {
                _currentRowIndex++;
            }
            return result;
        }
        #endregion

        #region indexers
        /// <summary>
        /// Get a row of data.
        /// </summary>
        /// <param name="index">zero-based index of the row</param>
        /// <returns>System.Data.DataRow containing the row of data</returns>
        public DataRow this[int index]
        {
            get { return GetRow(index); }
        }
        #endregion

        #region attributes
        /// <summary>
        /// The underlying System.Data.DataTable.
        /// </summary>
        public DataTable Table
        {
            get { return _table; }
            set { _table = value; }
        }

        /// <summary>
        /// Indicates the current index in the row of data.
        /// </summary>
        public int CurrentRowIndex
        {
            get { return _currentRowIndex; }
            set { _currentRowIndex = value; }
        }

        /// <summary>
        /// Indicates the number of rows of data.
        /// </summary>
        public int Count
        {
            get 
            {
                int result = 0;
                if (_table != null && _table.Rows != null)
                    result = _table.Rows.Count;
                return result; 
            }
        }
        #endregion
    }
}
