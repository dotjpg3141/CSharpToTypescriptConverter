using System.Collections.Generic;

namespace CSharpToTypescriptConverter.Reflection
{
	public interface ITypeProvider
	{
		IEnumerable<TypeInfo> GetTypes(IMemberProvider memberProvider);
	}
}
