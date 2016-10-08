using CatchTheBus.Service.Constants;
using CatchTheBus.Service.Models;
using CatchTheBus.Service.RocketChatModels;
using CatchTheBus.Service.Services;

namespace CatchTheBus.Service.TokenParseAlgorithms
{
	public class NotifyTimeParser : ITokenParser
	{
		public ValidationResult Validate(string str, ParsedUserCommand command)
		{
			int minutes;
			if (!int.TryParse(str, out minutes))
			{
				return new ValidationResult { IsValid = false, ErrorMessage = "Введено некорректное число, повторите" };
			}

			if (minutes < 0)
			{
				return new ValidationResult { IsValid = false, ErrorMessage = "Нельзя вводить отрицательное число минут" };
			}

			if (minutes > 60)
			{
				return new ValidationResult { IsValid = false, ErrorMessage = "Введите целое число от 1 до 60" };
			}

			return new ValidationResult { IsValid = true };
		}

		public string GetResult(ParsedUserCommand parsedCommand, string currentToken, bool isLast)
		{
			parsedCommand.NotifyTimeMinutes = int.Parse(currentToken);

			// ReSharper disable PossibleInvalidOperationException
			SubscriptionService.Instance.SaveUserSubscription(parsedCommand.UserName, new Subscription
			{
				Number = parsedCommand.Number,
				Direction = parsedCommand.Direction.Value,
				Kind = parsedCommand.TransportKind.Value,
				RequestedHours = parsedCommand.DesiredTime.Value.Hour,
				RequestedMinutes = parsedCommand.DesiredTime.Value.Minute,
				StopName = parsedCommand.StopToCome,
				NotifyTimeSpan = parsedCommand.NotifyTimeMinutes.Value
			});
			// ReSharper restore PossibleInvalidOperationException

			return isLast ? $"Хорошо. Я сообщу о том, что {TransportKind.GetKindLocalizedName(parsedCommand.TransportKind.Value)} " +
			                $"номер *{parsedCommand.Number}* будет на остановке *{parsedCommand.StopToCome}* за *{parsedCommand.NotifyTimeMinutes}* минут" : null;
		}
	}
}
