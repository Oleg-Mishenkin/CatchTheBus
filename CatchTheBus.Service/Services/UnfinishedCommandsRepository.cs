using System.Collections.Generic;
using CatchTheBus.Service.RocketChatModels;

namespace CatchTheBus.Service.Services
{
	public class UnfinishedCommandsRepository
	{
		private UnfinishedCommandsRepository() { }

		private static UnfinishedCommandsRepository _instance;

		public static UnfinishedCommandsRepository Get() => _instance ?? (_instance = new UnfinishedCommandsRepository());

		private readonly Dictionary<string, ParsedUserCommand> _usersCommands 
			= new Dictionary<string, ParsedUserCommand>();  // UserId => command

		public ParsedUserCommand UpdateCommand(string userId, ParsedUserCommand command)
		{
			_usersCommands[userId] = command;
			return command;
		}

		public bool Remove(string userId)
		{
			if (!_usersCommands.ContainsKey(userId))
			{
				return false;
			}

			return _usersCommands.Remove(userId);
		}

		public ParsedUserCommand GetCommandForUser(string userId)
		{
			if (!_usersCommands.ContainsKey(userId))
			{
				return null;
			}

			return _usersCommands[userId];
		}
	}
}
