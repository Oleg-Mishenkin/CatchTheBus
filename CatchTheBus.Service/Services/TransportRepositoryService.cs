using System.Collections.Concurrent;
using System.Collections.Generic;
using CatchTheBus.Service.Constants;
using CatchTheBus.Service.Models;

namespace CatchTheBus.Service.Services
{
	public sealed class TransportRepositoryService
	{
		private readonly ConcurrentDictionary<TransportKind.Kind, List<TransportListItem>> _savedItems = new ConcurrentDictionary<TransportKind.Kind, List<TransportListItem>>();

		public List<string> GetTransportKindNumbers(TransportKind.Kind kind)
		{
			return new List<string> { "44m", "42"};
		}

		private static readonly TransportRepositoryService instance = new TransportRepositoryService();

		private TransportRepositoryService() { }

		public static TransportRepositoryService Instance
		{
			get
			{
				return instance;
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