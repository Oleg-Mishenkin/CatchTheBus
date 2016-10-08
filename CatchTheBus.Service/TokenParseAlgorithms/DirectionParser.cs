using CatchTheBus.Service.Constants;
using CatchTheBus.Service.RocketChatModels;
using CatchTheBus.Service.Services;

namespace CatchTheBus.Service.TokenParseAlgorithms
{
	public class DirectionParser : ITokenParser
	{
		public ValidationResult Validate(string str, ParsedUserCommand command)
		{
			var ok = str == "п" || str == "о" || str == "в";
			if (!ok) return new ValidationResult { IsValid = false, ErrorMessage = "Некорректное направление" };

			return new ValidationResult { IsValid = true };
		}

		public string GetResult(ParsedUserCommand parsedCommand, string currentToken, bool isLast)
		{
			parsedCommand.Direction = currentToken == "п" ? DirectionType.Forward : DirectionType.Backward;
			string formattedStops = "Выберите остановку (введите начало названия в любом регистре)\n\n";

			var stops = TransportRepositoryService.Instance.GetStopNames(parsedCommand.TransportKind.Value,
				parsedCommand.Number, parsedCommand.Direction.Value);

			formattedStops += string.Join("\n", stops);

			return isLast ? formattedStops + "\n\n" + "На какую остановку подойдешь?" : null;
		}
	}
}
