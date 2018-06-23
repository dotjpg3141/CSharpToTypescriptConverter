using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CSharpToTypescriptConverter.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CSharpToTypescriptConverter.Test
{
	[TestClass]
	public class TypeScriptGeneratorClassDeclarationTest
	{
		[TestMethod]
		public void ClassDeclarationTest()
		{
			var types = new[] { new TypeInfo(typeof(Class1)) { Members = new MemberInfo[0] } };
			AssertGenerator(types, "export class Class1 {}");
		}

		[TestMethod]
		public void MakeClassToInterfaceDeclarationTest()
		{
			var types = new[] { new TypeInfo(typeof(Class1)) { Members = new MemberInfo[0] } };
			AssertGenerator(types, "export interface Class1 {}", options => options.MakeClassesToInterfaces = true);
		}

		[TestMethod]
		public void InterfaceDeclarationTest()
		{
			var types = new[] { new TypeInfo(typeof(Class1)) { Members = new MemberInfo[0] } };
			AssertGenerator(types, "export class IClass1 {}", options => options.ClassPrefix = "I");
		}

		[TestMethod]
		public void EnumDeclarationTest()
		{
			var types = new[] { new TypeInfo(typeof(Enum1)) { Members = new MemberInfo[0] } };
			AssertGenerator(types, "export enum Enum1 {}");
		}

		[TestMethod]
		public void ConstEnumDeclarationTest()
		{
			var types = new[] { new TypeInfo(typeof(Enum1)) { Members = new MemberInfo[0] } };
			AssertGenerator(types, "export const enum Enum1 {}", options => options.MakeEnumsConst = true);
		}

		private static void AssertGenerator(TypeInfo[] input, string expectedOuput, Action<TypeScriptGenerator> options = null)
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

		private class Class1
		{
		}

		private enum Enum1
		{
		}
	}
}
