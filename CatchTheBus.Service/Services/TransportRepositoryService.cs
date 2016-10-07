using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using CatchTheBus.Service.Constants;
using CatchTheBus.Service.Models;

namespace CatchTheBus.Service.Services
{
	public sealed class TransportRepositoryService
	{
		private readonly ConcurrentDictionary<TransportKind.Kind, List<TransportListItem>> _savedItems = new ConcurrentDictionary<TransportKind.Kind, List<TransportListItem>>();

		private TransportRepositoryService() { }

		private static readonly TransportRepositoryService instance = new TransportRepositoryService();

		public static TransportRepositoryService Instance
		{
			get
			{
				return instance;
			}
		}

		public List<Tuple<string, string>> GetTransportKindNumbers(TransportKind.Kind kind)
		{
			return GetTransportItems(kind).Select(x => new Tuple<string, string>(x.Number, x.Description)).ToList();
		}

		public Direction[] GetDirections(TransportKind.Kind kind, string number)
		{
			var item = GetTransportItems(kind).FirstOrDefault(x => x.Number == number);
			if (item == null) return null;

			return new[] { item.ForwardDirection, item.BackwardDirection };
		}

		public List<string> GetStopNames(TransportKind.Kind kind, DirectionType direction, string number)
		{
			var item = GetTransportItems(kind).FirstOrDefault(x => x.Number == number);
			if (item == null) return null;

			if (direction == DirectionType.Backward)
				return item.BackwardDirection.BusStops.Select(x => x.Key).ToList();
			return item.ForwardDirection.BusStops.Select(x => x.Key).ToList();
		}

		public List<TimeEntry> GetTimeEntries(TransportKind.Kind kind, DirectionType direction, string number, string busStop)
		{
			var item = GetTransportItems(kind).FirstOrDefault(x => x.Number == number);
			if (item == null) return null;

			if (direction == DirectionType.Backward)
			{
				var stop = item.BackwardDirection.BusStops.FirstOrDefault(x => x.Key.ToLower().StartsWith(busStop.ToLower()));
				if (stop.Equals(default(KeyValuePair<string, List<TimeEntry>>))) return null;
				return stop.Value;
			}
			else
			{
				var stop = item.ForwardDirection.BusStops.FirstOrDefault(x => x.Key.ToLower().StartsWith(busStop.ToLower()));
				if (stop.Equals(default(KeyValuePair<string, List<TimeEntry>>))) return null;
				return stop.Value;
			}
		}
		
		public Dictionary<DirectionType, string> GetDirectionList(TransportKind.Kind kind, string number)
		{
			return new Dictionary<DirectionType, string>
			{
				{ DirectionType.Backward,  "Обратное" },
				{ DirectionType.Forward,  "Прямое" }
			};
		}

		public List<TransportListItem> GetTransportItems(TransportKind.Kind kind)
		{
			return _savedItems.GetOrAdd(kind, new List<TransportListItem>());
		}

		public void SaveTransportItems(List<TransportListItem> items, TransportKind.Kind kind)
		{
			_savedItems.AddOrUpdate(kind, items, (key, list) => items);
		}
	}
}