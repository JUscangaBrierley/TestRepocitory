using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Configuration;

namespace TlogAuditFiles
{
    public class Program
    {
        private string m_DecryptPath = string.Empty;
        private string m_OutFileName = string.Empty;
        private string m_AEFilePrefix = string.Empty;
        private string m_KidsFilePrefix = string.Empty;
        private string m_FinalFileName = string.Empty;

        static void Main(string[] args)
        {
            Program prog = new Program();
            if (args.Length == 0)
            {
                prog.ProcessAEAudit();
                prog.ProcessKidsAudit();
            }
            else
            {
                switch (args[0].ToLower())
                {
                    case "-tlogaudit":
                        prog.ProcessAEAudit();
                        prog.ProcessKidsAudit();
                        break;
                    case "-class":
                        prog.ProcessAEClass();
                        prog.ProcessKidsClass();
                        break;
                    case "-store":
                        prog.ProcessAEStore();
                        break;
                    default:
                        prog.ProcessAEAudit();
                        prog.ProcessKidsAudit();
                        break;

                }
            }
        }
        public void ProcessAEAudit()
        {
            string fileName = string.Empty;
            string[] inputfiles = null;
            string recordIn = string.Empty;

            Console.WriteLine("Process AE Tlog Audit");
            m_AEFilePrefix = "AEOAUDIT_00";

            m_DecryptPath = ConfigurationManager.AppSettings["DecryptPath"];

            inputfiles = System.IO.Directory.GetFiles(m_DecryptPath, m_AEFilePrefix + "*.dat");

            if (inputfiles.Length > 0)
            {
                fileName = inputfiles[0].Remove(0, m_DecryptPath.Length+1);
            }
            else
            {
                throw new Exception("AE Audit File does not exist");
            }
            m_OutFileName = fileName.Replace(m_AEFilePrefix, "TlogAudit");
            m_FinalFileName = fileName;

            StreamReader sr = new StreamReader(m_DecryptPath + @"\" + fileName);
            StreamWriter sw = new StreamWriter(m_DecryptPath + @"\" + m_OutFileName);

            while (true)
            {
                recordIn = sr.ReadLine();

                if (recordIn == null)
                {
                    sr.Close();
                    sw.Close();
                    break;
                }
                sw.WriteLine(recordIn);
            }
        }
        public void ProcessKidsAudit()
        {
            string fileName = string.Empty;
            string[] inputfiles = null;
            string recordIn = string.Empty;

            m_KidsFilePrefix = "AEOAUDIT_50";
            Console.WriteLine("Process Kids Tlog Audit");

            m_DecryptPath = ConfigurationManager.AppSettings["DecryptPath"];

            inputfiles = System.IO.Directory.GetFiles(m_DecryptPath, m_KidsFilePrefix + "*.dat");

            if (inputfiles.Length > 0)
            {
                fileName = inputfiles[0].Remove(0, m_DecryptPath.Length + 1);
            }
            else
            {
                throw new Exception("77 Kids Audit File does not exist");
            }
            m_OutFileName = fileName.Replace(m_KidsFilePrefix, "TlogAudit");

            StreamReader sr = new StreamReader(m_DecryptPath + @"\" + fileName);
            StreamWriter sw = new StreamWriter(m_DecryptPath + @"\" + m_OutFileName, true);

            while (true)
            {
                recordIn = sr.ReadLine();

                if (recordIn == null)
                {
                    sr.Close();
                    sw.Close();
                    Console.WriteLine("Processing finished");
                    Console.WriteLine("Delete File: " + m_FinalFileName);
                    File.Delete(m_DecryptPath + @"\" + m_FinalFileName);

                    Console.WriteLine("Copy File: " + m_OutFileName + " to " + m_FinalFileName);
                    File.Copy(m_DecryptPath + @"\" + m_OutFileName, m_DecryptPath + @"\" + m_FinalFileName);

                    Console.WriteLine("Delete File: " + fileName);
                    File.Delete(m_DecryptPath + @"\" + fileName);

                    Console.WriteLine("Delete File: " + m_OutFileName);
                    File.Delete(m_DecryptPath + @"\" + m_OutFileName);
                    break;
                }
                sw.WriteLine(recordIn);
            }
        }
        public void ProcessAEClass()
        {
            string fileName = string.Empty;
            string[] inputfiles = null;
            string recordIn = string.Empty;

            m_AEFilePrefix = "IFCLCLS00";

            m_DecryptPath = ConfigurationManager.AppSettings["DecryptPath"];

            inputfiles = System.IO.Directory.GetFiles(m_DecryptPath, m_AEFilePrefix + "*.txt");

            if (inputfiles.Length > 0)
            {
                fileName = inputfiles[0].Remove(0, m_DecryptPath.Length + 1);
            }
            else
            {
                throw new Exception("AE Class File does not exist");
            }
            m_OutFileName = fileName.Replace(m_AEFilePrefix, "Class");
            m_FinalFileName = fileName;

            StreamReader sr = new StreamReader(m_DecryptPath + @"\" + fileName);
            StreamWriter sw = new StreamWriter(m_DecryptPath + @"\" + m_OutFileName);

            while (true)
            {
                recordIn = sr.ReadLine();

                if (recordIn == null)
                {
                    sr.Close();
                    sw.Close();
                    break;
                }
                sw.WriteLine(recordIn);
            }
        }
        public void ProcessKidsClass()
        {
            string fileName = string.Empty;
            string[] inputfiles = null;
            string recordIn = string.Empty;

            m_KidsFilePrefix = "IFCLCLS50";

            m_DecryptPath = ConfigurationManager.AppSettings["DecryptPath"];

            inputfiles = System.IO.Directory.GetFiles(m_DecryptPath, m_KidsFilePrefix + "*.txt");

            if (inputfiles.Length > 0)
            {
                fileName = inputfiles[0].Remove(0, m_DecryptPath.Length + 1);
            }
            else
            {
                throw new Exception("77 Kids Class File does not exist");
            }
            m_OutFileName = fileName.Replace(m_KidsFilePrefix, "Class");

            StreamReader sr = new StreamReader(m_DecryptPath + @"\" + fileName);
            StreamWriter sw = new StreamWriter(m_DecryptPath + @"\" + m_OutFileName, true);

            while (true)
            {
                recordIn = sr.ReadLine();

                if (recordIn == null)
                {
                    sr.Close();
                    sw.Close();
                    File.Delete(m_DecryptPath + @"\" + m_FinalFileName);
                    File.Copy(m_DecryptPath + @"\" + m_OutFileName, m_DecryptPath + @"\" + m_FinalFileName);
                    File.Delete(m_DecryptPath + @"\" + fileName);
                    File.Delete(m_DecryptPath + @"\" + m_OutFileName);
                    break;
                }
                sw.WriteLine(recordIn);
            }
        }
        public void ProcessAEStore()
        {
            string fileName = string.Empty;
            string filePrefix = "IFCLSTR";
            string[] inputfiles = null;
            string recordIn = string.Empty;

            m_DecryptPath = ConfigurationManager.AppSettings["DecryptPath"];

            inputfiles = System.IO.Directory.GetFiles(m_DecryptPath, filePrefix + "*.txt");

            if (inputfiles.Length > 0)
            {
                fileName = inputfiles[0].Remove(0, m_DecryptPath.Length + 1);
            }
            else
            {
                throw new Exception("AE Store file does not exist");
            }
            m_OutFileName = "AEStoreFile.txt";

            if (File.Exists(m_DecryptPath + @"\" + m_OutFileName))
            {
                File.Delete(m_DecryptPath + @"\" + m_OutFileName);
            }
            StreamWriter sw = new StreamWriter(m_DecryptPath + @"\" + m_OutFileName, true);

            foreach (string file in inputfiles)
            {
                StreamReader sr = new StreamReader(Path.Combine(m_DecryptPath, file));
                while (true)
                {
                    recordIn = sr.ReadLine();

                    if (recordIn == null)
                    {
                        sr.Close();
                        break;
                    }
                    sw.WriteLine(recordIn);
                }
            }
            sw.Close();
        }


    }
}
