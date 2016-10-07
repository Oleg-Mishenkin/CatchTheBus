using Newtonsoft.Json;

namespace CatchTheBus.Service.RocketChatModels
{
	public class MessageFromBot
	{
		[JsonProperty("icon_url")]
		public string IconUrl { get; set; }

		[JsonProperty("text")]
		public string Text { get; set; }

		[JsonProperty("channel")]
		public string To { get; set; }
	}
}
