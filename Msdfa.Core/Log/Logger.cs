using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using log4net;

namespace Msdfa.Core.Log
{
	public class Logger
	{
		public static ILog Log = null;
		public static bool IsEnabled = true;
		public static bool IsDebugEnabled = false;

		public static void Debug(string message)
		{
			if (Log != null && IsEnabled) Log.Debug(message);
			if (IsDebugEnabled) System.Diagnostics.Debug.WriteLine(message);
		}

		public static void Info(string message)
		{
			if (Log != null && IsEnabled) Log.Info(message);
			if (IsDebugEnabled) System.Diagnostics.Debug.WriteLine(message);
		}

		public static void Error(string message)
		{
			if (Log != null && IsEnabled) Log.Error(message);
			if (IsDebugEnabled) System.Diagnostics.Debug.WriteLine(message);
		}

		public static void Fatal(string message)
		{
			if (Log != null && IsEnabled) Log.Fatal(message);
			if (IsDebugEnabled) System.Diagnostics.Debug.WriteLine(message);
		}

		public static void Warn(string message)
		{
			if (Log != null && IsEnabled) Log.Warn(message);
			if (IsDebugEnabled) System.Diagnostics.Debug.WriteLine(message);
		}

		public static void CallingMethodInfo(string prefix, [CallerMemberName] string callerName = "",
			[CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
		{
			var txt = $"[{prefix}] [CallerName: {callerName}] [FilePath: {filePath}] [LineNumber: {lineNumber}]";
			Info(txt);
		}

        public static void Process(Msdfa.DB.Oracle.ActiveConnectionsChangedEventArgs e)
        {
            var str = ConnectionInfoLog.Process(e);
            Debug(str);
        }
	}

	
	public class LogService : ILogService
	{
		public void Info(string msg) => Logger.Info(msg);

		public void Error(string msg) => Logger.Error(msg);

		public void Warn(string msg) => Logger.Warn(msg);

		public void Debug(string msg) => Logger.Debug(msg);
	}
}