using System.Linq;
using CatchTheBus.Service.RocketChatModels;
using CatchTheBus.Service.Services;

namespace CatchTheBus.Service.TokenParseAlgorithms
{
	public class TransportNumberParser : ITokenParser
	{
		public ValidationResult Validate(string str, ParsedUserCommand command)
		{
			if (!TransportRepositoryService.Instance.GetTransportKindNumbers(command.TransportKind.Value).Any(x => x.Item1 == str))
			{
				return new ValidationResult { IsValid = false, ErrorMessage = "Нет такого номера" };
			}

			return new ValidationResult { IsValid = true };
		}

		public string GetResult(ParsedUserCommand parsedCommand, string currentToken, bool isLast)
		{
			parsedCommand.Number = currentToken;

			var directions = TransportRepositoryService.Instance.GetDirectionList(parsedCommand.TransportKind.Value, parsedCommand.Number);

			return isLast ? "Список направлений!!!" : null;
		}
	}
}
