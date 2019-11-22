using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace BuildTestFiles
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            string FileName = "";
            if (args.Length > 0)
            {
                foreach (string argument in args)
                {
                    FileName += argument + " ";
                }
            }
            Application.Run(new fMain(FileName));
        }
    }
}
