using System;
using System.Collections.Generic;
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
		public const string Number = "Number";
		public const string String = "String";
		public const string Any = "any";

		private readonly IndentWriter writer;

		public bool MakeClassesToInterfaces { get; set; }
		public string ClassPrefix { get; set; }
		public string EnumPrefix { get; set; }
		public bool MakeEnumsConst { get; set; }

		public DocumentationProvider DocumentationProvider { get; set; }

		public Dictionary<string, string> CSharpToTypescriptTypes = new Dictionary<string, string>()
		{
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
		};

		public TypeScriptGenerator(TextWriter writer)
		{
			this.DocumentationProvider = new DocumentationProvider();
			this.writer = new IndentWriter(writer);
		}

		public void Generate(IEnumerable<TypeInfo> types)
		{
			foreach (var type in types.OrderBy(type => type.InheritanceLevel).ThenBy(type => type.Name))
			{
				WriteTypeDeclaration(type);
			}
		}

		public void WriteTypeDeclaration(TypeInfo typeInfo)
		{
			if (this.DocumentationProvider.TryGetDocumentation(typeInfo, out var documentation))
			{
				this.WriteDocumentation(documentation);
			}

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
			if (typeInfo is TypeInfo.ArrayType arrayType)
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
				var prefix = typeInfo.Type == TypeInfoType.Enum ? this.EnumPrefix : this.ClassPrefix;
				this.writer.Write(prefix + typeInfo.Name);
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
