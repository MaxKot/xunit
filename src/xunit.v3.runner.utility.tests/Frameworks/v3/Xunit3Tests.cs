using System;
using System.Linq;
using System.Threading.Tasks;
using NSubstitute;
using Xunit;
using Xunit.Runner.Common;
using Xunit.Runner.v3;
using Xunit.Sdk;

public class Xunit3Tests
{
	readonly XunitProjectAssembly Assembly;
	readonly ITestFrameworkDiscoveryOptions DiscoveryOptions = TestData.TestFrameworkDiscoveryOptions(includeSourceInformation: true);
	readonly ITestFrameworkExecutionOptions ExecutionOptions = TestData.TestFrameworkExecutionOptions();

	public Xunit3Tests()
	{
		Assembly = TestData.XunitProjectAssembly<Xunit3Tests>();
	}

	[Fact]
	public void GuardClauses_Ctor()
	{
		Assert.Throws<ArgumentNullException>("projectAssembly", () => Xunit3.ForDiscoveryAndExecution(null!));

		var assembly = new XunitProjectAssembly(new XunitProject(), "/this/file/does/not/exist.exe", new(3, ".NETCoreApp,Version=v6.0"));
		var argEx = Assert.Throws<ArgumentException>("projectAssembly.AssemblyFileName", () => Xunit3.ForDiscoveryAndExecution(assembly));
		Assert.StartsWith("File not found: /this/file/does/not/exist.exe", argEx.Message);
	}

	[Fact]
	public async ValueTask GuardClauses_Find()
	{
		await using var xunit3 = Xunit3.ForDiscoveryAndExecution(Assembly);

		Assert.Throws<ArgumentNullException>("messageSink", () => xunit3.Find(null!, new FrontControllerFindSettings(DiscoveryOptions)));
		Assert.Throws<ArgumentNullException>("settings", () => xunit3.Find(SpyMessageSink.Capture(), null!));
	}

	[Fact]
	public async ValueTask GathersAssemblyInformation()
	{
		var expectedUniqueID = UniqueIDGenerator.ForAssembly(
			Assembly.AssemblyFileName,
			Assembly.ConfigFileName
		);

		await using var xunit3 = Xunit3.ForDiscoveryAndExecution(Assembly);

		Assert.False(xunit3.CanUseAppDomains);
#if NET472
		Assert.Equal(".NETFramework,Version=v4.7.2", xunit3.TargetFramework);
#elif NET6_0
		Assert.Equal(".NETCoreApp,Version=v6.0", xunit3.TargetFramework);
#else
#error Unknown target framework
#endif
		Assert.Equal(expectedUniqueID, xunit3.TestAssemblyUniqueID);
		Assert.Matches(@"xUnit.net v3 \d+\.\d+\.\d+(-pre\.\d+(-dev)?(\+[0-9a-f]+)?)?", xunit3.TestFrameworkDisplayName);
	}

	[Fact]
	public async ValueTask CanFindFilteredTestsAndRunThem_UsingFind_UsingRun()
	{
		var sourceInformationProvider = Substitute.For<ISourceInformationProvider, InterfaceProxy<ISourceInformationProvider>>();
		sourceInformationProvider.GetSourceInformation("Xunit3Tests", "GuardClauses_Ctor").Returns(("/path/to/source/file.cs", 2112));
		await using var xunit3 = Xunit3.ForDiscoveryAndExecution(Assembly, sourceInformationProvider);

		// Find
		var filters = new XunitFilters();
		filters.IncludedMethods.Add($"{typeof(Xunit3Tests).FullName}.{nameof(GuardClauses_Ctor)}");

		var findMessageSink = SpyMessageSink<DiscoveryComplete>.Create();
		var findProcess = xunit3.Find(findMessageSink, new FrontControllerFindSettings(DiscoveryOptions, filters));
		Assert.NotNull(findProcess);

		var findFinished = findMessageSink.Finished.WaitOne(60_000);
		Assert.True(findFinished, "Message sink did not see _DiscoveryComplete within 60 seconds");

		var testCases = findMessageSink.Messages.OfType<TestCaseDiscovered>();
		var testCase = Assert.Single(testCases);
		Assert.Equal("Xunit3Tests.GuardClauses_Ctor", testCase.TestCaseDisplayName);
		Assert.Equal("/path/to/source/file.cs", testCase.SourceFilePath);
		Assert.Equal(2112, testCase.SourceLineNumber);

		// Run
		var runMessageSink = SpyMessageSink<TestAssemblyFinished>.Create();
		var runProcess = xunit3.Run(runMessageSink, new FrontControllerRunSettings(ExecutionOptions, [testCase.Serialization]));
		Assert.NotNull(runProcess);

		var runFinished = runMessageSink.Finished.WaitOne(60_000);
		Assert.True(runFinished, "Message sink did not see _TestAssemblyFinished within 60 seconds");

		var results = runMessageSink.Messages.OfType<TestResultMessage>().ToList();
		var passed = Assert.Single(runMessageSink.Messages.OfType<TestPassed>());
		Assert.Equal(testCase.TestCaseUniqueID, passed.TestCaseUniqueID);
		Assert.Empty(results.OfType<TestFailed>());
		Assert.Empty(results.OfType<TestSkipped>());
		Assert.Empty(results.OfType<TestNotRun>());
	}

	[Fact]
	public async ValueTask CanFindFilteredTestsAndRunThem_UsingFindAndRun()
	{
		await using var xunit3 = Xunit3.ForDiscoveryAndExecution(Assembly);
		var filters = new XunitFilters();
		filters.IncludedMethods.Add($"{typeof(Xunit3Tests).FullName}.{nameof(GuardClauses_Ctor)}");
		var messageSink = SpyMessageSink<TestAssemblyFinished>.Create();
		var process = xunit3.FindAndRun(messageSink, new FrontControllerFindAndRunSettings(DiscoveryOptions, ExecutionOptions, filters));
		Assert.NotNull(process);

		var finished = messageSink.Finished.WaitOne(60_000);
		Assert.True(finished, "Message sink did not see _DiscoveryComplete within 60 seconds");

		var starting = Assert.Single(messageSink.Messages.OfType<TestStarting>());
		Assert.Equal("Xunit3Tests.GuardClauses_Ctor", starting.TestDisplayName);
		var passed = Assert.Single(messageSink.Messages.OfType<TestPassed>());
		Assert.Equal(starting.TestUniqueID, passed.TestUniqueID);
		Assert.Empty(messageSink.Messages.OfType<TestFailed>());
		Assert.Empty(messageSink.Messages.OfType<TestSkipped>());
		Assert.Empty(messageSink.Messages.OfType<TestNotRun>());
	}
}
