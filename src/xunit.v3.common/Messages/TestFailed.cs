using System;
using System.Collections.Generic;
using System.Globalization;
using Xunit.Internal;

namespace Xunit.Sdk;

/// <summary>
/// This message indicates that a test has failed.
/// </summary>
[JsonTypeID("test-failed")]
public sealed class TestFailed : TestResultMessage, IErrorMetadata, IWritableErrorMetadata, IWritableExecutionMetadata
{
	FailureCause cause = FailureCause.Exception;
	int[]? exceptionParentIndices;
	string?[]? exceptionTypes;
	string[]? messages;
	string?[]? stackTraces;

	/// <summary>
	/// Gets or sets the cause of the test failure.
	/// </summary>
	public FailureCause Cause
	{
		get => cause;
		set
		{
			Guard.ArgumentValid(
				() => string.Format(
					CultureInfo.CurrentCulture,
					"{0} is not a valid value from {1}",
					nameof(Cause),
					typeof(FailureCause).SafeName()
				),
				Enum.IsDefined(typeof(FailureCause), value),
				nameof(Cause)
			);
			cause = value;
		}
	}

	/// <inheritdoc/>
	public int[] ExceptionParentIndices
	{
		get => this.ValidateNullablePropertyValue(exceptionParentIndices, nameof(ExceptionParentIndices));
		set => exceptionParentIndices = Guard.ArgumentNotNullOrEmpty(value, nameof(ExceptionParentIndices));
	}

	/// <inheritdoc/>
	public string?[] ExceptionTypes
	{
		get => this.ValidateNullablePropertyValue(exceptionTypes, nameof(ExceptionTypes));
		set => exceptionTypes = Guard.ArgumentNotNullOrEmpty(value, nameof(ExceptionTypes));
	}

	/// <inheritdoc/>
	public string[] Messages
	{
		get => this.ValidateNullablePropertyValue(messages, nameof(Messages));
		set => messages = Guard.ArgumentNotNullOrEmpty(value, nameof(Messages));
	}

	/// <inheritdoc/>
	public string?[] StackTraces
	{
		get => this.ValidateNullablePropertyValue(stackTraces, nameof(StackTraces));
		set => stackTraces = Guard.ArgumentNotNullOrEmpty(value, nameof(StackTraces));
	}

	/// <summary>
	/// Creates a new <see cref="TestFailed"/> constructed from an <see cref="Exception"/> object.
	/// </summary>
	/// <param name="ex">The exception to use</param>
	/// <param name="assemblyUniqueID">The unique ID of the assembly</param>
	/// <param name="testCollectionUniqueID">The unique ID of the test collectioon</param>
	/// <param name="testClassUniqueID">The (optional) unique ID of the test class</param>
	/// <param name="testMethodUniqueID">The (optional) unique ID of the test method</param>
	/// <param name="testCaseUniqueID">The unique ID of the test case</param>
	/// <param name="testUniqueID">The unique ID of the test</param>
	/// <param name="executionTime">The execution time of the test (may be <c>null</c> if the test wasn't executed)</param>
	/// <param name="output">The (optional) output from the test</param>
	/// <param name="warnings">The (optional) warnings that were recorded during test execution</param>
	public static TestFailed FromException(
		Exception ex,
		string assemblyUniqueID,
		string testCollectionUniqueID,
		string? testClassUniqueID,
		string? testMethodUniqueID,
		string testCaseUniqueID,
		string testUniqueID,
		decimal executionTime,
		string? output,
		string[]? warnings)
	{
		Guard.ArgumentNotNull(ex);
		Guard.ArgumentNotNull(assemblyUniqueID);
		Guard.ArgumentNotNull(testCollectionUniqueID);
		Guard.ArgumentNotNull(testCaseUniqueID);
		Guard.ArgumentNotNull(testUniqueID);

		var errorMetadata = ExceptionUtility.ExtractMetadata(ex);

		return new TestFailed
		{
			AssemblyUniqueID = assemblyUniqueID,
			Cause = errorMetadata.Cause,
			ExceptionParentIndices = errorMetadata.ExceptionParentIndices,
			ExceptionTypes = errorMetadata.ExceptionTypes,
			ExecutionTime = executionTime,
			Messages = errorMetadata.Messages,
			Output = output ?? string.Empty,
			StackTraces = errorMetadata.StackTraces,
			TestCollectionUniqueID = testCollectionUniqueID,
			TestClassUniqueID = testClassUniqueID,
			TestMethodUniqueID = testMethodUniqueID,
			TestCaseUniqueID = testCaseUniqueID,
			TestUniqueID = testUniqueID,
			Warnings = warnings,
		};
	}

	/// <inheritdoc/>
	protected override void Deserialize(IReadOnlyDictionary<string, object?> root)
	{
		base.Deserialize(root);

		root.DeserializeErrorMetadata(this);
		root.DeserializeExecutionMetadata(this);

		if (JsonDeserializer.TryGetEnum<FailureCause>(root, nameof(Cause)) is FailureCause cause)
			Cause = cause;
	}

	/// <inheritdoc/>
	protected override void Serialize(JsonObjectSerializer serializer)
	{
		Guard.ArgumentNotNull(serializer);

		base.Serialize(serializer);

		serializer.SerializeErrorMetadata(this);
		serializer.SerializeExecutionMetadata(this);

		serializer.Serialize(nameof(Cause), Cause);
	}

	/// <inheritdoc/>
	protected override void ValidateObjectState(HashSet<string> invalidProperties)
	{
		base.ValidateObjectState(invalidProperties);

		ValidatePropertyIsNotNull(exceptionParentIndices, nameof(ExceptionParentIndices), invalidProperties);
		ValidatePropertyIsNotNull(exceptionTypes, nameof(ExceptionTypes), invalidProperties);
		ValidatePropertyIsNotNull(messages, nameof(Messages), invalidProperties);
		ValidatePropertyIsNotNull(stackTraces, nameof(StackTraces), invalidProperties);
	}
}
