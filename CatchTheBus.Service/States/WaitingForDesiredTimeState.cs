using System;
using CatchTheBus.Service.RocketChatModels;

namespace CatchTheBus.Service.States
{
	public class WaitingForDesiredTimeState : AbstractState
	{
		public override ValidationResult Validate(string token, ParsedUserCommand command)
		{
			var hoursAndMins = token.Split(new[] { ".", ":" }, StringSplitOptions.RemoveEmptyEntries);
			if (hoursAndMins.Length != 2)
			{
				return new ValidationResult { IsValid = false, ErrorMessage = "Введите корректное время" };
			}

			var hoursString = hoursAndMins[0];
			var minsString = hoursAndMins[1];
			int hours, mins;

			if (!int.TryParse(hoursString, out hours) || !int.TryParse(minsString, out mins))
			{
				return new ValidationResult { IsValid = false, ErrorMessage = "Введите корректное время" };
			}

			return new ValidationResult { IsValid = true };
		}

		public override AbstractState ParseToken(ParsedUserCommand command, string currentToken)
		{
			var hoursAndMins = currentToken.Split(new[] { ".", ":" }, StringSplitOptions.RemoveEmptyEntries);
			int hours = int.Parse(hoursAndMins[0]), mins = int.Parse(hoursAndMins[1]);

			command.DesiredTime = DateTime.Now.Date.AddHours(hours).AddMinutes(mins);
			return new WaitingForNotifyTimeState();
		}

		public override string GetMessageBefore(ParsedUserCommand command, string token) 
			=> "Во сколько ты хочешь сесть на транспорт (ЧЧ:ММ)?";

		public override string GetMessageAfter(ParsedUserCommand command, string token) 
			=> $"Выбранное время: {command.DesiredTime.Value.Hour.ToString("00")}:{command.DesiredTime.Value.Minute.ToString("00")}";
	}
}
