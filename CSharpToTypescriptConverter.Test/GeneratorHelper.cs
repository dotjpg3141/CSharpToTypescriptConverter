using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CSharpToTypescriptConverter.Generator;
using CSharpToTypescriptConverter.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CSharpToTypescriptConverter.Test
{
	internal static class GeneratorHelper
	{
		public static void AssertGenerator(TypeInfo[] input, string expectedOuput, Action<TypeScriptGenerator> options = null)
		{
			string Normalize(string value)
			{
				var whitespace = new Regex(@"\s+");
				return whitespace.Replace(value, "");
			}

			var writer = new StringWriter();
			var generator = new TypeScriptGenerator(writer);
			options?.Invoke(generator);
			generator.Generate(input);

			var actual = Normalize(writer.ToString());
			var expected = Normalize(expectedOuput);
			Assert.AreEqual(expected, actual);
		}
	}
}
