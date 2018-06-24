using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpToTypescriptConverter.Reflection
{
	[Serializable]
	public abstract class TypeInfo
	{
		private static readonly string IEnumerableType = typeof(System.Collections.IEnumerable).FullName;
		private static readonly string GenericIEnumerableType = typeof(IEnumerable<>).FullName;
		private static readonly string DynamicObjectType = typeof(System.Dynamic.DynamicObject).FullName;

		public static readonly ImmutableHashSet<string> ExludedInheritanceTypes = new[]
		{
			"",
			typeof(object).FullName,
			typeof(ValueType).FullName,
			typeof(Enum).FullName,
			DynamicObjectType,
		}.ToImmutableHashSet();

		private IReadOnlyList<MemberInfo> members;

		public string Namespace { get; }
		public string Name { get; }
		public string FullName { get; }
		public TypeInfo BaseType { get; }
		public int InheritanceLevel => (this.BaseType?.InheritanceLevel ?? 0) + 1;

		public TypeInfoType Type { get; }

		public IReadOnlyList<MemberInfo> Members
		{
			get => this.members;
			set
			{
				if (this.members != null)
				{
					throw new InvalidOperationException();
				}
				this.members = value;
			}
		}

		private TypeInfo(Type type)
		{
			this.Namespace = type.Namespace;
			this.Name = type.Name;
			this.FullName = type.FullName;
			this.Type = type.IsEnum ? TypeInfoType.Enum : TypeInfoType.Class;

			var baseTypeName = type.BaseType?.FullName ?? "";
			this.BaseType = ExludedInheritanceTypes.Contains(baseTypeName) ? null : FromType(type.BaseType);
		}

		public static TypeInfo FromType(Type type)
		{
			var interfaces = type.GetInterfaces();

			if (type.GetTypeAndBaseTypes().Any(t => t.FullName == DynamicObjectType))
			{
				return new DynamicType(type);
			}
			else if (interfaces.FirstOrDefault(itf => itf.IsGenericType
			  && itf.GetGenericTypeDefinition().FullName == GenericIEnumerableType) is Type arrayType)
			{
				var parameter = arrayType.GetGenericArguments().Single();
				return new ArrayType(type, FromType(parameter));
			}
			else if (type.GetInterfaces().Any(@interface => @interface.FullName == IEnumerableType))
			{
				return new ArrayType(type, FromType(typeof(object)));
			}
			return new SimpleType(type);
		}

		[Serializable]
		public sealed class SimpleType : TypeInfo
		{
			internal SimpleType(Type type)
				: base(type)
			{
			}
		}

		[Serializable]
		public sealed class ArrayType : TypeInfo
		{
			public TypeInfo ElementType { get; }

			internal ArrayType(Type type, TypeInfo elementType)
				: base(type)
			{
				this.ElementType = elementType;
			}
		}

		[Serializable]
		public sealed class DynamicType : TypeInfo
		{
			internal DynamicType(Type type)
				: base(type)
			{
			}
		}
	}
}
