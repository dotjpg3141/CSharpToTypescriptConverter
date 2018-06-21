using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpToTypescriptConverter
{
	public class MemberInfo
	{
		public string Name { get; set; }
		public Type Type { get; set; }
		public long? Value { get; set; }
		public MemberType MemberType { get; set; }
	}

	public enum MemberType
	{
		Property,
		Field,
		EnumMember,
	}
}
