//-----------------------------------------------------------------------
//(C) 2008 Brierley & Partners.  All Rights Reserved
//THIS IS UNPUBLISHED PROPRIETARY SOURCE CODE OF Brierley & Partners.
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;

using System.Xml;

using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Common.IO;

namespace Brierley.FrameWork.Common
{
    public class CodeGenBase : IDisposable
    {
        private string organizationName;
        private string path;
        private bool overwrite;
        private string fullFileName;
        private FileWriter writer;
        private bool success;
        private bool disposed;

        protected CodeGenBase(string organizationName,string path,bool overwrite)
        {
            this.organizationName = organizationName;
            this.path = path;
            this.overwrite = overwrite;
        }

        protected void Initialize(string filename)
        {
            fullFileName = path + filename;
            writer = new FileWriter(fullFileName, null, overwrite);           
        }

        protected void Initialize(string filename,Encoding encoding)
        {
            fullFileName = path + filename;
            writer = new FileWriter(fullFileName, encoding, overwrite);
        }

        protected string OrganizationName
        {
            get { return organizationName; }
        }

        protected string Path
        {
            get { return path; }
        }

        protected FileWriter Writer
        {
            get{return writer;}
        }

        public bool Success
        {
            get { return success; }
            set { success = value; }
        }

        public void Dispose()
        {
            Dispose(true);            
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    if (writer != null)
                    {
                        if (success)
                            writer.dispose();
                        else
                            writer.dispose(true);
                    }
                }
                disposed = true;
            }
        }

        public string FileName
        {
            get { return fullFileName; }
        }
        #region Helper methods
        
        #endregion
    }
}
