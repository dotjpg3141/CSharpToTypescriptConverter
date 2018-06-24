using System.Collections.Generic;
using System.Security.AccessControl;
using CommandLine;

namespace CSharpToTypescriptConverter
{
	class Options
	{
		[Option('i', "input", Required = true, HelpText = "Input directories containing .Net dll files")]
		public IEnumerable<string> InputDirectories { get; set; }

		[Option('o', "output", HelpText = "Ouput file")]
		public string OutputFile { get; set; }

		[Option("member-include-property", HelpText = "Include properties")]
		public bool IncludeProperties { get; set; } = true;
		[Option("member-include-field", HelpText = "Include fields")]
		public bool IncludeFields { get; set; }
		[Option("member-include-internal", HelpText = "Include internal members")]
		public bool IncludeInternal { get; set; }

		[Option("cls-inclusion", HelpText = "Wildcards matching inclusions")]
		public IEnumerable<string> Inclusions { get; set; }
		[Option("cls-exclusion", HelpText = "Wildcards matching exlusions")]
		public IEnumerable<string> Exclusions { get; set; }
		[Option("cls-include-internal", HelpText = "Include internal classes")]
		public bool IncludeInternalTypes { get; set; }
		[Option("cls-include-class", HelpText = "Include classes")]
		public bool IncludeClasses { get; set; } = true;
		[Option("cls-include-struct", HelpText = "Include structs")]
		public bool IncludeStructs { get; set; } = true;
		[Option("cls-include-enum", HelpText = "Include enums")]
		public bool IncludeEnums { get; set; } = true;

		[Option("gen-classes-to-interfaces")]
		public bool MakeClassesToInterfaces { get; set; }
		[Option("gen-class-prefix")]
		public string ClassPrefix { get; set; }
		[Option("gen-enum-prefix")]
		public string EnumPrefix { get; set; }
		[Option("gen-const-enum")]
		public bool MakeEnumsConst { get; set; } = true;
		[Option("gen-verbose")]
		public bool Verbose { get; set; }
		[Option("gen-external-types", HelpText = "External types in the form [C#-Type]=[TypeScriptType]")]
		public IEnumerable<string> ExternalTypes { get; set; }

		[Option("doc-files")]
		public IEnumerable<string> DocumentationFiles { get; set; }
	}
}
