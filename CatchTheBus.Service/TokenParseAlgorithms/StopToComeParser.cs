using System;
using System.Linq;
using CatchTheBus.Service.RocketChatModels;
using CatchTheBus.Service.Services;

namespace CatchTheBus.Service.TokenParseAlgorithms
{
	public class StopToComeParser : ITokenParser
	{
		public ValidationResult Validate(string str, ParsedUserCommand command)
		{
			var stops = TransportRepositoryService.Instance.GetStopNames(command.TransportKind.Value, command.Number, command.Direction.Value);

			if (!stops.Any(x => x.StartsWith(str, StringComparison.InvariantCultureIgnoreCase)))
			{
				return new ValidationResult { IsValid = false, ErrorMessage = "Такая остановка не найдена" };
			}

			return new ValidationResult { IsValid = true };
		}

		public string GetResult(ParsedUserCommand parsedCommand, string currentToken, bool isLast)
		{
			parsedCommand.StopToCome = currentToken;
			return isLast ? "Во сколько ты бы хотел сесть на транспорт (ЧЧ:ММ)?" : null;
		}
	}
}
