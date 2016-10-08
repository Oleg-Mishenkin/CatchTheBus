using System;
using CatchTheBus.Service.Constants;
using CatchTheBus.Service.Models;
using CatchTheBus.Service.Services;
using CatchTheBus.Service.Tasks;
using Nancy.Hosting.Self;

namespace CatchTheBus.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
			new TrackScheduleTask(TransportKind.Kind.Tram).Execute();
			SubscriptionService.Instance.SaveUserSubscription("@busbot", new Subscription
			{
				Direction = DirectionType.Forward,
				Kind = TransportKind.Kind.Tram,
				Number = "5с",
				RequestedHours = 11,
				RequestedMinutes = 40,
				StopName = "ЯМЗ",
				NotifyTimeSpan = 30
			});

			new ProcessSubscriptionsTask().Execute();
			/*using (var client = new WebClient())
			{
				var url = ConfigurationManager.AppSettings["IncomingWebhookUrl"];

				var asdf = new MessageFromBot
				{
					To = "@busbot",
					Text = "YO!!",
					IconUrl = ConfigurationManager.AppSettings["Icon"]
				};

				client.Headers["Content-Type"] = "application/json";
				client.UploadStringAsync(new Uri(url), JsonConvert.SerializeObject(asdf));
			}*/

			using (var host = new NancyHost(new Uri("http://localhost:8080")))
			{
				UrlBuilder.GetBaseUrl(); // костылик, чтобы ассембли с сервисом загрузилась в аппдомен

				host.Start();
				Console.WriteLine("Running on http://localhost:8080");
				Console.ReadLine();
			}
		}
    }
}
