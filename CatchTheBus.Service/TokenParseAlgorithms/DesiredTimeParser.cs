using System;
using CatchTheBus.Service.RocketChatModels;

namespace CatchTheBus.Service.TokenParseAlgorithms
{
	public class DesiredTimeParser : ITokenParseAlgorithm
	{
		public ValidationResult Validate(string str)
		{
			var hoursAndMins = str.Split(new[] { ".", ":" }, StringSplitOptions.RemoveEmptyEntries);
			if (hoursAndMins.Length != 2)
			{
				return new ValidationResult { IsValid = true, ErrorMessage = "Введите корректное время" };
			}

			var hoursString = hoursAndMins[0];
			var minsString = hoursAndMins[1];
			int hours, mins;

			if (!int.TryParse(hoursString, out hours) || !int.TryParse(minsString, out mins))
			{
				return new ValidationResult { IsValid = true, ErrorMessage = "Введите корректное время" };
			}

			return new ValidationResult { IsValid = true };
		}

		public string GetResult(ParsedUserCommand parsedCommand, string currentToken, bool isLast)
		{
			var hoursAndMins = currentToken.Split(new[] { ".", ":" }, StringSplitOptions.RemoveEmptyEntries);
			int hours = int.Parse(hoursAndMins[0]), mins = int.Parse(hoursAndMins[1]);

			var dt = DateTime.Now.Date.AddHours(hours).AddMinutes(mins);
			parsedCommand.DesiredTime = dt;

			return isLast ? "За сколько минут до прибытия транспорта предупредить?" : null;
		}
	}
}
