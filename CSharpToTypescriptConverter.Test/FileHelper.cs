using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CSharpToTypescriptConverter.Test
{
	internal static class FileHelper
	{
		public const string MockNamespace = "CSharpToTypescriptConverter.TestMock";
		public const string MockReferenceNamespace = "CSharpToTypescriptConverter.TestMockReference";

		public static string GetProjectFile(string project, string extension)
		{
			var debugPath = $@"..\..\..\{project}\bin\Debug\{project}.{extension}";
			var releasePath = $@"..\..\..\{project}\bin\Release\{project}.{extension}";

			return File.Exists(debugPath) ? debugPath :
				   File.Exists(releasePath) ? releasePath :
				   throw new AssertInconclusiveException(
						$"Cannot find {extension} neither at '{Path.GetFullPath(debugPath)}' nor at '{Path.GetFullPath(releasePath)}'");
		}
	}
}
