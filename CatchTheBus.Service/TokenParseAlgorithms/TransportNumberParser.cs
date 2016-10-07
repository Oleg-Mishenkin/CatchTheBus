using CatchTheBus.Service.RocketChatModels;

namespace CatchTheBus.Service.TokenParseAlgorithms
{
	public class TransportNumberParser : ITokenParseAlgorithm
	{
		// TODO: оно нам надо?
		public ValidationResult Validate(string str) => new ValidationResult { IsValid = true };

		public string GetResult(ParsedUserCommand parsedCommand, string currentToken, bool isLast)
		{
			parsedCommand.Number = currentToken;
			return isLast ? "Список направлений!!!" : null;
		}
	}
}
