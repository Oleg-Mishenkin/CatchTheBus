using System;
using CatchTheBus.Service.Helpers;
using CatchTheBus.Service.RocketChatModels;
using CatchTheBus.Service.TokenParseAlgorithms;

namespace CatchTheBus.Service.Services
{
	public class MessageProcessor
	{
		private string[] Tokenize(string str) => str.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);

		public void Handle(string str, string userId, string userName)
		{
			var result = ProcessSpecialMessages(str, userId, userName);
			if (result != null)
			{
				OutgoingMessagesHelper.Get().SendMessage(result, userName);
				return;
			}

			var tokens = Tokenize(str);

			var unfinishedCommand = UnfinishedCommandsRepository.Get().GetCommandForUser(userId);
			var command = unfinishedCommand ?? new ParsedUserCommand { UserName = userName };

			int tokensToParseCount;
			if (!(command.CurrentState is WaitingForKindState))
			{
				tokensToParseCount = 1; // позволяем довводить только по одной команде. ибо нефиг.
			}
			else
			{
				tokensToParseCount = tokens.Length;
			}

			// если у нас была незаконченная команда, то мы начинаем не с самого первого парсера
			for (int i = 0; i < tokensToParseCount; i++)
			{
				var guidingMessage = string.Empty;

				var isLastToken = i == tokensToParseCount - 1;

				var token = tokens[i];

				var validationResult = command.CurrentState.Validate(token, command);
				if (!validationResult.IsValid)
				{
					OutgoingMessagesHelper.Get().SendMessage(validationResult.ErrorMessage, userName);
					return;
				}

				var newState = command.CurrentState.ParseToken(command, token);

				var messageAfter = command.CurrentState.GetMessageAfter(command, token);
				if (!string.IsNullOrEmpty(messageAfter))
				{
					guidingMessage += messageAfter + "\n\n";
				}

				if (isLastToken)
				{
					if (newState is SubscriptionAddedState) // уже все попарсили. иначе идем на следующую итерацию
					{
						UnfinishedCommandsRepository.Get().Remove(userId);
					}
					else
					{
						UnfinishedCommandsRepository.Get().UpdateCommand(userId, command);
					}
				}

				command.CurrentState = newState;

				// отправляем приветствие для новой команды
				var messageBefore = command.CurrentState.GetMessageBefore(command, token);
				if (!string.IsNullOrEmpty(messageBefore))
				{
					guidingMessage += messageBefore;
				}

				if (!string.IsNullOrEmpty(guidingMessage) && isLastToken)
				{
					OutgoingMessagesHelper.Get().SendMessage(guidingMessage, userName);
				}
			}
		}

		private string ProcessSpecialMessages(string input, string userId, string userName)
		{
			var inputLower = input.ToLower();
			if (inputLower == "h" || input == "help" || input == "помощь")
			{
				return HintHelper.Help;
			}

			if (inputLower == "сброс")
			{
				UnfinishedCommandsRepository.Get().Remove(userId);
				SubscriptionService.Instance.RemoveAllSubscriptions(userName);
				return "Ваши подписки на транспорт и незавершенные команды удалены";
			}

			return null;
		}
	}
}
