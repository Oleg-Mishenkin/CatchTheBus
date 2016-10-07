using System;

namespace CatchTheBus.Service.Constants
{
	public static class TransportKind
	{
		public static string Bus = "а";

		public static string Trolleybus = "тб";

		public static string Tram = "тв";

		public static string Taxy = "м";

		public static string[] All =
		{
			Bus,
			Trolleybus,
			Tram,
			Taxy
		};

		public enum Kind
		{
			Bus,
			Trolleybus,
			Tram,
			Taxy
		}

		public static Kind Parse(string str)
		{
			switch (str)
			{
				case "а": return Kind.Bus;
				case "тб": return Kind.Trolleybus;
				case "тв": return Kind.Tram;
				case "м": return Kind.Taxy;
			}

			throw new InvalidOperationException("Could not find a transport kind with this alias");
		}
	}
}
