using CatchTheBus.Service.RocketChatModels;

namespace CatchTheBus.Service.TokenParseAlgorithms
{
	public class SubscriptionAddedState : IState
	{
		public ValidationResult Validate(string token, ParsedUserCommand command)
		{
			return new ValidationResult { IsValid = true };
		}

		public IState ParseToken(ParsedUserCommand command, string currentToken)
		{
			return new SubscriptionAddedState();
		}

		public string GetMessageBefore(ParsedUserCommand command, string token) => null;

		public string GetMessageAfter(ParsedUserCommand command, string token) => null;
	}
}
