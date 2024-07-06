using System;
using System.Collections.Generic;
using System.Linq;
using NSubstitute;
using Xunit;
using Xunit.Internal;
using Xunit.Sdk;

public class MessageBusTests
{
	[Fact]
	public static void QueuedMessageShowUpInMessageSink()
	{
		var messages = new List<MessageSinkMessage>();
		var sink = SpyMessageSink.Create(messages: messages);
		var msg1 = new MessageSinkMessage();
		var msg2 = new MessageSinkMessage();
		var msg3 = new MessageSinkMessage();

		using (var bus = new MessageBus(sink))
		{
			bus.QueueMessage(msg1);
			bus.QueueMessage(msg2);
			bus.QueueMessage(msg3);
		}

		Assert.Collection(
			messages,
			message => Assert.Same(msg1, message),
			message => Assert.Same(msg2, message),
			message => Assert.Same(msg3, message)
		);
	}

	[Fact]
	public static void TryingToQueueMessageAfterDisposingThrows()
	{
		var bus = new MessageBus(SpyMessageSink.Create());
		bus.Dispose();

		var exception = Record.Exception(
			() => bus.QueueMessage(new MessageSinkMessage())
		);

		Assert.IsType<ObjectDisposedException>(exception);
	}

	[Fact]
	public static void WhenSinkThrowsMessagesContinueToBeDelivered()
	{
		var sink = Substitute.For<IMessageSink>();
		var msg1 = new MessageSinkMessage();
		var msg2 = new MessageSinkMessage();
		var msg3 = new MessageSinkMessage();
		var messages = new List<MessageSinkMessage>();
		sink
			.OnMessage(Arg.Any<MessageSinkMessage>())
			.Returns(callInfo =>
			{
				var msg = (MessageSinkMessage)callInfo[0];
				if (msg == msg2)
					throw new DivideByZeroException("whee!");
				else
					messages.Add(msg);

				return false;
			});

		using (var bus = new MessageBus(sink))
		{
			bus.QueueMessage(msg1);
			bus.QueueMessage(msg2);
			bus.QueueMessage(msg3);
		}

		Assert.Collection(
			messages,
			message => Assert.Same(message, msg1),
			message =>
			{
				var errorMessage = Assert.IsAssignableFrom<ErrorMessage>(message);
				Assert.Equal("System.DivideByZeroException", errorMessage.ExceptionTypes.Single());
				Assert.Equal("whee!", errorMessage.Messages.Single());
			},
			message => Assert.Same(message, msg3)
		);
	}

	[Fact]
	public static void QueueReturnsTrueForFailIfStopOnFailFalse()
	{
		var messages = new List<MessageSinkMessage>();
		var sink = SpyMessageSink.Create(messages: messages);
		var msg1 = new MessageSinkMessage();
		var msg2 = TestData.TestFailed();
		var msg3 = new MessageSinkMessage();

		using (var bus = new MessageBus(sink))
		{
			Assert.True(bus.QueueMessage(msg1));
			Assert.True(bus.QueueMessage(msg2));
			Assert.True(bus.QueueMessage(msg3));
		}

		Assert.Collection(
			messages,
			message => Assert.Same(msg1, message),
			message => Assert.Same(msg2, message),
			message => Assert.Same(msg3, message)
		);
	}

	[Fact]
	public static void QueueReturnsFalseForFailIfStopOnFailTrue()
	{
		var messages = new List<MessageSinkMessage>();
		var sink = SpyMessageSink.Create(messages: messages);
		var msg1 = new MessageSinkMessage();
		var msg2 = TestData.TestFailed();
		var msg3 = new MessageSinkMessage();

		using (var bus = new MessageBus(sink, true))
		{
			Assert.True(bus.QueueMessage(msg1));
			Assert.False(bus.QueueMessage(msg2));
			Assert.False(bus.QueueMessage(msg3));
		}

		Assert.Collection(
			messages,
			message => Assert.Same(msg1, message),
			message => Assert.Same(msg2, message),
			message => Assert.Same(msg3, message)
		);
	}
}
