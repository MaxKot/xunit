using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit;
using Xunit.Internal;
using Xunit.Sdk;
using Xunit.v3;

// This file manufactures instances of the test object model
public static partial class TestData
{
	public static XunitDelayEnumeratedTheoryTestCase XunitDelayEnumeratedTheoryTestCase<TClassUnderTest>(
		string methodName,
		IXunitTestCollection? collection = null,
		TestMethodDisplay methodDisplay = TestMethodDisplay.ClassAndMethod,
		TestMethodDisplayOptions methodDisplayOptions = TestMethodDisplayOptions.None,
		bool @explicit = false,
		Dictionary<string, List<string>>? traits = null,
		int timeout = 0,
		string uniqueID = DefaultTestCaseUniqueID)
	{
		var methodInfo = typeof(TClassUnderTest).GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.FlattenHierarchy);
		Guard.ArgumentNotNull($"Could not find method '{methodName}' on type '{typeof(TClassUnderTest).FullName}'", methodInfo, nameof(methodName));

		var testClass = XunitTestClass<TClassUnderTest>(collection);
		var testMethod = XunitTestMethod(testClass, methodInfo);
		var theoryAttribute = new TheoryAttribute { Explicit = @explicit, Timeout = timeout };
		var discoveryOptions = TestFrameworkDiscoveryOptions(methodDisplay: methodDisplay, methodDisplayOptions: methodDisplayOptions);
		var details = TestIntrospectionHelper.GetTestCaseDetails(discoveryOptions, testMethod, theoryAttribute);

		return new(
			details.ResolvedTestMethod,
			details.TestCaseDisplayName,
			uniqueID ?? details.UniqueID,
			@explicit,
			traits ?? testMethod.Traits.ToReadWrite(StringComparer.OrdinalIgnoreCase),
			timeout: timeout
		);
	}

	public static XunitTest XunitTest(
		IXunitTestCase testCase,
		IXunitTestMethod? testMethod = null,
		bool? @explicit = null,
		string testDisplayName = DefaultTestDisplayName,
		IReadOnlyDictionary<string, IReadOnlyList<string>>? traits = null,
		int? timeout = null,
		string uniqueID = DefaultTestUniqueID)
	{
		testMethod ??= testCase.TestMethod;

		return new(testCase, testMethod, @explicit, testDisplayName, uniqueID, traits ?? testMethod.Traits, timeout);
	}

	public static XunitTest XunitTest<TClassUnderTest>(
		string methodName,
		bool? @explicit = null,
		string testDisplayName = DefaultTestDisplayName,
		IReadOnlyDictionary<string, IReadOnlyList<string>>? traits = null,
		int? timeout = null,
		string uniqueID = DefaultTestUniqueID) =>
			XunitTest(XunitTestCase<TClassUnderTest>(methodName), null, @explicit, testDisplayName, traits, timeout, uniqueID);

	public static XunitTestAssembly XunitTestAssembly(
		Assembly assembly,
		string? configFileName = null,
		Version? version = null,
		string uniqueID = DefaultAssemblyUniqueID) =>
			new(assembly, configFileName, version, uniqueID);

	public static XunitTestAssembly XunitTestAssembly<TClassUnderTest>(
		string? configFileName = null,
		Version? version = null,
		string uniqueID = DefaultAssemblyUniqueID) =>
			XunitTestAssembly(typeof(TClassUnderTest).Assembly, configFileName, version, uniqueID);

	public static XunitTestCase XunitTestCase(
		IXunitTestMethod testMethod,
		bool? @explicit = null,
		TestMethodDisplay methodDisplay = TestMethodDisplay.ClassAndMethod,
		TestMethodDisplayOptions methodDisplayOptions = TestMethodDisplayOptions.None,
		string? skipReason = null,
		object?[]? testMethodArguments = null,
		int? timeout = null,
		Dictionary<string, List<string>>? traits = null,
		string uniqueID = DefaultTestCaseUniqueID)
	{
		var factAttribute = testMethod.Method.GetMatchingCustomAttributes(typeof(IFactAttribute)).FirstOrDefault() as IFactAttribute;
		Assert.NotNull(factAttribute);

		var discoveryOptions = TestFrameworkDiscoveryOptions(methodDisplay: methodDisplay, methodDisplayOptions: methodDisplayOptions);
		var details = TestIntrospectionHelper.GetTestCaseDetails(discoveryOptions, testMethod, factAttribute);

		return new(
			details.ResolvedTestMethod,
			details.TestCaseDisplayName,
			uniqueID ?? details.UniqueID,
			@explicit ?? details.Explicit,
			skipReason ?? details.SkipReason,
			traits ?? testMethod.Traits.ToReadWrite(StringComparer.OrdinalIgnoreCase),
			testMethodArguments,
			timeout: timeout ?? factAttribute.Timeout
		);
	}

	public static XunitTestCase XunitTestCase<TClassUnderTest>(
		string methodName,
		bool? @explicit = null,
		TestMethodDisplay methodDisplay = TestMethodDisplay.ClassAndMethod,
		TestMethodDisplayOptions methodDisplayOptions = TestMethodDisplayOptions.None,
		string? skipReason = null,
		IXunitTestCollection? testCollection = null,
		object?[]? testMethodArguments = null,
		int? timeout = null,
		Dictionary<string, List<string>>? traits = null,
		string uniqueID = DefaultTestCaseUniqueID)
	{
		var methodInfo = typeof(TClassUnderTest).GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.FlattenHierarchy);
		Guard.ArgumentNotNull($"Could not find method '{methodName}' on type '{typeof(TClassUnderTest).FullName}'", methodInfo, nameof(methodName));

		var testClass = XunitTestClass<TClassUnderTest>(testCollection);
		var testMethod = XunitTestMethod(testClass, methodInfo, testMethodArguments);
		return XunitTestCase(testMethod, @explicit, methodDisplay, methodDisplayOptions, skipReason, testMethodArguments, timeout, traits, uniqueID);
	}

	public static XunitTestClass XunitTestClass(
		Type @class,
		IXunitTestCollection? testCollection = null,
		string uniqueID = DefaultTestClassUniqueID) =>
			new(@class, testCollection ?? XunitTestCollection(XunitTestAssembly(@class.Assembly)), uniqueID);

	public static XunitTestClass XunitTestClass<TClassUnderTest>(
		IXunitTestCollection? collection = null,
		string uniqueID = DefaultTestClassUniqueID) =>
			XunitTestClass(typeof(TClassUnderTest), collection ?? XunitTestCollection<TClassUnderTest>(), uniqueID);

	public static XunitTestCollection XunitTestCollection(
		IXunitTestAssembly assembly,
		Type? collectionDefinition = null,
		bool? disableParallelization = null,
		string? displayName = null,
		string uniqueID = DefaultTestCollectionUniqueID) =>
			new(assembly, collectionDefinition, disableParallelization ?? false, displayName ?? $"[Unit Test] Collection for '{assembly.AssemblyName}'", uniqueID);

	public static XunitTestCollection XunitTestCollection<TClassUnderTest>(
		Type? collectionDefinition = null,
		bool? disableParallelization = null,
		string? displayName = null,
		string uniqueID = DefaultTestCollectionUniqueID)
	{
		var testAssembly = XunitTestAssembly<TClassUnderTest>();
		var standardCollection = new CollectionPerClassTestCollectionFactory(testAssembly).Get(typeof(TClassUnderTest));
		collectionDefinition ??= standardCollection.CollectionDefinition;
		disableParallelization ??= standardCollection.DisableParallelization;
		displayName ??= standardCollection.TestCollectionDisplayName;

		return XunitTestCollection(testAssembly, collectionDefinition, disableParallelization, displayName, uniqueID);
	}

	public static XunitTestMethod XunitTestMethod(
		IXunitTestClass testClass,
		MethodInfo methodInfo,
		object?[]? testMethodArguments = null,
		string uniqueID = DefaultTestMethodUniqueID) =>
			new(testClass, methodInfo, testMethodArguments ?? [], uniqueID);

	public static XunitTestMethod XunitTestMethod<TClassUnderTest>(
		string methodName,
		object?[]? testMethodArguments = null,
		string uniqueID = DefaultTestMethodUniqueID)
	{
		var methodInfo = typeof(TClassUnderTest).GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.FlattenHierarchy);
		Guard.ArgumentNotNull($"Could not find method '{methodName}' on type '{typeof(TClassUnderTest).FullName}'", methodInfo, nameof(methodName));

		var testClass = XunitTestClass<TClassUnderTest>();
		return XunitTestMethod(testClass, methodInfo, testMethodArguments, uniqueID);
	}
}
