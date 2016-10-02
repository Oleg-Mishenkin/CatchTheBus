using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatchTheBus.Service.Services
{
    public static class UrlBuilder
    {
        public static Uri GetBaseUrl()
        {
            return new Uri(ConfigurationManager.AppSettings["TrackUrl"]);
        }

        public static Uri GetBusListUrl()
        {
            return new Uri(GetBaseUrl(), "list.php?vt=1&nl=0");
        }

        public static Uri GetTrolleyBusListUrl()
        {
            return new Uri(GetBaseUrl(), "list.php?vt=2&nl=0");
        }

        public static Uri GetTramListUrl()
        {
            return new Uri(GetBaseUrl(), "list.php?vt=3&nl=0");
        }

        public static Uri GetTaxiListUrl()
        {
            return new Uri(GetBaseUrl(), "list.php?vt=4&nl=0");
        }
    }
}
