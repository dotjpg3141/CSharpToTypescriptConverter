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
using static CSharpToTypescriptConverter.Test.GeneratorHelper;

namespace CSharpToTypescriptConverter.Test
{
	[TestClass]
	public class TypeScriptGeneratorClassDeclarationTest
	{
		[TestMethod]
		public void ClassDeclarationTest()
		{
			var type = TypeInfo.FromType(typeof(Class1));
			if (type.Members == null)
				type.Members = new MemberInfo[0];

			AssertGenerator(new[] { type }, "export class Class1 {}");
		}

		[TestMethod]
		public void MakeClassToInterfaceDeclarationTest()
		{
			var type = TypeInfo.FromType(typeof(Class1));
			if (type.Members == null)
				type.Members = new MemberInfo[0];

			AssertGenerator(new[] { type }, "export interface Class1 {}", options => options.MakeClassesToInterfaces = true);
		}

		[TestMethod]
		public void InterfaceDeclarationTest()
		{
			var type = TypeInfo.FromType(typeof(Class1));
			if (type.Members == null)
				type.Members = new MemberInfo[0];

			AssertGenerator(new[] { type }, "export class IClass1 {}", options => options.ClassPrefix = "I");
		}

		[TestMethod]
		public void EnumDeclarationTest()
		{
			var type = TypeInfo.FromType(typeof(Enum1));
			if (type.Members == null)
				type.Members = new MemberInfo[0];

			AssertGenerator(new[] { type }, "export enum Enum1 {}");
		}

		[TestMethod]
		public void ConstEnumDeclarationTest()
		{
			var type = TypeInfo.FromType(typeof(Enum1));
			if (type.Members == null)
				type.Members = new MemberInfo[0];

			AssertGenerator(new[] { type }, "export const enum Enum1 {}", options => options.MakeEnumsConst = true);
		}

		private class Class1
		{
		}

		private enum Enum1
		{
		}
	}
}
