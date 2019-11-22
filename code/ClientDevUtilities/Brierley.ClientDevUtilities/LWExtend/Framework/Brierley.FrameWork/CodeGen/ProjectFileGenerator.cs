//-----------------------------------------------------------------------
//(C) 2008 Brierley & Partners.  All Rights Reserved
//THIS IS UNPUBLISHED PROPRIETARY SOURCE CODE OF Brierley & Partners.
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.IO;

namespace Brierley.FrameWork.CodeGen
{
    public class ProjectFileGenerator : CodeGenBase
    {
        private string _templatePath;
		private string _templateName;
        private string _organizationName;

        public ProjectFileGenerator(string organizationName, string templatePath, string templateName, string outPath, string projectFileName, bool overwrite)
            : base(organizationName, outPath, overwrite)
        {
            _templatePath = templatePath;
			_templateName = templateName;
            _organizationName = organizationName;

            Encoding encoding = Encoding.GetEncoding("windows-1252");
			//Initialize("MemberProcessing.btproj", encoding);
			Initialize(projectFileName, encoding);
        }

        public void Generate()
        {
            try
            {
				//string str = IOUtils.GetFromFile(_templatePath + "MemberProcessing.btproj");
				string str = IOUtils.GetFromFile(_templatePath + _templateName);
                str = str.Replace("##OrganizationName##", _organizationName);                                
                Writer.writeLine(str);
                Success = true;                 
            }
            finally
            {
                Dispose();
            }
        }		        
    }
}
