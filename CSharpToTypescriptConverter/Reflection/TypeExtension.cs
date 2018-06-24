using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpToTypescriptConverter.Reflection
{
	public static class TypeExtension
	{
		public static IEnumerable<Type> GetTypeAndBaseTypes(this Type type)
		{
			while (type != null)
			{
				yield return type;
				type = type.BaseType;
			}
		}
	}
}
