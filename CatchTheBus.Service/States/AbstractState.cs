using CatchTheBus.Service.RocketChatModels;

namespace CatchTheBus.Service.States
{
	public abstract class AbstractState
	{
		public abstract ValidationResult Validate(string token, ParsedUserCommand command);

		public abstract AbstractState ParseToken(ParsedUserCommand command, string currentToken);

		public abstract string GetMessageBefore(ParsedUserCommand command, string token);

		public abstract string GetMessageAfter(ParsedUserCommand command, string token);

		public virtual ValidationResult CanExecute(string token, ParsedUserCommand command)
		{
			return new ValidationResult { IsValid = true };
		}
	}
}
