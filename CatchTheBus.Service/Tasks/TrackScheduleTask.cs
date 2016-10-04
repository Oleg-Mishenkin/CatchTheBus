using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using AngleSharp;
using CatchTheBus.Service.Constants;
using CatchTheBus.Service.Models;
using CatchTheBus.Service.Services;
using HtmlAgilityPack;
using log4net;

namespace CatchTheBus.Service.Tasks
{
    public class TrackScheduleTask : NCron.CronJob
    {
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public override void Execute()
        {
            Logger.Info("Start executing TrackScheduleTask");

            /*HtmlWeb hw = new HtmlWeb();
            hw.AutoDetectEncoding = false;
            hw.OverrideEncoding = Encoding.GetEncoding("windows-1251");
            try
            {
                HtmlDocument doc = hw.Load(UrlBuilder.GetTaxiListUrl().AbsoluteUri);
                var taxes = ExtractAllAHrefTags(hw, doc);
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }*/

            try
            {
                var config = Configuration.Default.WithDefaultLoader();
                var address = UrlBuilder.GetTaxiListUrl();
                var document = BrowsingContext.New(config).OpenAsync(address.AbsoluteUri).Result;
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

                foreach (var item in listItems)
                {
                    var itemAddr = new Uri(address, item.Url);
                    var itemDocument = BrowsingContext.New(config).OpenAsync(itemAddr.AbsoluteUri).Result;
                    var itemTrSelector = "tr";

                    foreach (var innerTr in document.QuerySelectorAll("table > tr"))
                    {
                       // if (innerTr.InnerHtml.Contains(DirectionDescription.ForwardDirection))
                    }

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private List<TransportListItem> ExtractAllAHrefTags(HtmlWeb hw, HtmlDocument htmlSnippet)
        {
            var baseUrl = UrlBuilder.GetBaseUrl();
            var listItems = new List<TransportListItem>();

            foreach (HtmlNode link in htmlSnippet.DocumentNode.SelectNodes("//a[@href]"))
            {
                var description = link.InnerText.Split(new[] {"&nbsp;"}, StringSplitOptions.RemoveEmptyEntries);
                var item = new TransportListItem();
                HtmlAttribute att = link.Attributes["href"];
                item.Url = att.Value;
                item.Number = description[0].Trim();
                item.Description = description[1].Trim();
            }

            foreach (var item in listItems)
            {
                HtmlDocument doc = hw.Load(new Uri(baseUrl, item.Url).AbsoluteUri);

                foreach (HtmlNode link in htmlSnippet.DocumentNode.SelectNodes("//tr"))
                {
                }
            }

            return listItems;
        }
    }
}
