using CatchTheBus.Service.RocketChatModels;

namespace CatchTheBus.Service.TokenParseAlgorithms
{
	public interface IState
	{
		ValidationResult Validate(string token, ParsedUserCommand command);

		IState ParseToken(ParsedUserCommand command, string currentToken);

		string GetMessageBefore(ParsedUserCommand command, string token);

		string GetMessageAfter(ParsedUserCommand command, string token);
	}
}
