using CatchTheBus.ConsoleApp.Constants;

namespace CatchTheBus.ConsoleApp.Models
{
	public class ParsedUserCommand
	{
		public TransportKind.Kind TransportKind { get; set; }

		public string Number { get; set; }

		public string Direction { get; set; }

		public int NotifyTimeMinutes { get; set; }
	}
}
