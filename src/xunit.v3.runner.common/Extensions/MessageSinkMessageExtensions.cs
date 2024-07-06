using Xunit.Internal;
using Xunit.Runner.Common;

namespace Xunit.Sdk;

/// <summary>
/// Extension methods for <see cref="MessageSinkMessage"/>.
/// </summary>
public static partial class MessageSinkMessageExtensions
{
	/// <summary>
	/// Handles a message of a specific type by testing it for the type, as well as verifying that there
	/// is a registered callback.
	/// </summary>
	/// <param name="message">The message to dispatch.</param>
	/// <param name="callback">The callback to dispatch the message to.</param>
	/// <returns>Returns <c>true</c> if processing should continue; <c>false</c> otherwise.</returns>
	public static bool DispatchWhen<TMessage>(
		this MessageSinkMessage message,
		MessageHandler<TMessage>? callback)
			where TMessage : MessageSinkMessage
	{
		Guard.ArgumentNotNull(message);

		if (callback is not null && message is TMessage castMessage)
		{
			var args = new MessageHandlerArgs<TMessage>(castMessage);
			callback(args);
			return !args.IsStopped;
		}

		return true;
	}
}
