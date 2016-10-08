using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AngleSharp;
using AngleSharp.Dom;
using CatchTheBus.Service.Constants;
using CatchTheBus.Service.Models;
using CatchTheBus.Service.Services;
using CatchTheBus.Service.Tasks.OT76;
using log4net;

namespace CatchTheBus.Service.Tasks.Yargortrans
{
	public class YargortransStopsFillTask : NCron.CronJob
	{
		private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		private readonly TransportKind.Kind Kind;

		public YargortransStopsFillTask(TransportKind.Kind kind)
		{
			Kind = kind;
		}

		public override void Execute()
		{
			Logger.Info("Start executing " + nameof(YargortransStopsFillTask));
			try
			{
				var config = Configuration.Default.WithDefaultLoader();
				var address = YargortransUrlBuilder.GetUriByTransportKind(Kind);
				var document = BrowsingContext.New(config).OpenAsync(address.AbsoluteUri).Result;

				var items = ParseTransport(document, address, config);
				TransportRepositoryService.Instance.SaveTransportItems(items, Kind);
			}
			catch (Exception e)
			{
				Logger.Error(e);
			}
		}

		private List<TransportListItem> ParseTransport(IDocument document, Uri address, IConfiguration config)
		{
			var cells = document.QuerySelectorAll("table.info > tbody > tr > td > table > tbody > tr ");

			var listItems = cells.Skip(1).Select(m =>
			{
				var refs = m.QuerySelectorAll("a");
				var number = refs[0].InnerHtml.Trim();
				var link = refs[0].Attributes["href"].Value.Trim();
				var description = refs[1].InnerHtml.Trim();

				return new TransportListItem
				{
					Number = number,
					Description = description,
					Url = link,
					ForwardDirection = new Direction { Type = DirectionType.Forward, BusStops = new Dictionary<string, List<TimeEntry>>() },
					BackwardDirection = new Direction { Type = DirectionType.Backward, BusStops = new Dictionary<string, List<TimeEntry>>() }
				};
			}).ToList();

			var resultItems = new List<TransportListItem>();
			foreach (var item in listItems)
			{
				var itemAddr = new Uri(address, item.Url);
				var itemDocument = BrowsingContext.New(config).OpenAsync(itemAddr.AbsoluteUri).Result;
				string prevStopName = "";
				bool forwardDirection = true;
				foreach (var element in itemDocument.QuerySelectorAll("table.info > tbody > tr > td > center > a"))
				{
					if (prevStopName.Equals(element.InnerHtml.Trim(), StringComparison.InvariantCultureIgnoreCase))
						forwardDirection = false;
					if (forwardDirection)
					{
						item.ForwardDirection.BusStops[element.InnerHtml.Trim()] = new List<TimeEntry>();
					}

					if (!forwardDirection)
					{
						item.BackwardDirection.BusStops[element.InnerHtml.Trim()] = new List<TimeEntry>();
					}

					prevStopName = element.InnerHtml.Trim();
				}

				resultItems.Add(item);
			}

			return resultItems;
		}
	}
}