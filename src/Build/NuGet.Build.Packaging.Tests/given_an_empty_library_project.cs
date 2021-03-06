﻿using System.Linq;
using Microsoft.Build.Execution;
using Xunit;
using Xunit.Abstractions;

namespace NuGet.Build.Packaging
{
	public class given_an_empty_library_project
	{
		ITestOutputHelper output;

		public given_an_empty_library_project(ITestOutputHelper output)
		{
			this.output = output;
		}

		[Fact]
		public void when_getting_package_contents_then_includes_output_assembly()
		{
			var result = Builder.BuildScenario(nameof(given_an_empty_library_project));

			result.AssertSuccess(output);
			Assert.Contains(result.Items, item => item.Matches(new
			{
				Extension = ".dll",
				Kind = "Lib"
			}));
		}

		[Fact]
		public void when_include_outputs_in_package_is_false_then_does_not_include_main_assembly()
		{
			var result = Builder.BuildScenario(nameof(given_an_empty_library_project), new
			{
				IncludeOutputsInPackage = false,
			});

			result.AssertSuccess(output);
			Assert.DoesNotContain(result.Items, item => item.Matches(new
			{
				Extension = ".dll",
				Kind = "Lib"
			}));
		}

		[Fact]
		public void when_getting_package_contents_then_includes_symbols()
		{
			var result = Builder.BuildScenario(nameof(given_an_empty_library_project), new { Configuration = "Debug" });

			result.AssertSuccess(output);
			Assert.Contains(result.Items, item => item.Matches(new
			{
				Extension = ".pdb",
			}));
		}

		[Fact]
		public void when_include_symbols_in_package_is_true_but_include_outputs_is_false_then_does_not_include_symbols()
		{
			var result = Builder.BuildScenario(nameof(given_an_empty_library_project), new
			{
				IncludeOutputsInPackage = false,
				IncludeSymbolsInPackage = true,
			});

			result.AssertSuccess(output);
			Assert.DoesNotContain(result.Items, item => item.Matches(new
			{
				Extension = ".pdb",
			}));
		}

		[Fact]
		public void when_include_symbols_in_package_is_false_then_does_not_include_symbols()
		{
			var result = Builder.BuildScenario(nameof(given_an_empty_library_project), new
			{
				IncludeSymbolsInPackage = false,
			});

			result.AssertSuccess(output);
			Assert.DoesNotContain(result.Items, item => item.Matches(new
			{
				Extension = ".pdb",
			}));
		}

		[Fact]
		public void when_getting_package_contents_then_includes_xmldoc()
		{
			var result = Builder.BuildScenario(nameof(given_an_empty_library_project));

			Assert.Equal(TargetResultCode.Success, result.ResultCode);
			Assert.Contains(result.Items, item => item.Matches(new
			{
				Extension = ".xml",
				Kind = PackageItemKind.Lib,
			}));
		}

		[Fact]
		public void when_include_output_in_package_is_false_then_does_not_include_xmldoc()
		{
			var result = Builder.BuildScenario(nameof(given_an_empty_library_project), new
			{
				IncludeOutputsInPackage = false,
			});

			result.AssertSuccess(output);
			Assert.DoesNotContain(result.Items, item => item.Matches(new
			{
				Extension = ".xml",
				Kind = PackageItemKind.Lib,
			}));
		}

		[Fact]
		public void when_getting_package_contents_then_annotates_items_with_package_id()
		{
			var result = Builder.BuildScenario(nameof(given_an_empty_library_project), new { PackageId = "Foo" }, output: output);

			result.AssertSuccess(output);
			Assert.All(result.Items, item => Assert.Equal("Foo", item.GetMetadata("PackageId")));
		}

		[Fact]
		public void when_getting_package_contents_then_includes_framework_reference()
		{
			var result = Builder.BuildScenario(nameof(given_an_empty_library_project), new { PackageId = "Foo" }, output: output);

			result.AssertSuccess(output);
			Assert.Contains(result.Items, item => item.Matches(new
			{
				Identity = "System.Core",
				Kind = PackageItemKind.FrameworkReference,
			}));
		}

		[Fact]
		public void when_include_framework_references_in_package_is_false_then_does_not_include_framework_reference()
		{
			var result = Builder.BuildScenario(nameof(given_an_empty_library_project), new
			{
				IncludeFrameworkReferencesInPackage = false,
				PackageId = "Foo",
			}, output: output);

			result.AssertSuccess(output);
			Assert.DoesNotContain(result.Items, item => item.Matches(new
			{
				Identity = "System.Core",
				Kind = PackageItemKind.FrameworkReference,
			}));
		}

		[Fact]
		public void when_packing_with_empty_version_then_build_fails()
		{
			var result = Builder.BuildScenario(nameof(given_an_empty_library_project), new
			{
				PackageId = "Library",
				PackageVersion = "",
				Version = "",
			}, target: "Pack");

			Assert.Equal(TargetResultCode.Failure, result.ResultCode);
			Assert.Equal("NG1002", result.Logger.Errors[0].Code);
		}
	}
}
