using CatchTheBus.Service.Constants;

namespace CatchTheBus.Service.Models
{
	public class Subscription
	{
		public int RequestedHours { get; set; }

		public int RequestedMinutes { get; set; }

		public int NotifyTimeSpan { get; set; }

		public TransportKind.Kind Kind { get; set; }

		public DirectionType Direction { get; set; }

		public string Number { get; set; }

		public string StopName { get; set; }
	}
}