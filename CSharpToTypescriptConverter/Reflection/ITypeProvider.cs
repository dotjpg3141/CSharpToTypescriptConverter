using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSharpToTypescriptConverter.Reflection;

namespace CSharpToTypescriptConverter
{
	public interface ITypeProvider
	{
		IEnumerable<TypeInfo> GetTypes(IMemberProvider memberProvider);
	}
}
