//-----------------------------------------------------------------------
//(C) 2008 Brierley & Partners.  All Rights Reserved
//THIS IS UNPUBLISHED PROPRIETARY SOURCE CODE OF Brierley & Partners.
//-----------------------------------------------------------------------


using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Reflection;
using System.Text;

using Chilkat;

using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Logging;

namespace Brierley.FrameWork.Common.IO
{
	public class ZipUtils
    {
        #region Fields		
        private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_FRAMEWORK);
		private const string _className = "ZipUtils";
        #endregion
        
        public static bool ZipFolder(string srcFolder, string dstFile)
		{
			const string methodName = "ZipFolder";
            bool result = false;

            Zip zip = new Zip();
            zip.TempDir = Path.GetDirectoryName(dstFile);
            if (!zip.UnlockComponent("RWENTXZIP_gO0huPmmAAyd"))
            {
                _logger.Critical(_className, methodName, "Unable to unlock Chilkat zip components.");
                throw new LWException("Unable to unlock Chilkat zip components.");
            }
            if (zip.NewZip(dstFile))
            {
                string strResult = zip.AppendFromDir = srcFolder;
                if (zip.AppendFilesEx("*", true, true, false, true, true))
                {
                    result = zip.WriteZipAndClose();
                    if (!result)
                    {
						_logger.Error(_className, methodName, 
                            string.Format("Unable to write files to zip file {0}.  Error = {1}.", dstFile, zip.LastErrorText));
                    }
                    else
                    {
                        result = true;
                    }
                }
                else
                {
					_logger.Error(_className, methodName, 
                            string.Format("Unable to append files to {0}.  Error = {1}.", dstFile, zip.LastErrorText));                    
                }                                       
            }
            else
            {
				_logger.Error(_className, methodName, 
                             string.Format("Unable to create zip file {0}.  Error = {1}.", dstFile, zip.LastErrorText));                
            }
            return result;                          
		}

        public static bool UnZip(string zipFile, string dstFolder, bool overwriteExisting, bool deleteIt)
        {
			const string methodName = "UnZip";
            bool result = false;

            Zip zip = new Zip();
            if (!zip.UnlockComponent("RWENTXZIP_gO0huPmmAAyd"))
            {
				_logger.Critical(_className, methodName, "Unable to unlock Chilkat zip components.");
                throw new LWException("Unable to unlock Chilkat zip components.");
            }
			zip.IgnoreAccessDenied = false;
            if (zip.OpenZip(zipFile))
            {
                zip.OverwriteExisting = overwriteExisting;
                result = zip.Extract(dstFolder);
                if (!result)
                {
					_logger.Error(_className, methodName, 
                           string.Format("Unable to extract files to {0}.  Error = {1}.", dstFolder, zip.LastErrorText));
                }
                zip.CloseZip();
                if (deleteIt)
                {
                    File.Delete(zipFile);
                }
            }
            else
            {
				_logger.Error(_className, methodName, "Unable to open zip file " + zipFile);
            }
            return result;
        }

        public static void UnZip(string zipFile, string dstFolder, bool deleteIt)
        {
            UnZip(zipFile, dstFolder, true, deleteIt);
        }
	}
}
