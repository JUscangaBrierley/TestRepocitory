using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Logging;

namespace Brierley.FrameWork.Common.Process
{
	public class ManagedProcess
	{
		private const string _className = "ManagedProcess";
		private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_FRAMEWORK);

		private StringBuilder _stdout = new StringBuilder();
		private StringBuilder _stderr = new StringBuilder();

		public bool ClearOutputOnExecute { get; set; }
		public string StdOut { get { return _stdout.ToString(); } }
		public string StdErr { get { return _stderr.ToString(); } }

		public ManagedProcess()
		{
			ClearOutputOnExecute = true;
		}

		public int Execute(string cmd, string args)
		{
			const string methodName = "Execute";

			if (ClearOutputOnExecute)
			{
				_stdout.Clear();
				_stderr.Clear();
			}

			System.Diagnostics.Process process = new System.Diagnostics.Process();
			int exitCode = -1;
			try
			{
				_logger.Debug(_className, methodName, "Command file: " + cmd);
				_logger.Debug(_className, methodName, "Arguments: " + args);

				process.StartInfo.FileName = cmd;
				process.StartInfo.CreateNoWindow = true;
				process.StartInfo.Arguments = args;
				process.StartInfo.UseShellExecute = false;
				process.StartInfo.RedirectStandardOutput = true;
				process.StartInfo.RedirectStandardError = true;
				process.OutputDataReceived += new System.Diagnostics.DataReceivedEventHandler(process_OutputDataReceived);
				process.ErrorDataReceived += new DataReceivedEventHandler(process_ErrorDataReceived);
				process.Start();
				process.BeginOutputReadLine();
				process.BeginErrorReadLine();
				process.WaitForExit();
				exitCode = process.ExitCode;

				_logger.Debug(_className, methodName, "Exit Code: " + exitCode);

				if (exitCode != 0)
				{
					_logger.Error(_className, methodName, "Command returned nonzero exit code: " + exitCode);
					_logger.Error(_className, methodName, "Command file: " + cmd);
					_logger.Error(_className, methodName, "Command arguments: " + args);
					if (_stderr.Length > 0)
					{
						_logger.Error(_className, methodName, "Command stderr: " + Environment.NewLine + _stderr.ToString());
					}
					if (_stdout.Length > 0)
					{
						_logger.Error(_className, methodName, "Command stdout: " + Environment.NewLine + _stdout.ToString());
					}
				}
			}
			catch (Exception ex)
			{
				string msg = string.Format("Error executing command: {0} {1}", cmd, args);
				_logger.Error(_className, methodName, msg, ex);
				throw;
			}
			finally
			{
				process.Close();
			}
			return exitCode;
		}

		void process_OutputDataReceived(object sender, DataReceivedEventArgs e)
		{
			if (e != null && e.Data != null && !string.IsNullOrEmpty(e.Data))
			{
				_stdout.AppendLine(e.Data);
			}
		}

		void process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
		{
			if (e != null && e.Data != null && !string.IsNullOrEmpty(e.Data))
			{
				_stderr.AppendLine(e.Data);
			}
		}
	}
}
