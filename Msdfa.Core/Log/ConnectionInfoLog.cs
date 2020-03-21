using System;
using System.Collections.Generic;
using Msdfa.DB;

namespace Msdfa.Core.Log
{
	/// <summary>
	/// klasa ułatwia logowanie połaczeń 
	/// </summary>
	public static class ConnectionInfoLog
	{
		private static readonly Dictionary<string, ConnectionStatus> CnnPool = new Dictionary<string, ConnectionStatus>();

		private class ConnectionStatus
		{
			private DateTime Start { get; set; }
			private string Description { get; set; }

			public ConnectionStatus(Msdfa.DB.Oracle.ActiveConnectionsChangedEventArgs e)
			{
				Guid = e.Guid;
				Start = DateTime.Now;
				Description = e.Description;
			}

			public string Guid { get; private set; }
			public int Count { get; set; }
			public DateTime? End { get; set; }

			public override string ToString()
			{
				var timeTxt = "wykonanie w : " + (End.HasValue ? $"{(End.Value - Start).TotalMilliseconds:N0} ms " : "").PadLeft(13);
				return $"Start {Start:HH:mm:ss}, cnnGuid : {Guid} , cnnCount : {Count} , {timeTxt}, metoda: {Description}";
			}
		}

		public static string Process(Msdfa.DB.Oracle.ActiveConnectionsChangedEventArgs e)
		{
			if (!CnnPool.ContainsKey(e.Guid))
			{
				return AddToPool(e);
			}
			else
			{
				return UpdatePool(e);
			}
		}

		private static string AddToPool(Msdfa.DB.Oracle.ActiveConnectionsChangedEventArgs e)
		{
			var status = new ConnectionStatus(e) { Count = CnnPool.Count + 1 };
			CnnPool.Add(status.Guid, status);
			return status.ToString();
		}

		private static string UpdatePool(Msdfa.DB.Oracle.ActiveConnectionsChangedEventArgs e)
		{
			if (!e.IsClosed) return String.Empty;

			var status = CnnPool[e.Guid];
			status.End = DateTime.Now;
			CnnPool.Remove(e.Guid);
			return status.ToString();
		}
	}
}
