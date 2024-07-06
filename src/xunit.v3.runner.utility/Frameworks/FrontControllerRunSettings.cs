using System.Collections.Generic;
using Xunit.Internal;
using Xunit.Sdk;

namespace Xunit;

/// <summary>
/// Contains the information by <see cref="IFrontController.Run"/>.
/// </summary>
public class FrontControllerRunSettings : FrontControllerSettingsBase
{
	/// <summary>
	/// Initializes a new instance of the <see cref="FrontControllerFindSettings"/> class.
	/// </summary>
	/// <param name="options">The options used during execution</param>
	/// <param name="serializedTestCases">The test cases to be run</param>
	public FrontControllerRunSettings(
		ITestFrameworkExecutionOptions options,
		IReadOnlyCollection<string> serializedTestCases)
	{
		Options = Guard.ArgumentNotNull(options);
		SerializedTestCases = Guard.ArgumentNotNull(serializedTestCases);
	}

	/// <summary>
	/// The options used during execution.
	/// </summary>
	public ITestFrameworkExecutionOptions Options { get; }

	/// <summary>
	/// Get the list of test cases to be run.
	/// </summary>
	public IReadOnlyCollection<string> SerializedTestCases { get; }
}
