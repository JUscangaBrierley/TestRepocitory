using System;
using System.IO;

//using ICSharpCode.SharpZipLib.Zip;

namespace Brierley.FrameWork.Common.IO
{
	/// <summary>
	/// Summary description for IOUtils.
	/// </summary>
	public static class IOUtils
	{
		#region Path

		public static string AppendSeparatorToFolderPath(string folderPath)
		{
			if (folderPath[folderPath.Length - 1] != Path.DirectorySeparatorChar)
			{
				folderPath += Path.DirectorySeparatorChar;
			}
			return folderPath;
		}

		public static string ConvertUriToLocalPath(string uriPath)
		{
			string localPath = uriPath;
			try
			{
				Uri uri = new Uri(uriPath);
				localPath = uri.LocalPath;
			}
			catch
			{
			}
			return localPath;
		}

		#endregion

		#region Files
		public static  bool SaveToFile(string fileName,string data)
		{			
			StreamWriter writer = null;
			try
			{
				if ( File.Exists(fileName) == true )
					File.Delete(fileName);
				writer = new StreamWriter(File.Create(fileName));
				writer.Write(data);
			}			
			finally
			{
				if ( writer != null )
				{
					writer.Close();
				}
			}

			return true;
		}

		public static bool SaveToFile(string fileName, byte[] data)
		{
			BinaryWriter writer = null;
			try
			{
				if (File.Exists(fileName) == true)
					File.Delete(fileName);
				writer = new BinaryWriter(File.Create(fileName));
				writer.Write(data);
			}
			finally
			{
				if (writer != null)
				{
					writer.Close();
				}
			}

			return true;
		}

		public static  string GetFromFile(string fileName)
		{			
			StreamReader reader = null;
			string data = null;
			try
			{
				if ( !File.Exists(fileName) )
				{
                    throw new Exceptions.LWException(fileName + " not found.");					
				}
				reader = new StreamReader(fileName);
				data = reader.ReadToEnd();
			}			
			finally
			{
				if ( reader != null )
				{
					reader.Close();
				}
			}

			return data;
		}

        public static byte[] BytesFromFile(string fileName)
        {
            BinaryReader reader = null;
            byte[] data = null;
            try
            {
                if (!File.Exists(fileName))
                {
                    throw new Exceptions.LWException("File not found.");
                }
                FileInfo fi = new FileInfo(fileName);
                data = new byte[fi.Length];                
                reader = new BinaryReader(File.OpenRead(fileName));
                reader.Read(data, 0, (int)fi.Length);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
            }

            return data;
        }

		public static void CopyFile(string srcPath,string destPath,bool replace)
		{			
			if ( File.Exists(destPath) == true)
			{
				if ( replace == true )
				{                    
					File.Delete(destPath);
				}
				else
				{
					string errMsg = "File " + destPath + " already exists.";
                    throw new Exceptions.LWException(errMsg);                    
				}
			}			            
            int i;
            FileStream fin = null;            
            FileStream fout = null;
            try
            {
                // open input file
                try
                {
                    fin = new FileStream(srcPath, FileMode.Open, FileAccess.Read);                    
                }
                catch (FileNotFoundException)
                {
                    string errMsg = "File " + srcPath + " not found.";
                    throw new Exceptions.LWException(errMsg);
                }

                // open output file
                try
                {
                    fout = new FileStream(destPath, FileMode.Create);
                }
                catch (IOException)
                {
                    string errMsg = "Error opening file " + destPath + " for writing.";
                    throw new Exceptions.LWException(errMsg);
                }

                // Copy File
                do
                {
                    i = fin.ReadByte();
                    if (i != -1) fout.WriteByte((byte)i);
                } while (i != -1);                
            }            
            finally
            {
                if (fin != null)
                {
                    fin.Close();
                }
                if (fout != null)
                {
                    fout.Close();
                }
            }

            

		}

		public static void MoveFile(string srcPath,string destPath,bool replace)
		{
			//string opName = "moveFile";

			if ( File.Exists(destPath) == true)
			{
				if ( replace == true )
				{					
					File.Delete(destPath);
				}
				else
				{
					string errMsg = "File " + destPath + " already exists.";
                    throw new Exceptions.LWException(errMsg);
				}
			}			
			File.Move(srcPath,destPath);
		}
		#endregion
        
		#region Directories
		public static void CreateDirectory(string dirName)
		{
            //const string methodName = "CreateImageFolder";
			string[] tokens = dirName.Split(new char[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar });
			string dir = "";
			for (int i = 0; i < tokens.Length; i++)
			{
				dir += tokens[i] + Path.DirectorySeparatorChar;
				if (!Directory.Exists(dir))
				{					
					Directory.CreateDirectory(dir);
				}
			}
		}

        public static void RemoveDirectoryIfEmpty(string dirName)
        {
            if (!string.IsNullOrEmpty(dirName) && Directory.Exists(dirName) == true)
            {
                string[] files = Directory.GetFiles(dirName);
                if (files == null || files.Length == 0)
                {
                    try
                    {
                        Directory.Delete(dirName);
                    }
                    catch
                    {
                    }
                }
            }
        }

		public static  void RemoveDirectory(string dirName)
		{
			if ( Directory.Exists(dirName) == true )
			{
				string [] files = Directory.GetFiles(dirName);
				foreach ( string file in files )
				{
					File.Delete(file);
				}
				Directory.Delete(dirName);
			}
		}

		public static  void RemoveDirectoryTree(string dirName,bool emptyIt)
		{
			if ( dirName == null )
				return;

			if ( Directory.Exists(dirName) == true )
			{
                if (emptyIt)
                {
                    foreach (string fName in Directory.GetFiles(dirName))
                    {
                        File.Delete(fName);
                    }
                }
				string [] dirs = Directory.GetDirectories(dirName);
				if ( dirs != null )
				{
					foreach ( string dir in dirs )
					{
						RemoveDirectoryTree(dir,emptyIt);
					}
				}
				// now remove all the files in this directory
				string [] files = Directory.GetFiles(dirName);
				foreach ( string file in files )
				{
					File.Delete(file);
				}
				Directory.Delete(dirName);
			}
		}

        /// <summary>
        /// This method either gets the temporary directory pointed to by LWTempPath in the
        /// application configuration file, or if not defined then gets the system temporary path
        /// for the user.
        /// </summary>
        /// <returns>Th etemporary path.</returns>
        public static string GetTempDirectory()
        {
            string tempDir = string.Empty;            
            tempDir = System.Configuration.ConfigurationManager.AppSettings["LWTempPath"];
            if (string.IsNullOrEmpty(tempDir))
            {
                tempDir = Path.GetTempPath();
            }
            if (!tempDir.EndsWith(Path.DirectorySeparatorChar.ToString()))
                tempDir += Path.DirectorySeparatorChar;
            return tempDir;
		}

        /// <summary>
        /// This method either gets the temporary directory pointed to by LWTempPath in the
        /// application configuration file, or if not defined then gets the system temporary path
        /// for the user.
        /// </summary>
        /// <returns>The temporary path.</returns>
        public static string GetTempDirectoryWithPid()
        {
            string tempDir = GetTempDirectory();
            
            // now create a subdirectiectory underneath it with the pid.
            System.Diagnostics.Process p = System.Diagnostics.Process.GetCurrentProcess();
            tempDir = string.Format("{0}{1}{2}", tempDir, p.Id, Path.DirectorySeparatorChar);
            if ( !Directory.Exists(tempDir) )
            {
                Directory.CreateDirectory(tempDir);
            }
            return tempDir;
        }

		#endregion
	}
}
