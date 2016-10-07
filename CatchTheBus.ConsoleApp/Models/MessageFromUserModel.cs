using Newtonsoft.Json;

namespace CatchTheBus.ConsoleApp.Models
{
	public class MessageFromUserModel
	{
		[JsonProperty("user_id")]
		public string UserId { get; set; }

		[JsonProperty("user_name")]
		public string UserName { get; set; }

		[JsonProperty("text")]
		public string Text { get; set; }
	}
}
