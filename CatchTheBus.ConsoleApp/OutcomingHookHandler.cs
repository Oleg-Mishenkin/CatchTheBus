using System;
using CatchTheBus.ConsoleApp.Models;
using Nancy;
using Nancy.Extensions;
using Newtonsoft.Json;

namespace CatchTheBus.ConsoleApp
{
	public class OutcomingHookHandler : NancyModule
	{
		public OutcomingHookHandler()
		{
			Post["/outcomingHook"] = _ =>
			{
				var msg = JsonConvert.DeserializeObject<MessageFromUserModel>(Request.Body.AsString());

				Console.WriteLine(new MessageProcessor().GetResponse(msg.Text));

				return HttpStatusCode.OK;
			};
		}
	}
}
