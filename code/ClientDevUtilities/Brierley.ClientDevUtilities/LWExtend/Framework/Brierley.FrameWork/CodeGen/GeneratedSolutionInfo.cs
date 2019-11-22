//-----------------------------------------------------------------------
//(C) 2008 Brierley & Partners.  All Rights Reserved
//THIS IS UNPUBLISHED PROPRIETARY SOURCE CODE OF Brierley & Partners.
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;

namespace Brierley.FrameWork.CodeGen
{
    public class GeneratedSolutionInfo
    {
        private string solutionPath = string.Empty;
        private string assembilyFileName = string.Empty;
        private string zipFileName = string.Empty;

        public string SolutionPath
        {
            get { return solutionPath; }
            set { solutionPath = value; }
        }

        public string AssemblyFileName
        {
            get { return assembilyFileName; }
            set { assembilyFileName = value; }
        }

        public string ZipFileName
        {
            get { return zipFileName; }
            set { zipFileName = value; }
        }
    }
}
