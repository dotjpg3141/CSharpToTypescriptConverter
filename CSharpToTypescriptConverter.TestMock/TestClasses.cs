#pragma warning disable 1591
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSharpToTypescriptConverter.TestMockReference;

namespace CSharpToTypescriptConverter.TestMock
{
	/// <summary>
	/// Class Documentation
	/// </summary>
	public class PublicClass
	{
		/// <summary>
		/// Property Documentation
		/// </summary>
		public int IntProperty { get; set; }
	}

	public class PublicClass2
	{

	}

	internal class InternalClass
	{
	}

	public enum PublicEnum
	{
		/// <summary>
		/// Enum Value Documentation
		/// </summary>
		EnumValue,
	}

	public class BaseClass
	{

	}

	public class InheritedClass : BaseClass
	{

	}

	public class InheritedExternalClass : ExternalBaseClass
	{

	}
}