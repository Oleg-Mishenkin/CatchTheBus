using CatchTheBus.Service.RocketChatModels;

namespace CatchTheBus.Service.TokenParseAlgorithms
{
	public interface ITokenParseAlgorithm
	{
		ValidationResult Validate(string str, ParsedUserCommand command);

		string GetResult(ParsedUserCommand parsedCommand, string currentToken, bool isLast);
	}
}
