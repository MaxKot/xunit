using Xunit.Internal;
using Xunit.Sdk;

public class TestSkippedWithDisplayName : TestResultMessage, ITestResultWithDisplayName
{
	internal TestSkippedWithDisplayName(
		TestSkipped testSkipped,
		string testDisplayName)
	{
		AssemblyUniqueID = testSkipped.AssemblyUniqueID;
		ExecutionTime = testSkipped.ExecutionTime;
		Output = testSkipped.Output;
		Reason = testSkipped.Reason;
		TestCaseUniqueID = testSkipped.TestCaseUniqueID;
		TestClassUniqueID = testSkipped.TestClassUniqueID;
		TestCollectionUniqueID = testSkipped.TestCollectionUniqueID;
		TestDisplayName = testDisplayName;
		TestMethodUniqueID = testSkipped.TestMethodUniqueID;
		TestUniqueID = testSkipped.TestUniqueID;
		Warnings = testSkipped.Warnings;
	}

	public string Reason { get; set; }

	public string TestDisplayName { get; set; }

	/// <inheritdoc/>
	public override string ToString() =>
		$"{GetType().Name}({TestUniqueID.Quoted()}, {TestDisplayName.Quoted()})";
}
