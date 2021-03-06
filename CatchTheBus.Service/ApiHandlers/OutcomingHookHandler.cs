﻿using System;
using CatchTheBus.Service.RocketChatModels;
using CatchTheBus.Service.Services;
using Nancy;
using Nancy.Extensions;
using Newtonsoft.Json;

namespace CatchTheBus.Service.ApiHandlers
{
	public class OutcomingHookHandler : NancyModule
	{
		public OutcomingHookHandler()
		{
			Post["/outcomingHook"] = _ =>
			{
				var msg = JsonConvert.DeserializeObject<MessageFromUserModel>(Request.Body.AsString());

				new MessageProcessor().Handle(msg.Text, msg.UserId, msg.UserName);
				return HttpStatusCode.OK;
			};
		}
	}
}
