using System;
using System.Configuration;
using System.Net;
using CatchTheBus.Service.RocketChatModels;
using CatchTheBus.Service.Services;
using Nancy.Hosting.Self;
using Newtonsoft.Json;

namespace CatchTheBus.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
			// new TrackScheduleTask().Execute();

			using (var client = new WebClient())
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
			}

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
