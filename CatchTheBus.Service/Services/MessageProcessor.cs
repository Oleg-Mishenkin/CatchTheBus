using System;
using System.Collections.Generic;
using System.Linq;
using CatchTheBus.Service.Constants;
using CatchTheBus.Service.RocketChatModels;

namespace CatchTheBus.Service.Services
{
	public class MessageProcessor
	{
		// порядок важен
		private readonly List<ITokenParseAlgorithm> _parsers = new List<ITokenParseAlgorithm>
		{
			new TransportKindParser(),
			new TransportNumberParser(),
			new DirectionParser(),
			new DesiredTimeParser(),
			new StopToComeParser(),
			new NotifyTimeParser()
		};

		private string[] Tokenize(string str) => str.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);

		public string GetResponse(string str, string userId)
		{
			var tokens = Tokenize(str);

			var command = new ParsedUserCommand();
			var unfinishedCommand = UnfinishedCommandsRepository.Get().GetCommandForUser(userId);

			int tokensToParseCount;
			if (unfinishedCommand != null)
			{
				tokensToParseCount = 1; // позволяем довводить только по одной команде. ибо нефиг.
			}
			else
			{
				tokensToParseCount = Math.Min(tokens.Length, _parsers.Count);
			}

			// если у нас была незаконченная команда, то мы начинаем не с самого первого парсера
			for (int i = 0, j = (unfinishedCommand?.LastFilledStep + 1) ?? 0; i < tokensToParseCount; i++, j++)
			{
				var isLastToken = i == tokensToParseCount - 1;

				var token = tokens[i];
				var parser = _parsers[j];

				var validationResult = parser.Validate(token);
				if (!validationResult.IsValid)
				{
					return validationResult.ErrorMessage;
				}

				var parsingResult = parser.GetResult(command, token, isLastToken);
				if (isLastToken)
				{
					if (command.IsCompletelyFilled()) // уже все попарсили. иначе идем на следующую итерацию
					{
						UnfinishedCommandsRepository.Get().Remove(userId);
					}
					else
					{
						UnfinishedCommandsRepository.Get().NewCommandChunk(userId, command, tokensToParseCount);
					}

					return parsingResult;
				}
			}

			return "Какая жалость! Не удалось разобрать команду.";
		}
	}

	public interface ITokenParseAlgorithm
	{
		ValidationResult Validate(string str);

		string GetResult(ParsedUserCommand parsedCommand, string currentToken, bool isLast);
	}

	public class ValidationResult
	{
		public string ErrorMessage { get; set; }

		public bool IsValid { get; set; }
	}

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

	public class TransportNumberParser : ITokenParseAlgorithm
	{
		// TODO: оно нам надо?
		public ValidationResult Validate(string str) => new ValidationResult { IsValid = true };

		public string GetResult(ParsedUserCommand parsedCommand, string currentToken, bool isLast)
		{
			parsedCommand.Number = currentToken;
			return isLast ? "Список направлений!!!" : null;
		}
	}

	public class DirectionParser : ITokenParseAlgorithm
	{
		public ValidationResult Validate(string str)
		{
			var ok = str == "п" || str == "о" || str == "в";
			if (!ok) return new ValidationResult { IsValid = false, ErrorMessage = "Некорректное направление" };

			return new ValidationResult { IsValid = true };
		}

		public string GetResult(ParsedUserCommand parsedCommand, string currentToken, bool isLast)
		{
			parsedCommand.Number = currentToken;
			return isLast ? "На какую остановку подойдешь?" : null;
		}
	}

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

	public class NotifyTimeParser : ITokenParseAlgorithm
	{
		public ValidationResult Validate(string str)
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
