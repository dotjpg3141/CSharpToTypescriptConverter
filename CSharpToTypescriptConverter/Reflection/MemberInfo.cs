using System;

namespace CSharpToTypescriptConverter.Reflection
{
	[Serializable]
	public class MemberInfo
	{
		public string Name { get; set; }
		public Type Type { get; set; }
		public long? Value { get; set; }
		public MemberInfoType MemberInfoType { get; set; }
	}

	public enum MemberInfoType
	{
		Property,
		Field,
		EnumMember,
	}
}
