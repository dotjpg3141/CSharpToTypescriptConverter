using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpToTypescriptConverter.Reflection
{
	[Serializable]
	public class TypeInfo
	{
		public static readonly ImmutableHashSet<string> ExludedInheritanceTypes = new[]
		{
			"",
			"System.Object",
			"System.ValueType",
			"System.Enum",
		}.ToImmutableHashSet();

		public string Namespace { get; }
		public string Name { get; }
		public string FullName { get; }
		public TypeInfo BaseType { get; }
		public int InheritanceLevel => (this.BaseType?.InheritanceLevel ?? 0) + 1;

		public TypeInfoType Type { get; }
		public IEnumerable<MemberInfo> Members { get; internal set; }

		public TypeInfo(Type type)
		{
			this.Namespace = type.Namespace;
			this.Name = type.Name;
			this.FullName = type.FullName;
			this.Type = type.IsEnum ? TypeInfoType.Enum : TypeInfoType.Class;

			var baseTypeName = type.BaseType?.FullName ?? "";
			this.BaseType = ExludedInheritanceTypes.Contains(baseTypeName) ? null : new TypeInfo(type.BaseType);
		}
	}
}
