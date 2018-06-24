using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Schema;
using CSharpToTypescriptConverter.Reflection;

namespace CSharpToTypescriptConverter
{
	public class TypeScriptGenerator
	{
		private static readonly Regex Whitespace = new Regex(@"\s+", RegexOptions.Compiled);
		public const string Number = "number";
		public const string String = "string";
		public const string Any = "any";
		public const string Boolean = "boolean";
		public const string Date = "Date";

		private readonly IndentWriter writer;

		public bool MakeClassesToInterfaces { get; set; }
		public string ClassPrefix { get; set; }
		public string EnumPrefix { get; set; }
		public bool MakeEnumsConst { get; set; }
		public bool Verbose { get; set; }

		public DocumentationProvider DocumentationProvider { get; set; }

		public Dictionary<string, string> CSharpToTypescriptTypes = new Dictionary<string, string>()
		{
			[typeof(bool).FullName] = Boolean,
			[typeof(byte).FullName] = Number,
			[typeof(short).FullName] = Number,
			[typeof(int).FullName] = Number,
			[typeof(long).FullName] = Number,
			[typeof(byte).FullName] = Number,
			[typeof(sbyte).FullName] = Number,
			[typeof(ushort).FullName] = Number,
			[typeof(uint).FullName] = Number,
			[typeof(ulong).FullName] = Number,
			[typeof(float).FullName] = Number,
			[typeof(double).FullName] = Number,
			[typeof(char).FullName] = String,
			[typeof(string).FullName] = String,
			[typeof(object).FullName] = Any,
			[typeof(DateTime).FullName] = Date,
		};

		private ImmutableHashSet<TypeInfo> generatingTypes = ImmutableHashSet<TypeInfo>.Empty;

		public TypeScriptGenerator(TextWriter writer)
		{
			this.DocumentationProvider = new DocumentationProvider();
			this.writer = new IndentWriter(writer);
		}

		public void Generate(IEnumerable<TypeInfo> types)
		{
			this.generatingTypes = types.ToImmutableHashSet();

			foreach (var type in this.generatingTypes.OrderBy(type => type.InheritanceLevel).ThenBy(type => type.Name))
			{
				WriteTypeDeclaration(type);
			}

			this.generatingTypes = ImmutableHashSet<TypeInfo>.Empty;
		}

		private void WriteTypeDeclaration(TypeInfo typeInfo)
		{
			if (this.Verbose)
			{
				this.writer.WriteLine("// " + typeInfo.FullName);
			}

			if (this.DocumentationProvider.TryGetDocumentation(typeInfo, out var documentation))
			{
				this.WriteDocumentation(documentation);
			}

			if (typeInfo is TypeInfo.DynamicType)
			{
				this.writer.BeginLine();
				this.writer.Write("export type ");
				this.WriteTypeName(typeInfo);
				this.writer.Write($" = {Any};");
				this.writer.EndLine();
			}
			else if (typeInfo is TypeInfo.ArrayType arrayType)
			{
				this.writer.BeginLine();
				this.writer.Write("export type ");
				this.WriteTypeName(typeInfo);
				this.writer.Write(" = ");
				this.WriteType(arrayType.ElementType);
				this.writer.Write("[];");
				this.writer.EndLine();
			}
			else
			{
				this.writer.BeginLine();
				this.writer.Write("export ");
				WriteTypeInfoType(typeInfo);
				this.writer.Write(" ");
				WriteType(typeInfo);
				if (typeInfo.BaseType != null)
				{
					this.writer.Write(" extends ");
					this.WriteType(typeInfo.BaseType);
				}
				this.writer.Write(" {");
				this.writer.EndLine();

				this.writer.Indent();
				foreach (var member in typeInfo.Members)
				{
					WriteMemberDeclaration(typeInfo, member);
				}
				this.writer.Unindent();

				this.writer.WriteLine("}");
				this.writer.WriteLine();
			}
		}

		private void WriteDocumentation(DocumentationInfo documentation)
		{
			this.writer.WriteLine("/**");
			if (documentation.Summary != null)
			{
				var info = Whitespace.Replace(documentation.Summary, " ").Trim();
				this.writer.WriteLine("* " + info);
			}
			this.writer.WriteLine("*/");
		}

		private void WriteMemberDeclaration(TypeInfo type, MemberInfo member)
		{
			if (this.DocumentationProvider.TryGetDocumentation(type, member, out var documentation))
			{
				this.WriteDocumentation(documentation);
			}

			this.writer.BeginLine();
			if (member.MemberInfoType == MemberInfoType.EnumMember)
			{
				this.writer.Write(member.Name);
				if (member.Value != null)
				{
					this.writer.Write(" = ");
					this.writer.Write(member.Value);
				}
				this.writer.Write(",");
			}
			else
			{
				this.writer.Write("public ");
				this.writer.Write(member.Name);
				this.writer.Write(": ");
				WriteType(member.Type);
				this.writer.Write(";");
			}
			this.writer.EndLine();
		}

		private void WriteType(TypeInfo typeInfo)
		{
			if (this.generatingTypes.Contains(typeInfo))
			{
				this.WriteTypeName(typeInfo);
			}
			else if (typeInfo is TypeInfo.ArrayType arrayType)
			{
				this.WriteType(arrayType.ElementType);
				this.writer.Write("[]");
			}
			else if (this.CSharpToTypescriptTypes.TryGetValue(typeInfo.FullName, out var typescriptType))
			{
				this.writer.Write(typescriptType);
			}
			else
			{
				//WriteTypeName(typeInfo);
				WriteTypeName(typeInfo);
			}
		}

		private void WriteTypeName(TypeInfo typeInfo)
		{
			if (this.CSharpToTypescriptTypes.TryGetValue(typeInfo.FullName, out var typescriptType))
			{
				this.writer.Write(typescriptType);
			}
			else if (this.generatingTypes.Contains(typeInfo))
			{
				var prefix = typeInfo.Type == TypeInfoType.Enum ? this.EnumPrefix : this.ClassPrefix;
				this.writer.Write(prefix + typeInfo.Name);
			}
			else
			{
				this.writer.Write($"{Any}/*({typeInfo.Name})*/");
			}
		}

		private void WriteTypeInfoType(TypeInfo typeInfo)
		{
			if (typeInfo.Type == TypeInfoType.Enum)
			{
				this.writer.Write(this.MakeEnumsConst ? "const enum" : "enum");
			}
			else
			{
				this.writer.Write(this.MakeClassesToInterfaces ? "interface" : "class");
			}
		}
	}
}
