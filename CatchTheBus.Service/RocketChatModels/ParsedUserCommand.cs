﻿using System;
using CatchTheBus.Service.Constants;
using CatchTheBus.Service.States;

namespace CatchTheBus.Service.RocketChatModels
{
	public class ParsedUserCommand
	{
		public ParsedUserCommand()
		{
			CurrentState = new WaitingForKindState();
		}

		public string UserName { get; set; }

		public TransportKind.Kind? TransportKind { get; set; }

		public string Number { get; set; }

		public DirectionType? Direction { get; set; }

		public string StopToCome { get; set; }

		public DateTime? DesiredTime { get; set; }

		public int? NotifyTimeMinutes { get; set; }

		public AbstractState CurrentState { get; set; }

		public ParsedUserCommand MergeWith(ParsedUserCommand other)
		{
			var result = new ParsedUserCommand();
			result.TransportKind = TransportKind;
			if (!string.IsNullOrEmpty(other.Number))
			{
				result.Number = other.Number;
				return result;
			}

			if (other.Direction != null)
			{
				result.Direction = other.Direction;
				return result;
			}

			if (other.DesiredTime != null)
			{
				result.DesiredTime = other.DesiredTime;
				return result;
			}

			if (other.StopToCome != null)
			{
				result.StopToCome = other.StopToCome;
				return result;
			}

			if (other.NotifyTimeMinutes != null)
			{
				result.NotifyTimeMinutes = other.NotifyTimeMinutes;
				return result;
			}

			// all fields are filled
			return result;
		}

		public bool IsCompletelyFilled()
		{
			return TransportKind != null && !string.IsNullOrEmpty(Number) && Direction != null && NotifyTimeMinutes != null;
		}
	}
}
