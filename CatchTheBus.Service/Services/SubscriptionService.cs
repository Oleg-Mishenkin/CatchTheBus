using System.Collections.Concurrent;
using System.Collections.Generic;
using CatchTheBus.Service.Models;

namespace CatchTheBus.Service.Services
{
	public class SubscriptionService
	{
		private readonly ConcurrentDictionary<string, List<Subscription>> _subscriptions = new ConcurrentDictionary<string, List<Subscription>>();

		private SubscriptionService() { }

		private static readonly SubscriptionService instance = new SubscriptionService();

		public static SubscriptionService Instance
		{
			get
			{
				return instance;
			}
		}

		public List<Subscription> GetUserSubscriptions(string userName)
		{
			return _subscriptions.GetOrAdd(userName, new List<Subscription>());
		}

		public void SaveUserSubscription(string userName, Subscription item)
		{
			_subscriptions.AddOrUpdate(userName, new List<Subscription> { item }, (key, list) => { list.Add(item); return list;});
		}
	}
}