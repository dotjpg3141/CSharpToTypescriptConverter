using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CSharpToTypescriptConverter.Reflection
{
	[Serializable]
	public class MemberProvider : IMemberProvider
	{
		public bool IncludeProperties { get; set; }
		public bool IncludeFields { get; set; }
		public bool IncludeInternal { get; set; }

		public MemberProvider()
		{
			this.IncludeProperties = true;
			this.IncludeFields = false;
			this.IncludeInternal = false;
		}

		public IEnumerable<MemberInfo> GetMembers(Type type)
		{
			return type.IsEnum
				? GetEnumMembers(type)
				: GetClassMembers(type);
		}

		private IEnumerable<MemberInfo> GetClassMembers(Type type)
		{
			var result = new List<MemberInfo>();

			var bindingFlags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic;

			if (this.IncludeProperties)
			{
				result.AddRange(type
					.GetProperties(bindingFlags)
					.Where(FilterPropertyModifier)
					.Select(property => new MemberInfo()
					{
						MemberInfoType = MemberInfoType.Property,
						Type = property.PropertyType,
						Name = property.Name,
					}));
			}

			if (this.IncludeFields)
			{
				result.AddRange(type
					.GetFields(bindingFlags)
					.Where(FilterFieldModifier)
					.Select(property => new MemberInfo()
					{
						MemberInfoType = MemberInfoType.Field,
						Type = property.FieldType,
						Name = property.Name,
					}));
			}

			return result;
		}

		private IEnumerable<MemberInfo> GetEnumMembers(Type type)
		{
			return type.GetFields()
				.Where(f => f.FieldType.IsEnum)
				.Select(f => new MemberInfo()
				{
					MemberInfoType = MemberInfoType.EnumMember,
					Name = f.Name,
					Value = Convert.ToInt64(f.GetRawConstantValue()),
				})
				.ToList();
		}

		private bool FilterPropertyModifier(PropertyInfo property)
		{
			if (property.GetMethod == null)
				return false;

			return property.GetMethod.IsPublic ||
				(property.GetMethod.IsAssembly && this.IncludeInternal);
		}

		private bool FilterFieldModifier(FieldInfo field)
		{
			return field.IsPublic ||
				   (field.IsAssembly && this.IncludeInternal);
		}
	}
}
