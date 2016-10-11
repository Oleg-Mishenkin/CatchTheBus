using System;
using System.Linq;
using CatchTheBus.Service.RocketChatModels;
using CatchTheBus.Service.Services;

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

			if (DateTime.Now > DateTime.Now.Date.AddHours(hours).AddMinutes(mins))
				return new ValidationResult { IsValid = false, ErrorMessage = "Вводимое время не может быть меньше текущего" };

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
		{
			var resultText = "";
			var timeEntries = TransportRepositoryService.Instance.GetTimeEntries(command.TransportKind.Value, command.Direction.Value, command.Number, command.StopToCome).Where(x => x.Hours > DateTime.Now.Hour || (x.Hours == DateTime.Now.Hour && x.Minutes >= DateTime.Now.Minute)).OrderBy(x => x.Hours).ThenBy(x => x.Minutes);
			if (timeEntries.Any())
			{
				resultText = "Ближайшее время прибытия транспорта:\n";
				resultText += string.Join("\n", timeEntries.Select(x => $"{x.Hours.ToString("00")}:{x.Minutes.ToString("00")}").ToArray());
			}

			resultText += "\n\nВо сколько ты хочешь сесть на транспорт (ЧЧ:ММ)?";
			return resultText;
		}

		public override string GetMessageAfter(ParsedUserCommand command, string token) 
			=> $"Выбранное время: {command.DesiredTime.Value.Hour.ToString("00")}:{command.DesiredTime.Value.Minute.ToString("00")}";
	}
}
