using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CSharpToTypescriptConverter
{
	public class TypeProvider
	{
		private static readonly HashSet<string> ExludedInheritanceTypes = new HashSet<string>()
		{
			"System.Object",
			"System.ValueType",
			"System.Enum",
		};

		public IEnumerable<string> DllFilePaths { get; set; }
		public IEnumerable<Regex> Inclusions { get; set; }
		public IEnumerable<Regex> Exclusions { get; set; }
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

		public IEnumerable<Type> GetTypes()
		{
			return this.DllFilePaths
				.SelectMany(LoadTypesFromDll)
				.Where(type => !type.IsNested)
				.Where(FilterByModifier)
				.Where(FilterByType)
				.Where(FilterByInclusion)
				.Where(FilterByExclusion)
				.SelectMany(GetTypeAndBaseTypes)
				.Distinct();
		}

		internal IEnumerable<Type> LoadTypesFromDll(string dllPath)
		{
			AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve += ReflectionOnlyAssemblyResolve;
			var assembly = Assembly.ReflectionOnlyLoadFrom(dllPath);
			var result = assembly.DefinedTypes.ToList();
			AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve -= ReflectionOnlyAssemblyResolve;
			return result;

			Assembly ReflectionOnlyAssemblyResolve(object sender, ResolveEventArgs args)
			{
				var assemblyName = args.Name;
				var index = assemblyName.IndexOf(',');
				if (index != -1)
				{
					assemblyName = assemblyName.Substring(0, index);
				}

				var assemblyPath = this.DllFilePaths.FirstOrDefault(path =>
				{
					var name = Path.GetFileNameWithoutExtension(path) ?? "";
					return name.Equals(assemblyName, StringComparison.InvariantCultureIgnoreCase);
				});

				if (assemblyPath == null)
				{
					throw new InvalidOperationException($"Cannot resolve assembly '{args.Name}'");
				}

				return Assembly.ReflectionOnlyLoadFrom(assemblyPath);
			}
		}

		internal IEnumerable<Type> GetTypeAndBaseTypes(Type type)
		{
			while (type != null && !ExludedInheritanceTypes.Contains(type.FullName))
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
			var name = type.FullName ?? throw new InvalidOperationException($"Cannot read type name of type {type}");
			return this.Inclusions.Any(rule => rule.IsMatch(name));
		}

		internal bool FilterByExclusion(Type type)
		{
			var name = type.FullName ?? throw new InvalidOperationException($"Cannot read type name of type {type}");
			return this.Exclusions.All(rule => !rule.IsMatch(name));
		}
	}
}
