using System;
using System.Configuration;
using System.Reflection;
using CatchTheBus.Service.Constants;

namespace CatchTheBus.Service.Tasks.OT76
{
    public static class OT76UrlBuilder
    {
        public static Uri GetBaseUrl()
        {
            return new Uri(ConfigurationManager.AppSettings["OT76TrackUrl"]);
        }

        public static Uri GetBusListUrl()
        {
            return new Uri(GetBaseUrl(), "list.php?vt=1&nl=0");
        }

        public static Uri GetTrolleybusListUrl()
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

	    public static Uri GetUriByTransportKind(TransportKind.Kind kind)
	    {
			Type type = typeof(OT76UrlBuilder);
			MethodInfo method = type.GetMethod("Get" + kind + "ListUrl");
		    return method.Invoke(null, null) as Uri;
	    }
    }
}
