using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.CompilerServices;
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

		private static readonly Dictionary<Type, TypeInfo> TypeInfoCache = new Dictionary<Type, TypeInfo>();

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
			this.FullName = type.FullName ?? "";
			this.Type = type.IsEnum ? TypeInfoType.Enum : TypeInfoType.Class;

			var baseTypeName = type.BaseType?.FullName ?? "";
			this.BaseType = ExludedInheritanceTypes.Contains(baseTypeName) ? null : FromType(type.BaseType);
		}

		public static TypeInfo FromType(Type type)
		{
			if (TypeInfoCache.TryGetValue(type, out var typeInfo))
			{
				return typeInfo;
			}

			var interfacesNames = new Dictionary<string, Type>();
			foreach (var itf in type.GetInterfaces())
			{
				var fullName = itf.IsGenericType ? itf.GetGenericTypeDefinition().FullName : itf.FullName;
				interfacesNames[fullName ?? ""] = itf;
			}

			if (type.GetTypeAndBaseTypes().Any(t => t.FullName == DynamicObjectType))
			{
				var dynamicTypeInfo = new DynamicType(type);
				TypeInfoCache[type] = dynamicTypeInfo;
				return dynamicTypeInfo;
			}
			else if (interfacesNames.TryGetValue(GenericIEnumerableType, out var arrayType))
			{
				var parameter = arrayType.GetGenericArguments().Single();

				var arrayTypeInfo = new ArrayType(type);
				TypeInfoCache[type] = arrayTypeInfo;
				arrayTypeInfo.ElementType = FromType(parameter);
				return arrayTypeInfo;
			}
			else if (interfacesNames.ContainsKey(IEnumerableType))
			{
				var arrayTypeInfo = new ArrayType(type);
				TypeInfoCache[type] = arrayTypeInfo;
				arrayTypeInfo.ElementType = FromType(typeof(object));
				return arrayTypeInfo;
			}
			else
			{
				var simpleTypeInfo = new SimpleType(type);
				TypeInfoCache[type] = simpleTypeInfo;
				return simpleTypeInfo;
			}
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
			private TypeInfo elementType;

			public TypeInfo ElementType
			{
				get => this.elementType;
				set
				{
					if (this.elementType != null)
					{
						throw new InvalidOperationException();
					}
					this.elementType = value;
				}
			}

			internal ArrayType(Type type)
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
