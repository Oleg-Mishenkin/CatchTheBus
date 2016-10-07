using System;
using System.Configuration;
using System.Net;
using System.Reflection;
using System.Text;
using CatchTheBus.Service.RocketChatModels;
using log4net;
using Newtonsoft.Json;

namespace CatchTheBus.Service.Services
{
	public class OutgoingMessagesHelper
	{
		private static readonly ILog Log =
			LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		private OutgoingMessagesHelper() { }

		private static OutgoingMessagesHelper _instance;

		public static OutgoingMessagesHelper Get() => _instance ?? (_instance = new OutgoingMessagesHelper());

		public void SendMessage(string text, string destination)
		{
			using (var client = new WebClient())
			{
				var url = ConfigurationManager.AppSettings["IncomingWebhookUrl"];

				var messageModel = new MessageFromBot
				{
					To = destination,
					Text = text,
					IconUrl = ConfigurationManager.AppSettings["Icon"]
				};

				client.Headers["Content-Type"] = "application/json";
				client.Encoding = Encoding.UTF8;

				try
				{
					client.UploadStringAsync(new Uri(url), JsonConvert.SerializeObject(messageModel));
				}
				catch (Exception e)
				{
					Log.Error("Could not send a message to Rocket.Chat", e);
				}
			}
		}
	}
}
