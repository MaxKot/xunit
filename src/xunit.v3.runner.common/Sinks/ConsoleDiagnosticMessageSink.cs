#pragma warning disable CA2002  // The console writer is not cross app-domain

using System;
using System.Globalization;
using System.IO;
using Xunit.Internal;
using Xunit.Sdk;

namespace Xunit.Runner.Common;

/// <summary>
/// Logs diagnostic messages to the system console.
/// </summary>
public class ConsoleDiagnosticMessageSink : IMessageSink
{
	readonly TextWriter consoleWriter;
	readonly string displayNewlineReplace;
	readonly string? displayPrefixDiagnostic;
	readonly string? displayPrefixInternal;
	readonly bool noColor;

	ConsoleDiagnosticMessageSink(
		TextWriter consoleWriter,
		bool noColor,
		bool showDiagnosticMessages,
		bool showInternalDiagnosticMessages,
		string? assemblyDisplayName)
	{
		Guard.ArgumentNotNull(consoleWriter);

		this.consoleWriter = consoleWriter;
		this.noColor = noColor;

		displayPrefixDiagnostic = (showDiagnosticMessages, assemblyDisplayName, noColor) switch
		{
			(false, _, _) => null,
			(true, null, false) => "",
			(true, null, true) => "[D] ",
			(true, _, false) => string.Format(CultureInfo.InvariantCulture, "[{0}] ", assemblyDisplayName),
			(true, _, true) => string.Format(CultureInfo.InvariantCulture, "[D::{0}] ", assemblyDisplayName)
		};
		displayPrefixInternal = (showInternalDiagnosticMessages, assemblyDisplayName, noColor) switch
		{
			(false, _, _) => null,
			(true, null, false) => "",
			(true, null, true) => "[I] ",
			(true, _, false) => string.Format(CultureInfo.InvariantCulture, "[{0}] ", assemblyDisplayName),
			(true, _, true) => string.Format(CultureInfo.InvariantCulture, "[I::{0}] ", assemblyDisplayName)
		};
		displayNewlineReplace = "\n" + new string(' ', (displayPrefixDiagnostic?.Length ?? displayPrefixInternal?.Length ?? 0) + 4);
	}

	/// <inheritdoc/>
	public bool OnMessage(MessageSinkMessage message)
	{
		Guard.ArgumentNotNull(message);

		if (message is DiagnosticMessage diagnosticMessage && displayPrefixDiagnostic is not null)
		{
			lock (consoleWriter)
			{
				if (!noColor)
					ConsoleHelper.SetForegroundColor(ConsoleColor.Yellow);

				consoleWriter.WriteLine("    {0}{1}", displayPrefixDiagnostic, diagnosticMessage.Message.Replace("\n", displayNewlineReplace));

				if (!noColor)
					ConsoleHelper.ResetColor();
			}
		}

		if (message is InternalDiagnosticMessage internalDiagnosticMessage && displayPrefixInternal is not null)
		{
			lock (consoleWriter)
			{
				if (!noColor)
					ConsoleHelper.SetForegroundColor(ConsoleColor.DarkGray);

				consoleWriter.WriteLine("    {0}{1}", displayPrefixInternal, internalDiagnosticMessage.Message.Replace("\n", displayNewlineReplace));

				if (!noColor)
					ConsoleHelper.ResetColor();
			}
		}

		return true;
	}

	/// <summary>
	/// Tries to create a new instance of the <see cref="ConsoleDiagnosticMessageSink"/> which will display instances
	/// of <see cref="DiagnosticMessage"/> and <see cref="InternalDiagnosticMessage"/> to the <see cref="Console"/>.
	/// May return <c>null</c> if both <paramref name="showDiagnosticMessages"/> and <paramref name="showInternalDiagnosticMessages"/>
	/// are <c>false</c>.
	/// </summary>
	/// <param name="consoleWriter">The text writer used to write console messages</param>
	/// <param name="noColor">A flag to indicate that the user has asked for no color</param>
	/// <param name="showDiagnosticMessages">A flag to indicate whether diagnostic messages should be shown</param>
	/// <param name="showInternalDiagnosticMessages">A flag to indicate whether internal diagnostic messages should be shown</param>
	/// <param name="assemblyDisplayName">The optional assembly display name to delineate the messages</param>
	public static ConsoleDiagnosticMessageSink? TryCreate(
		TextWriter consoleWriter,
		bool noColor,
		bool showDiagnosticMessages = false,
		bool showInternalDiagnosticMessages = false,
		string? assemblyDisplayName = null) =>
			showDiagnosticMessages || showInternalDiagnosticMessages
				? new(consoleWriter, noColor, showDiagnosticMessages, showInternalDiagnosticMessages, assemblyDisplayName)
				: null;
}
