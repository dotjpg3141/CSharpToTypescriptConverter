using System;
using System.Collections.Generic;

namespace CSharpToTypescriptConverter.Reflection
{
	public interface IMemberProvider
	{
		IEnumerable<MemberInfo> GetMembers(Type type);
	}
}
