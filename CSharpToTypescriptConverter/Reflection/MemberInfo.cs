using System;

namespace CSharpToTypescriptConverter.Reflection
{
	[Serializable]
	public class MemberInfo
	{
		public string Name { get; set; }
		public TypeInfo Type { get; set; }
		public object Value { get; set; }
		public MemberInfoType MemberInfoType { get; set; }
	}

	public enum MemberInfoType
	{
		Property,
		Field,
		EnumMember,
	}
}
