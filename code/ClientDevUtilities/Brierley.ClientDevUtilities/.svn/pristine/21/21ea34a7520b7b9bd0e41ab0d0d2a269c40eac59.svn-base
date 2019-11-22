//-----------------------------------------------------------------------
//(C) 2008 Brierley & Partners.  All Rights Reserved
//THIS IS UNPUBLISHED PROPRIETARY SOURCE CODE OF Brierley & Partners.
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;

using System.IO;

namespace Brierley.FrameWork.Common.IO
{
    public class FileWriter : IDisposable
    {
        private StreamWriter writer_;
        private string fileName_ = null;
        public bool _disposed = false;



        public FileWriter(string fileName, Encoding encoding,bool overwrite, string newLineCharacter = "")
        {

            if (File.Exists(fileName))
            {
                if (overwrite == true)
                {
                    File.Delete(fileName);
                }
                else
                {
                    // throw an exception
                    throw new Exceptions.LWException(fileName + " already exists");
                }
            }

            FileStream fstream = File.Create(fileName);
            if (encoding != null)
            {
                writer_ = new StreamWriter(fstream, encoding);
            }
            else
            {
                writer_ = new StreamWriter(fstream);
            }

            fileName_ = fileName;

            //adding an overwrite for the new line character per LW3311
            if(newLineCharacter != null)
            {
                switch(newLineCharacter)
                {
                    case "CRLF":
                        writer_.NewLine = "\r\n";
                        break;
                    case "LF":
                        writer_.NewLine = "\n";
                        break;
                    default:
                        writer_.NewLine = Environment.NewLine;
                        break;
                }

            }

            
        }

        public Encoding CurrentEncoding
        {
            get { return writer_.Encoding; }            
        }

        public void write(string stuff)
        {
            //string opName = "write";

            try
            {
                if (writer_ != null)
                {
                    writer_.Write(stuff);
                }
            }
            catch (Exception e)
            {
                throw new Exceptions.LWException("Error writing to file", e);           
            }
        }

        public void indent(int idnt)
        {
            for (int idx = 0; idx < idnt; idx++)
            {
                write(" ");
            }
        }

        public void emptyLine()
        {
            writeLine(" ");
        }

        public void write(int idnt, string stuff)
        {
            //string opName = "write";

            indent(idnt);

            try
            {
                if (writer_ != null)
                {
                    writer_.Write(stuff);
                }
            }
            catch (Exception e)
            {
                throw new Exceptions.LWException("Error writing to file", e);
            }
        }

        public void writeLine(string stuff)
        {
            //string opName = "writeLine";

            try
            {
                if (writer_ != null)
                {
                    writer_.WriteLine(stuff);
                }
            }
            catch (Exception e)
            {
                throw new Exceptions.LWException("Error writing to file", e);
            }
        }

        public void writeLine(int idnt, string stuff)
        {
            //string opName = "writeLine";

            indent(idnt);

            try
            {
                if (writer_ != null)
                {
                    writer_.WriteLine(stuff);
                }
            }
            catch (Exception e)
            {
                throw new Exceptions.LWException("Error writing to file", e);
            }
        }
        public void newLine()
        {
            //string opName = "newLine";

            try
            {
                if (writer_ != null)
                {
                    writer_.WriteLine();
                }
            }
            catch (Exception e)
            {
                throw new Exceptions.LWException("Error writing to file", e);
            }
        }

        public void dispose()
        {
            if (writer_ != null)
            {
                try
                {
                    writer_.Close();
                    writer_ = null;
                }
                catch (Exception)
                {
                }
            }
            _disposed = true;
        }

        public void dispose(bool delete)
        {
            dispose();
            if (delete && fileName_ != null && File.Exists(fileName_) == true)
            {
                try
                {
                    File.Delete(fileName_);
                }
                catch (Exception)
                {
                }
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
