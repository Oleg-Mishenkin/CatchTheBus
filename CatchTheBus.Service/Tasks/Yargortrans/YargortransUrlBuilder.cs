using System;
using System.Configuration;
using System.Reflection;
using CatchTheBus.Service.Constants;

namespace CatchTheBus.Service.Tasks.Yargortrans
{
	public static class YargortransUrlBuilder
	{
		public static Uri GetBaseUrl()
		{
			return new Uri(ConfigurationManager.AppSettings["YargortransTrackUrl"]);
		}

		public static Uri GetBusListUrl()
		{
			return new Uri(GetBaseUrl(), "list.php?vt=1");
		}

		public static Uri GetTrolleybusListUrl()
		{
			return new Uri(GetBaseUrl(), "list.php?vt=2");
		}

		public static Uri GetTramListUrl()
		{
			return new Uri(GetBaseUrl(), "list.php?vt=3");
		}

		public static Uri GetTaxiListUrl()
		{
			return new Uri(GetBaseUrl(), "list.php?vt=4");
		}

		public static Uri GetUriByTransportKind(TransportKind.Kind kind)
		{
			Type type = typeof(YargortransUrlBuilder);
			MethodInfo method = type.GetMethod("Get" + kind + "ListUrl");
			return method.Invoke(null, null) as Uri;
		}
	}
}