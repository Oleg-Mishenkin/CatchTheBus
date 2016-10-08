using CatchTheBus.Service.Constants;
using CatchTheBus.Service.RocketChatModels;
using CatchTheBus.Service.Services;

namespace CatchTheBus.Service.TokenParseAlgorithms
{
	public class WaitingForDirectionState : IState
	{
		public ValidationResult Validate(string token, ParsedUserCommand command)
		{
			var ok = token == "п" || token == "о" || token == "в";
			if (!ok) return new ValidationResult { IsValid = false, ErrorMessage = "Некорректное направление" };

			return new ValidationResult { IsValid = true };
		}

		public IState ParseToken(ParsedUserCommand command, string currentToken)
		{
			command.Direction = currentToken == "п" ? DirectionType.Forward : DirectionType.Backward;
			return new WaitingForStopNameState();
		}

		public string GetMessageBefore(ParsedUserCommand command, string token)
		{
			var directions = TransportRepositoryService.Instance.GetRouteDirections(command.TransportKind.Value, command.Number);
			var formattedDirections = "Выберите направление маршрута:\n\n";
			formattedDirections += $"*п* - {directions.Item1.Description ?? "Направление недоступно" }";
			formattedDirections += "\n";
			formattedDirections += $"*о* - {directions.Item2.Description ?? "Направление недоступно" }";

			return formattedDirections;
		}

		public string GetMessageAfter(ParsedUserCommand command, string token)
		{
			var formattedDirection = command.Direction == DirectionType.Forward ? "прямое" : "обратное";
			return $"Выбрано {formattedDirection} направление";
		}
	}
}
