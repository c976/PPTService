using System;

namespace PPTService.Ink.Core.Windows.Notifications
{
	public sealed class MessageResult : IEquatable<MessageResult>
	{
		public static readonly MessageResult Canceled = new MessageResult(0);

		public static readonly MessageResult Negative = new MessageResult(-1);

		public static readonly MessageResult Positive = new MessageResult(1);

		private readonly int _id;

		public int Id => _id;

		public MessageResult(int id)
		{
			_id = id;
		}

		public override bool Equals(object obj)
		{
			MessageResult messageResult = obj as MessageResult;
			if (messageResult != null)
			{
				return Equals(messageResult);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return _id;
		}

		public bool Equals(MessageResult other)
		{
			return Id == other.Id;
		}

		public static bool operator ==(MessageResult a, MessageResult b)
		{
			if ((object)a == b)
			{
				return true;
			}
			if ((object)a == null || (object)b == null)
			{
				return false;
			}
			return a.Id == b.Id;
		}

		public static bool operator !=(MessageResult a, MessageResult b)
		{
			return !(a == b);
		}

		public static implicit operator MessageResult(int id)
		{
			return new MessageResult(id);
		}

		public static explicit operator int(MessageResult result)
		{
			return result.Id;
		}
	}
}
