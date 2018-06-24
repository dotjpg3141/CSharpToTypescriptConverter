using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using CSharpToTypescriptConverter.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CSharpToTypescriptConverter.Test
{
	[TestClass]
	public class TypeProviderTest
	{
		private static readonly (string, string) PublicClass = (FileHelper.MockNamespace, "PublicClass");
		private static readonly (string, string) PublicClass2 = (FileHelper.MockNamespace, "PublicClass2");
		private static readonly (string, string) InternalClass = (FileHelper.MockNamespace, "InternalClass");
		private static readonly (string, string) PublicEnum = (FileHelper.MockNamespace, "PublicClass");
		private static readonly (string, string) InheritedClass = (FileHelper.MockNamespace, "InheritedClass");
		private static readonly (string, string) BaseClass = (FileHelper.MockNamespace, "BaseClass");
		private static readonly (string, string) InheritedExternalClass = (FileHelper.MockNamespace, "InheritedExternalClass");
		private static readonly (string, string) ExternalBaseClass = (FileHelper.MockReferenceNamespace, "ExternalBaseClass");
		private static readonly (string, string) DynamicClass = (FileHelper.MockNamespace, "DynamicClass");

		[TestMethod]
		public void LoadTypesFromDllTest()
		{
			var types = GetTypeFromTypeProvider();

			Assert.AreEqual(9, types.Length);

			Assert.IsNotNull(types.TryFindType(PublicClass));
			Assert.IsNotNull(types.TryFindType(PublicClass2));
			Assert.IsNotNull(types.TryFindType(InternalClass));
			Assert.IsNotNull(types.TryFindType(PublicEnum));
			Assert.IsNotNull(types.TryFindType(InheritedClass));
			Assert.IsNotNull(types.TryFindType(BaseClass));
			Assert.IsNotNull(types.TryFindType(InheritedExternalClass));
			Assert.IsNotNull(types.TryFindType(ExternalBaseClass));
			Assert.IsNotNull(types.TryFindType(DynamicClass));
		}

		[TestMethod]
		public void FilterByModifierPublicTest()
		{
			var types = GetTypeFromTypeProvider(options => options.IncludeInternalTypes = false);

			Assert.IsNotNull(types.TryFindType(PublicClass));
			Assert.IsNull(types.TryFindType(InternalClass));
		}

		[TestMethod]
		public void FilterByModifierInternalTest()
		{
			var types = GetTypeFromTypeProvider(options => options.IncludeInternalTypes = true);

			Assert.IsNotNull(types.TryFindType(PublicClass));
			Assert.IsNotNull(types.TryFindType(InternalClass));
		}

		[TestMethod]
		public void FilterByTypeNoEnumsNoClassTest()
		{
			var types = GetTypeFromTypeProvider(options =>
			{
				options.IncludeClasses = false;
				options.IncludeEnums = false;
			});

			Assert.IsNull(types.TryFindType(PublicClass));
			Assert.IsNull(types.TryFindType(PublicEnum));
		}

		[TestMethod]
		public void FilterByTypeWithEnumsWithClassTest()
		{
			var types = GetTypeFromTypeProvider(options =>
			{
				options.IncludeClasses = true;
				options.IncludeEnums = true;
			});

			Assert.IsNotNull(types.TryFindType(PublicClass));
			Assert.IsNotNull(types.TryFindType(PublicEnum));
		}

		[TestMethod]
		public void FilterByInclusionTest()
		{
			var types = GetTypeFromTypeProvider(options =>
			{
				options.Inclusions = new[] { new Regex("Class2") };
			});

			Assert.IsNull(types.TryFindType(PublicClass));
			Assert.IsNotNull(types.TryFindType(PublicClass2));
		}

		[TestMethod]
		public void FilterByExclusionTest()
		{
			var types = GetTypeFromTypeProvider(options =>
			{
				options.Exclusions = new[] { new Regex("Class2") };
			});

			Assert.IsNotNull(types.TryFindType(PublicClass));
			Assert.IsNull(types.TryFindType(PublicClass2));
		}

		private static TypeInfo[] GetTypeFromTypeProvider(Action<TypeProvider> options = null)
		{
			var dllName = FileHelper.GetProjectFile(FileHelper.MockNamespace, "dll");

			var provider = new TypeProvider()
			{
				DllFilePaths = new[]
				{
					Directory.GetParent(dllName).FullName,
				},
				IncludeInternalTypes = true,
				IncludeEnums = true,
				IncludeClasses = true,
				IncludeStructs = true,
			};

			options?.Invoke(provider);
			return provider.GetTypes(new MemberProviderMock()).ToArray();
		}

		[Serializable]
		private class MemberProviderMock : IMemberProvider
		{
			public IEnumerable<MemberInfo> GetMembers(Type type)
			{
				return new MemberInfo[0];
			}
		}
	}

	internal static class TypeProviderTestExtension
	{
		public static TypeInfo TryFindType(this IEnumerable<TypeInfo> types, (string @namespace, string name) info)
			=> types.SingleOrDefault(t => t.Namespace == info.@namespace && t.Name == info.name);
	}
}