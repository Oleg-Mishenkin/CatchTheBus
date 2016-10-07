using System.Collections.Concurrent;
using System.Collections.Generic;
using CatchTheBus.Service.Constants;

namespace CatchTheBus.Service.Services
{
	public class TransportRepositoryService
	{
		public List<string> GetTransportKindNumbers(TransportKind.Kind kind)
		{
			return new List<string> { "44m", "42"};
		}

		public Dictionary<DirectionType, string> GetDirectionList(TransportKind.Kind kind, string number)
		{
			return new Dictionary<DirectionType, string>
			{
				{ DirectionType.Backward,  "Обратное" },
				{ DirectionType.Forward,  "Прямое" }
			};
		}
	}
}