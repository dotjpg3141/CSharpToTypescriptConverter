#pragma warning disable 1591
using System;
using System.Collections;
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

		public int[] IntArray { get; set; }
		public List<int> IntList { get; set; }
		public int[][] IntArrayArray { get; set; }
		public List<List<int>> IntListList { get; set; }
		public ArrayList ArrayListType { get; set; }
	}

	public class PublicClass2
	{

	}

	internal class InternalClass
	{
	}

	/// <summary>
	/// Enum Declaration
	/// </summary>
	public enum PublicEnum
	{
		/// <summary>
		/// Enum Value Documentation
		/// </summary>
		EnumValue = 77,
	}

	public class BaseClass
	{

	}

	public class InheritedClass : BaseClass
	{
		public int Property1 { get; set; }
	}

	public class InheritedExternalClass : ExternalBaseClass
	{
		public int Property2 { get; set; }
	}
}