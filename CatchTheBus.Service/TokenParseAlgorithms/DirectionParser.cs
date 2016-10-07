﻿using CatchTheBus.Service.RocketChatModels;

namespace CatchTheBus.Service.TokenParseAlgorithms
{
	public class DirectionParser : ITokenParser
	{
		public ValidationResult Validate(string str, ParsedUserCommand command)
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
}
