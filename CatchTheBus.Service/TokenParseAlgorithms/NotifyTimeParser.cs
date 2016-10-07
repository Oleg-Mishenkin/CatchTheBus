using CatchTheBus.Service.RocketChatModels;

namespace CatchTheBus.Service.TokenParseAlgorithms
{
	public class NotifyTimeParser : ITokenParseAlgorithm
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
				return new ValidationResult { IsValid = false, ErrorMessage = "Введите число, меньшее 60" };
			}

			return new ValidationResult { IsValid = true };
		}

		public string GetResult(ParsedUserCommand parsedCommand, string currentToken, bool isLast)
		{
			parsedCommand.NotifyTimeMinutes = int.Parse(currentToken);
			return isLast ? "Зарегистрировано" : null;
		}
	}
}
