using CatchTheBus.Service.Constants;
using CatchTheBus.Service.RocketChatModels;
using CatchTheBus.Service.Services;

namespace CatchTheBus.Service.States
{
	public class WaitingForDirectionState : AbstractState
	{
		public override ValidationResult CanExecute(string token, ParsedUserCommand command)
		{
			var directions = TransportRepositoryService.Instance.GetRouteDirections(command.TransportKind.Value, command.Number);
			if (directions.Item1.Description == null || directions.Item2.Description == null)
				return new ValidationResult
				{
					IsValid = false,
					ErrorMessage = "На данный момент по Вашему запросу нет доступного транспорта"
				};

			return base.CanExecute(token, command);
		}

		public override ValidationResult Validate(string token, ParsedUserCommand command)
		{
			var ok = token == "п" || token == "о" || token == "в";
			if (!ok) return new ValidationResult { IsValid = false, ErrorMessage = "Некорректное направление" };

			return new ValidationResult { IsValid = true };
		}

		public override AbstractState ParseToken(ParsedUserCommand command, string currentToken)
		{
			command.Direction = currentToken == "п" ? DirectionType.Forward : DirectionType.Backward;
			return new WaitingForStopNameState();
		}

		public override string GetMessageBefore(ParsedUserCommand command, string token)
		{
			var directions = TransportRepositoryService.Instance.GetRouteDirections(command.TransportKind.Value, command.Number);
			var formattedDirections = "Выберите направление маршрута:\n\n";
			formattedDirections += $"*п* - {directions.Item1.Description ?? "Направление недоступно" }";
			formattedDirections += "\n";
			formattedDirections += $"*о* - {directions.Item2.Description ?? "Направление недоступно" }";

			return formattedDirections;
		}

		public override string GetMessageAfter(ParsedUserCommand command, string token)
		{
			var formattedDirection = command.Direction == DirectionType.Forward ? "прямое" : "обратное";
			return $"Выбрано {formattedDirection} направление";
		}
	}
}
