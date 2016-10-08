using System;
using System.Linq;
using CatchTheBus.Service.RocketChatModels;
using CatchTheBus.Service.Services;

namespace CatchTheBus.Service.States
{
	public class WaitingForStopNameState : AbstractState
	{
		public override ValidationResult Validate(string token, ParsedUserCommand command)
		{
			var stops = TransportRepositoryService.Instance.GetStopNames(command.TransportKind.Value, command.Number, command.Direction.Value);

			if (!stops.Any(x => x.StartsWith(token, StringComparison.InvariantCultureIgnoreCase)))
			{
				return new ValidationResult { IsValid = false, ErrorMessage = "Такая остановка не найдена" };
			}

			return new ValidationResult { IsValid = true };
		}

		public override AbstractState ParseToken(ParsedUserCommand command, string currentToken)
		{
			var stops = TransportRepositoryService.Instance.GetStopNames(command.TransportKind.Value, command.Number, command.Direction.Value);
			command.StopToCome = stops.First(x => x.StartsWith(currentToken, StringComparison.InvariantCultureIgnoreCase));
			return new WaitingForDesiredTimeState();
		}

		public override string GetMessageBefore(ParsedUserCommand command, string token)
		{
			string formattedStops = "Выберите остановку (введите начало названия в любом регистре)\n\n";

			var stops = TransportRepositoryService.Instance.GetStopNames(command.TransportKind.Value,
				command.Number, command.Direction.Value);

			formattedStops += string.Join("\n", stops);
			return formattedStops + "\n\n" + "На какую остановку подойдешь?";
		}

		public override string GetMessageAfter(ParsedUserCommand command, string token)
		{
			return $"Выбранная остановка - {command.StopToCome}";
		}
	}
}
