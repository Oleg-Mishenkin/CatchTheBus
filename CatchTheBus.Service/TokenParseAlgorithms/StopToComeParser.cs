using CatchTheBus.Service.RocketChatModels;

namespace CatchTheBus.Service.TokenParseAlgorithms
{
	public class StopToComeParser : ITokenParseAlgorithm
	{
		public ValidationResult Validate(string str)
		{
			int stopNumber;
			if (!int.TryParse(str, out stopNumber))
			{
				return new ValidationResult { IsValid = false, ErrorMessage = "Не найдена остановка с таким номером" };
			}

			// TODO: провалидировать
			return new ValidationResult { IsValid = true };
		}

		public string GetResult(ParsedUserCommand parsedCommand, string currentToken, bool isLast)
		{
			parsedCommand.StopToCome = int.Parse(currentToken);
			return isLast ? "Во сколько ты бы хотел сесть на транспорт?" : null;
		}
	}
}
