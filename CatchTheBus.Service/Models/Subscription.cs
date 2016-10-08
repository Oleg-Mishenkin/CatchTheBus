using System;
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

		protected bool Equals(Subscription other)
		{
			return RequestedHours == other.RequestedHours && RequestedMinutes == other.RequestedMinutes &&
				   NotifyTimeSpan == other.NotifyTimeSpan && Kind == other.Kind && Direction == other.Direction &&
				   string.Equals(Number, other.Number, StringComparison.InvariantCultureIgnoreCase) &&
				   string.Equals(StopName, other.StopName, StringComparison.InvariantCultureIgnoreCase);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((Subscription)obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = RequestedHours;
				hashCode = (hashCode * 397) ^ RequestedMinutes;
				hashCode = (hashCode * 397) ^ NotifyTimeSpan;
				hashCode = (hashCode * 397) ^ (int)Kind;
				hashCode = (hashCode * 397) ^ (int)Direction;
				hashCode = (hashCode * 397) ^ (Number != null ? StringComparer.InvariantCultureIgnoreCase.GetHashCode(Number) : 0);
				hashCode = (hashCode * 397) ^ (StopName != null ? StringComparer.InvariantCultureIgnoreCase.GetHashCode(StopName) : 0);
				return hashCode;
			}
		}
	}
}