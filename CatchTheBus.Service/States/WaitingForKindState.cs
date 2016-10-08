using System.Linq;
using CatchTheBus.Service.Constants;
using CatchTheBus.Service.Helpers;
using CatchTheBus.Service.RocketChatModels;

namespace CatchTheBus.Service.States
{
	public class WaitingForKindState : AbstractState
	{
		public override ValidationResult Validate(string token, ParsedUserCommand command)
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

		public override AbstractState ParseToken(ParsedUserCommand command, string currentToken)
		{
			command.TransportKind = TransportKind.Parse(currentToken);
			return new WaitingForNumberState();
		}

		public override string GetMessageBefore(ParsedUserCommand command, string token)
		{
			return null;
		}

		public override string GetMessageAfter(ParsedUserCommand command, string token)
		{
			return $"Выбранный вид транспорта - {TransportKind.GetKindLocalizedName(command.TransportKind.Value)}";
		}
	}
}
