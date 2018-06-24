using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using CommandLine;
using CSharpToTypescriptConverter.Generator;
using CSharpToTypescriptConverter.Reflection;

namespace CSharpToTypescriptConverter
{
	internal static class Program
	{
		private static void Main(string[] args)
		{
			Parser.Default.ParseArguments<Options>(args).WithParsed(Run);
		}

		private static void Run(Options options)
		{
			var memberProvider = GetMemberProvider(options);
			var typeProvider = GetTypeProvider(options);
			var generator = GetGenerator(options);
			generator.DocumentationProvider = GetDocumentationProvider(options);

			try
			{
				var types = typeProvider.GetTypes(memberProvider);
				generator.Generate(types);
				generator.InnerWriter.Flush();

			}
			catch (Exception e)
			{
				Console.Error.WriteLine($"[ERROR] {e.Message}");
			}
			finally
			{
				if (generator.InnerWriter != null
					&& generator.InnerWriter != Console.Out
					&& generator.InnerWriter != Console.Error)
				{
					generator.InnerWriter.Close();
				}
			}
		}

		private static IMemberProvider GetMemberProvider(Options options)
		{
			var memberProvider = new MemberProvider()
			{
				IncludeInternal = options.IncludeInternal,
				IncludeProperties = options.IncludeProperties,
				IncludeFields = options.IncludeFields,
			};

			return memberProvider;
		}

		private static ITypeProvider GetTypeProvider(Options options)
		{
			var typeProvider = new TypeProvider()
			{
				IncludeInternalTypes = options.IncludeInternalTypes,
				DllFilePaths = options.InputDirectories.ToArray(),
				IncludeEnums = options.IncludeEnums,
				IncludeClasses = options.IncludeClasses,
				IncludeStructs = options.IncludeStructs,
				Exclusions = ParseWildcards(options.Exclusions),
				Inclusions = ParseWildcards(options.Inclusions),
			};

			return typeProvider;

			Regex[] ParseWildcards(IEnumerable<string> input)
			{
				if (input == null)
					input = new string[0];

				return input.Select(item => Wildcard.ToRegex(item)).ToArray();
			}
		}

		private static DocumentationProvider GetDocumentationProvider(Options options)
		{
			var documentationProvider = new DocumentationProvider();
			foreach (var docFile in options.DocumentationFiles)
			{
				documentationProvider.LoadFile(docFile);
			}
			return documentationProvider;
		}

		private static TypeScriptGenerator GetGenerator(Options options)
		{
			var output = string.IsNullOrEmpty(options.OutputFile) ? Console.Out : new StreamWriter(options.OutputFile);
			var generator = new TypeScriptGenerator(output)
			{
				ClassPrefix = options.ClassPrefix,
				EnumPrefix = options.EnumPrefix,
				MakeClassesToInterfaces = options.MakeClassesToInterfaces,
				MakeEnumsConst = options.MakeEnumsConst,
				Verbose = options.Verbose,
			};

			ParseDictionary(generator.CSharpToTypescriptTypes, options.ExternalTypes);

			void ParseDictionary(Dictionary<string, string> dict, IEnumerable<string> input)
			{
				if (input == null)
					input = new string[0];

				foreach (var kvp in input)
				{
					var parts = kvp.Split(new[] { '=' }, 2);
					var key = parts.Length >= 1 ? parts[0] : "???";
					var value = parts.Length >= 2 ? parts[1] : "???";
					dict[key] = value;
				}
			}

			return generator;
		}

		//public const string MockNamespace = "CSharpToTypescriptConverter.TestMock";
		//public const string MockReferenceNamespace = "CSharpToTypescriptConverter.TestMockReference";



		//static void Main(string[] args)
		//{
		//	var input = GetProjectDirectory(MockNamespace);

		//	var typeProvider = new TypeProvider()
		//	{
		//		DllFilePaths = new[] { input },
		//	};
		//	var memberProvider = new MemberProvider();

		//	var documentationProvider = new DocumentationProvider();
		//	documentationProvider.LoadFile(GetProjectDirectory(MockNamespace) + MockNamespace + ".xml");

		//	var types = typeProvider.GetTypes(memberProvider);
		//	var generator = new TypeScriptGenerator(Console.Out)
		//	{
		//		DocumentationProvider = documentationProvider,
		//		Verbose = true,
		//	};
		//	generator.Generate(types);
		//}

		//public static string GetProjectDirectory(string project)
		//{
		//	var debugPath = $@"..\..\..\{project}\bin\Debug\";
		//	var releasePath = $@"..\..\..\{project}\bin\Release\";
		//	return Directory.Exists(debugPath) ? debugPath : releasePath;
		//}
	}
}
