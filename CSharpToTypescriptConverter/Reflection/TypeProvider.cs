using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Security.Permissions;
using System.Text.RegularExpressions;

namespace CSharpToTypescriptConverter.Reflection
{
	[Serializable]
	public class TypeProvider : ITypeProvider
	{
		public string[] DllFilePaths { get; set; }
		public Regex[] Inclusions { get; set; }
		public Regex[] Exclusions { get; set; }
		public bool IncludeInternalTypes { get; set; }
		public bool IncludeClasses { get; set; }
		public bool IncludeStructs { get; set; }
		public bool IncludeEnums { get; set; }

		public TypeProvider()
		{
			this.DllFilePaths = Array.Empty<string>();
			this.Inclusions = Array.Empty<Regex>();
			this.Exclusions = Array.Empty<Regex>();
			this.IncludeInternalTypes = false;
			this.IncludeClasses = true;
			this.IncludeEnums = true;
		}

		public IEnumerable<TypeInfo> GetTypes(IMemberProvider memberProvider)
		{
			var files = this.DllFilePaths.SelectMany(dir => Directory.GetFiles(dir, "*.dll")).ToArray();

			var appDomain = GetTempAppDomain();
			var type = typeof(TypeInspector);
			try
			{
				var instance = (TypeInspector)appDomain.CreateInstanceAndUnwrap(type.Assembly.FullName, type.FullName);
				var result = instance.LoadTypeInfos(files, this, memberProvider);
				var errorMessage = instance.NotLoadableAssemblies; // TODO(jpg)
				return result;
			}
			finally
			{
				AppDomain.Unload(appDomain);
			}
		}

		private Type[] SelectTypes(IEnumerable<Type> source)
		{
			return source
				.Where(FilterByModifier)
				.Where(FilterByType)
				.Where(FilterByInclusion)
				.Where(FilterByExclusion)
				.SelectMany(GetTypeAndBaseTypes)
				.Distinct()
				.ToArray();
		}

		internal IEnumerable<Type> GetTypeAndBaseTypes(Type type)
		{
			while (type != null && !TypeInfo.ExludedInheritanceTypes.Contains(type.FullName))
			{
				yield return type;
				type = type.BaseType;
			}
		}

		internal bool FilterByModifier(Type type)
		{
			var isPublic = type.IsPublic;
			var isInternal = type.IsNotPublic;

			return isPublic || (isInternal && this.IncludeInternalTypes);
		}

		internal bool FilterByType(Type type)
		{
			var isStruct = type.IsValueType && !type.IsEnum;

			return (type.IsClass && this.IncludeClasses)
				   || (type.IsEnum && this.IncludeEnums)
				   || (isStruct && this.IncludeStructs);
		}

		internal bool FilterByInclusion(Type type)
		{
			if (this.Inclusions.Length == 0)
				return true;

			var name = type.FullName ?? throw new InvalidOperationException($"Cannot read type name of type {type}");
			return this.Inclusions.Any(rule => rule.IsMatch(name));
		}

		internal bool FilterByExclusion(Type type)
		{
			if (this.Exclusions.Length == 0)
				return true;

			var name = type.FullName ?? throw new InvalidOperationException($"Cannot read type name of type {type}");
			return this.Exclusions.All(rule => !rule.IsMatch(name));
		}

		private static AppDomain GetTempAppDomain()
		{
			var appName = typeof(TypeInspector) + "-AppDomain";
			var domainSetup = new AppDomainSetup()
			{
				ApplicationName = appName,
				ShadowCopyFiles = "false",
				ApplicationBase = AppDomain.CurrentDomain.SetupInformation.ApplicationBase,
				ConfigurationFile = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile,
				DynamicBase = AppDomain.CurrentDomain.SetupInformation.DynamicBase,
				LicenseFile = AppDomain.CurrentDomain.SetupInformation.LicenseFile,
				LoaderOptimization = AppDomain.CurrentDomain.SetupInformation.LoaderOptimization,
				PrivateBinPath = AppDomain.CurrentDomain.SetupInformation.PrivateBinPath,
				PrivateBinPathProbe = AppDomain.CurrentDomain.SetupInformation.PrivateBinPathProbe
			};

			var evidence = AppDomain.CurrentDomain.Evidence;
			return AppDomain.CreateDomain(appName, evidence, domainSetup, new PermissionSet(PermissionState.Unrestricted));
		}

		private class TypeInspector : MarshalByRefObject
		{
			public List<AssemblyName> NotLoadableAssemblies { get; } = new List<AssemblyName>();

			public IEnumerable<TypeInfo> LoadTypeInfos(string[] dllPath, TypeProvider typeProvider, IMemberProvider memberProvider)
			{
				this.NotLoadableAssemblies.Add(typeof(object).Assembly.GetName());

				var types = typeProvider.SelectTypes(LoadTypes(dllPath));
				var typeInfos = new List<TypeInfo>();

				foreach (var type in types)
				{
					var memberInfos = memberProvider.GetMembers(type);
					var typeInfo = TypeInfo.FromType(type);
					typeInfo.Members = memberInfos.ToArray();
					typeInfos.Add(typeInfo);
				}
				return typeInfos;
			}

			private IEnumerable<Type> LoadTypes(string[] dllPath)
			{
				AppDomain.CurrentDomain.AssemblyResolve += (sender, e) =>
				{
					var assembly = Assembly.Load(e.Name);
					return assembly ?? throw new TypeLoadException($"Could not load assembly '{e.Name}'");
				};

				var assemblies = dllPath.Select(Assembly.LoadFrom).ToList();
				//var assembliesWithRefernces = assemblies.Where(TryLoadReferences).ToList();
				//var types = assembliesWithRefernces.SelectMany(type => type.DefinedTypes).ToList();
				var types = assemblies.SelectMany(type => type.DefinedTypes).ToList();
				return types;
			}

			private bool TryLoadReferences(Assembly assembly)
			{
				foreach (var referencedAssemblyName in assembly.GetReferencedAssemblies())
				{
					try
					{
						var reference = Assembly.Load(referencedAssemblyName.FullName);
						if (!TryLoadReferences(reference))
						{
							return false;
						}
					}
					catch (FileNotFoundException)
					{
						this.NotLoadableAssemblies.Add(referencedAssemblyName);
						return false;
					}
				}
				return true;
			}
		}
	}
}
