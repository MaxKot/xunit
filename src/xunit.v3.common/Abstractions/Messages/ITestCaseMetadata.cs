using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Xunit.Sdk;

/// <summary>
/// Represents metadata about a test case.
/// </summary>
public interface ITestCaseMetadata
{
	/// <summary>
	/// Gets the display text for the reason a test is being skipped; if the test
	/// is not skipped, returns <c>null</c>.
	/// </summary>
	string? SkipReason { get; }

	/// <summary>
	/// Gets the source file name. A <c>null</c> value indicates that the
	/// source file name is not known.
	/// </summary>
	string? SourceFilePath { get; }

	/// <summary>
	/// Gets the source file line number. A <c>null</c> value indicates that the
	/// source file line number is not known.
	/// </summary>
	int? SourceLineNumber { get; }

	/// <summary>
	/// Gets the display name of the test case.
	/// </summary>
	string TestCaseDisplayName { get; }

	/// <summary>
	/// Gets the name of the class where the test is defined. If the test did not originiate
	/// in a class, will return <c>null</c>.
	/// </summary>
	[NotNullIfNotNull(nameof(TestMethodName))]
	string? TestClassName { get; }

	/// <summary>
	/// Gets the namespace of the class where the test is defined. If the test did not
	/// originate in a class, or the class it originated in does not reside in a namespace,
	/// will return <c>null</c>.
	/// </summary>
	string? TestClassNamespace { get; }

	/// <summary>
	/// Gets the method name where the test is defined, in the <see cref="TestClassName"/> class.
	/// If the test did not originiate in a method, will return <c>null</c>.
	/// </summary>
	string? TestMethodName { get; }

	/// <summary>
	/// Gets the trait values associated with this test case. If there are none, or the framework
	/// does not support traits, this should return an empty dictionary (not <c>null</c>).
	/// </summary>
	IReadOnlyDictionary<string, IReadOnlyList<string>> Traits { get; }

	/// <summary>
	/// Gets a unique identifier for the test case.
	/// </summary>
	/// <remarks>
	/// The unique identifier for a test case should be able to discriminate among test cases, even those
	/// which are varied invocations against the same test method (i.e., theories). This identifier should
	/// remain stable until such time as the developer changes some fundamental part of the identity
	/// (assembly, class name, test name, or test data). Recompilation of the test assembly is reasonable
	/// as a stability changing event.
	/// </remarks>
	string UniqueID { get; }
}

internal interface IWritableTestCaseMetadata
{
	string? SkipReason { get; set; }
	string? SourceFilePath { get; set; }
	int? SourceLineNumber { get; set; }
	string TestCaseDisplayName { get; set; }
	string? TestClassName { get; set; }
	string? TestClassNamespace { get; set; }
	string? TestMethodName { get; set; }
	IReadOnlyDictionary<string, IReadOnlyList<string>> Traits { get; set; }
}
