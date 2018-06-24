using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSharpToTypescriptConverter.Reflection;

namespace CSharpToTypescriptConverter
{
	class Program
	{
		public const string MockNamespace = "CSharpToTypescriptConverter.TestMock";
		public const string MockReferenceNamespace = "CSharpToTypescriptConverter.TestMockReference";

		static void Main(string[] args)
		{
			var input = GetProjectDirectory(MockNamespace);

			var typeProvider = new TypeProvider()
			{
				DllFilePaths = new[] { input },
			};
			var memberProvider = new MemberProvider();

			var documentationProvider = new DocumentationProvider();
			documentationProvider.LoadFile(GetProjectDirectory(MockNamespace) + MockNamespace + ".xml");

			var types = typeProvider.GetTypes(memberProvider);
			var generator = new TypeScriptGenerator(Console.Out)
			{
				DocumentationProvider = documentationProvider,
				Verbose = true,
			};
			generator.Generate(types);
		}

		public static string GetProjectDirectory(string project)
		{
			var debugPath = $@"..\..\..\{project}\bin\Debug\";
			var releasePath = $@"..\..\..\{project}\bin\Release\";
			return Directory.Exists(debugPath) ? debugPath : releasePath;
		}
	}
}
