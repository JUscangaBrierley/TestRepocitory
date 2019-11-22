using System;
using System.Collections.Generic;
using System.Text;

namespace Brierley.FrameWork.Common.Exceptions
{
    /// <summary>
    /// Indicates that a cirular path was found in the state diagram.
    /// </summary>
    public class CircularPathException : Exception
    {
        private long _duplicateStateID = -1;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="message">a message</param>
        /// <param name="duplicateStateID">the unique identifier of the state involved in the circular path</param>
        public CircularPathException(string message, long duplicateStateID)
        {
            _duplicateStateID = duplicateStateID;
        }

        /// <summary>
        /// The unique identifier of the state involved in the circular path.
        /// </summary>
        public long DuplicateStateID
        {
            get { return _duplicateStateID; }
            set { _duplicateStateID = value; }
        }
    }
}
