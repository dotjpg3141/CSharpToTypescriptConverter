using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static CSharpToTypescriptConverter.Test.FileHelper;

namespace CSharpToTypescriptConverter.Test
{
	[TestClass]
	public class TypeProviderTest
	{
		private List<Type> LoadedTypes { get; set; }

		private Type PublicClass => this.LoadedTypes.Single(t => t.FullName == MockNamespace + ".PublicClass");
		private Type PublicClass2 => this.LoadedTypes.Single(t => t.FullName == MockNamespace + ".PublicClass2");
		private Type InternalClass => this.LoadedTypes.Single(t => t.FullName == MockNamespace + ".InternalClass");
		private Type PublicEnum => this.LoadedTypes.Single(t => t.FullName == MockNamespace + ".PublicEnum");

		private Type InheritedClass => this.LoadedTypes.Single(t => t.FullName == MockNamespace + ".InheritedClass");
		private Type BaseClass => this.LoadedTypes.Single(t => t.FullName == MockNamespace + ".BaseClass");
		private Type InheritedExternalClass => this.LoadedTypes.Single(t => t.FullName == MockNamespace + ".InheritedExternalClass");
		private Type ExternalBaseClass => this.LoadedTypes.Single(t => t.FullName == MockReferenceNamespace + ".ExternalBaseClass");

		[TestInitialize]
		public void TestInitialize()
		{
			var dllName = GetProjectFile(MockNamespace, "dll");
			var reference = GetProjectFile(MockReferenceNamespace, "dll");

			var provider = new TypeProvider()
			{
				DllFilePaths = new[] { dllName, reference },
			};

			this.LoadedTypes = provider.LoadTypesFromDll(dllName)
				.SelectMany(provider.GetTypeAndBaseTypes)
				.Distinct()
				.ToList();
		}

		[TestMethod]
		public void LoadTypesFromDllTest()
		{
			Assert.IsTrue(this.LoadedTypes.Count == 8);

			Assert.IsNotNull(this.PublicClass);
			Assert.IsNotNull(this.PublicClass2);
			Assert.IsNotNull(this.InternalClass);
			Assert.IsNotNull(this.PublicEnum);

			Assert.IsNotNull(this.InheritedClass);
			Assert.IsNotNull(this.BaseClass);
			Assert.IsNotNull(this.InheritedExternalClass);
			Assert.IsNotNull(this.ExternalBaseClass);
		}

		[TestMethod]
		public void FilterByModifierPublicTest()
		{
			var provider = new TypeProvider
			{
				IncludeInternalTypes = false
			};

			Assert.IsTrue(provider.FilterByModifier(this.PublicClass));
			Assert.IsFalse(provider.FilterByModifier(this.InternalClass));
		}

		[TestMethod]
		public void FilterByModifierInternalTest()
		{
			var provider = new TypeProvider
			{
				IncludeInternalTypes = true
			};

			Assert.IsTrue(provider.FilterByModifier(this.PublicClass));
			Assert.IsTrue(provider.FilterByModifier(this.InternalClass));
		}

		[TestMethod]
		public void FilterByTypeNoEnumsNoClassTest()
		{
			var provider = new TypeProvider()
			{
				IncludeClasses = false,
				IncludeEnums = false,
			};

			Assert.IsFalse(provider.FilterByType(this.PublicClass));
			Assert.IsFalse(provider.FilterByType(this.PublicEnum));
		}

		[TestMethod]
		public void FilterByTypeWithEnumsWithClassTest()
		{
			var provider = new TypeProvider()
			{
				IncludeClasses = true,
				IncludeEnums = true,
			};

			Assert.IsTrue(provider.FilterByType(this.PublicClass));
			Assert.IsTrue(provider.FilterByType(this.PublicEnum));
		}

		[TestMethod]
		public void FilterByInclusionTest()
		{
			var provider = new TypeProvider()
			{
				Inclusions = new[] { new Regex("Class2") },
			};

			Assert.IsFalse(provider.FilterByInclusion(this.PublicClass));
			Assert.IsTrue(provider.FilterByInclusion(this.PublicClass2));
		}

		[TestMethod]
		public void FilterByExclusionTest()
		{
			var provider = new TypeProvider()
			{
				Exclusions = new[] { new Regex("Class2") },
			};

			Assert.IsTrue(provider.FilterByExclusion(this.PublicClass));
			Assert.IsFalse(provider.FilterByExclusion(this.PublicClass2));
		}
	}
}