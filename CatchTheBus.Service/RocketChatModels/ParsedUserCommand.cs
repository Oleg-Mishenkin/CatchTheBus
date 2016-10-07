using CatchTheBus.Service.Constants;

namespace CatchTheBus.Service.RocketChatModels
{
	public class ParsedUserCommand
	{
		public TransportKind.Kind TransportKind { get; set; }

		public string Number { get; set; }

		public DirectionType Direction { get; set; }

		public int NotifyTimeMinutes { get; set; }
	}
}
