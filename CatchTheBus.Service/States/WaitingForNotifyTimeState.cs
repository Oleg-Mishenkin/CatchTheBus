using System.Linq;
using CatchTheBus.Service.Constants;
using CatchTheBus.Service.Models;
using CatchTheBus.Service.RocketChatModels;
using CatchTheBus.Service.Services;

namespace CatchTheBus.Service.States
{
	public class WaitingForNotifyTimeState : AbstractState
	{
		public override ValidationResult Validate(string token, ParsedUserCommand command)
		{
			int minutes;
			if (!int.TryParse(token, out minutes))
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

		public override AbstractState ParseToken(ParsedUserCommand command, string currentToken)
		{
			command.NotifyTimeMinutes = int.Parse(currentToken);

			// ReSharper disable PossibleInvalidOperationException
			SubscriptionService.Instance.SaveUserSubscription(command.UserName, new Subscription
			{
				Number = command.Number,
				Direction = command.Direction.Value,
				Kind = command.TransportKind.Value,
				RequestedHours = command.DesiredTime.Value.Hour,
				RequestedMinutes = command.DesiredTime.Value.Minute,
				StopName = command.StopToCome,
				NotifyTimeSpan = command.NotifyTimeMinutes.Value
			});
			// ReSharper restore PossibleInvalidOperationException

			return new SubscriptionAddedState();
		}

		public override string GetMessageBefore(ParsedUserCommand command, string token)
		{
			return "За сколько минут до прибытия транспорта предупредить?";
		}

		public override string GetMessageAfter(ParsedUserCommand command, string token)
		{
			return $"Хорошо. Я начну оповещать о том, что {TransportKind.GetKindLocalizedName(command.TransportKind.Value)} " +
			       $"номер *{command.Number}* будет на остановке *{command.StopToCome}*, за *{command.NotifyTimeMinutes}* минут";
		}
	}
}
