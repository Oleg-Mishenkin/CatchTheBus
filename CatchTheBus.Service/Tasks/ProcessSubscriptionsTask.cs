using System;
using System.Linq;
using System.Reflection;
using CatchTheBus.Service.Constants;
using CatchTheBus.Service.Models;
using CatchTheBus.Service.Services;
using log4net;

namespace CatchTheBus.Service.Tasks
{
	public class ProcessSubscriptionsTask : NCron.CronJob
	{
		private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		public override void Execute()
		{
			Logger.Info("Start executing " + nameof(ProcessSubscriptionsTask));

			var subscriptionService = SubscriptionService.Instance;
			var transportService = TransportRepositoryService.Instance;
			try
			{
				foreach (var userName in subscriptionService.GetAllUsers())
				{
					var subscriptions = subscriptionService.GetUserSubscriptions(userName);

					foreach (var subscription in subscriptions)
					{
						if (ProcessSubscription(transportService, userName, subscription))
							subscriptionService.RemoveSubscription(userName, subscription);
					}
				}
			}
			catch (Exception e)
			{
				Logger.Error(e);
			}
		}

		private bool ProcessSubscription(TransportRepositoryService transportService, string userName, Subscription subscription)
		{
			TimeSpan interval = new TimeSpan(0, subscription.NotifyTimeSpan, 0);
			TimeSpan targetTime = new TimeSpan(subscription.RequestedHours, subscription.RequestedMinutes, 0);

			if (new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, subscription.RequestedHours, subscription.RequestedMinutes, DateTime.Now.Second) - DateTime.Now > interval) return false;

			var timeEntries = transportService.GetTimeEntries(subscription.Kind, subscription.Direction, subscription.Number, subscription.StopName).Where(x => x.Hours > targetTime.Hours || (x.Hours == targetTime.Hours && x.Minutes == targetTime.Minutes)).OrderBy(x => x.Hours).ThenBy(x => x.Minutes);

			var text =
				$"{TransportKind.GetKindLocalizedName(subscription.Kind).FirstCharToUpper()} номер {subscription.Number} будет на остановке {subscription.StopName} в ";
			text += string.Join("\n", timeEntries.Select(x => $"{x.Hours}:{x.Minutes}").ToArray());

			OutgoingMessagesHelper.Get().SendMessage(text, userName);

			return true;
		}

	}

	public static class StringEx
	{
		public static string FirstCharToUpper(this string input)
		{
			if (string.IsNullOrEmpty(input))
				throw new ArgumentException("Fail");
			return input.First().ToString().ToUpper() + input.Substring(1);
		}
	}
}