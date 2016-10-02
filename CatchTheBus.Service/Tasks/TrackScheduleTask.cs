using System.Collections.Generic;
using System.Reflection;
using System.Text;
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

            HtmlWeb hw = new HtmlWeb();
            hw.AutoDetectEncoding = false;
            hw.OverrideEncoding = Encoding.GetEncoding("windows-1251");

            HtmlDocument doc = hw.Load(UrlBuilder.GetTaxiListUrl().AbsoluteUri);
            ExtractAllAHrefTags(doc);
        }

        private List<TransportListItem> ExtractAllAHrefTags(HtmlDocument htmlSnippet)
        {
            var hrefTags = new List<TransportListItem>();

            foreach (HtmlNode link in htmlSnippet.DocumentNode.SelectNodes("//a[@href]"))
            {
                var item = new TransportListItem();
                HtmlAttribute att = link.Attributes["href"];
                item.Url = att.Value;
                item.Number = link.InnerText.Split(' ')[0];
                item.Description = link.InnerText.Split(' ')[1];
            }

            return hrefTags;
        }
    }
}
