using System;

namespace GitServer.Settings
{
	public class LogSettings
    {
		private string _logFile;

		/// <summary>
		/// The path to the log file. <c>{0}</c> will be substituted with current date by getter.
		/// </summary>
		public string LogFile
		{
			get { return string.Format(_logFile, DateTime.Now.ToString("yyyyMMdd")); }
			set { _logFile = value; }
		}
    }
}
