using System.Linq;
using CatchTheBus.Service.Constants;
using CatchTheBus.Service.RocketChatModels;

namespace CatchTheBus.Service.TokenParseAlgorithms
{
	public class TransportKindParser : ITokenParseAlgorithm
	{
		public ValidationResult Validate(string str) =>
					TransportKind.All.Contains(str)
						? new ValidationResult { IsValid = true }
						: new ValidationResult { IsValid = false, ErrorMessage = "Некорректный вид транспорта" };

		public string GetResult(ParsedUserCommand parsedCommand, string currentToken, bool isLast)
		{
			parsedCommand.TransportKind = TransportKind.Parse(currentToken);
			return isLast ? "Список маршрутов - тут" : null;
		}
	}
}
