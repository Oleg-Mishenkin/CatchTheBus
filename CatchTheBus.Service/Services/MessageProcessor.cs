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
			new TransportDirectionParser()
		};

		private string[] Tokenize(string str) => str.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);

		public string GetResponse(string str)
		{
			var tokens = Tokenize(str);

			var command = new ParsedUserCommand();
			var tokensToParse = Math.Min(tokens.Length, _parsers.Count);

			for (int i = 0; i < tokensToParse; i++)
			{
				var token = tokens[i];
				var parser = _parsers[i];

				var validationResult = parser.Validate(token);
				if (!validationResult.IsValid)
				{
					return validationResult.ErrorMessage;
				}

				var parsingResult = parser.GetResult(command, token, i == tokensToParse - 1);
				if (!string.IsNullOrEmpty(parsingResult))
				{
					return parsingResult; // уже все попарсили. иначе идем на следующую итерацию
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

	public class TransportDirectionParser : ITokenParseAlgorithm
	{
		public ValidationResult Validate(string str)
		{
			var ok = str == "1" || str == "2";
			if (!ok) return new ValidationResult { IsValid = false, ErrorMessage = "Некорректное направление" };

			return new ValidationResult { IsValid = true };
		}

		public string GetResult(ParsedUserCommand parsedCommand, string currentToken, bool isLast)
		{
			parsedCommand.Number = currentToken;
			return isLast ? "Список остановок :)" : null;
		}
	}
}
