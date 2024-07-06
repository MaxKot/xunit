using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Sdk;

public class DynamicSkipAcceptanceTests
{
	public class Skip : AcceptanceTestV3
	{
		[Fact]
		public void GuardClause()
		{
			Assert.Throws<ArgumentNullException>("reason", () => Assert.Skip(null!));
		}

		[Fact]
		public async Task AcceptanceTest()
		{
			var results = await RunAsync(typeof(ClassUnderTest));

			var skipResult = Assert.Single(results.OfType<TestSkipped>());
			Assert.Equal("This test was skipped", skipResult.Reason);
		}

		class ClassUnderTest
		{
			[Fact]
			public void Unconditional()
			{
				Assert.Skip("This test was skipped");
			}
		}
	}

	public class SkipUnless : AcceptanceTestV3
	{
		[Fact]
		public void GuardClause()
		{
			Assert.Throws<ArgumentNullException>("reason", () => Assert.SkipUnless(true, null!));
		}

		[Fact]
		public async Task AcceptanceTest()
		{
			var results = await RunAsync(typeof(ClassUnderTest));

			var skipResult = Assert.Single(results.OfType<TestSkipped>());
			var skipMethodStarting = Assert.Single(results.OfType<TestMethodStarting>().Where(s => s.TestMethodUniqueID == skipResult.TestMethodUniqueID));
			Assert.Equal("Skipped", skipMethodStarting.MethodName);
			Assert.Equal("This test was skipped", skipResult.Reason);
			var passResult = Assert.Single(results.OfType<TestPassed>());
			var passMethodStarting = results.OfType<TestMethodStarting>().Where(ts => ts.TestMethodUniqueID == passResult.TestMethodUniqueID).Single();
			Assert.Equal("Passed", passMethodStarting.MethodName);
		}

		class ClassUnderTest
		{
			[Fact]
			public void Skipped()
			{
				Assert.SkipUnless(false, "This test was skipped");
			}

			[Fact]
			public void Passed()
			{
				Assert.SkipUnless(true, "This test is not skipped");
			}
		}
	}

	public class SkipWhen : AcceptanceTestV3
	{
		[Fact]
		public void GuardClause()
		{
			Assert.Throws<ArgumentNullException>("reason", () => Assert.SkipWhen(true, null!));
		}

		[Fact]
		public async Task AcceptanceTest()
		{
			var results = await RunAsync(typeof(ClassUnderTest));

			var skipResult = Assert.Single(results.OfType<TestSkipped>());
			var skipMethodStarting = Assert.Single(results.OfType<TestMethodStarting>().Where(s => s.TestMethodUniqueID == skipResult.TestMethodUniqueID));
			Assert.Equal("Skipped", skipMethodStarting.MethodName);
			Assert.Equal("This test was skipped", skipResult.Reason);
			var passResult = Assert.Single(results.OfType<TestPassed>());
			var passMethodStarting = results.OfType<TestMethodStarting>().Where(ts => ts.TestMethodUniqueID == passResult.TestMethodUniqueID).Single();
			Assert.Equal("Passed", passMethodStarting.MethodName);
		}

		class ClassUnderTest
		{
			[Fact]
			public void Skipped()
			{
				Assert.SkipWhen(true, "This test was skipped");
			}

			[Fact]
			public void Passed()
			{
				Assert.SkipWhen(false, "This test is not skipped");
			}
		}
	}
}
