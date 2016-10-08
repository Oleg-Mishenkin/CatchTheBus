using System.Linq;
using CatchTheBus.Service.Constants;
using CatchTheBus.Service.RocketChatModels;
using CatchTheBus.Service.Services;

namespace CatchTheBus.Service.TokenParseAlgorithms
{
	class WaitingForNumberState : IState
	{
		public ValidationResult Validate(string token, ParsedUserCommand command)
		{
			if (!TransportRepositoryService.Instance.GetTransportKindNumbers(command.TransportKind.Value).Any(x => x.Item1 == token))
			{
				return new ValidationResult { IsValid = false, ErrorMessage = "Нет такого номера" };
			}

			return new ValidationResult { IsValid = true };
		}

		public IState ParseToken(ParsedUserCommand command, string currentToken)
		{
			command.Number = currentToken;
			return new WaitingForDirectionState();
		}

		public string GetMessageBefore(ParsedUserCommand command, string token)
		{
			var numbers = TransportRepositoryService.Instance.GetTransportKindNumbers(command.TransportKind.Value);
			var formattedNumbers = string.Join("\n", numbers.Select((item, i) => $"*{item.Item1}* - {item.Item2}"));

			return formattedNumbers + "\n\n" +
			       $"Выберите {TransportKind.GetKindLocalizedName(command.TransportKind.Value)}";
		}

		public string GetMessageAfter(ParsedUserCommand command, string token)
		{
			return $"Выбран номер {command.Number}";
		}
	}
}
