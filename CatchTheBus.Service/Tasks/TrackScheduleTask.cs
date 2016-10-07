using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using AngleSharp;
using AngleSharp.Dom;
using CatchTheBus.Service.Constants;
using CatchTheBus.Service.Models;
using CatchTheBus.Service.Services;
using log4net;

namespace CatchTheBus.Service.Tasks
{
    public class TrackScheduleTask : NCron.CronJob
    {
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
	    private readonly TransportKind.Kind Kind;

		public TrackScheduleTask(TransportKind.Kind kind)
		{
			Kind = kind;
		}

		public override void Execute()
        {
            Logger.Info("Start executing TrackScheduleTask");
            try
            {
				var config = Configuration.Default.WithDefaultLoader();
                var address = UrlBuilder.GetUriByTransportKind(Kind);
                var document = BrowsingContext.New(config).OpenAsync(address.AbsoluteUri).Result;

				var stopwatch = new Stopwatch();
				stopwatch.Start();

                var items = ParseTransport(document, address, config);
	            TransportRepositoryService.Instance.SaveTransportItems(items, Kind);

				stopwatch.Stop();
				if (Logger.IsInfoEnabled)
					Logger.Info("Elapsed sec:" + stopwatch.Elapsed.TotalSeconds + " mill: " + stopwatch.ElapsedMilliseconds);
			}
            catch (Exception e)
            {
				Logger.Error(e);
			}
        }

	    private List<TransportListItem> ParseTransport(IDocument document, Uri address, IConfiguration config)
	    {
			var existingItems = TransportRepositoryService.Instance.GetTransportItems(Kind);

			var cells = document.QuerySelectorAll("a");
		    var listItems = cells.Select(m =>
		    {
				var index = m.TextContent.IndexOf(' ');
				var description = m.TextContent.Substring(index + 1).Trim();
				var number = m.TextContent.Substring(0, index).Trim();

				var olditem = existingItems.FirstOrDefault(x => x.Number == number);
				var item = new TransportListItem();
				item.Url = m.Attributes["href"].Value;
				item.Description = description;
				item.Number = number;
				item.ForwardDirection = olditem?.ForwardDirection ?? new Direction();
				item.BackwardDirection = olditem?.BackwardDirection ?? new Direction();
				item.ForwardDirection.BusStops = item.ForwardDirection.BusStops ?? new Dictionary<string, List<TimeEntry>>();
				item.BackwardDirection.BusStops = item.BackwardDirection.BusStops ?? new Dictionary<string, List<TimeEntry>>();
				// remove old time entries
				var btempItems = new Dictionary<string, List<TimeEntry>>(item.BackwardDirection.BusStops);
				foreach (var key in item.BackwardDirection.BusStops.Keys)
				{
					btempItems[key] = new List<TimeEntry>();
				}

				item.BackwardDirection.BusStops = btempItems;

				var ftempItems = new Dictionary<string, List<TimeEntry>>(item.ForwardDirection.BusStops);
				foreach (var key in item.ForwardDirection.BusStops.Keys)
				{
					ftempItems[key] = new List<TimeEntry>();
				}

				item.ForwardDirection.BusStops = ftempItems;

				return item;
			}).ToList();
			
			var resultItems = new List<TransportListItem>();
		    foreach (var item in listItems)
		    {
			    var itemAddr = new Uri(address, item.Url);
			    var itemDocument = BrowsingContext.New(config).OpenAsync(itemAddr.AbsoluteUri).Result;
			    bool? found = null;
			    foreach (var innerTr in itemDocument.QuerySelectorAll("table tr"))
			    {
				    if (innerTr.InnerHtml.Contains(DirectionDescription.ForwardDirection))
				    {
					    found = true;
					    item.ForwardDirection.Type = DirectionType.Forward;
					    var description = innerTr.QuerySelector("td h3").InnerHtml;
					    description = description.Substring(description.IndexOf("<br>", StringComparison.InvariantCulture) + 5);

					    item.ForwardDirection.Description = description.Remove(description.LastIndexOf(")"));
					    continue;
				    }

				    if (innerTr.InnerHtml.Contains(DirectionDescription.BackwardDirection))
				    {
					    found = false;
					    item.BackwardDirection.Type = DirectionType.Backward;
					    var description = innerTr.QuerySelector("td h3").InnerHtml;
					    description = description.Substring(description.IndexOf("<br>", StringComparison.InvariantCulture) + 5);

					    item.BackwardDirection.Description = description.Remove(description.LastIndexOf(")"));
					    continue;
				    }

				    if (found.HasValue && found.Value && !innerTr.InnerHtml.Contains("№"))
				    {
					    List<BusStop> busStops = ParseBusStop(innerTr, config, address);
					    MergeStops(busStops, item.ForwardDirection.BusStops);
					    continue;
				    }

				    if (found.HasValue && !found.Value && !innerTr.InnerHtml.Contains("№"))
				    {
					    List<BusStop> busStops = ParseBusStop(innerTr, config, address);
					    MergeStops(busStops, item.BackwardDirection.BusStops);
					    continue;
				    }
			    }

			    resultItems.Add(item);
		    }

			return resultItems;
	    }

	    private void MergeStops(List<BusStop> busStops, Dictionary<string, List<TimeEntry>> mergeToBusStops)
	    {
		    foreach (var busStop in busStops)
		    {
			    if (mergeToBusStops.ContainsKey(busStop.Name))
			    {
					mergeToBusStops[busStop.Name].Add(busStop.TimeEntry);
				}

				mergeToBusStops[busStop.Name] = new List<TimeEntry> { busStop.TimeEntry };
			}
	    }

		private List<BusStop> ParseBusStop(IElement innerHtml, IConfiguration config, Uri baseAddress)
		{
			var link = innerHtml.QuerySelector("a").Attributes["href"].Value;
			var itemAddr = new Uri(baseAddress, link);
			var document = BrowsingContext.New(config).OpenAsync(itemAddr.AbsoluteUri).Result;

			List<BusStop> busStops = new List<BusStop>();
		    foreach (var innerTr in document.QuerySelectorAll("table tr").SkipWhile(x => !x.InnerHtml.Contains("Прогноз")).Skip(1))
		    {
			    var tds = innerTr.QuerySelectorAll("td");
				busStops.Add(new BusStop
				{
					Name = tds[0].InnerHtml,
					TimeEntry = new TimeEntry { Hours = byte.Parse(tds[1].InnerHtml.Split(':')[0]), Minutes = byte.Parse(tds[1].InnerHtml.Split(':')[1]) }
				});
			}

		    return busStops;
	    }
    }
}
