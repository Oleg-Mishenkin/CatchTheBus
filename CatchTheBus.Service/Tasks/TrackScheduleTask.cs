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
        public override void Execute()
        {
            Logger.Info("Start executing TrackScheduleTask");
            try
            {
                var config = Configuration.Default.WithDefaultLoader();
                var addressBus = UrlBuilder.GetBusListUrl();
                var addressTram = UrlBuilder.GetTramListUrl();
                var addressTrolley = UrlBuilder.GetTrolleyBusListUrl();
                var addressTaxi = UrlBuilder.GetTaxiListUrl();
                var documentBus = BrowsingContext.New(config).OpenAsync(addressBus.AbsoluteUri).Result;
                var documentTram = BrowsingContext.New(config).OpenAsync(addressTram.AbsoluteUri).Result;
                var documentTrolley = BrowsingContext.New(config).OpenAsync(addressTrolley.AbsoluteUri).Result;
                var documentTaxi = BrowsingContext.New(config).OpenAsync(addressTaxi.AbsoluteUri).Result;

				var stopwatch = new Stopwatch();
				stopwatch.Start();
                var busItems = ParseTransport(documentBus, addressBus, config);
                var tramItems = ParseTransport(documentTram, addressTram, config);
                var trolleyItems = ParseTransport(documentTrolley, addressTrolley, config);
                var taxiItems = ParseTransport(documentTaxi, addressTaxi, config);
				stopwatch.Stop();
				Console.WriteLine("Elapsed sec:" + stopwatch.Elapsed.TotalSeconds + " mill: " + stopwatch.ElapsedMilliseconds);
			}
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

	    private List<TransportListItem> ParseTransport(IDocument document, Uri address, IConfiguration config)
	    {
		    var cells = document.QuerySelectorAll("a");
		    var listItems = cells.Select(m =>
		    {
				var index = m.TextContent.IndexOf(' ');
				return new TransportListItem
				{
					Url = m.Attributes["href"].Value,
					Description = m.TextContent.Substring(index + 1).Trim(),
					Number = m.TextContent.Substring(0, index).Trim()
				};
		    });

		    var resultItems = new List<TransportListItem>();
		    foreach (var item in listItems)
		    {
			    var itemAddr = new Uri(address, item.Url);
			    var itemDocument = BrowsingContext.New(config).OpenAsync(itemAddr.AbsoluteUri).Result;
			    var itemTrSelector = "tr";
			    bool? found = null;
			    foreach (var innerTr in itemDocument.QuerySelectorAll("table tr"))
			    {
				    if (innerTr.InnerHtml.Contains(DirectionDescription.ForwardDirection))
				    {
					    found = true;
					    item.ForwardDirection = new Direction();
					    item.ForwardDirection.Type = DirectionType.Forward;
					    var description = innerTr.QuerySelector("td h3").InnerHtml;
					    description = description.Substring(description.IndexOf("<br>", StringComparison.InvariantCulture) + 5);

					    item.ForwardDirection.Description = description.Remove(description.LastIndexOf(")"));
					    item.ForwardDirection.BusStops = new Dictionary<string, List<TimeEntry>>();
					    continue;
				    }

				    if (innerTr.InnerHtml.Contains(DirectionDescription.BackwardDirection))
				    {
					    found = false;
					    item.BackwardDirection = new Direction();
					    item.BackwardDirection.Type = DirectionType.Backward;
					    var description = innerTr.QuerySelector("td h3").InnerHtml;
					    description = description.Substring(description.IndexOf("<br>", StringComparison.InvariantCulture) + 5);

					    item.BackwardDirection.Description = description.Remove(description.LastIndexOf(")"));
					    item.BackwardDirection.BusStops = new Dictionary<string, List<TimeEntry>>();
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
