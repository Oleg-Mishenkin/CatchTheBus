﻿using System;
using System.Collections.Generic;
using CatchTheBus.Service.RocketChatModels;
using CatchTheBus.Service.TokenParseAlgorithms;

namespace CatchTheBus.Service.Services
{
	public class MessageProcessor
	{
		// порядок важен
		private readonly List<ITokenParser> _parsers = new List<ITokenParser>
		{
			new TransportKindParser(),
			new TransportNumberParser(),
			new DirectionParser(),
			new StopToComeParser(),
			new DesiredTimeParser(),
			new NotifyTimeParser()
		};

		private string[] Tokenize(string str) => str.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);

		public string GetResponse(string str, string userId)
		{
			var tokens = Tokenize(str);

			var unfinishedCommand = UnfinishedCommandsRepository.Get().GetCommandForUser(userId);
			var command = unfinishedCommand?.Command ?? new ParsedUserCommand();

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

				var validationResult = parser.Validate(token, command);
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
						UnfinishedCommandsRepository.Get().UpdateCommand(userId, command, tokensToParseCount);
					}

					return parsingResult;
				}
			}

			return "Какая жалость! Не удалось разобрать команду.";
		}
	}
}
