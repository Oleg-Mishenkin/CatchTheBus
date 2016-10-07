using System.Linq;
using CatchTheBus.Service.Constants;
using CatchTheBus.Service.RocketChatModels;
using CatchTheBus.Service.Services;

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

			var numbers = TransportRepositoryService.Instance.GetTransportKindNumbers(parsedCommand.TransportKind.Value);
			var formattedNumbers = string.Join("\n", numbers.Select((item, i) => $"*{item.Item1}* - {item.Item2}"));

			return isLast 
					? formattedNumbers + "\n\n" +
								$"Какой {TransportKind.GetKindLocalizedName(parsedCommand.TransportKind.Value)} нужен?"
					: null;
		}
	}
}
