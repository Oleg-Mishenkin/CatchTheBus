using System.Linq;
using CatchTheBus.Service.Constants;
using CatchTheBus.Service.Helpers;
using CatchTheBus.Service.RocketChatModels;

namespace CatchTheBus.Service.TokenParseAlgorithms
{
	public class WaitingForKindState : IState
	{
		public ValidationResult Validate(string token, ParsedUserCommand command)
		{
			return TransportKind.All.Contains(token)
				? new ValidationResult { IsValid = true }
				: new ValidationResult
				{
					IsValid = false,
					ErrorMessage =
					$"Некорректный вид транспорта. Прочтите помощь по использованию бота:\n\n{HintHelper.Help}"
				};
		}

		public IState ParseToken(ParsedUserCommand command, string currentToken)
		{
			command.TransportKind = TransportKind.Parse(currentToken);
			return new WaitingForNumberState();
		}

		public string GetMessageBefore(ParsedUserCommand command, string token)
		{
			return null;
		}

		public string GetMessageAfter(ParsedUserCommand command, string token)
		{
			return $"Выбранный вид транспорта - {TransportKind.GetKindLocalizedName(command.TransportKind.Value)}";
		}
	}
}
