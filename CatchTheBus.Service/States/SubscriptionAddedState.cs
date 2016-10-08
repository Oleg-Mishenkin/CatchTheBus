using CatchTheBus.Service.RocketChatModels;

namespace CatchTheBus.Service.States
{
	public class SubscriptionAddedState : AbstractState
	{
		public override ValidationResult Validate(string token, ParsedUserCommand command)
		{
			return new ValidationResult { IsValid = true };
		}

		public override AbstractState ParseToken(ParsedUserCommand command, string currentToken)
		{
			return new SubscriptionAddedState();
		}

		public override string GetMessageBefore(ParsedUserCommand command, string token) => null;

		public override string GetMessageAfter(ParsedUserCommand command, string token) => null;
	}
}
