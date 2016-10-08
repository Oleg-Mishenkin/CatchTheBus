using System;
using System.Collections.Generic;
using CatchTheBus.Service.RocketChatModels;

namespace CatchTheBus.Service.Services
{
	public class UnfinishedCommandsRepository
	{
		private UnfinishedCommandsRepository() { }

		private static UnfinishedCommandsRepository _instance;

		public static UnfinishedCommandsRepository Get() => _instance ?? (_instance = new UnfinishedCommandsRepository());

		private readonly Dictionary<string, UnfinishedCommand> _usersCommands 
			= new Dictionary<string, UnfinishedCommand>();  // UserId => command

		public ParsedUserCommand NewCommandChunk(string userId, ParsedUserCommand commandChunk, int newTokensCount)
		{
			var c = _usersCommands.ContainsKey(userId) ? _usersCommands[userId] : new UnfinishedCommand();
			if (c.Command != null)
			{
				c.Command = c.Command.MergeWith(commandChunk);
				c.LastFilledStep++; // позволяем вводить только один чанк в незавершенную команду, поэтому ++
			}
			else
			{
				c.Command = commandChunk;
				c.LastFilledStep += (newTokensCount - 1);
			}

			_usersCommands[userId] = c;

			return c.Command;
		}

		public ParsedUserCommand UpdateCommand(string userId, ParsedUserCommand commandChunk, int newTokensCount)
		{
			var c = _usersCommands.ContainsKey(userId) ? _usersCommands[userId] : new UnfinishedCommand();

			if (c.Command != null)
			{
				c.LastFilledStep += 1;
			}
			else
			{
				c.LastFilledStep += (newTokensCount - 1);
			}

			c.Command = commandChunk;
			_usersCommands[userId] = c;

			return c.Command;
		}

		public void Remove(string userId)
		{
			if (!_usersCommands.ContainsKey(userId))
			{
				throw new InvalidOperationException("Cannot find a user with id " + userId);
			}

			_usersCommands.Remove(userId);
		}

		public UnfinishedCommand GetCommandForUser(string userId)
		{
			if (!_usersCommands.ContainsKey(userId))
			{
				return null;
			}

			return _usersCommands[userId];
		}

		public class UnfinishedCommand
		{
			public ParsedUserCommand Command { get; set; }

			public int LastFilledStep { get; set; }
		}
	}
}
